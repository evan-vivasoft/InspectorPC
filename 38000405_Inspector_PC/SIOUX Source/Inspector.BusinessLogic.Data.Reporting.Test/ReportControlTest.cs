/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Data.Reporting.Results;
using Inspector.Infra;
using Inspector.Infra.Utils;
using Inspector.Model;
using Inspector.Model.InspectionProcedure;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Reporting.Test
{
    [TestFixture]
    public class ReportControlTest
    {
        private string m_PrsIdentification = "PRS ID";
        private string m_PrsName = "UNIT TEST 1 | 5 007 230.1";
        private string m_PrsCode = "UNIT TEST PDASNR=0D9015E4830E0";
        private string m_GasControlLineName = "UNIT TEST 5 007 230.1.1 OS LS";
        private string m_GclIdentification = "UNIT TEST 23.05.2012 5 007 230.1.1";
        private string m_GCLCode = "UNIT TEST GCL Code";
        private string m_CRC = "UNIT TEST CRC";

        private string m_DM1 = "UNIT TEST HM3500DLM110,MOD00B,1048113";
        private string m_DM2 = "UNIT TEST DM2";
        private string m_BT_Address = "UNIT TEST 00:80:98:C4:20:F4";

        private string m_InspectionProcedureName = "UNIT TEST B-insp MON-VA LS (Pd &gt; 300 mbar)";
        private string m_InspectionProcedureVersion = "1.0";

        private DateTime m_Testtime1 = new DateTime(2012, 3, 30, 9, 17, 0);
        private string m_MeasurePoint = "00205";
        private string m_MeasurePoint2 = "00206";
        private string m_ObjectName = "OName";
        private string m_ObjectId = "OId";
        private string m_TextwithInvalidChars = @"Invaled are: Less: < Greater: >, single: ' , double: "" quotes and ampersand & ";
        private string m_TextEmpty = String.Empty;
        private string m_TextRemark = "Remark";
        private string m_MeasurePointId = "Measure";


        private int m_FieldNo1 = 15;
        private int m_FieldNo2 = 47;

        UnitOfMeasurement m_UnitOfMeasureMentDm3h = UnitOfMeasurement.ItemDm3h;
        UnitOfMeasurement m_UnitOfMeasureMentMbar = UnitOfMeasurement.ItemMbar;
        UnitOfMeasurement m_UnitOfMeasureMentBar = UnitOfMeasurement.ItemBar;
        UnitOfMeasurement m_UnitOfMeasureMentMbarMin = UnitOfMeasurement.ItemMbarMin;

        double m_MeasureValue = 5.8;
        double m_MeasureValue2 = 10.3;

        DateTime m_Testtime2 = new DateTime(2012, 3, 30, 13, 21, 12);
        List<string> m_Lists = new List<string>() { "A", "B", "C" };

        [SetUp]
        public void Setup()
        {
            var path = Path.GetDirectoryName(typeof(ReportControlTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);
        }

        [TearDown]
        public void TearDown()
        {
        }

        /// <summary>
        /// Reports the control happy flow test.
        /// </summary>
        [Test]
        public void ReportControlHappyFlowTest()
        {
            LogHelper.DebugExtended = true;

            ReportControl reportControl = new ReportControl();
            string xmlTempFileLocation = Path.Combine(SettingsUtils.LookupTemporaryPath(), ReportConstants.TMPRESULTS_FILENAME);
            string xmlFileLocation = Path.Combine(SettingsUtils.LookupXmlFilePath(), ReportConstants.INSPECTOR_RESULTS_FILENAME);

            if (File.Exists(xmlTempFileLocation))
            {
                File.Delete(xmlTempFileLocation);
            }

            DateTime start = new DateTime(2012, 3, 29, 16, 19, 00);
            DateTime end = new DateTime(2012, 3, 29, 16, 20, 00);

            // Start Inspection
            reportControl.StartInspectionReport(InspectionStatus.StartNotCompleted, start);
            Assert.AreEqual("2012-03-29", reportControl.InspectionReport.InspectionResults.First().DateTimeStamp.StartDate, "StartInspectionReport");
            Assert.AreEqual("16:19:00", reportControl.InspectionReport.InspectionResults.First().DateTimeStamp.StartTime, "StartInspectionReport");
            Assert.AreEqual(2, reportControl.InspectionReport.InspectionResults.First().Status, "StartInspectionReport");
            Assert.IsFalse(File.Exists(xmlTempFileLocation), "Validating XML File is not created.");

            // Update the Status
            reportControl.UpdateStatus(InspectionStatus.GclOrPrsDeletedByUser);
            Assert.AreEqual(4, reportControl.InspectionReport.InspectionResults.First().Status, "UpdateStatus");

            // Add an Inspection Procedure
            reportControl.AddInspectionProcedure(new InspectionProcedureGenericInformation
                (
                InspectionStatus.NoInspection, m_PrsIdentification, m_PrsName, m_PrsCode, m_GasControlLineName, m_GclIdentification, m_GCLCode, m_CRC, m_InspectionProcedureName, m_InspectionProcedureVersion)
                );
            Assert.AreEqual(m_PrsIdentification, reportControl.InspectionReport.InspectionResults.First().PRSIdentification, "PRSIdentification");
            Assert.AreEqual(m_PrsName, reportControl.InspectionReport.InspectionResults.First().PRSName, "PRSName");
            Assert.AreEqual(m_PrsCode, reportControl.InspectionReport.InspectionResults.First().PRSCode, "PRSCode");
            Assert.AreEqual(m_GasControlLineName, reportControl.InspectionReport.InspectionResults.First().GasControlLineName, "GasControlLineName");
            Assert.AreEqual(m_GclIdentification, reportControl.InspectionReport.InspectionResults.First().GCLIdentification, "GCLIdentification");
            Assert.AreEqual(m_GCLCode, reportControl.InspectionReport.InspectionResults.First().GCLCode, "GCLCode");
            Assert.AreEqual(m_CRC, reportControl.InspectionReport.InspectionResults.First().CRC, "CRC");
            Assert.AreEqual(m_InspectionProcedureName, reportControl.InspectionReport.InspectionResults.First().InspectionProcedure.Name, "Name");
            Assert.AreEqual(m_InspectionProcedureVersion, reportControl.InspectionReport.InspectionResults.First().InspectionProcedure.Version, "Version");
            Assert.AreEqual(1, reportControl.InspectionReport.InspectionResults.First().Status, "UpdateStatus");

            // Add Manometer Information
            reportControl.AddManometerIdentification(MeterNumber.ID_DM1, m_DM1);
            Assert.AreEqual(m_DM1, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM1);
            Assert.AreEqual(string.Empty, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM2);

            reportControl.AddManometerIdentification(MeterNumber.ID_DM2, m_DM2);
            reportControl.AddManometerIdentification(MeterNumber.ID_DM1, m_DM1);
            Assert.AreEqual(m_DM1, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM1);
            Assert.AreEqual(m_DM2, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM2);
            Assert.AreEqual(string.Empty, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.BT_Address);

            // Add Bluetooth Address
            reportControl.AddBluetoothAddress(m_BT_Address);
            Assert.AreEqual(m_BT_Address, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.BT_Address);
            Assert.IsFalse(File.Exists(xmlTempFileLocation), "Validating XML File is not created.");

            // Add Result
            reportControl.AddResult(new InspectionProcedureStepResult(1, m_MeasurePoint, m_Testtime1, m_FieldNo1, null, null, null, null, null, null));
            Assert.AreEqual(m_FieldNo1, reportControl.InspectionReport.InspectionResults.First().Results.First().FieldNo);
            Assert.AreEqual(m_MeasurePoint, reportControl.InspectionReport.InspectionResults.First().Results.First().MeasurePoint);
            Assert.IsEmpty(reportControl.InspectionReport.InspectionResults.First().Results.First().MeasurePointID);
            Assert.IsNull(reportControl.InspectionReport.InspectionResults.First().Results.First().MeasureValue, "MeasureValue is nullable");
            Assert.IsEmpty(reportControl.InspectionReport.InspectionResults.First().Results.First().ObjectID);
            Assert.IsEmpty(reportControl.InspectionReport.InspectionResults.First().Results.First().ObjectName);
            Assert.IsNull(reportControl.InspectionReport.InspectionResults.First().Results.First().Text, "Text is nullable");
            Assert.IsNull(reportControl.InspectionReport.InspectionResults.First().Results.First().List, "List is nullable");
            Assert.AreEqual(m_Testtime1.ToString("HH:mm:ss"), reportControl.InspectionReport.InspectionResults.First().Results.First().Time, "");
            Assert.IsTrue(File.Exists(xmlTempFileLocation), "Validating XML File is created.");

            // Add another result, this time with all required fields using UOM dm3/h
            InspectionProcedureStepResultMeasureValue measureValue = new InspectionProcedureStepResultMeasureValue(m_MeasureValue, m_UnitOfMeasureMentDm3h);
            reportControl.AddResult(new InspectionProcedureStepResult(2, m_MeasurePoint2,m_Testtime2, m_FieldNo2, m_ObjectName, m_ObjectId, m_MeasurePointId, m_TextwithInvalidChars,"objectNameDescription","measurePointDescription", m_Lists, measureValue));
            Assert.AreEqual(m_FieldNo2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).FieldNo);
            Assert.AreEqual(m_MeasurePoint2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).MeasurePoint);
            Assert.AreEqual(m_MeasurePointId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).MeasurePointID);
            Assert.AreEqual("dm3/h", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).MeasureValue.UOM.GetDescription(), "Cast");
            Assert.AreEqual(m_MeasureValue, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).MeasureValue.Value, "MeasureValue is nullable");
            Assert.AreEqual(m_ObjectId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).ObjectID);
            Assert.AreEqual(m_ObjectName, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).ObjectName);
            Assert.AreEqual(m_TextwithInvalidChars, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).Text, "Text contains XML invalid chars, xsd validation should succee");
            Assert.AreEqual("objectNameDescription", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).ObjectNameDescription);
            Assert.AreEqual("measurePointDescription", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).MeasurePointDescription);
            // Add another result, this time with all required fields, using UOM bar
            measureValue = new InspectionProcedureStepResultMeasureValue(m_MeasureValue, m_UnitOfMeasureMentBar);
            reportControl.AddResult(new InspectionProcedureStepResult(3, m_MeasurePoint2, m_Testtime2, m_FieldNo2, m_ObjectName, m_ObjectId, m_MeasurePointId, m_TextwithInvalidChars, string.Empty, string.Empty, m_Lists, measureValue));
            Assert.AreEqual(m_FieldNo2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).FieldNo);
            Assert.AreEqual(m_MeasurePoint2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).MeasurePoint);
            Assert.AreEqual(m_MeasurePointId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).MeasurePointID);
            Assert.AreEqual("bar", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).MeasureValue.UOM.GetDescription(), "Cast");
            Assert.AreEqual(m_MeasureValue, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).MeasureValue.Value, "MeasureValue is nullable");
            Assert.AreEqual(m_ObjectId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).ObjectID);
            Assert.AreEqual(m_ObjectName, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).ObjectName);
            Assert.AreEqual(m_TextwithInvalidChars, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(2).Text, "Text contains XML invalid chars, xsd validation should succee");

            // Add another result, this time with all required fields, using UOM mbar
            measureValue = new InspectionProcedureStepResultMeasureValue(m_MeasureValue, m_UnitOfMeasureMentMbar);
            reportControl.AddResult(new InspectionProcedureStepResult(4, m_MeasurePoint2, m_Testtime2, m_FieldNo2, m_ObjectName, m_ObjectId, m_MeasurePointId, m_TextwithInvalidChars, string.Empty, string.Empty, m_Lists, measureValue));
            Assert.AreEqual(m_FieldNo2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).FieldNo);
            Assert.AreEqual(m_MeasurePoint2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).MeasurePoint);
            Assert.AreEqual(m_MeasurePointId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).MeasurePointID);
            Assert.AreEqual("mbar", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).MeasureValue.UOM.GetDescription(), "Cast");
            Assert.AreEqual(m_MeasureValue, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).MeasureValue.Value, "MeasureValue is nullable");
            Assert.AreEqual(m_ObjectId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).ObjectID);
            Assert.AreEqual(m_ObjectName, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).ObjectName);
            Assert.AreEqual(m_TextwithInvalidChars, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(3).Text, "Text contains XML invalid chars, xsd validation should succee");

            // Add another result, this time with all required fields, using UOM mbar/min
            measureValue = new InspectionProcedureStepResultMeasureValue(m_MeasureValue, m_UnitOfMeasureMentMbarMin);
            reportControl.AddResult(new InspectionProcedureStepResult(5, m_MeasurePoint2, m_Testtime2, m_FieldNo2, m_ObjectName, m_ObjectId, m_MeasurePointId, m_TextwithInvalidChars, string.Empty, string.Empty, m_Lists, measureValue));
            Assert.AreEqual(m_FieldNo2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).FieldNo);
            Assert.AreEqual(m_MeasurePoint2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).MeasurePoint);
            Assert.AreEqual(m_MeasurePointId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).MeasurePointID);
            Assert.AreEqual("mbar/min", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).MeasureValue.UOM.GetDescription(), "Cast");
            Assert.AreEqual(m_MeasureValue, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).MeasureValue.Value, "MeasureValue is nullable");
            Assert.AreEqual(m_ObjectId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).ObjectID);
            Assert.AreEqual(m_ObjectName, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).ObjectName);
            Assert.AreEqual(m_TextwithInvalidChars, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(4).Text, "Text contains XML invalid chars, xsd validation should succee");

            // Add a remark result with empty values, after that update it.
            reportControl.AddResult(new InspectionProcedureStepResult(6, m_MeasurePoint2,  m_Testtime2, m_FieldNo2, m_ObjectName,  m_ObjectId, m_MeasurePointId, m_TextEmpty, string.Empty,string.Empty));
            Assert.AreEqual(m_FieldNo2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(5).FieldNo);
            Assert.AreEqual(m_MeasurePoint2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(5).MeasurePoint);
            Assert.AreEqual(m_MeasurePointId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(5).MeasurePointID);
            Assert.AreEqual(m_ObjectId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(5).ObjectID);
            Assert.AreEqual(m_ObjectName, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(5).ObjectName);
            Assert.AreEqual(m_TextEmpty, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(5).Text, "Xsd validation should succee");

            reportControl.UpdateRemark(6, m_TextRemark);
            Assert.AreEqual(m_TextRemark, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(5).Text, "Xsd validation should succee");

            for (int i = 0; i < m_Lists.Count; i++)
            {
                Assert.AreEqual(m_Lists.ElementAt(i), reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).List.ElementAt(i));
            }
            Assert.AreEqual(m_Testtime2.ToString("HH:mm:ss"), reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(1).Time, "");

            reportControl.FinishInspectionReport(InspectionStatus.Completed, end);
            Assert.AreEqual(end.ToString("HH:mm:ss"), reportControl.InspectionReport.InspectionResults.First().DateTimeStamp.EndTime);
            Assert.AreEqual(3, reportControl.InspectionReport.InspectionResults.First().Status);
            Assert.IsTrue(File.Exists(xmlFileLocation), "Validating Result File is created.");
            Assert.IsFalse(File.Exists(xmlTempFileLocation), "Validating TmpResult file is removed.");
        }

        /// <summary>
        /// Tests that adding duplicate results, leads to the last being stored.
        /// </summary>
        [Test]
        public void ReportControlDuplicateResultTest()
        {
            LogHelper.DebugExtended = true;

            ReportControl reportControl = new ReportControl();
            string xmlTempFileLocation = Path.Combine(SettingsUtils.LookupTemporaryPath(), ReportConstants.TMPRESULTS_FILENAME);
            string xmlFileLocation = Path.Combine(SettingsUtils.LookupXmlFilePath(), ReportConstants.INSPECTOR_RESULTS_FILENAME);

            if (File.Exists(xmlTempFileLocation))
            {
                File.Delete(xmlTempFileLocation);
            }

            DateTime start = new DateTime(2012, 3, 29, 16, 19, 00);
            DateTime end = new DateTime(2012, 3, 29, 16, 20, 00);

            // Start Inspection
            reportControl.StartInspectionReport(InspectionStatus.StartNotCompleted, start);
            Assert.AreEqual("2012-03-29", reportControl.InspectionReport.InspectionResults.First().DateTimeStamp.StartDate, "StartInspectionReport");
            Assert.AreEqual("16:19:00", reportControl.InspectionReport.InspectionResults.First().DateTimeStamp.StartTime, "StartInspectionReport");
            Assert.AreEqual(2, reportControl.InspectionReport.InspectionResults.First().Status, "StartInspectionReport");
            Assert.IsFalse(File.Exists(xmlTempFileLocation), "Validating XML File is not created.");

            // Update the Status
            reportControl.UpdateStatus(InspectionStatus.GclOrPrsDeletedByUser);
            Assert.AreEqual(4, reportControl.InspectionReport.InspectionResults.First().Status, "UpdateStatus");

            // Add an Inspection Procedure
            reportControl.AddInspectionProcedure(new InspectionProcedureGenericInformation
                (
                InspectionStatus.NoInspection, m_PrsIdentification, m_PrsName, m_PrsCode, m_GasControlLineName, m_GclIdentification, m_GCLCode, m_CRC, m_InspectionProcedureName, m_InspectionProcedureVersion)
                );
            Assert.AreEqual(m_PrsIdentification, reportControl.InspectionReport.InspectionResults.First().PRSIdentification, "PRSIdentification");
            Assert.AreEqual(m_PrsName, reportControl.InspectionReport.InspectionResults.First().PRSName, "PRSName");
            Assert.AreEqual(m_PrsCode, reportControl.InspectionReport.InspectionResults.First().PRSCode, "PRSCode");
            Assert.AreEqual(m_GasControlLineName, reportControl.InspectionReport.InspectionResults.First().GasControlLineName, "GasControlLineName");
            Assert.AreEqual(m_GclIdentification, reportControl.InspectionReport.InspectionResults.First().GCLIdentification, "GCLIdentification");
            Assert.AreEqual(m_GCLCode, reportControl.InspectionReport.InspectionResults.First().GCLCode, "GCLCode");
            Assert.AreEqual(m_CRC, reportControl.InspectionReport.InspectionResults.First().CRC, "CRC");
            Assert.AreEqual(m_InspectionProcedureName, reportControl.InspectionReport.InspectionResults.First().InspectionProcedure.Name, "Name");
            Assert.AreEqual(m_InspectionProcedureVersion, reportControl.InspectionReport.InspectionResults.First().InspectionProcedure.Version, "Version");
            Assert.AreEqual(1, reportControl.InspectionReport.InspectionResults.First().Status, "UpdateStatus");

            // Add Manometer Information
            reportControl.AddManometerIdentification(MeterNumber.ID_DM1, m_DM1);
            Assert.AreEqual(m_DM1, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM1);
            Assert.AreEqual(string.Empty, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM2);

            reportControl.AddManometerIdentification(MeterNumber.ID_DM2, m_DM2);
            reportControl.AddManometerIdentification(MeterNumber.ID_DM1, m_DM1);
            Assert.AreEqual(m_DM1, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM1);
            Assert.AreEqual(m_DM2, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.ID_DM2);
            Assert.AreEqual(string.Empty, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.BT_Address);

            // Add Bluetooth Address
            reportControl.AddBluetoothAddress(m_BT_Address);
            Assert.AreEqual(m_BT_Address, reportControl.InspectionReport.InspectionResults.First().Measurement_Equipment.BT_Address);
            Assert.IsFalse(File.Exists(xmlTempFileLocation), "Validating XML File is not created.");

            // Add a result, this time with all required fields using UOM dm3/h
            InspectionProcedureStepResultMeasureValue measureValue = new InspectionProcedureStepResultMeasureValue(m_MeasureValue, m_UnitOfMeasureMentDm3h);
            reportControl.AddResult(new InspectionProcedureStepResult(2, m_MeasurePoint, m_Testtime1, m_FieldNo1, m_ObjectName, m_ObjectId, m_MeasurePointId, m_TextwithInvalidChars, string.Empty, string.Empty, m_Lists, measureValue));
            Assert.AreEqual(1, reportControl.InspectionReport.InspectionResults.Count);
            Assert.AreEqual(m_FieldNo1, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).FieldNo);
            Assert.AreEqual(m_MeasurePoint, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasurePoint);
            Assert.AreEqual(m_MeasurePointId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasurePointID);
            Assert.AreEqual("dm3/h", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasureValue.UOM.GetDescription(), "Cast");
            Assert.AreEqual(m_MeasureValue, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasureValue.Value, "MeasureValue is nullable");
            Assert.AreEqual(m_ObjectId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).ObjectID);
            Assert.AreEqual(m_ObjectName, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).ObjectName);
            Assert.AreEqual(m_TextwithInvalidChars, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).Text, "Text contains XML invalid chars, xsd validation should succee");

            // Add another result with the same sequence number, but different info
            measureValue = new InspectionProcedureStepResultMeasureValue(m_MeasureValue2, m_UnitOfMeasureMentBar);
            reportControl.AddResult(new InspectionProcedureStepResult(2, m_MeasurePoint2, m_Testtime2, m_FieldNo2, m_ObjectName, m_ObjectId, m_MeasurePointId, m_TextwithInvalidChars, string.Empty, string.Empty, m_Lists, measureValue));
            Assert.AreEqual(1, reportControl.InspectionReport.InspectionResults.Count);
            Assert.AreEqual(m_FieldNo2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).FieldNo);
            Assert.AreEqual(m_MeasurePoint2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasurePoint);
            Assert.AreEqual(m_MeasurePointId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasurePointID);
            Assert.AreEqual("bar", reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasureValue.UOM.GetDescription(), "Cast");
            Assert.AreEqual(m_MeasureValue2, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).MeasureValue.Value, "MeasureValue is nullable");
            Assert.AreEqual(m_ObjectId, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).ObjectID);
            Assert.AreEqual(m_ObjectName, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).ObjectName);
            Assert.AreEqual(m_TextwithInvalidChars, reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).Text, "Text contains XML invalid chars, xsd validation should succee");

            Assert.AreEqual(m_Testtime2.ToString("HH:mm:ss"), reportControl.InspectionReport.InspectionResults.First().Results.ElementAt(0).Time, "");

            reportControl.FinishInspectionReport(InspectionStatus.Completed, end);
            Assert.AreEqual(end.ToString("HH:mm:ss"), reportControl.InspectionReport.InspectionResults.First().DateTimeStamp.EndTime);
            Assert.AreEqual(3, (int)reportControl.InspectionReport.InspectionResults.First().Status);
            Assert.IsTrue(File.Exists(xmlFileLocation), "Validating Result File is created.");
            Assert.IsFalse(File.Exists(xmlTempFileLocation), "Validating TmpResult file is removed.");
        }

        /// <summary>
        /// Adds the temporary file to result test.
        /// </summary>
        [Test]
        public void AddTemporaryFileToResultTest()
        {
            ReportControl reportControl = new ReportControl();
            reportControl.AddTemporaryFileToResult();
        }

        /// <summary>
        /// Starts the inspection report test.
        /// </summary>
        [Test]
        public void StartInspectionReportTest()
        {
            ReportControl reportControl = new ReportControl();
            reportControl.StartInspectionReport(InspectionStatus.NoInspection, DateTime.Now);
        }

        /// <summary>
        /// Adds the inspection procedure test.
        /// </summary>
        [Test]
        public void AddInspectionProcedureTest()
        {
            ReportControl reportControl = new ReportControl();
            try
            {
                InspectionProcedureGenericInformation procedure = new InspectionProcedureGenericInformation();
                reportControl.AddInspectionProcedure(procedure);
            }
            catch (InspectorReportControlException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("unexpected exception");
            }
        }

        /// <summary>
        /// Adds the manometer identification test.
        /// </summary>
        [Test]
        public void AddManometerIdentificationTest()
        {
            ReportControl reportControl = new ReportControl();
            try
            {
                reportControl.AddManometerIdentification(MeterNumber.ID_DM1, "");
            }
            catch (InspectorReportControlException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("unexpected exception");
            }
        }

        /// <summary>
        /// Adds the bluetooth address test.
        /// </summary>
        [Test]
        public void AddBluetoothAddressTest()
        {
            try
            {
                ReportControl reportControl = new ReportControl();
                reportControl.AddBluetoothAddress("123");
            }
            catch (InspectorReportControlException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("unexpected exception");
            }
        }

        /// <summary>
        /// Adds the result test.
        /// </summary>
        [Test]
        public void AddResultTest()
        {
            ReportControl reportControl = new ReportControl();
            InspectionProcedureStepResult result = new InspectionProcedureStepResult();
            try
            {
                reportControl.AddResult(result);
            }
            catch (InspectorReportControlException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("unexpected exception");
            }
        }

        /// <summary>
        /// Updates the status test.
        /// </summary>
        [Test]
        public void UpdateStatusTest()
        {
            ReportControl reportControl = new ReportControl();
            try
            {
                reportControl.UpdateStatus(InspectionStatus.NoInspection);
            }
            catch (InspectorReportControlException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("unexpected exception");
            }
        }

        /// <summary>
        /// Finishes the inspection report test.
        /// </summary>
        [Test]
        public void FinishInspectionReportTest()
        {
            ReportControl reportControl = new ReportControl();
            try
            {
                reportControl.FinishInspectionReport(InspectionStatus.NoInspection, DateTime.Now);
            }
            catch (InspectorReportControlException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("unexpected exception");
            }
        }
    }
}
