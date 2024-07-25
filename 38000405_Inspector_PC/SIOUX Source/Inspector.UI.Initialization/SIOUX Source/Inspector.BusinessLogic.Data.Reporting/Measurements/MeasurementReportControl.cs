/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Data.Reporting.Measurements.Model;
using Inspector.Infra.Utils;
using Inspector.Model.MeasurementResult;
using KAM.INSPECTOR.Infra;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements
{
    /// <summary>
    /// MeasurementReportControl
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class MeasurementReportControl : IMeasurementReportControl
    {
        #region consts
        private const string SETTINGS_CATEGORY = "APPLICATION";
        private const string SETTINGS_RETURN_NO_VALUE = "<NO VALUE>";
        private const string SETTINGS_MEASUREMENT_WRITE_INTERVAL = "MeasurementFileWriteInterval";
        private const string SETTINGS_MEASUREMENT_EXTENSION = "MeasurementFileExtenstion";
        private const int DEFAULT_MEASUREMENT_WRITE_INTERVAL = 5000;
        private const string DEFAULT_MEASUREMENT_EXTENSION = "fpr";
        #endregion consts

        #region Class Members
        private static readonly char[] invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();

        private MeasurementReport m_MeasurementReport;
        private readonly object m_ProcessingLock = new object();
        private volatile bool m_Stopping = false;
        private volatile bool m_MeasurementFinishing = false;
        private volatile bool m_ExtraDataMeasurementStarted = false;
        private List<double> m_RawMeasurements;
        private List<double> m_RawExtraMeasurements;
        private ManualResetEvent m_QueueEvent;
        private ManualResetEvent m_StoppingEvent;
        private ManualResetEvent m_MeasurementFinishedEvent;
        private string m_MeasurementUnit;

        private Timer m_WriteTimer;

        private string m_MeasurementFileExtension;
        private int m_MeasurementFileWriteInterval;
        private string m_MeasurementFilesPath;

        private string m_CurrentMeasurementFileName;

        private DateTime m_InspectionStartDateTime;
        private DateTime m_CurrentMeasurementStartDateTime;
        private bool m_Disposed = false;
        private bool m_CloseWorkerThreadRequested = false;
        private bool m_MeasurementFileInitialized = false;
        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets the measurement report.
        /// For unit testing only!
        /// </summary>
        internal MeasurementReport MeasurementReport
        {
            get { return m_MeasurementReport; }
        }

        /// <summary>
        /// Sets the queue.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
        internal IList<double> DataQueue
        {
            private get
            {
                return m_RawMeasurements;
            }
            set
            {
                if (value != null)
                {
                    lock (m_ProcessingLock)
                    {
                        m_RawMeasurements.AddRange(value);
                        m_QueueEvent.Set();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the queue.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
        internal IList<double> ExtraDataQueue
        {
            private get
            {
                return m_RawExtraMeasurements;
            }
            set
            {
                if (value != null)
                {
                    lock (m_ProcessingLock)
                    {
                        m_RawExtraMeasurements.AddRange(value);
                        m_QueueEvent.Set();
                    }
                }
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementReportControl"/> class.
        /// </summary>
        public MeasurementReportControl()
        {
            m_QueueEvent = new ManualResetEvent(false);
            m_StoppingEvent = new ManualResetEvent(false);
            m_MeasurementFinishedEvent = new ManualResetEvent(false);
            m_RawMeasurements = new List<double>();
            m_RawExtraMeasurements = new List<double>();

            m_MeasurementFileExtension = GetMeasurementFileExtension();
            m_MeasurementFileWriteInterval = GetMeasurementFileWriteInterval();
            m_MeasurementFilesPath = SettingsUtils.LookupMeasurementFilesPath();

            m_WriteTimer = new Timer(OnWriteInterval, null, Timeout.Infinite, Timeout.Infinite);
        }

        #endregion Constructors

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
                    m_CloseWorkerThreadRequested = true;
                    m_QueueEvent.Set();

                    if (m_WriteTimer != null)
                    {
                        m_WriteTimer.Dispose();
                        m_WriteTimer = null;
                    }

                    if (m_MeasurementFinishedEvent != null)
                    {
                        m_MeasurementFinishedEvent.Dispose();
                        m_MeasurementFinishedEvent = null;
                    }

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
                }
            }

            m_Disposed = true;
        }
        #endregion Dispose

        #region IMeasurementReportControl
        /// <summary>
        /// Starts the measurement report.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="startDateTime">The start date time.</param>
        public void InitializeMeasurementReport(string prsName, string gclName, DateTime startDateTime)
        {
            m_InspectionStartDateTime = startDateTime;
            prsName = new string(prsName.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
            if (String.IsNullOrEmpty(gclName))
            {
                gclName = String.Empty;
            }
            else
            {
                gclName = new string(gclName.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
            }
            string start = startDateTime.ToString("yyMMddHHmm", CultureInfo.InvariantCulture);

            lock (m_ProcessingLock)
            {
                m_MeasurementReport = new MeasurementReport()
                    {
                        PrsName = prsName,
                        GclName = gclName,
                        StartDateTime = start
                    };
                m_MeasurementReport.Measurements = new List<Measurement>();
            }
        }

        /// <summary>
        /// Adds the inspection procedure meta data.
        /// </summary>
        /// <param name="measurementReportMetadata">The measurement report meta data.</param>
        /// <exception cref="MeasurementReportControlException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void AddInspectionProcedureMetadata(InspectionProcedureMetadata measurementReportMetadata)
        {
            FileHeader fileHeader = new FileHeader()
            {
                PlexorName = measurementReportMetadata.PlexorName,
                PlexorBtAddress = measurementReportMetadata.PlexorBtAddress,
                TH1SerialNumber = measurementReportMetadata.TH1SerialNumber,
                TH2SerialNumber = measurementReportMetadata.TH2SerialNumber,
                Station = measurementReportMetadata.Station,
                StationCode = measurementReportMetadata.StationCode,
                GasControlLine = measurementReportMetadata.GasControlLine,
                GasControlLineIdentificationCode = measurementReportMetadata.GasControlLineIdentificationCode,
                TestProgram = measurementReportMetadata.TestProgram,
                InspectionProcedureVersion = measurementReportMetadata.InspectionProcedureVersion,
                InspectorVersion = measurementReportMetadata.InspectorVersion,
                FSDStart = measurementReportMetadata.FSDStart != "-1" ? measurementReportMetadata.FSDStart : String.Empty,
                StartDate = m_InspectionStartDateTime.Date.ToShortDateString(),
                StartTime = m_InspectionStartDateTime.ToLongTimeString()
            };

            try
            {
                lock (m_ProcessingLock)
                {
                    m_MeasurementReport.FileHeader = fileHeader;
                }
            }
            catch
            {
                throw new MeasurementReportControlException("Error adding procedure metadata (fileHeader)");
            }
        }

        /// <summary>
        /// Starts the measurement.
        /// </summary>
        /// <param name="measurementUnit">The pressure unit</param>
        /// <param name="measurementStartDate">The measurement start date.</param>
        /// <exception cref="MeasurementReportControlException"></exception>
        public void StartMeasurement(string measurementUnit, DateTime measurementStartDate)
        {
            Measurement measurement = new Measurement();

            try
            {
                lock (m_ProcessingLock)
                {
                    m_MeasurementUnit = measurementUnit;
                    m_CurrentMeasurementStartDateTime = measurementStartDate;
                    m_ExtraDataMeasurementStarted = false;
                    m_MeasurementReport.Measurements.Add(measurement);
                    m_MeasurementReport.Measurements.Last().Data = new Model.Data(m_MeasurementUnit);
                }
            }
            catch
            {
                throw new MeasurementReportControlException("Error adding measurement");
            }
        }

        /// <summary>
        /// Starts the extra data measurement.
        /// </summary>
        /// <param name="measurementUnit">The measurement unit.</param>
        /// <exception cref="MeasurementReportControlException">Thrown when extra data could not be added to the mesurements.</exception>
        public void StartExtraDataMeasurement()
        {
            string startTime = DateTime.Now.ToLongTimeString();
            try
            {
                lock (m_ProcessingLock)
                {
                    m_ExtraDataMeasurementStarted = true;
                    m_MeasurementReport.Measurements.Last().ExtraData = new ExtraData(m_MeasurementUnit, startTime);
                }
            }
            catch
            {
                throw new MeasurementReportControlException("Error adding Extra Data");
            }
        }

        /// <summary>
        /// Measurementses the received.
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        public void MeasurementsReceived(IList<double> measurements)
        {
            if (m_ExtraDataMeasurementStarted)
            {
                ExtraDataQueue = measurements;
            }
            else
            {
                DataQueue = measurements;
            }
        }

        /// <summary>
        /// Sets up measurement file when required.
        /// </summary>
        public void SetUpMeasurementFileWhenRequired()
        {
            if (!m_MeasurementFileInitialized)
            {
                // Create filename
                string filename;
                lock (m_ProcessingLock)
                {
                    filename = String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}.{3}", m_MeasurementReport.PrsName, m_MeasurementReport.GclName, m_MeasurementReport.StartDateTime, m_MeasurementFileExtension);
                }
                m_CurrentMeasurementFileName = Path.Combine(m_MeasurementFilesPath, filename);
                m_WriteTimer.Change(m_MeasurementFileWriteInterval, Timeout.Infinite);
                m_MeasurementFileInitialized = true;
            }
        }

        /// <summary>
        /// Thread function that handles all the processing of measurements
        /// </summary>
        public void MeasurementDataWorkerThread()
        {
            while (m_QueueEvent.WaitOne())
            {
                lock (m_ProcessingLock)
                {
                    if (m_CloseWorkerThreadRequested)
                    {
                        break;
                    }

                    m_QueueEvent.Reset();

                    if (ExtraDataQueue.Count > 0)
                    {
                        m_MeasurementReport.Measurements.Last().ExtraData.MeasurementValues.AddRange(ExtraDataQueue);
                    }

                    if (DataQueue.Count > 0)
                    {
                        m_MeasurementReport.Measurements.Last().Data.MeasurementValues.AddRange(DataQueue);
                    }

                    DataQueue.Clear();
                    ExtraDataQueue.Clear();

                    if (m_MeasurementFinishing)
                    {
                        m_MeasurementFinishing = false;
                        m_MeasurementFinishedEvent.Set();
                    }

                    if (m_Stopping)
                    {
                        m_Stopping = false;
                        m_StoppingEvent.Set();
                    }
                }
            }
        }

        /// <summary>
        /// Adds the measurement metadata.
        /// </summary>
        /// <param name="measurementMetadata">The measurement metadata.</param>
        /// <exception cref="MeasurementReportControlException">Thrown when measurement meta data could not be added</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void AddMeasurementMetadata(MeasurementMetadata measurementMetadata)
        {
            bool measurementFinishing = false;

            lock (m_ProcessingLock)
            {
                if (DataQueue.Count > 0 || ExtraDataQueue.Count > 0)
                {
                    m_MeasurementFinishedEvent.Reset();
                    m_MeasurementFinishing = true;
                    measurementFinishing = true;
                }
            }

            if (measurementFinishing)
            {
                m_MeasurementFinishedEvent.WaitOne();
            }

            try
            {
                int countTotal;
                lock (m_ProcessingLock)
                {
                    int countData = 0;
                    if (m_MeasurementReport.Measurements.Last().Data != null)
                    {
                        countData = m_MeasurementReport.Measurements.Last().Data.MeasurementValues.Count;
                    }

                    int countExtraData = 0;
                    if (m_MeasurementReport.Measurements.Last().ExtraData != null)
                    {
                        countExtraData = m_MeasurementReport.Measurements.Last().ExtraData.MeasurementValues.Count;
                    }

                    countTotal = countData + countExtraData;
                }

                string endOfMeasurement = (measurementMetadata.EndOfMeasurement != null) ? measurementMetadata.EndOfMeasurement.Value.ToString() : String.Empty;

                DataHeader dataHeader = new DataHeader()
                {
                    ScriptCommand = measurementMetadata.ScriptCommand,
                    StartOfMeasurement = m_CurrentMeasurementStartDateTime.ToString(),
                    EndOfMeasurement = endOfMeasurement,
                    CountTotal = countTotal,
                    Interval = measurementMetadata.Interval,
                    FieldInAccessDatabase = measurementMetadata.FieldInAccessDatabase,
                    ObjectName = measurementMetadata.ObjectName,
                    Measurepoint = measurementMetadata.Measurepoint,
                    Value = measurementMetadata.Value,
                };

                lock (m_ProcessingLock)
                {
                    m_MeasurementReport.Measurements.Last().DataHeader = dataHeader;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddMeasurementMetadata: Error adding measurement metadata (dataHeader)\n" + ex);
                throw new MeasurementReportControlException("Error adding measurement metadata (dataHeader)");
            }
        }

        /// <summary>
        /// Finishes the measurement report.
        /// </summary>
        /// <exception cref="MeasurementReportControlException">Thrown when the measurement report could not be finished.</exception>
        public void FinishMeasurementReport()
        {
            try
            {
                FinishWriteMeasurementReport();
            }
            catch
            {
                throw new MeasurementReportControlException("Failed to finish measurement report.");
            }
        }
        #endregion IMeasurementReportControl

        #region Private functions
        /// <summary>
        /// Gets the measurement file extension.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetMeasurementFileExtension()
        {
            string measurementFileExtension = String.Empty;
            try
            {
                clsSettings settings = new clsSettings();
                string extensionFromSettings = settings.get_GetSetting(SETTINGS_CATEGORY, SETTINGS_MEASUREMENT_EXTENSION).ToString();

                if (extensionFromSettings.Equals(SETTINGS_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
                {
                    measurementFileExtension = DEFAULT_MEASUREMENT_EXTENSION;
                }
                else
                {
                    measurementFileExtension = extensionFromSettings;
                }
            }
            catch
            {
                measurementFileExtension = DEFAULT_MEASUREMENT_EXTENSION;
            }

            return measurementFileExtension;
        }

        /// <summary>
        /// Gets the measurement file write interval.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static int GetMeasurementFileWriteInterval()
        {
            int measurementFileWriteInterval = DEFAULT_MEASUREMENT_WRITE_INTERVAL;
            try
            {
                clsSettings settings = new clsSettings();
                string writeIntervalFromSettings = settings.get_GetSetting(SETTINGS_CATEGORY, SETTINGS_MEASUREMENT_EXTENSION).ToString();

                if (writeIntervalFromSettings.Equals(SETTINGS_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
                {
                    measurementFileWriteInterval = DEFAULT_MEASUREMENT_WRITE_INTERVAL;
                }
                else
                {
                    measurementFileWriteInterval = int.Parse(writeIntervalFromSettings, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                measurementFileWriteInterval = DEFAULT_MEASUREMENT_WRITE_INTERVAL;
            }

            return measurementFileWriteInterval;
        }

        /// <summary>
        /// Called when [write interval].
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnWriteInterval(object state)
        {
            WriteMeasurementFile();
            m_WriteTimer.Change(m_MeasurementFileWriteInterval, Timeout.Infinite);
        }

        /// <summary>
        /// Stops the write measurement.
        /// </summary>
        private void FinishWriteMeasurementReport()
        {
            if (m_MeasurementFileInitialized)
            {
                m_MeasurementFileInitialized = false;
                m_WriteTimer.Change(Timeout.Infinite, Timeout.Infinite);

                bool stopping = false;

                lock (m_ProcessingLock)
                {
                    if (DataQueue.Count > 0 || ExtraDataQueue.Count > 0)
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

                WriteMeasurementFile();
            }

            lock (m_ProcessingLock)
            {
                m_MeasurementReport = null;
            }
        }

        /// <summary>
        /// Writes the measurement file.
        /// </summary>
        private void WriteMeasurementFile()
        {
            bool validMeasurement;
            lock (m_ProcessingLock)
            {
                validMeasurement = (m_MeasurementReport != null);
            }

            if (validMeasurement)
            {
                string filedata = String.Empty;
                lock (m_ProcessingLock)
                {
                    filedata = m_MeasurementReport.ToString();
                }

                if (!Directory.Exists(m_MeasurementFilesPath))
                {
                    Directory.CreateDirectory(m_MeasurementFilesPath);
                }

                using (StreamWriter measurementFile = new StreamWriter(m_CurrentMeasurementFileName, append: false, encoding: Infra.Encodings.WindowsAnsi))
                {
                    measurementFile.Write(filedata);
                }
            }
        }
        #endregion Private functions
    }
}
