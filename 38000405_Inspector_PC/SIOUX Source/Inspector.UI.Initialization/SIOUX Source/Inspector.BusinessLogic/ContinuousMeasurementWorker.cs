/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Threading;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Model.InspectionMeasurement;

namespace Inspector.BusinessLogic
{
    /// <summary>
    /// Delegate for receiving calculated measurement values of a scriptcommand5x
    /// </summary>
    public delegate EventHandler<MeasurementValuesEventArgs> MeasurementValuesReceived(object sender, MeasurementValuesEventArgs e);

    /// <summary>
    /// Processes measurement data
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal class ContinuousMeasurementWorker : IDisposable
    {
        #region Constants
        /// <summary>
        /// This indicates the time in millisecond in which frequency-number of measurements are expected
        /// </summary>
        private const int MEASUREMENTSFREQUENCY_TIMEUNIT_MS = 1000;

        /// <summary>
        /// This indicates the time in seconds (with fraction) in which frequency-number of measurements are expected
        /// </summary>
        private const double MEASUREMENTSFREQUENCY_TIMEUNIT = MEASUREMENTSFREQUENCY_TIMEUNIT_MS / (double)1000;

        /// <summary>
        /// Conversion unit for using bar or mbar with measurements
        /// </summary>
        private static readonly Dictionary<string, double> MeasurementUnitMap = new Dictionary<string, double> { { "bar", 1000.0 }, { "mbar", 1.0 } };
        #endregion Constants

        #region Class members
        private long m_CurrentSequenceNumber;
        private double m_CurrentMinimum;
        private double m_CurrentMaximum;
        private double m_CurrentTotal;
        private double m_InitialPressure;
        private double m_PressureUnit;
        private List<double> m_RawMeasurements;
        private ManualResetEvent m_QueueEvent;
        private readonly object m_ProcessingLock = new object();

        private volatile bool m_Stopping;
        private ManualResetEvent m_StoppingEvent;
        private bool m_Disposed = false;
        private volatile bool m_CloseWorkerThreadRequested = false;
        private ManualResetEvent m_ClosingThreadEvent;

        private volatile string m_MeasurementUnit;
        #endregion Class members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousMeasurementWorker"/> class.
        /// </summary>
        public ContinuousMeasurementWorker()
        {
            m_QueueEvent = new ManualResetEvent(false);
            m_StoppingEvent = new ManualResetEvent(false);
            m_ClosingThreadEvent = new ManualResetEvent(false);
            InitializeMeasurement();
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the measurement frequency.
        /// </summary>
        /// <value>The measurement frequency.</value>
        public int MeasurementFrequency { get; set; }

        /// <summary>
        /// Gets or sets the VolumeVa. (V1)
        /// </summary>
        /// <value>The VolumeVa.</value>
        public double VolumeVa { get; set; }

        /// <summary>
        /// Gets or sets the VolumeVak. (V2)
        /// </summary>
        /// <value>The VolumeVak.</value>
        public double VolumeVak { get; set; }

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
        public int Resolution { get; set; }

        /// <summary>
        /// Gets or sets the measurement unit (bar or mbar)
        /// </summary>
        /// <value>The measurement unit.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the unit value is not equal to bar or mbar.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public string MeasurementUnit
        {
            get
            {
                System.Diagnostics.Debug.WriteLine("Get measurement unit '{0}'", new object[] { m_MeasurementUnit });
                return m_MeasurementUnit;
            }
            set
            {
                if (value.Equals("mbar", StringComparison.Ordinal) || value.Equals("bar", StringComparison.Ordinal))
                {
                    m_MeasurementUnit = value;
                    m_PressureUnit = MeasurementUnitMap[value];
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Tried to set invalid measurement unit '{0}'", new object[] { value });
                    throw new ArgumentOutOfRangeException("MeasurementUnit");
                }
            }
        }

        /// <summary>
        /// Sets the queue.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
        public IList<double> Enqueue
        {
            private get
            {
                return m_RawMeasurements;
            }
            set
            {
                if (value == null)
                    return;

                lock (m_ProcessingLock)
                {
                    if (value.Count > 0)
                    {
                        if (Double.IsNaN(m_InitialPressure))
                        {
                            m_InitialPressure = value[0];
                        }

                        m_RawMeasurements.AddRange(value);
                        m_QueueEvent.Set();
                    }
                }
            }
        }
        #endregion Properties

        #region Dispose
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    lock (m_ProcessingLock)
                    {
                        m_Stopping = false;
                        m_CloseWorkerThreadRequested = true;
                        m_QueueEvent.Set();
                    }
                }

                // Do not wait indefinitely to dispose
                m_ClosingThreadEvent.WaitOne(TimeSpan.FromMilliseconds(5000));
                DisposeManualResetEvents();
            }
            m_Disposed = true;
        }
        #endregion Dispose

        #region Public
        /// <summary>
        /// Thread function that handles all the processing of measurements
        /// </summary>
        public void WorkerThread()
        {
            while (m_QueueEvent.WaitOne())
            {
                lock (m_ProcessingLock)
                {
                    if (m_CloseWorkerThreadRequested)
                    {
                        break;
                    }

                    List<double> measurementCopy = new List<double>();

                    m_QueueEvent.Reset();
                    measurementCopy.AddRange(Enqueue);
                    Enqueue.Clear();

                    List<ScriptCommand5XMeasurement> calculatedValues = CalculateScriptCommand5xValues(measurementCopy);
                    MeasurementValuesEventArgs eventArgs = new MeasurementValuesEventArgs { MeasurementValues = calculatedValues };
                    OnMeasurementValuesReceived(this, eventArgs);

                    if (m_Stopping)
                    {
                        m_Stopping = false;
                        m_StoppingEvent.Set();
                    }
                }
            }

            m_ClosingThreadEvent.Set();
        }

        /// <summary>
        /// Wait until all remaining measurements are processed and prepare for the next measurement run
        /// </summary>
        public void FinishMeasurements()
        {
            bool stopping = false;

            lock (m_ProcessingLock)
            {
                if (Enqueue.Count > 0)
                {
                    m_StoppingEvent.Reset();
                    m_Stopping = true;
                    stopping = true;
                }
            }

            if (stopping)
            {
                m_StoppingEvent.WaitOne();
            }

            InitializeMeasurement();
        }
        #endregion Public

        #region Events
        /// <summary>
        /// Occurs when [measurement values received].
        /// </summary>
        public event EventHandler<MeasurementValuesEventArgs> MeasurementValuesReceived;
        #endregion Events

        #region Eventhandlers
        /// <summary>
        /// Called when [measurement values received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Inspector.BusinessLogic.Interfaces.Events.MeasurementValuesEventArgs"/> instance containing the event data.</param>
        public void OnMeasurementValuesReceived(object sender, MeasurementValuesEventArgs e)
        {
            if (MeasurementValuesReceived != null)
            {
                MeasurementValuesReceived(sender, e);
            }
        }
        #endregion Eventhandlers

        #region Private
        /// <summary>
        /// Calculates the script command5x values.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private List<ScriptCommand5XMeasurement> CalculateScriptCommand5xValues(List<double> measurements)
        {
            List<ScriptCommand5XMeasurement> calculatedValues = new List<ScriptCommand5XMeasurement>();
            double measurementFreq = 1 / (double)MeasurementFrequency;
            double pressureUnit = m_PressureUnit;

            foreach (double measurement in measurements)
            {
                double roundedMeasurement = Math.Round(measurement, Resolution, MidpointRounding.AwayFromZero);
                if (measurement < m_CurrentMinimum || Double.IsNaN(m_CurrentMinimum))
                {
                    m_CurrentMinimum = roundedMeasurement;
                }
                if (measurement > m_CurrentMaximum || Double.IsNaN(m_CurrentMaximum))
                {
                    m_CurrentMaximum = roundedMeasurement;
                }
                m_CurrentTotal += measurement;
                long totalMeasurements = m_CurrentSequenceNumber + 1;
                double leakageValue = ((measurement - m_InitialPressure) * pressureUnit) / ((totalMeasurements * measurementFreq) / 60);
                double roundedLeakageValue = Math.Round(leakageValue, Resolution, MidpointRounding.AwayFromZero);
                double roundedAverage = Math.Round(m_CurrentTotal / (double)totalMeasurements, Resolution, MidpointRounding.AwayFromZero);
                double roundedLeakageMembrane = Math.Round(leakageValue * 0.1, Resolution, MidpointRounding.AwayFromZero);

                double roundedLeakageV1 = Double.NaN;
                double roundedLeakageV2 = Double.NaN;
                if (!Double.IsNaN(VolumeVa))
                {
                    roundedLeakageV1 = Math.Round((leakageValue * VolumeVa * 6) / 100.0, Resolution, MidpointRounding.AwayFromZero);
                }
                if (!Double.IsNaN(VolumeVak))
                {
                    roundedLeakageV2 = Math.Round((leakageValue * VolumeVak * 6) / 100.0, Resolution, MidpointRounding.AwayFromZero);
                }

                ScriptCommand5XMeasurement scriptCommand5XMeasurement = new ScriptCommand5XMeasurement
                {
                    SequenceNumber = m_CurrentSequenceNumber,
                    Measurement = roundedMeasurement,
                    Minimum = m_CurrentMinimum,
                    Maximum = m_CurrentMaximum,
                    Average = roundedAverage,
                    LeakageValue = roundedLeakageValue,
                    LeakageMembrane = roundedLeakageMembrane,
                    LeakageV1 = roundedLeakageV1,
                    LeakageV2 = roundedLeakageV2,
                };
                calculatedValues.Add(scriptCommand5XMeasurement);

                m_CurrentSequenceNumber++;
            }
            return calculatedValues;
        }

        /// <summary>
        /// Initializes the measurement.
        /// </summary>
        private void InitializeMeasurement()
        {
            m_QueueEvent.Reset();
            m_RawMeasurements = new List<double>();
            m_CurrentSequenceNumber = 0;
            m_CurrentMaximum = double.NaN;
            m_CurrentMinimum = double.NaN;
            m_CurrentTotal = 0.0;
            m_InitialPressure = double.NaN;

            m_Stopping = false;
            m_StoppingEvent.Reset();
            m_MeasurementUnit = String.Empty;

            MeasurementFrequency = 0; // this will cause an divide-by-zero exception if not set before a measurement (which is mandatory so it shouldn't be zero to begin with)
            VolumeVa = 0;
            VolumeVak = 0;
        }

        /// <summary>
        /// Disposes the manual reset events.
        /// </summary>
        private void DisposeManualResetEvents()
        {
            if (m_QueueEvent != null)
            {
                m_QueueEvent.Dispose();
                m_QueueEvent = null;
            }

            if (m_StoppingEvent != null)
            {
                m_StoppingEvent.Dispose();
                m_StoppingEvent = null;
            }

            if (m_ClosingThreadEvent != null)
            {
                m_ClosingThreadEvent.Dispose();
                m_ClosingThreadEvent = null;
            }
        }
        #endregion Private
    }
}