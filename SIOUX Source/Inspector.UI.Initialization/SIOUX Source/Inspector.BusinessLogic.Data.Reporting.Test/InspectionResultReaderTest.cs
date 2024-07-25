/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.BusinessLogic.Data.Reporting.Results;
using Inspector.Model.InspectionReportingResults;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Reporting.Test
{
    [TestFixture]
    public class InspectionResultReaderTest
    {
        [Test]
        public void LookupLastReportResultTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string lastTime = "16:17:28";
            string measurePoint = "00255";
            string objectName = String.Empty;
            int fieldNumber = 4;

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportResult reportResult = inspectionResultReader.LookupLastReportResult(prsName, gclName, measurePoint, objectName, fieldNumber);

            Assert.IsNotNull(reportResult);
            Assert.AreEqual(lastTime, reportResult.Time);
            Assert.AreEqual(String.Empty, reportResult.ObjectName);
            Assert.AreEqual(String.Empty, reportResult.ObjectID);
            Assert.AreEqual(measurePoint, reportResult.MeasurePoint);
            Assert.AreEqual(String.Empty, reportResult.MeasurePointID);
            Assert.AreEqual(fieldNumber, reportResult.FieldNo.Value);
            Assert.AreEqual("Markus", reportResult.Text);
        }

        [Test]
        public void LookupLastReportResultFieldNumberOneTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string lastTime = "12:24:12";
            string measurePoint = String.Empty;
            string objectName = String.Empty;
            int fieldNumber = 10000;

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportResult reportResult = inspectionResultReader.LookupLastReportResult(prsName, gclName, measurePoint, objectName, fieldNumber);

            Assert.IsNotNull(reportResult);
            Assert.AreEqual(lastTime, reportResult.Time);
            Assert.AreEqual(String.Empty, reportResult.ObjectName);
            Assert.AreEqual(String.Empty, reportResult.ObjectID);
            Assert.AreEqual(measurePoint, reportResult.MeasurePoint);
            Assert.AreEqual(String.Empty, reportResult.MeasurePointID);
            Assert.AreEqual(fieldNumber, reportResult.FieldNo.Value);
            Assert.AreEqual("In Ordnung", reportResult.Text);
        }


        [Test]
        public void LookupLastReportResultMeasurementTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string lastTime = "16:15:55";
            string measurePoint = "00246";
            string objectName = String.Empty;
            int fieldNumber = 39;

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportResult reportResult = inspectionResultReader.LookupLastReportResult(prsName, gclName, measurePoint, objectName, fieldNumber);

            Assert.IsNotNull(reportResult);
            Assert.AreEqual(lastTime, reportResult.Time);
            Assert.AreEqual(String.Empty, reportResult.ObjectName);
            Assert.AreEqual(String.Empty, reportResult.ObjectID);
            Assert.AreEqual(measurePoint, reportResult.MeasurePoint);
            Assert.AreEqual(String.Empty, reportResult.MeasurePointID);
            Assert.AreEqual(fieldNumber, reportResult.FieldNo.Value);
            Assert.AreEqual(null, reportResult.Text);
            Assert.AreEqual(0.0005, reportResult.MeasureValue.Value, 10e-5);
            Assert.AreEqual(Inspector.Model.UnitOfMeasurement.ItemBar, reportResult.MeasureValue.UOM);
        }

        [Test]
        public void LookupPreviousToLastReportResultTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string lastTime = "16:17:20";
            string measurePoint = "00003";
            string objectName = "ObjectName";
            int fieldNumber = 3;

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportResult reportResult = inspectionResultReader.LookupLastReportResult(prsName, gclName, measurePoint, objectName, fieldNumber);

            Assert.IsNotNull(reportResult);
            Assert.AreEqual(lastTime, reportResult.Time);
            Assert.AreEqual(objectName, reportResult.ObjectName);
            Assert.AreEqual("ObjectId", reportResult.ObjectID);
            Assert.AreEqual(measurePoint, reportResult.MeasurePoint);
            Assert.AreEqual("MeasurePointId", reportResult.MeasurePointID);
            Assert.AreEqual(fieldNumber, reportResult.FieldNo.Value);
            Assert.AreEqual("Tiny", reportResult.Text);
        }

        [Test]
        public void LookUpNonExistingLastReportResultReturnsNullTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string measurePoint = "00003";
            string objectName = "ObjectName";
            int fieldNumber = 3;

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportResult reportResult = inspectionResultReader.LookupLastReportResult(prsName, measurePoint, objectName, fieldNumber);
            Assert.IsNull(reportResult);
        }

        [Test]
        public void LookUpNonExistingPreviousToLastReportResultReturnsNullTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string measurePoint = "00003";
            string objectName = "ObjectName";
            int fieldNumber = 3;

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportResult reportResult = inspectionResultReader.LookupPreviousToLastReportResult(prsName, measurePoint, objectName, fieldNumber);
            Assert.IsNull(reportResult);
        }

        [Test]
        public void LookupLastInspectionResultTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string dtLast = "2012-02-06";

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportInspectionResult reportInspectionResult = inspectionResultReader.LookupLastResult(prsName, gclName);

            Assert.NotNull(reportInspectionResult);
            Assert.AreEqual(prsName, reportInspectionResult.PRSName);
            Assert.AreEqual(gclName, reportInspectionResult.GasControlLineName);
            Assert.AreEqual(dtLast, reportInspectionResult.DateTimeStamp.StartDate);
        }

        [Test]
        public void LookupPreviousToLastInspectionResultTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string dtLast = "2012-02-05";

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportInspectionResult reportInspectionResult = inspectionResultReader.LookupPreviousToLastResult(prsName, gclName);

            Assert.NotNull(reportInspectionResult);
            Assert.AreEqual(prsName, reportInspectionResult.PRSName);
            Assert.AreEqual(gclName, reportInspectionResult.GasControlLineName);
            Assert.AreEqual(dtLast, reportInspectionResult.DateTimeStamp.StartDate);
        }

        [Test]
        public void LookupLastInspectionResultUnknownResultTest()
        {
            string prsName = "Unknown";
            string gclName = "Unknown";

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportInspectionResult reportInspectionResult = inspectionResultReader.LookupLastResult(prsName, gclName);

            Assert.Null(reportInspectionResult);
        }

        [Test]
        public void LookupPreviousToLastInspectionResultUnknownResultTest()
        {
            string prsName = "Unknown";
            string gclName = "Unknown";

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportInspectionResult reportInspectionResult = inspectionResultReader.LookupPreviousToLastResult(prsName, gclName);

            Assert.Null(reportInspectionResult);
        }

        [Test]
        public void LookupLastInspectionResultEmptyGclTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = null;

            InspectionResultReader inspectionResultReader = new InspectionResultReader();
            ReportInspectionResult reportInspectionResult = inspectionResultReader.LookupPreviousToLastResult(prsName, gclName);

            Assert.Null(reportInspectionResult);
        }

    }
}
