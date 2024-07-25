/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Model;
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

        #endregion Constants

        #region Class members
        private long m_CurrentSequenceNumber;
        private double m_CurrentMinimum;
        private double m_CurrentMaximum;
        private double m_CurrentTotal;
        private double m_InitialPressure;
        private List<Measurement> m_RawMeasurements;
        private ManualResetEvent m_QueueEvent;
        private readonly object m_ProcessingLock = new object();

        private volatile bool m_Stopping;
        private ManualResetEvent m_StoppingEvent;
        private bool m_Disposed = false;
        private volatile bool m_CloseWorkerThreadRequested = false;
        private ManualResetEvent m_ClosingThreadEvent;

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
        public string MeasurementUnit { get; set; }

        /// <summary>
        /// Gets or sets the measure ment conversion factor.
        /// </summary>
        /// <value>
        /// The measure ment conversion factor.
        /// </value>
        public double LowHighPressureFactor { get; set; }

        /// <summary>
        /// Gets or sets the change rate conversion factor.
        /// </summary>
        /// <value>
        /// The change rate conversion factor.
        /// </value>
        public double MbarMinToLeakageFactor { get; set; }

        /// <summary>
        /// Gets or sets the change rate conversion factor.
        /// </summary>
        /// <value>
        /// The change rate conversion factor.
        /// </value>
        public double MeasuredLeakageToMbarMinFactor { get; set; }

        /// <summary>
        /// Gets or sets the QVS conversion factor.
        /// </summary>
        /// <value>
        /// The QVS conversion factor.
        /// </value>
        public double QVSFactor { get; set; }

        /// <summary>
        /// Gets or sets the change rate unit.
        /// </summary>
        /// <value>
        /// The change rate unit.
        /// </value>
        public string ChangeRateUnit { get; set; }

        /// <summary>
        /// Gets or sets the leakage unit.
        /// </summary>
        /// <value>
        /// The leakage unit.
        /// </value>
        public string QVSLeakageUnit { get; set; }

        /// <summary>
        /// Sets the queue.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
        public IList<Measurement> Enqueue
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
                            m_InitialPressure = value[0].Value;
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
                    List<Measurement> measurementCopy = new List<Measurement>();

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
        private List<ScriptCommand5XMeasurement> CalculateScriptCommand5xValues(List<Measurement> measurements)
        {
            List<ScriptCommand5XMeasurement> calculatedValues = new List<ScriptCommand5XMeasurement>();
            double measurementFreq = 1 / (double)MeasurementFrequency;

            foreach (var measurementObject in measurements)
            {
                var measurement = measurementObject.Value;

                if (measurement < m_CurrentMinimum || Double.IsNaN(m_CurrentMinimum))
                {
                    m_CurrentMinimum = measurement;
                }
                if (measurement > m_CurrentMaximum || Double.IsNaN(m_CurrentMaximum))
                {
                    m_CurrentMaximum = measurement;
                }
                m_CurrentTotal += measurement;
                long totalMeasurements = m_CurrentSequenceNumber + 1;
                double leakageValueMbarMin = (((measurement - m_InitialPressure) / LowHighPressureFactor) / ((totalMeasurements * measurementFreq) / 60)) * MeasuredLeakageToMbarMinFactor;
                double roundedLeakageValue = Math.Round(leakageValueMbarMin * MbarMinToLeakageFactor, Resolution, MidpointRounding.AwayFromZero);
                double roundedAverage = Math.Round(m_CurrentTotal / (double)totalMeasurements, Resolution, MidpointRounding.AwayFromZero);
                double roundedLeakageMembrane = Math.Round(leakageValueMbarMin  * 0.1 * QVSFactor, Resolution, MidpointRounding.AwayFromZero);

                double roundedLeakageV1 = Double.NaN;
                double roundedLeakageV2 = Double.NaN;
                if (!Double.IsNaN(VolumeVa))
                {
                    roundedLeakageV1 = Math.Round(((leakageValueMbarMin * VolumeVa * 6) / 100.0)*QVSFactor, Resolution, MidpointRounding.AwayFromZero);
                }
                if (!Double.IsNaN(VolumeVak))
                {
                    roundedLeakageV2 = Math.Round(((leakageValueMbarMin * VolumeVak * 6) / 100.0)*QVSFactor, Resolution, MidpointRounding.AwayFromZero);
                }

                ScriptCommand5XMeasurement scriptCommand5XMeasurement = new ScriptCommand5XMeasurement
                {
                    SequenceNumber = m_CurrentSequenceNumber,
                    Measurement = measurement,
                    Minimum = m_CurrentMinimum,
                    Maximum = m_CurrentMaximum,
                    Average = roundedAverage,
                    LeakageValue = roundedLeakageValue,
                    LeakageMembrane = roundedLeakageMembrane,
                    LeakageV1 = roundedLeakageV1,
                    LeakageV2 = roundedLeakageV2,
                    IoStatus = measurementObject.IoStatus
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
            m_RawMeasurements = new List<Measurement>();
            m_CurrentSequenceNumber = 0;
            m_CurrentMaximum = double.NaN;
            m_CurrentMinimum = double.NaN;
            m_CurrentTotal = 0.0;
            m_InitialPressure = double.NaN;

            m_Stopping = false;
            m_StoppingEvent.Reset();
            MeasurementUnit = String.Empty;

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