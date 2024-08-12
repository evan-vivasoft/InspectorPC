using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Data.Reporting.Results.Automapper;
using Inspector.BusinessLogic.Data.Reporting.Results.Model;
using Inspector.Infra.Ioc;
using Inspector.Infra.Utils;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionReportingResults;
using JSONParser.InspectionResults;

namespace Inspector.BusinessLogic.Data.Reporting.Results
{
    internal static class InspectionReportsData
    {
        private static readonly string ResultsPath = Path.Combine(SettingsUtils.LookupXmlFilePath(), ReportConstants.INSPECTOR_RESULTS_FILENAME);

        private static readonly string ResultsLastPath = Path.Combine(SettingsUtils.LookupXmlFilePath(), ReportConstants.RESULTS_LAST_FILENAME);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1504:ReviewMisleadingFieldNames")]
        private static IInspectionInformationManager m_InspectionInformationManager;

        private static IInspectionInformationManager InspectionInformationManager
        {
            get
            {
                return m_InspectionInformationManager ?? (m_InspectionInformationManager = ContextRegistry.Context.Resolve<IInspectionInformationManager>());
            }
        } 

        private static readonly object InspectionResultLock = new object();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1504:ReviewMisleadingFieldNames")]
        private static List<InspectionResult> m_InspectionResultsDescendingOrder;

        private static List<InspectionResult> InspectionResultsDescendingOrder
        {
            get
            {
                if (m_InspectionResultsDescendingOrder == null)
                {
                    ReadResults();
                }

                return m_InspectionResultsDescendingOrder;
            }
            set
            {
                m_InspectionResultsDescendingOrder = value;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        internal static ReportInspectionResult LookupLastResult(string prsName, string gclName = null)
        {
            lock (InspectionResultLock)
            {
                var inspectionResults = LookupInspectionResult(prsName, gclName);
                var inspectionResult = inspectionResults.FirstOrDefault();
                
                if (inspectionResult != null)
                {
                    AddDescriptionsToResults(inspectionResult.Results);
                }
                
                return ConvertToReportInspectionResult(inspectionResult);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        internal static ReportInspectionResult LookupPreviousToLastResult(string prsName, string gclName = null)
        {
            lock (InspectionResultLock)
            {
                var inspectionResults = LookupInspectionResult(prsName, gclName);
                var inspectionResult = inspectionResults.Skip(1).FirstOrDefault();

                if (inspectionResult != null)
                {
                    AddDescriptionsToResults(inspectionResult.Results);
                }
                
                return ConvertToReportInspectionResult(inspectionResult);
            }
        }

        internal static List<ReportInspectionResult> LookupAllResults(string prsName, string gclName)
        {
            lock (InspectionResultLock)
            {
                var inspectionResults = LookupInspectionResult(prsName, gclName).ToList();
                if(inspectionResults != null && inspectionResults.Any())
                {
                    inspectionResults.ForEach(i => AddDescriptionsToResults(i.Results));
                    return inspectionResults.Select(i => ConvertToReportInspectionResult(i)).ToList();
                }
            }
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        internal static ReportResult LookupLastReportResult(string prsName, string measurePoint, string objectName, int? fieldNumber = null)
        {
            lock (InspectionResultLock)
            {
                var inspectionResults = LookupInspectionResult(prsName);

                var criteria = LookupInspectionResultPredicate(measurePoint, objectName, fieldNumber);
                var individualResults = inspectionResults.SelectMany(inspectionResult => inspectionResult.Results).Where(criteria);
                var result = individualResults.FirstOrDefault();

                AddDescriptionsToResults(new List<Result>{result});
                
                return ConvertToReportResult(result);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        internal static ReportResult LookupLastReportResult(string prsName, string gclName, string measurePoint, string objectName, int? fieldNumber = null)
        {
            lock (InspectionResultLock)
            {
                var inspectionResults = LookupInspectionResult(prsName, gclName);

                var criteria = LookupInspectionResultPredicate(measurePoint, objectName, fieldNumber);
                var individualResults = inspectionResults.SelectMany(inspectionResult => inspectionResult.Results).Where(criteria);
                var result = individualResults.FirstOrDefault();

                AddDescriptionsToResults(new List<Result> { result });
                
                return ConvertToReportResult(result);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        internal static ReportResult LookupPreviousToLastReportResult(string prsName, string measurePoint, string objectName, int? fieldNumber = null)
        {
            lock (InspectionResultLock)
            {
                var inspectionResults = LookupInspectionResult(prsName);

                var criteria = LookupInspectionResultPredicate(measurePoint, objectName, fieldNumber);
                var individualResults = inspectionResults.SelectMany(inspectionResult => inspectionResult.Results).Where(criteria);
                var result = individualResults.Skip(1).FirstOrDefault();

                AddDescriptionsToResults(new List<Result> { result });
                
                return ConvertToReportResult(result);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        internal static ReportResult LookupPreviousToLastReportResult(string prsName, string gclName, string measurePoint, string objectName, int? fieldNumber = null)
        {
            lock (InspectionResultLock)
            {
                var inspectionResults = LookupInspectionResult(prsName, gclName);

                var criteria = LookupInspectionResultPredicate(measurePoint, objectName, fieldNumber);
                var individualResults = inspectionResults.SelectMany(inspectionResult => inspectionResult.Results).Where(criteria);
                var result = individualResults.Skip(1).FirstOrDefault();

                AddDescriptionsToResults(new List<Result> { result });
                
                return ConvertToReportResult(result);
            }
        }

        /// <summary>
        /// Deserializes the specified resultFile to InspectionReport
        /// </summary>
        /// <param name="resultFile">The result file.</param>
        internal static InspectionReport ReadResultFile(string resultFile)
        {
            InspectionReport inspectionReport = new InspectionReport();
            try
            {
                using (var adapter = new InspectionReportAdapter())
                {
                    inspectionReport.InspectionResults = MapperClass.Instance.Mapper.Map<List<InspectionResult>>(adapter.results);
                }
                Console.WriteLine(resultFile);
                //inspectionReport = XMLUtils.Load<InspectionReport>(resultFile);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, "Could not read xml file. Exception: {0}", exception);
                
                throw new InspectorReportControlException(error, exception);
            }
            return inspectionReport;
        }


        internal static void AddCurrentResults(List<InspectionResult> list)
        {
            lock (InspectionResultLock)
            {
                InspectionResultsDescendingOrder = list.Concat(InspectionResultsDescendingOrder).OrderByDescending(result => FinishedDateTime(result.DateTimeStamp)).ToList();
            }
        }

        private static void AddDescriptionsToResults(List<Result> results)
        {
            foreach (var result in results.Where(result => result != null))
            {
                string ond;
                string mpd;
                
                InspectionInformationManager.FindObjectNameDescriptionAndMeasurePointDescription(result.ObjectName,result.MeasurePoint,result.FieldNo, out ond, out mpd);

                result.ObjectNameDescription = ond;
                result.MeasurePointDescription = mpd;
            }
        }

        /// <summary>
        /// Reads the results.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void ReadResults()
        {
            lock (InspectionResultLock)
            {
                var results = ReadResultFile(ResultsPath);
                var resultLast = ReadResultFile(ResultsLastPath);
                IEnumerable<InspectionResult> inspectionResults = new List<InspectionResult>();

                if (results != null)
                {
                    inspectionResults = inspectionResults.Concat(results.InspectionResults);
                }

                if (resultLast != null)
                {
                    inspectionResults = inspectionResults.Concat(resultLast.InspectionResults);
                }


                m_InspectionResultsDescendingOrder = inspectionResults.OrderByDescending(result => FinishedDateTime(result.DateTimeStamp)).ToList();

            }
        }

        private static DateTime FinishedDateTime(DateTimeStamp dateTimeStamp)
        {
            var finished = DateTime.MaxValue;

            if (!string.IsNullOrEmpty(dateTimeStamp.StartDate) && !String.IsNullOrEmpty(dateTimeStamp.EndTime))
            {
                var date = DateTime.ParseExact(dateTimeStamp.StartDate, DateTimeStamp.DATE_FORMAT, CultureInfo.InvariantCulture);
                var time = DateTime.ParseExact(dateTimeStamp.EndTime, DateTimeStamp.TIME_FORMAT, CultureInfo.InvariantCulture);
                
                finished = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
            }

            return finished;
        }

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
                            List = (item.List ?? new List<string>()).ToList(), // toList() copies the list of strings
                            MeasurePoint = item.MeasurePoint,
                            MeasurePointID = item.MeasurePointID,
                            MeasureValue = new ReportMeasureValue
                            {
                                Value = item.MeasureValue != null ? item.MeasureValue.Value : double.NaN,
                                UOM =
                                    item.MeasureValue != null
                                        ? item.MeasureValue.UOM
                                        : Inspector.Model.UnitOfMeasurement.UNSET,
                            },
                            ObjectID = item.ObjectID,
                            ObjectName = item.ObjectName,
                            Text = item.Text,
                            Time = item.Time,
                            ObjectNameDescription = item.ObjectNameDescription,
                            MeasurePointDescription = item.MeasurePointDescription
                        }
                        )),
                    Status = inspectionResult.Status ?? (int)InspectionStatus.Unset,
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
        /// Looks up the inspection result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The list of inspectionresults for the PRS/GCL combination</returns>
        private static IEnumerable<InspectionResult> LookupInspectionResult(string prsName, string gclName = null)
        {
            var inspectionResults = string.IsNullOrEmpty(gclName) ? 
                InspectionResultsDescendingOrder.Where(result => result.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(result.GasControlLineName)).ToList() : 
                InspectionResultsDescendingOrder.Where(result => result.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase) && !(string.IsNullOrEmpty(result.GasControlLineName)) && result.GasControlLineName.Equals(gclName, StringComparison.OrdinalIgnoreCase)).ToList();

            return inspectionResults;
        }

        /// <summary>
        /// Lookups the inspection result predicate.
        /// </summary>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="fieldNo">The field number.</param>
        /// <returns>The inspection result predicate function.</returns>
        private static Func<Result, bool> LookupInspectionResultPredicate(string measurePoint, string objectName, int? fieldNo = null)
        {
            Func<Result, bool> criteria;

            if (fieldNo == null)
            {
                criteria = result => result.MeasurePoint.Equals(measurePoint, StringComparison.Ordinal) && result.ObjectName.Equals(objectName, StringComparison.Ordinal);
            }
            else
            {
                criteria = result => result.MeasurePoint.Equals(measurePoint, StringComparison.Ordinal) && result.ObjectName.Equals(objectName, StringComparison.Ordinal) && result.FieldNo == fieldNo;
            }
            
            return criteria;
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
                    List = (result.List ?? new List<string>()).ToList(),
                    MeasurePoint = result.MeasurePoint,
                    MeasurePointID = result.MeasurePointID,
                    ObjectID = result.ObjectID,
                    ObjectName = result.ObjectName,
                    Text = result.Text,
                    Time = result.Time,
                    ObjectNameDescription = result.ObjectNameDescription,
                    MeasurePointDescription = result.MeasurePointDescription
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
    }
}
