/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Inspector.BusinessLogic.Data.Reporting.Results;
using Inspector.BusinessLogic.Data.Reporting.Results.Model;
using Inspector.Infra.Utils;
using Inspector.Model;
using Inspector.Model.InspectionProcedure;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Reporting.Test
{
    public class ReportControlWriterTest
    {


        #region TestData

        private string m_PrsIdentification = "PRS ID";
        private string m_PrsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
        private string m_PrsCode = "PDASNR=0D9015E4830E0";
        private string m_GasControlLineName = "5 007 230.1.1 OS LS";
        private string m_GclIdentification = "23.05.2012 5 007 230.1.1";
        private string m_GCLCode = "GCL Code";
        private string m_CRC = "CRC";

        private string m_DM1 = "HM3500DLM110,MOD00B,1048113";
        private string m_DM2 = string.Empty;
        private string m_BT_Address = "00:80:98:C4:20:F4";

        DateTime m_StartTime = new DateTime(2012, 2, 6, 15, 58, 52);
        DateTime m_EndTime = new DateTime(2012, 2, 6, 16, 17, 30);

        private static List<string> m_Listexample = new List<string>(new string[] { "1", "2", "3", "4", "5" });
        Result m_ResultMaximum = new Result(1, "ObjectNameMAX", "ObjectIdMAX", "msrPointMAX", "msrPointIdMAX", 1, new TimeSpan(12, 5, 30), new MeasureValue(0.0002, UnitOfMeasurement.ItemDm3h), "Text Example MAX", m_Listexample);

        private string m_PrsIdentification2 = "PRS ID2";
        private string m_PrsName2 = "PRS Name2";
        private string m_PrsCode2 = "PRS Code2";
        private string m_GasControlLineName2 = "GasControlLineName2";
        private string m_GclIdentification2 = "GCL ID2";
        private string m_GCLCode2 = "GCL Code2";
        private string m_CRC2 = "CRC2";

        private string m_InspectionName2 = "Inspect Name2";
        private string m_InspectionVersion2 = "2.0.0";

        private InspectionStatus m_InspectionStatus2 = InspectionStatus.StartNotCompleted;


        DateTime m_StartTime2 = new DateTime(2012, 3, 26, 16, 30, 0);
        DateTime m_EndTime2 = new DateTime(2012, 3, 26, 17, 00, 59);

        #endregion TestData

        DirectoryInfo m_WorkingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        private string m_XmlTargetFileName = Path.Combine(SettingsUtils.LookupXmlFilePath(), "InspectionResultFromUnitTest.xml");

        private string m_ResultsXmlFileFromClient = Path.Combine(SettingsUtils.LookupXmlFilePath(), "Results.xml");
        private string m_ResultsXmlFileFromSerialazation = Path.Combine(SettingsUtils.LookupXmlFilePath(), "InspectionResultFromSerialazation.xml");
        private string m_ResultsDataXsdFile = Path.Combine(SettingsUtils.LookupXsdFilePath(), "InspectionResultsData.xsd");

        private string m_InspectionStatusXsd = Path.Combine(SettingsUtils.LookupXsdFilePath(), "InspectionStatus.xsd");
        private string m_InspectionStatusXml = Path.Combine(SettingsUtils.LookupXmlFilePath(), "InspectionStatus.xml");

        private string m_InspectionProcedureXsd = Path.Combine(SettingsUtils.LookupXsdFilePath(), "InspectionProcedure.xsd");
        private string m_InspectionProcedureXml = Path.Combine(SettingsUtils.LookupXmlFilePath(), "InspectionProcedure.xml");

        private string m_InspectorInfoXsd = Path.Combine(SettingsUtils.LookupXsdFilePath(), "InspectorInfo.xsd");
        private string m_InspectorInfoXml = Path.Combine(SettingsUtils.LookupXmlFilePath(), "InspectorInfo.xml");

        private string m_ObjectInformationXsd = Path.Combine(SettingsUtils.LookupXsdFilePath(), "ObjectInformation.xsd");
        private string m_ObjectInformationXml = Path.Combine(SettingsUtils.LookupXmlFilePath(), "ObjectInformation.xml");

        private string m_PlexorXsd = Path.Combine(SettingsUtils.LookupXsdFilePath(), "Plexor.xsd");
        private string m_PlexorXml = Path.Combine(SettingsUtils.LookupXmlFilePath(), "Plexor.xml");

        private string m_StationInformationXsd = Path.Combine(SettingsUtils.LookupXsdFilePath(), "StationInformation.xsd");
        private string m_StationInformationXml = Path.Combine(SettingsUtils.LookupXmlFilePath(), "StationInformation.xml");

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportControlWriterTest"/> class.
        /// </summary>
        public ReportControlWriterTest()
        {
        }


        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestFixtureSetUp]
        public void setup()
        {

        }

        /// <summary>
        /// Teardowns this instance.
        /// </summary>
        [TestFixtureTearDown]
        public void Teardown()
        {
        }

        /// <summary>
        /// Removes the test data.
        /// </summary>
        private void RemoveTestData()
        {
            string targetFile = m_WorkingDirectory.FullName + m_XmlTargetFileName;

            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
        }


        /// <summary>
        /// Gets the report test object.
        /// </summary>
        /// <returns></returns>
        private InspectionReport GetReportTestObject()
        {
            InspectionReport testObject = new InspectionReport();

            MeasurementEquipment msEquipment = new MeasurementEquipment(m_DM1, m_DM2, m_BT_Address);
            InspectionProcedure isProcedure = new InspectionProcedure(m_InspectionName2, m_InspectionVersion2);
            DateTimeStamp dtStamp = new DateTimeStamp(m_StartTime, m_EndTime);
            DateTimeStamp dtStamp2 = new DateTimeStamp(m_StartTime, m_EndTime);

            Result resultMinimum = new Result(1, "ObjectName", "ObjectId", "msrPoint", "msrPointId", new TimeSpan(12, 5, 30));

            testObject.InspectionResults.Add(new InspectionResult(InspectionStatus.CompletedValueOutOfLimits, m_PrsIdentification, m_PrsName, m_PrsCode, m_GasControlLineName, m_GclIdentification, m_GCLCode, m_CRC, msEquipment, isProcedure, dtStamp));

            testObject.InspectionResults.ElementAt(0).Results.Add(resultMinimum);
            testObject.InspectionResults.ElementAt(0).Results.Add(m_ResultMaximum);

            testObject.InspectionResults.Add(new InspectionResult(m_InspectionStatus2, m_PrsIdentification2, m_PrsName2, m_PrsCode2, m_GasControlLineName2, m_GclIdentification2, m_GCLCode2, m_CRC2, msEquipment, isProcedure, dtStamp2));
            testObject.InspectionResults.ElementAt(1).Results.Add(m_ResultMaximum);

            testObject.InspectionResults.Add(new InspectionResult(m_InspectionStatus2, m_PrsIdentification2, m_PrsName2, m_PrsCode2, m_CRC2, isProcedure, dtStamp2));
            testObject.InspectionResults.ElementAt(2).Results.Add(resultMinimum);
            testObject.InspectionResults.ElementAt(2).Results.Add(m_ResultMaximum);

            return testObject;
        }


        /// <summary>
        /// Validates the original file test.
        /// </summary>
        [Test]
        public void ValidateClientFiles()
        {
            XMLUtils.ValidateXmlFile(m_ResultsXmlFileFromClient, m_ResultsDataXsdFile);
            XMLUtils.ValidateXmlFile(m_ResultsXmlFileFromSerialazation, m_ResultsDataXsdFile);
            XMLUtils.ValidateXmlFile(m_InspectionStatusXml, m_InspectionStatusXsd);
            XMLUtils.ValidateXmlFile(m_InspectionProcedureXml, m_InspectionProcedureXsd);
            XMLUtils.ValidateXmlFile(m_ObjectInformationXml, m_ObjectInformationXsd);
            XMLUtils.ValidateXmlFile(m_PlexorXml, m_PlexorXsd);
            XMLUtils.ValidateXmlFile(m_StationInformationXml, m_StationInformationXsd);
        }

        /// <summary>
        /// Saves to XML test.
        /// </summary>
        [Test]
        public void SaveToXmlTest()
        {
            try
            {
                RemoveTestData();

                ReportControl rptCtrl = new ReportControl();
                rptCtrl.InspectionReport = GetReportTestObject();

                rptCtrl.WriteResultFile(rptCtrl.InspectionReport, m_XmlTargetFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Saves to XML empty object.
        /// </summary>
        [Test]
        public void SaveToXmlEmptyObject()
        {
            try
            {
                RemoveTestData();

                ReportControl rptCtrl = new ReportControl();

                rptCtrl.InspectionReport.InspectionResults.Add(new InspectionResult(DateTime.Now));
                rptCtrl.InspectionReport.InspectionResults.First().Results.Add(new Result(TimeSpan.FromMinutes(DateTime.Now.Minute)));

                rptCtrl.WriteResultFile(rptCtrl.InspectionReport, m_XmlTargetFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Des the serialize test.
        /// </summary>
        [Test]
        public void DeSerializeTest()
        {
            RemoveTestData();

            ReportControl reportControlSerializer = new ReportControl();
            reportControlSerializer.InspectionReport = GetReportTestObject();

            reportControlSerializer.InspectionReport.InspectionResults.RemoveAt(2);
            reportControlSerializer.InspectionReport.InspectionResults.RemoveAt(0);

            reportControlSerializer.WriteResultFile(reportControlSerializer.InspectionReport, m_XmlTargetFileName);

            ReportControl reportControlDesrializer = new ReportControl();
            reportControlDesrializer.InspectionReport = ReportControl.ReadResultFile(m_XmlTargetFileName);

            InspectionResult deserializedReport = reportControlDesrializer.InspectionReport.InspectionResults.First();

            Assert.AreEqual(m_CRC2, deserializedReport.CRC);
            Assert.AreEqual("2012-02-06", deserializedReport.DateTimeStamp.StartDate);
            Assert.AreEqual("15:58:52", deserializedReport.DateTimeStamp.StartTime);
            Assert.AreEqual("16:17:30", deserializedReport.DateTimeStamp.EndTime);
            Assert.AreEqual("No", deserializedReport.DateTimeStamp.TimeSettings.DST);
            Assert.AreEqual("GMT+01:00", deserializedReport.DateTimeStamp.TimeSettings.TimeZone);
            Assert.AreEqual(m_GasControlLineName2, deserializedReport.GasControlLineName);
            Assert.AreEqual(m_GCLCode2, deserializedReport.GCLCode);
            Assert.AreEqual(m_GclIdentification2, deserializedReport.GCLIdentification);
            Assert.AreEqual(m_InspectionName2, deserializedReport.InspectionProcedure.Name);
            Assert.AreEqual(m_InspectionVersion2, deserializedReport.InspectionProcedure.Version);
            Assert.AreEqual(m_BT_Address, deserializedReport.Measurement_Equipment.BT_Address);
            Assert.AreEqual(m_DM1, deserializedReport.Measurement_Equipment.ID_DM1);
            Assert.AreEqual(m_DM2, deserializedReport.Measurement_Equipment.ID_DM2);
            Assert.AreEqual(m_PrsCode2, deserializedReport.PRSCode);
            Assert.AreEqual(m_PrsIdentification2, deserializedReport.PRSIdentification);
            Assert.AreEqual(m_PrsName2, deserializedReport.PRSName);
            Assert.AreEqual(m_ResultMaximum.MeasurePoint, deserializedReport.Results.First().MeasurePoint);
            Assert.AreEqual(m_ResultMaximum.MeasurePointID, deserializedReport.Results.First().MeasurePointID);
            Assert.AreEqual(m_ResultMaximum.MeasureValue.UOM, deserializedReport.Results.First().MeasureValue.UOM);
            Assert.AreEqual(m_ResultMaximum.MeasureValue.Value, deserializedReport.Results.First().MeasureValue.Value);
            Assert.AreEqual(m_ResultMaximum.ObjectID, deserializedReport.Results.First().ObjectID);
            Assert.AreEqual(m_ResultMaximum.ObjectName, deserializedReport.Results.First().ObjectName);
            Assert.AreEqual(m_ResultMaximum.Text, deserializedReport.Results.First().Text);
            Assert.AreEqual(m_ResultMaximum.Time, deserializedReport.Results.First().Time);
            Assert.AreEqual(m_Listexample.Count, deserializedReport.Results.First().List.Count);
            Assert.AreEqual(m_Listexample.First(), deserializedReport.Results.First().List.First());
            Assert.AreEqual(m_Listexample.Last(), deserializedReport.Results.First().List.Last());

            Assert.AreEqual((int)m_InspectionStatus2, deserializedReport.Status);
        }
    }
}
