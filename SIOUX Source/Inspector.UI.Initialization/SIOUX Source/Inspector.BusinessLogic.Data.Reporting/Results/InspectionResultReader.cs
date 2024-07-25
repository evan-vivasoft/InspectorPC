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
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Data.Reporting.Results.Model;
using Inspector.Infra.Utils;
using Inspector.Model.InspectionReportingResults;

namespace Inspector.BusinessLogic.Data.Reporting.Results
{
    /// <summary>
    /// Reader that looks up the last or previous to last results from
    /// Result.xml and LastResult.xml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class InspectionResultReader : IInspectionResultReader
    {
        #region Members
        private FileSystemWatcher fileSystemWatcher = null;
        private bool m_InspectorResultFileChanged = true;
        private bool m_ResultsLastFileChanged = true;

        private IEnumerable<InspectionResult> m_InspectionResultsDecendingOrder = new List<InspectionResult>();
        private IEnumerable<InspectionResult> m_ResultInspectionResults = null;
        private IEnumerable<InspectionResult> m_LastResultInspectionResults = null;

        private string m_InspectorResultPath = Path.Combine(SettingsUtils.LookupXmlFilePath(), ReportConstants.INSPECTOR_RESULTS_FILENAME);
        private string m_InspectorResultLastPath = Path.Combine(SettingsUtils.LookupXmlFilePath(), ReportConstants.RESULTS_LAST_FILENAME);

        #endregion Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionResultReader"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public InspectionResultReader()
        {
            string watchFilter = "Results*.xml";
            fileSystemWatcher = new FileSystemWatcher(SettingsUtils.LookupXmlFilePath(), watchFilter);
            fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime;

            fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.Renamed += new RenamedEventHandler(fileSystemWatcher_Renamed);

            fileSystemWatcher.EnableRaisingEvents = true;
        }
        #endregion Constructors

        #region EventHandlers
        /// <summary>
        /// Handles the Changed event of the fileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
        void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            UpdateXmlResultFileStatus(e.Name);
        }

        /// <summary>
        /// Handles the Renamed event of the fileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.RenamedEventArgs"/> instance containing the event data.</param>
        void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            UpdateXmlResultFileStatus(e.OldName);
        }
        #endregion EventHandlers

        #region Private
        /// <summary>
        /// Updates the XML file status.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void UpdateXmlResultFileStatus(string fileName)
        {
            if (fileName == ReportConstants.INSPECTOR_RESULTS_FILENAME)
            {
                m_InspectorResultFileChanged = true;
            }
            else if (fileName == ReportConstants.RESULTS_LAST_FILENAME)
            {
                m_ResultsLastFileChanged = true;
            }
        }

        /// <summary>
        /// Looks up the inspection result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The list of inspectionresults for the PRS/GCL combination</returns>
        private IEnumerable<InspectionResult> LookupInspectionResult(string prsName, string gclName = null)
        {
            IEnumerable<InspectionResult> inspectionResults = new List<InspectionResult>();
            if (String.IsNullOrEmpty(gclName))
            {
                inspectionResults = m_InspectionResultsDecendingOrder.Where(result => result.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase) &&
                                                                                      String.IsNullOrEmpty(result.GasControlLineName));
            }
            else
            {
                inspectionResults = m_InspectionResultsDecendingOrder.Where(result => result.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase) &&
                                                                                     !(String.IsNullOrEmpty(result.GasControlLineName)) &&
                                                                                     result.GasControlLineName.Equals(gclName, StringComparison.OrdinalIgnoreCase));
            }
            return inspectionResults;
        }

        /// <summary>
        /// Updates the inspection results if any changes were detected by the filesystemwatcher.
        /// </summary>
        private void UpdateInspectionResults()
        {
            InspectionReport inspectionReport = null;
            bool hasChanges = m_InspectorResultFileChanged || m_ResultsLastFileChanged;

            if (m_InspectorResultFileChanged)
            {
                m_ResultInspectionResults = new List<InspectionResult>();
                if (File.Exists(m_InspectorResultPath))
                {
                    inspectionReport = XMLUtils.Load<InspectionReport>(m_InspectorResultPath);
                    m_ResultInspectionResults = inspectionReport.InspectionResults;
                }
            }
            if (m_ResultsLastFileChanged)
            {
                m_LastResultInspectionResults = new List<InspectionResult>();
                if (File.Exists(m_InspectorResultLastPath))
                {
                    inspectionReport = XMLUtils.Load<InspectionReport>(m_InspectorResultLastPath);
                    m_LastResultInspectionResults = inspectionReport.InspectionResults;
                }
            }

            if (hasChanges)
            {
                m_InspectionResultsDecendingOrder = m_ResultInspectionResults.Concat(m_LastResultInspectionResults)
                                                                             .OrderByDescending(result => FinishedDateTime(result.DateTimeStamp));
            }
        }

        /// <summary>
        /// Creates the date time.
        /// </summary>
        /// <param name="dateTimeStamp">The date time stamp.</param>
        /// <returns>DateTime.MaxValue when the endTime or startDate are not set, otherwise the finish time.</returns>
        private static DateTime FinishedDateTime(DateTimeStamp dateTimeStamp)
        {
            DateTime finished = DateTime.MaxValue;

            if (!String.IsNullOrEmpty(dateTimeStamp.StartDate) && !String.IsNullOrEmpty(dateTimeStamp.EndTime))
            {
                DateTime date = DateTime.ParseExact(dateTimeStamp.StartDate, DateTimeStamp.DATE_FORMAT, CultureInfo.InvariantCulture);
                DateTime time = DateTime.ParseExact(dateTimeStamp.EndTime, DateTimeStamp.TIME_FORMAT, CultureInfo.InvariantCulture);
                finished = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
            }

            return finished;
        }

        /// <summary>
        /// Converts to report inspection result.
        /// </summary>
        /// <param name="inspectionResult">The inspection result.</param>
        /// <returns>The report inspection result or null when inspectionResult is null</returns>
        private static ReportInspectionResult ConvertToReportInspectionResult(InspectionResult inspectionResult)
        {
            ReportInspectionResult reportInspectionResult = null;
            if (inspectionResult != null)
            {
                #region Convert from InspectionResult to ReportInspectionResult
                reportInspectionResult = new ReportInspectionResult
                {
                    CRC = inspectionResult.CRC,
                    GasControlLineName = inspectionResult.GasControlLineName,
                    GCLCode = inspectionResult.GCLCode,
                    GCLIdentification = inspectionResult.GCLIdentification,
                    InspectionProcedureName = inspectionResult.InspectionProcedure.Name,
                    InspectionProcedureVersion = inspectionResult.InspectionProcedure.Version,
                    Measurement_Equipment = new ReportMeasurementEquipment
                    {
                        IdentityDM1 = inspectionResult.Measurement_Equipment.ID_DM1,
                        IdentityDM2 = inspectionResult.Measurement_Equipment.ID_DM2,
                        BluetoothAddress = inspectionResult.Measurement_Equipment.BT_Address,
                    },
                    PRSCode = inspectionResult.PRSCode,
                    PRSIdentification = inspectionResult.PRSIdentification,
                    PRSName = inspectionResult.PRSName,
                    Results = new List<ReportResult>(inspectionResult.Results.Select(item =>
                        new ReportResult
                        {
                            FieldNo = item.FieldNo,
                            List = item.List.ToList(), // toList() copies the list of strings
                            MeasurePoint = item.MeasurePoint,
                            MeasurePointID = item.MeasurePointID,
                            MeasureValue = new ReportMeasureValue
                            {
                                Value = item.MeasureValue != null ? item.MeasureValue.Value : double.NaN,
                                UOM = item.MeasureValue != null ? item.MeasureValue.UOM : Inspector.Model.UnitOfMeasurement.UNSET,
                            },
                            ObjectID = item.ObjectID,
                            ObjectName = item.ObjectName,
                            Text = item.Text,
                            Time = item.Time,
                        }
                        )),
                    Status = inspectionResult.Status,
                    DateTimeStamp = new ReportDateTimeStamp
                    {
                        EndTime = inspectionResult.DateTimeStamp.EndTime,
                        StartDate = inspectionResult.DateTimeStamp.StartDate,
                        StartTime = inspectionResult.DateTimeStamp.StartTime,
                        TimeSettings = new ReportTimeSetting
                        {
                            DST = inspectionResult.DateTimeStamp.TimeSettings.DST,
                            TimeZone = inspectionResult.DateTimeStamp.TimeSettings.TimeZone,
                        },
                    },
                };
                #endregion Convert from InspectionResult to ReportInspectionResult
            }

            return reportInspectionResult;
        }

        /// <summary>
        /// Looks up the result.
        /// </summary>
        /// <param name="skipCount">The skip count.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The latests report inspection result, while skipping the first <paramref name="skipCount"/></returns>
        private ReportInspectionResult LookupResult(int skipCount, string prsName, string gclName)
        {
            UpdateInspectionResults();
            InspectionResult inspectionResult = null;
            IEnumerable<InspectionResult> inspectionResults = LookupInspectionResult(prsName, gclName);
            inspectionResult = inspectionResults.Skip(skipCount).FirstOrDefault();
            return ConvertToReportInspectionResult(inspectionResult);
        }

        /// <summary>
        /// Looks up the report result.
        /// </summary>
        /// <param name="skipCount">The skip count.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number.</param>
        /// <returns>The <paramref name="skipCount"/>'s most recent report result, or null if no report results are available./returns>
        private ReportResult LookupReportResult(int skipCount, string prsName, string gclName, string measurePoint, string objectName, int? fieldNumber)
        {
            UpdateInspectionResults();
            IEnumerable<InspectionResult> inspectionResults = LookupInspectionResult(prsName, gclName);

            Func<Result, bool> criteria = LookupInspectionResultPredicate(measurePoint, objectName, fieldNumber);
            IEnumerable<Result> individualResults = inspectionResults.SelectMany(inspectionResult => inspectionResult.Results)
                                                                     .Where(criteria);
            Result result = individualResults.Skip(skipCount).FirstOrDefault();
            return ConvertToReportResult(result);
        }

        /// <summary>
        /// Converts Result to report result.
        /// </summary>
        /// <param name="result">The Result.</param>
        /// <returns>The report result or null when the conversion fails.</returns>
        private static ReportResult ConvertToReportResult(Result result)
        {
            ReportResult reportResult = null;
            if (result != null)
            {
                reportResult = new ReportResult
                {
                    FieldNo = result.FieldNo,
                    List = result.List.ToList(),
                    MeasurePoint = result.MeasurePoint,
                    MeasurePointID = result.MeasurePointID,
                    ObjectID = result.ObjectID,
                    ObjectName = result.ObjectName,
                    Text = result.Text,
                    Time = result.Time,
                };

                if (result.MeasureValue != null)
                {
                    reportResult.MeasureValue = new ReportMeasureValue
                    {
                        UOM = result.MeasureValue.UOM,
                        Value = result.MeasureValue.Value,
                    };
                }
            }
            return reportResult;
        }

        /// <summary>
        /// Lookups the inspection result predicate.
        /// </summary>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="fieldNo">The field number.</param>
        /// <returns>The inspection result predicate function.</returns>
        private static Func<Result, bool> LookupInspectionResultPredicate(string measurePoint, string objectName, int? fieldNo)
        {
            Func<Result, bool> criteria = null;
            if (fieldNo == null)
            {
                criteria = result => result.MeasurePoint.Equals(measurePoint, StringComparison.Ordinal) &&
                                     result.ObjectName.Equals(objectName, StringComparison.Ordinal);
            }
            else
            {
                criteria = result => result.MeasurePoint.Equals(measurePoint, StringComparison.Ordinal) &&
                                     result.ObjectName.Equals(objectName, StringComparison.Ordinal) &&
                                     result.FieldNo == fieldNo;
            }
            return criteria;
        }
        #endregion Private

        #region IInspectionResultReader Members
        /// <summary>
        /// Looks up the last inspection result.
        /// </summary>
        /// <param name="prsName"></param>
        /// <param name="gclName"></param>
        /// <returns>The last inspection result.</returns>
        public ReportInspectionResult LookupLastResult(string prsName, string gclName = null)
        {
            return LookupResult(0, prsName, gclName);
        }

        /// <summary>
        /// Looks up the previous to last inspection result.
        /// </summary>
        /// <param name="prsName"></param>
        /// <param name="gclName"></param>
        /// <returns>The last to previous inspection result.</returns>
        public ReportInspectionResult LookupPreviousToLastResult(string prsName, string gclName = null)
        {
            return LookupResult(1, prsName, gclName);
        }

        /// <summary>
        /// Looks up the last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>
        /// The last report result that matches the criteria, otherwise null.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupLastReportResult(string prsName, string measurePoint, string objectName, int? FieldNumber = null)
        {
            return LookupReportResult(0, prsName, null, measurePoint, objectName, FieldNumber);
        }

        /// <summary>
        /// Looks up the last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>
        /// The last report result that matches the criteria, otherwise null.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupLastReportResult(string prsName, string gclName, string measurePoint, string objectName, int? FieldNumber = null)
        {
            return LookupReportResult(0, prsName, gclName, measurePoint, objectName, FieldNumber);
        }

        /// <summary>
        /// Looks up the previous to last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>The last report result that matches the criteria, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupPreviousToLastReportResult(string prsName, string measurePoint, string objectName, int? FieldNumber = null)
        {
            return LookupReportResult(1, prsName, null, measurePoint, objectName, FieldNumber);
        }

        /// <summary>
        /// Looks up the previous to last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>The last report result that matches the criteria, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupPreviousToLastReportResult(string prsName, string gclName, string measurePoint, string objectName, int? FieldNumber = null)
        {
            return LookupReportResult(1, prsName, gclName, measurePoint, objectName, FieldNumber);
        }

        #endregion IInspectionResultReader Members
    }
}
