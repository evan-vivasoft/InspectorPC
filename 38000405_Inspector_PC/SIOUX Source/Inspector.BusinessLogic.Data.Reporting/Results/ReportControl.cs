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
using System.Xml;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Data.Reporting.Results.Model;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Infra.Utils;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionReportingResults;
using log4net;

namespace Inspector.BusinessLogic.Data.Reporting.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportControl : IReportControl
    {
        #region Class members
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private static readonly ILog cLogger = LogManager.GetLogger(typeof(ReportControl).FullName);

        private string m_XsdPath = string.Empty;
        private string m_ResultsPath = string.Empty;
        private string m_TmpResultsPath = string.Empty;

        #endregion Class members

        #region properties
        /// <summary>
        /// Gets or sets the inspection report, public for testing
        /// </summary>
        /// <value>
        /// The inspection report.
        /// </value>
        public InspectionReport InspectionReport { get; set; }

        #endregion properties

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportControl"/> class.
        /// </summary>
        public ReportControl()
        {
            m_XsdPath = Path.Combine(SettingsUtils.LookupXsdFilePath(), ReportConstants.XSD_FILENAME);
            m_ResultsPath = Path.Combine(SettingsUtils.LookupXmlFilePath(), ReportConstants.INSPECTOR_RESULTS_FILENAME);
            m_TmpResultsPath = Path.Combine(SettingsUtils.LookupTemporaryPath(), ReportConstants.TMPRESULTS_FILENAME);

            if (!File.Exists(m_XsdPath))
            {
                string error = string.Format(CultureInfo.InvariantCulture, "Required XSD file '{0}' not found in directory: '{1}'.", ReportConstants.XSD_FILENAME, m_XsdPath);
                //LogHelper.Log(cLogger, LogHelper.LogLevel.Error, error);
                throw new FileNotFoundException(error);
            }
            else
            {
               // LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "ReportControl() Successfully validated XSD file {0} existence.", ReportConstants.XSD_FILENAME));
            }

            InspectionReport = new InspectionReport();
        }


        #endregion Constructor

        #region Public funtions
        /// <summary>
        /// Saves to XML, after which the XML is validated to the XSD
        /// </summary>
        /// <param name="resultFile">The target file location.</param>
        /// <param name="resultFileXsd">The test report XSD.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the result file could not be written or correctly verified</exception>
        public void WriteResultFile(InspectionReport report, string resultFile)
        {
            string tempResultFile = resultFile + ".tmp";
            WriteToXML(report, tempResultFile);
            try
            {
                XMLUtils.ValidateXmlFile(tempResultFile, m_XsdPath);
            }
            catch (XmlException xmlException)
            {
                throw new InspectorReportControlException("Failed to validate the result file.", xmlException);
            }
            MoveResultFile(tempResultFile, resultFile);
        }


        #endregion Public funtions

        #region IReportControl Functions
        /// <summary>
        /// Adds the temporary file to result.
        /// </summary>
        /// <exception cref="InspectorReportControlException">Thrown when the temporary file could not be added to the result.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object[])")]
        public void AddTemporaryFileToResult()
        {
            try
            {
                //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Started adding tempory result file to result file."));

                // Step 0: Only execute this if tmpResults.xml exists
                if (File.Exists(m_TmpResultsPath))
                {
                    try
                    {
                        // Step 0a: Validate tmpResults.xml                        
                        XMLUtils.ValidateXmlFile(m_TmpResultsPath, m_XsdPath);
                    }
                    catch (Exception ex)
                    {
                        throw new InspectorPresentResultValidationFailedException(string.Format(CultureInfo.InvariantCulture, "Failed validating result file: '{0}' Exception: {1}", m_TmpResultsPath, ex.Message), ex);
                    }

                    if (File.Exists(m_ResultsPath))
                    {
                        try
                        {
                            // Step 1: Validate Results.xml
                            XMLUtils.ValidateXmlFile(m_ResultsPath, m_XsdPath);
                        }
                        catch (Exception ex)
                        {
                            throw new InspectorResultValidationFailedException(string.Format(CultureInfo.InvariantCulture, "Failed validating result file: '{0}' Exception: {1}", m_ResultsPath, ex.Message), ex);
                        }

                        //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Successfully validated temporary result file result file to XSD. Starting merge from temporary to result."));

                        // Step 2: Load tmpResults.xml and results.xml
                        InspectionReport currentInspection = InspectionReportsData.ReadResultFile(m_TmpResultsPath);
                        InspectionReport totalInspections = InspectionReportsData.ReadResultFile(m_ResultsPath);

                        // Step 3: Add tmpResult's Inspection Report to Result
                        totalInspections.InspectionResults.Add(currentInspection.InspectionResults.First());

                        // Step 4: Write Result
                        WriteResultFile(totalInspections, m_ResultsPath);

                        //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Successfully updated result file with tempory results file."));

                        RemoveResultFile(m_TmpResultsPath);
                    }
                    else
                    {
                        //LogHelper.Log(cLogger, LogHelper.LogLevel.Warning, string.Format(CultureInfo.InvariantCulture, "Result file does not exists, moving temporary file to results file."));
                        // Step 0b: If Results.xml does not exist, move tmp to result
                        MoveResultFile(m_TmpResultsPath, m_ResultsPath);
                    }
                }
                else
                {
                    string logMessage = string.Format(CultureInfo.InvariantCulture, "Tempory result file: {0} does not exists, combining with {1} skipped.", ReportConstants.TMPRESULTS_FILENAME, ReportConstants.INSPECTOR_RESULTS_FILENAME);
                    //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, logMessage);
                }
            }
            catch (InspectorPresentResultValidationFailedException exception)
            {
                throw new InspectorReportControlException("Failed to validate the present result XML file.", exception);
            }
            catch (InspectorResultValidationFailedException exception)
            {
                throw new InspectorReportControlException("Failed to validate the result XML file.", exception);
            }
        }

        /// <summary>
        /// Starts the inspection report.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="startTime">The start time.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inspector report control failed to start.</exception>
        public void StartInspectionReport(InspectionStatus status, DateTime startTime)
        {
            try
            {
                CreateBackupIfRequired();
            }
            catch (Exception exception)
            {
                throw new InspectorReportControlException("Failed to start inspection report.", exception);
            }

            InspectionReport = new InspectionReport();
            InspectionReport.InspectionResults.Add(new InspectionResult(status, startTime));

            //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "InpectionStatus set to: {0}, with StartDate: {1}", status.ToString(), startTime.ToString()));
        }

        /// <summary>
        /// Adds the inspection procedure.
        /// </summary>
        /// <param name="inspectionProcedure">The inspection procedure.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inspection procedure could not be added.</exception>
        public void AddInspectionProcedure(InspectionProcedureGenericInformation inspectionProcedure)
        {
            InspectionResult inspectionResult = GetInspectionResult();

            inspectionResult.SetInspectionResultValues(
                    inspectionProcedure.InpectionStatus,
                    ValueOrDefault(inspectionProcedure.PrsIdentification),
                    ValueOrDefault(inspectionProcedure.PrsName),
                    ValueOrDefault(inspectionProcedure.PrsCode),
                    inspectionProcedure.GclName,
                    inspectionProcedure.GclCode,
                    inspectionProcedure.GclIdentification,
                    ValueOrDefault(inspectionProcedure.Crc),
                    ValueOrDefault(inspectionProcedure.ProcedureName),
                    ValueOrDefault(inspectionProcedure.ProcedureVersion)
            );

           // LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Succesfully set inspection procedure data for: {0}", inspectionProcedure.PrsName));
        }

        /// <summary>
        /// Adds the manometer id1.
        /// </summary>
        /// <param name="meterNumber">The meter number.</param>
        /// <param name="manometerIdentification">The manometer identification.</param>
        /// <exception cref="InspectorReportControlException">Thrown when manometer identification could not be added to the report file.</exception>
        public void AddManometerIdentification(MeterNumber meterNumber, string manometerIdentification)
        {
            try
            {
                GetInspectionResult().Measurement_Equipment.SetMeterValue(meterNumber, ValueOrDefault(manometerIdentification));
            }
            catch
            {
                throw new InspectorReportControlException("Failed to add manometer identification to the report file.");
            }
            //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "MeterNumber: {0}, set to: {1}", meterNumber.ToString(), manometerIdentification));
        }

        /// <summary>
        /// Adds the bluetooth address.
        /// </summary>
        /// <param name="bluetoothAddress">The bluetooth address.</param>
        /// <exception cref="InspectorReportControlException">Thrown when bluetooth address could not be added to the report file.</exception>
        public void AddBluetoothAddress(string bluetoothAddress)
        {
            GetInspectionResult().Measurement_Equipment.SetBlueToothAddress(ValueOrDefault(bluetoothAddress));
            //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Bluetoothaddress set to: {0}", bluetoothAddress));
        }


        /// <summary>
        /// Adds the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the result could not be added to the result file.</exception>
        public void AddResult(InspectionProcedureStepResult result)
        {
            MeasureValue measureValue = null;

            if (result.MeasureValue != null)
            {
                measureValue = new MeasureValue(result.MeasureValue.Value.Value, result.MeasureValue.Value.UOM);
            }

            Result oldResult = GetInspectionResult().Results.SingleOrDefault(item => item.SequenceNumber == result.SequenceNumber);

            if (oldResult != null)
            {
                GetInspectionResult().Results.Remove(oldResult);
            }

            Result newResult = new Result(
                result.SequenceNumber,
                ValueOrDefault(result.ObjectName),
                ValueOrDefault(result.ObjectId),
                ValueOrDefault(result.MeasurePoint),
                ValueOrDefault(result.MeasurePointId),
                result.FieldNumber,
                TimeSpan.FromTicks(result.Executed.Ticks),
                measureValue,
                result.Text,
                result.Options,
                ValueOrDefault(result.ObjectNameDescription),
                ValueOrDefault(result.MeasurePointDescription)
                );

            GetInspectionResult().Results.Add(newResult);

            //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Successfully added Result with MeasurePoint: '{0}'", result.MeasurePoint));
            WriteResultFile(InspectionReport, m_TmpResultsPath);
        }

        /// <summary>
        /// Updates the remark.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="remark">The remark.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the remark could not be updated.</exception>
        public void UpdateRemark(long sequenceNumber, string remark)
        {
            Result remarkResult = GetInspectionResult().Results.FirstOrDefault(result => result.SequenceNumber == sequenceNumber);
            if (remarkResult != null)
            {
                remarkResult.SetText(remark);
                WriteResultFile(InspectionReport, m_TmpResultsPath);
            }
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inspection status could not be updated.</exception>
        public void UpdateStatus(InspectionStatus status)
        {
            GetInspectionResult().SetInspectionStatus(status);

           // LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Successfully updated status to: '{0}'", GetInspectionResult().Status));
        }

        /// <summary>
        /// Finishes the inspection report.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="endTime">The end time.</param>
        /// <exception cref="InspectorReportControlException">Thrown when the inpsection report could not be finished.</exception>
        public void FinishInspectionReport(InspectionStatus status, DateTime endTime)
        {
            if (GetInspectionResult().Results.Count > 0)
            {
                UpdateStatus(status);

                GetInspectionResult().DateTimeStamp.SetEndTime(endTime);
               
                // LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Finished inspection report, Status set to: '{0}'; EndTime Set to: '{1}'", GetInspectionResult().Status, GetInspectionResult().DateTimeStamp.EndTime));

                InspectionReportsData.AddCurrentResults(InspectionReport.InspectionResults);

                WriteResultFile(InspectionReport, m_TmpResultsPath);
                AddTemporaryFileToResult();
            }
        }
        #endregion ReportControl Functions

        #region Private Functions

       
        /// <summary>
        /// Creates the backup if required.
        /// </summary>
        private void CreateBackupIfRequired()
        {
            if (File.Exists(m_TmpResultsPath))
            {
                string destination = Path.Combine(SettingsUtils.LookupTemporaryPath(), Path.GetTempFileName());
                File.Copy(m_TmpResultsPath, destination, overwrite: true);
            }
        }

        /// <summary>
        /// Moves the result file.
        /// </summary>
        /// <param name="tempResultFile">The temp result file.</param>
        /// <param name="resultFile">The result file.</param>
        /// <exception cref="InspectorReportControlException">Thrown when a error occured while moving the temporary report file to the final report file.</exception>
        private static void MoveResultFile(string tempResultFile, string resultFile)
        {
           // LogHelper.Log(cLogger, LogHelper.LogLevel.Debug, string.Format(CultureInfo.InvariantCulture, "Moving file '{0}'to : '{1}'", tempResultFile, resultFile));

            try
            {
                if (File.Exists(resultFile))
                {
                    File.Delete(resultFile);
                }
                File.Move(tempResultFile, resultFile);  // atomic action on NTFS
            }
            catch (Exception ex)
            {
                string error = string.Format(CultureInfo.InvariantCulture, "An error occured moving file '{0}'to : '{1}'. Exception: {2}", tempResultFile, resultFile, ex.Message);
               // LogHelper.Log(cLogger, LogHelper.LogLevel.Error, error);
                throw new InspectorReportControlException(error, ex);
            }
        }

        /// <summary>
        /// Removes the result file.
        /// </summary>
        /// <param name="resultFile">The result file.</param>
        private static void RemoveResultFile(string resultFile)
        {
           // LogHelper.Log(cLogger, LogHelper.LogLevel.Debug, string.Format(CultureInfo.InvariantCulture, "Removing file: {0}", resultFile));

            try
            {
                File.Delete(resultFile);
            }
            catch (Exception ex)
            {
                string error = string.Format(CultureInfo.InvariantCulture, "An error occured deleting file: {0}. Exception: {1}", resultFile, ex.Message);
                //LogHelper.Log(cLogger, LogHelper.LogLevel.Error, error);
                throw new InspectorReportControlException(error, ex);
            }
        }

        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <param name="targetFileLocation">The target file location.</param>
        /// <exception cref="InspectorReportControlException">Thrown when a error occured while saving the report file.</exception>
        private static void WriteToXML(InspectionReport report, string targetFileLocation)
        {
            //LogHelper.Log(cLogger, LogHelper.LogLevel.Debug, string.Format(CultureInfo.InvariantCulture, "Writing report to: {0}", targetFileLocation));

            try
            {
                XMLUtils.Save(report, targetFileLocation);
            }
            catch (Exception ex)
            {
                string error = string.Format(CultureInfo.InvariantCulture, "An error writing report to: {0}. Exception: {1}", targetFileLocation, ex.Message);
                //LogHelper.Log(cLogger, LogHelper.LogLevel.Error, error);
                throw new InspectorReportControlException(error, ex);
            }
        }

        /// <summary>
        /// Values the or default.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value, or an empty string if null</returns>
        private static string ValueOrDefault(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Gets the inspection result.
        /// </summary>
        /// <returns>The first inspection result of the the inspection report.</returns>
        /// <exception cref="InspectorReportControlException">Thrown when no inspection results are available in the inspection report.</exception>
        private InspectionResult GetInspectionResult()
        {
            InspectionResult inspectionResult = null;

            if (InspectionReport.InspectionResults.Count == 1)
            {
                inspectionResult = InspectionReport.InspectionResults.First();
            }
            else
            {
                string error = string.Format(CultureInfo.InvariantCulture, "Unable to retrieve the InspectionResult from inspection result data. There is not 1 InspectionResult.");
                //LogHelper.Log(cLogger, LogHelper.LogLevel.Error, error);
                throw new InspectorReportControlException(error);
            }

            return inspectionResult;
        }
        #endregion
    }

}
