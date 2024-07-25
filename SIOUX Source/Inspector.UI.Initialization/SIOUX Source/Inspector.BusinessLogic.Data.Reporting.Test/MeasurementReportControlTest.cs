using System;
using System.Collections.Generic;
using System.Threading;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Data.Reporting.Measurements;
using Inspector.BusinessLogic.Data.Reporting.Measurements.Model;
using Inspector.Model.MeasurementResult;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Reporting.Test
{
    [TestFixture]
    public class MeasurementReportControlTest
    {
        private static InspectionProcedureMetadata CreateInspectionProcedureMetaData()
        {
            InspectionProcedureMetadata inspectionProcedureMetadata = new InspectionProcedureMetadata()
            {
                PlexorName = "PlexorName",
                PlexorBtAddress = "PlexorBtAddress",
                TH1SerialNumber = "TH1SerialNumber",
                TH2SerialNumber = "TH2SerialNumber",
                Station = "Station",
                StationCode = "StationCode",
                GasControlLine = "GasControlLine",
                GasControlLineIdentificationCode = "GasControlLineIdentificationCode",
                TestProgram = "TestProgram",
                InspectionProcedureVersion = "InspectionProcedureVersion",
                InspectorVersion = "InspectorVersion",
                FSDStart = "FSDStart",
            };
            return inspectionProcedureMetadata;
        }

        private static MeasurementMetadata CreateMeasurementMetadata()
        {
            MeasurementMetadata measurementMetadata = new MeasurementMetadata()
            {
                ScriptCommand = "ScriptCommand",
                EndOfMeasurement = new DateTime(2012, 1, 2, 3, 4, 7),
                Interval = 10,
                FieldInAccessDatabase = "FieldInAccessDatabase",
                ObjectName = "ObjectName",
                Measurepoint = "Measurepoint",
                Value = "Value",
            };
            return measurementMetadata;
        }

        [Test]
        public void MeasurementReportTest()
        {
            MeasurementReportControl measurementReportControl = new MeasurementReportControl();
            InspectionProcedureMetadata inspectionProcedureMetadata = CreateInspectionProcedureMetaData();
            MeasurementMetadata measurementMetadata = CreateMeasurementMetadata();
            List<double> queue1 = new List<double>() { 1.1, 1.2, 1.3, 1.4 };
            List<double> queue2 = new List<double>() { 2.1, 2.2, 2.3, 2.4 };
            List<double> queue3 = new List<double>() { 3.1, 3.2, 3.3, 3.4 };
            List<double> queue4 = new List<double>() { 4.1, 4.2, 4.3, 4.4 };
            List<double> queue5 = new List<double>() { 5.1, 5.2, 5.3, 5.4 };

            Thread measurementThread = new Thread(() => measurementReportControl.MeasurementDataWorkerThread());
            measurementThread.Name = "measurementThread";
            measurementThread.Start();

            measurementReportControl.InitializeMeasurementReport(@"prs\123", @"gcl>456", new DateTime(2012, 01, 02, 03, 04, 05));
            measurementReportControl.AddInspectionProcedureMetadata(inspectionProcedureMetadata);

            measurementReportControl.SetUpMeasurementFileWhenRequired();
            measurementReportControl.StartMeasurement("mbar", DateTime.Now);

            measurementReportControl.MeasurementsReceived(queue1);
            measurementReportControl.MeasurementsReceived(queue2);
            measurementReportControl.MeasurementsReceived(queue3);
            measurementReportControl.MeasurementsReceived(queue4);

            measurementReportControl.StartExtraDataMeasurement();

            measurementReportControl.MeasurementsReceived(queue5);

            measurementReportControl.AddMeasurementMetadata(measurementMetadata);

            Assert.IsNotNull(measurementReportControl.MeasurementReport, "MeasurementReport should be filled");
            Assert.AreEqual("prs123", measurementReportControl.MeasurementReport.PrsName);
            Assert.AreEqual("gcl456", measurementReportControl.MeasurementReport.GclName);
            Assert.AreEqual("1201020304", measurementReportControl.MeasurementReport.StartDateTime);

            Assert.IsNotNull(measurementReportControl.MeasurementReport.FileHeader, "Fileheader should be filled");
            Assert.AreEqual("PlexorName", measurementReportControl.MeasurementReport.FileHeader.PlexorName);
            Assert.AreEqual("2-1-2012", measurementReportControl.MeasurementReport.FileHeader.StartDate);
            Assert.AreEqual("3:04:05", measurementReportControl.MeasurementReport.FileHeader.StartTime);

            Assert.IsNotNull(measurementReportControl.MeasurementReport.Measurements, "Measurements should be filled");
            Assert.AreEqual(1, measurementReportControl.MeasurementReport.Measurements.Count);

            Measurement firstMeasurement = measurementReportControl.MeasurementReport.Measurements[0];

            Assert.AreEqual("ScriptCommand", firstMeasurement.DataHeader.ScriptCommand);
            Assert.AreEqual("2-1-2012 3:04:07", firstMeasurement.DataHeader.EndOfMeasurement);

            Assert.IsNotNull(firstMeasurement.Data, "Data should be filled");
            Assert.AreEqual(16, firstMeasurement.Data.MeasurementValues.Count);
            Assert.AreEqual(1.1, firstMeasurement.Data.MeasurementValues[0]);
            Assert.AreEqual(1.2, firstMeasurement.Data.MeasurementValues[1]);
            Assert.AreEqual(1.3, firstMeasurement.Data.MeasurementValues[2]);
            Assert.AreEqual(1.4, firstMeasurement.Data.MeasurementValues[3]);
            Assert.AreEqual(2.1, firstMeasurement.Data.MeasurementValues[4]);
            Assert.AreEqual(2.2, firstMeasurement.Data.MeasurementValues[5]);
            Assert.AreEqual(2.3, firstMeasurement.Data.MeasurementValues[6]);
            Assert.AreEqual(2.4, firstMeasurement.Data.MeasurementValues[7]);
            Assert.AreEqual(3.1, firstMeasurement.Data.MeasurementValues[8]);
            Assert.AreEqual(3.2, firstMeasurement.Data.MeasurementValues[9]);
            Assert.AreEqual(3.3, firstMeasurement.Data.MeasurementValues[10]);
            Assert.AreEqual(3.4, firstMeasurement.Data.MeasurementValues[11]);
            Assert.AreEqual(4.1, firstMeasurement.Data.MeasurementValues[12]);
            Assert.AreEqual(4.2, firstMeasurement.Data.MeasurementValues[13]);
            Assert.AreEqual(4.3, firstMeasurement.Data.MeasurementValues[14]);
            Assert.AreEqual(4.4, firstMeasurement.Data.MeasurementValues[15]);

            Assert.IsNotNull(firstMeasurement.ExtraData, "Extra data should be filled");
            Assert.AreEqual(4, firstMeasurement.ExtraData.MeasurementValues.Count);
            Assert.AreEqual(5.1, firstMeasurement.ExtraData.MeasurementValues[0]);
            Assert.AreEqual(5.2, firstMeasurement.ExtraData.MeasurementValues[1]);
            Assert.AreEqual(5.3, firstMeasurement.ExtraData.MeasurementValues[2]);
            Assert.AreEqual(5.4, firstMeasurement.ExtraData.MeasurementValues[3]);

            Assert.IsNotNull(firstMeasurement.DataHeader, "DataHeader should be filled");
            Assert.AreEqual(20, firstMeasurement.DataHeader.CountTotal);
            Assert.AreEqual("FieldInAccessDatabase", firstMeasurement.DataHeader.FieldInAccessDatabase);

            measurementReportControl.FinishMeasurementReport();
        }

        [Test]
        public void MeasurementReportNotInitializedTest()
        {
            MeasurementReportControlException exception;
            MeasurementReportControl measurementReportControl = new MeasurementReportControl();
            InspectionProcedureMetadata inspectionProcedureMetadata = CreateInspectionProcedureMetaData();
            MeasurementMetadata measurementMetadata = CreateMeasurementMetadata();

            exception = Assert.Throws<MeasurementReportControlException>(() => measurementReportControl.AddInspectionProcedureMetadata(inspectionProcedureMetadata));
            Assert.AreEqual("Error adding procedure metadata (fileHeader)", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");

            exception = Assert.Throws<MeasurementReportControlException>(() => measurementReportControl.StartMeasurement("mbar", DateTime.Now));
            Assert.AreEqual("Error adding measurement", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");

            exception = Assert.Throws<MeasurementReportControlException>(() => measurementReportControl.StartExtraDataMeasurement());
            Assert.AreEqual("Error adding Extra Data", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");

            exception = Assert.Throws<MeasurementReportControlException>(() => measurementReportControl.AddMeasurementMetadata(measurementMetadata));
            Assert.AreEqual("Error adding measurement metadata (dataHeader)", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");
        }
    }
}

