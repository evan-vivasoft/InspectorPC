using System;
using System.Collections.Generic;
using System.IO;
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
        [SetUp]
        public void Setup()
        {
            var path = Path.GetDirectoryName(typeof(MeasurementReportControlTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);
        }

        [TearDown]
        public void TearDown()
        {
        }

        private static InspectionProcedureMetadata CreateInspectionProcedureMetaData()
        {
            var inspectionProcedureMetadata = new InspectionProcedureMetadata
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
            var measurementMetadata = new MeasurementMetadata
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
            var measurementReportControl = new MeasurementReportControl();
            var inspectionProcedureMetadata = CreateInspectionProcedureMetaData();
            var measurementMetadata = CreateMeasurementMetadata();
            var queue1 = new List<Model.Measurement> { new Model.Measurement(1.1, 0), new Model.Measurement(1.2,0), new Model.Measurement(1.3,0), new Model.Measurement(1.4,0) };
            var queue2 = new List<Model.Measurement> { new Model.Measurement(2.1, 0), new Model.Measurement(2.2, 0), new Model.Measurement(2.3, 0), new Model.Measurement(2.4, 0) };
            var queue3 = new List<Model.Measurement> { new Model.Measurement(3.1, 0), new Model.Measurement(3.2, 0), new Model.Measurement(3.3, 0), new Model.Measurement(3.4, 0) };
            var queue4 = new List<Model.Measurement> { new Model.Measurement(4.1, 0), new Model.Measurement(4.2, 0), new Model.Measurement(4.3, 0), new Model.Measurement(4.4, 0) };
            var queue5 = new List<Model.Measurement> { new Model.Measurement(5.1, 0), new Model.Measurement(5.2, 0), new Model.Measurement(5.3, 0), new Model.Measurement(5.4, 0) };

            var measurementThread = new Thread(measurementReportControl.MeasurementDataWorkerThread)
            {
                Name = "measurementThread"
            };
            measurementThread.Start();

            measurementReportControl.InitializeMeasurementReport(@"prs\123", @"gcl>456", new DateTime(2012, 01, 02, 03, 04, 05));
            measurementReportControl.AddInspectionProcedureMetadata(inspectionProcedureMetadata);

            measurementReportControl.SetUpMeasurementFileWhenRequired();
            measurementReportControl.StartMeasurement("mbar", DateTime.Now, false);

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
            Assert.AreEqual("02-01-2012", measurementReportControl.MeasurementReport.FileHeader.StartDate);
            Assert.AreEqual("03:04:05", measurementReportControl.MeasurementReport.FileHeader.StartTime);

            Assert.IsNotNull(measurementReportControl.MeasurementReport.Measurements, "Measurements should be filled");
            Assert.AreEqual(1, measurementReportControl.MeasurementReport.Measurements.Count);

            Measurement firstMeasurement = measurementReportControl.MeasurementReport.Measurements[0];

            Assert.AreEqual("ScriptCommand", firstMeasurement.DataHeader.ScriptCommand);
            Assert.AreEqual("02-01-2012 03:04:07", firstMeasurement.DataHeader.EndOfMeasurement);

            Assert.IsNotNull(firstMeasurement.Data, "Data should be filled");
            Assert.AreEqual(16, firstMeasurement.Data.MeasurementValues.Count);
            Assert.AreEqual(1.1, firstMeasurement.Data.MeasurementValues[0].Value);
            Assert.AreEqual(1.2, firstMeasurement.Data.MeasurementValues[1].Value);
            Assert.AreEqual(1.3, firstMeasurement.Data.MeasurementValues[2].Value);
            Assert.AreEqual(1.4, firstMeasurement.Data.MeasurementValues[3].Value);
            Assert.AreEqual(2.1, firstMeasurement.Data.MeasurementValues[4].Value);
            Assert.AreEqual(2.2, firstMeasurement.Data.MeasurementValues[5].Value);
            Assert.AreEqual(2.3, firstMeasurement.Data.MeasurementValues[6].Value);
            Assert.AreEqual(2.4, firstMeasurement.Data.MeasurementValues[7].Value);
            Assert.AreEqual(3.1, firstMeasurement.Data.MeasurementValues[8].Value);
            Assert.AreEqual(3.2, firstMeasurement.Data.MeasurementValues[9].Value);
            Assert.AreEqual(3.3, firstMeasurement.Data.MeasurementValues[10].Value);
            Assert.AreEqual(3.4, firstMeasurement.Data.MeasurementValues[11].Value);
            Assert.AreEqual(4.1, firstMeasurement.Data.MeasurementValues[12].Value);
            Assert.AreEqual(4.2, firstMeasurement.Data.MeasurementValues[13].Value);
            Assert.AreEqual(4.3, firstMeasurement.Data.MeasurementValues[14].Value);
            Assert.AreEqual(4.4, firstMeasurement.Data.MeasurementValues[15].Value);

            Assert.IsNotNull(firstMeasurement.Data.ExtraMeasurementValues, "Extra data should be filled");
            Assert.AreEqual(4, firstMeasurement.Data.ExtraMeasurementValues.Count);
            Assert.AreEqual(5.1, firstMeasurement.Data.ExtraMeasurementValues[0].Value);
            Assert.AreEqual(5.2, firstMeasurement.Data.ExtraMeasurementValues[1].Value);
            Assert.AreEqual(5.3, firstMeasurement.Data.ExtraMeasurementValues[2].Value);
            Assert.AreEqual(5.4, firstMeasurement.Data.ExtraMeasurementValues[3].Value);

            Assert.IsNotNull(firstMeasurement.DataHeader, "DataHeader should be filled");
            Assert.AreEqual(20, firstMeasurement.DataHeader.CountTotal);
            Assert.AreEqual("FieldInAccessDatabase", firstMeasurement.DataHeader.FieldInAccessDatabase);

            measurementReportControl.FinishMeasurementReport();
        }

        [Test]
        public void MeasurementReportNotInitializedTest()
        {
            var measurementReportControl = new MeasurementReportControl();
            var inspectionProcedureMetadata = CreateInspectionProcedureMetaData();
            var measurementMetadata = CreateMeasurementMetadata();

            var exception = Assert.Throws<MeasurementReportControlException>(() => measurementReportControl.AddInspectionProcedureMetadata(inspectionProcedureMetadata));
            Assert.AreEqual("Error adding procedure metadata (fileHeader)", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");

            exception = Assert.Throws<MeasurementReportControlException>(() => measurementReportControl.StartMeasurement("mbar", DateTime.Now, false));
            Assert.AreEqual("Error adding measurement", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");

            exception = Assert.Throws<MeasurementReportControlException>(measurementReportControl.StartExtraDataMeasurement);
            Assert.AreEqual("Error adding Extra Data", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");

            exception = Assert.Throws<MeasurementReportControlException>(() => measurementReportControl.AddMeasurementMetadata(measurementMetadata));
            Assert.AreEqual("Error adding measurement metadata (dataHeader)", exception.Message);

            Assert.IsNull(measurementReportControl.MeasurementReport, "MeasurementReport should be null");
        }
    }
}

