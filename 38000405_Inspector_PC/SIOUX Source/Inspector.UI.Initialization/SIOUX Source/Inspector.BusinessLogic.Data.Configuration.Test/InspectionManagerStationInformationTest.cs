using System;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.Model;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionProcedureStatus;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Configuration.Test
{
    [TestFixture]
    class InspectionManagerStationInformationTest
    {
        #region Test data
        private static string StationInformationXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <PRSData xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
        <PRS>
            <Route>Route 1</Route>
            <PRSCode>PDASNR=0D9015E4830E0</PRSCode>
            <PRSName>APELDOORN ORDERMOLENWEG 1 | 5 007 230.1</PRSName>
            <PRSIdentification>5 007 230.1</PRSIdentification>
            <Information> OS 1 </Information>
            <InspectionProcedure/>
            <PRSObjects>
                <ObjectName>PRS ObjectName 1</ObjectName>
                <ObjectID>PRS ObjectID 1</ObjectID>
                <MeasurePoint>PRS MP 1</MeasurePoint>
                <MeasurePointID>PRS MPID 1</MeasurePointID>
                <FieldNo>1</FieldNo>
            </PRSObjects>
            <PRSObjects>
                <ObjectName>PRS ObjectName 2</ObjectName>
                <ObjectID>PRS ObjectID 2</ObjectID>
                <MeasurePoint>PRS MP 2</MeasurePoint>
                <MeasurePointID>PRS MPID 2</MeasurePointID>
                <FieldNo>2</FieldNo>
            </PRSObjects>
        </PRS>
        <GasControlLine>
            <PRSName>APELDOORN ORDERMOLENWEG 1 | 5 007 230.1</PRSName>
            <PRSIdentification>5 007 230.1</PRSIdentification>
            <GasControlLineName>5 007 230.1.1 OS LS</GasControlLineName>
            <PeMin/>
            <PeMax/>
            <VolumeVA>150dm3</VolumeVA>
            <VolumeVAK>150dm3</VolumeVAK>
            <PaRangeDM>0..17bar</PaRangeDM>
            <PeRangeDM>0..17bar</PeRangeDM>
            <GCLIdentification>23.05.2012 5 007 230.1.1</GCLIdentification>
            <InspectionProcedure>B-insp MON-VA LS (Pd &gt; 300 mbar)</InspectionProcedure>
            <FSDStart>900</FSDStart>
            <GCLObjects>
                <ObjectName/>
                <ObjectID/>
                <MeasurePoint>00249</MeasurePoint>
                <MeasurePointID/>
                <FieldNo>15</FieldNo>
                <Boundaries>
                    <ValueMax>4200</ValueMax>
                    <ValueMin>3800</ValueMin>
                    <UOV>mbar</UOV>
                </Boundaries>
            </GCLObjects>
        </GasControlLine>
        </PRSData>";


        private static string StationInformationIncorrectPRSNameXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <PRSData xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
        <PRS>
            <Route>Route 1</Route>
            <PRSCode>PDASNR=0D9015E4830E0</PRSCode>
            <PRSName>PRSName</PRSName>
            <PRSIdentification>PRSID</PRSIdentification>
            <Information> OS 1 </Information>
            <InspectionProcedure/>
            <PRSObjects>
                <ObjectName>PRS ObjectName 1</ObjectName>
                <ObjectID>PRS ObjectID 1</ObjectID>
                <MeasurePoint>PRS MP 1</MeasurePoint>
                <MeasurePointID>PRS MPID 1</MeasurePointID>
                <FieldNo>1</FieldNo>
            </PRSObjects>
            <PRSObjects>
                <ObjectName>PRS ObjectName 2</ObjectName>
                <ObjectID>PRS ObjectID 2</ObjectID>
                <MeasurePoint>PRS MP 2</MeasurePoint>
                <MeasurePointID>PRS MPID 2</MeasurePointID>
                <FieldNo>2</FieldNo>
            </PRSObjects>
        </PRS>
        <GasControlLine>
            <PRSName>PRSName wrong</PRSName>
            <PRSIdentification>PRSID</PRSIdentification>
            <GasControlLineName>5 007 230.1.1 OS LS</GasControlLineName>
            <PeMin/>
            <PeMax/>
            <VolumeVA>150dm3</VolumeVA>
            <VolumeVAK>150dm3</VolumeVAK>
            <PaRangeDM>0..17bar</PaRangeDM>
            <PeRangeDM>0..17bar</PeRangeDM>
            <GCLIdentification>23.05.2012 5 007 230.1.1</GCLIdentification>
            <InspectionProcedure>B-insp MON-VA LS (Pd &gt; 300 mbar)</InspectionProcedure>
            <FSDStart>900</FSDStart>
            <GCLObjects>
                <ObjectName/>
                <ObjectID/>
                <MeasurePoint>00249</MeasurePoint>
                <MeasurePointID/>
                <FieldNo>15</FieldNo>
                <Boundaries>
                    <ValueMax>4200</ValueMax>
                    <ValueMin>3800</ValueMin>
                    <UOV>mbar</UOV>
                </Boundaries>
            </GCLObjects>
        </GasControlLine>
        </PRSData>";

        private static string StationInformationIncorrectPRSIDXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <PRSData xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
        <PRS>
            <Route>Route 1</Route>
            <PRSCode>PDASNR=0D9015E4830E0</PRSCode>
            <PRSName>PRSName</PRSName>
            <PRSIdentification>PRSID</PRSIdentification>
            <Information> OS 1 </Information>
            <InspectionProcedure/>
            <PRSObjects>
                <ObjectName>PRS ObjectName 1</ObjectName>
                <ObjectID>PRS ObjectID 1</ObjectID>
                <MeasurePoint>PRS MP 1</MeasurePoint>
                <MeasurePointID>PRS MPID 1</MeasurePointID>
                <FieldNo>1</FieldNo>
            </PRSObjects>
            <PRSObjects>
                <ObjectName>PRS ObjectName 2</ObjectName>
                <ObjectID>PRS ObjectID 2</ObjectID>
                <MeasurePoint>PRS MP 2</MeasurePoint>
                <MeasurePointID>PRS MPID 2</MeasurePointID>
                <FieldNo>2</FieldNo>
            </PRSObjects>
        </PRS>
        <GasControlLine>
            <PRSName>PRSName</PRSName>
            <PRSIdentification>PRSID wrong</PRSIdentification>
            <GasControlLineName>5 007 230.1.1 OS LS</GasControlLineName>
            <PeMin/>
            <PeMax/>
            <VolumeVA>150dm3</VolumeVA>
            <VolumeVAK>150dm3</VolumeVAK>
            <PaRangeDM>0..17bar</PaRangeDM>
            <PeRangeDM>0..17bar</PeRangeDM>
            <GCLIdentification>23.05.2012 5 007 230.1.1</GCLIdentification>
            <InspectionProcedure>B-insp MON-VA LS (Pd &gt; 300 mbar)</InspectionProcedure>
            <FSDStart>900</FSDStart>
            <GCLObjects>
                <ObjectName/>
                <ObjectID/>
                <MeasurePoint>00249</MeasurePoint>
                <MeasurePointID/>
                <FieldNo>15</FieldNo>
                <Boundaries>
                    <ValueMax>4200</ValueMax>
                    <ValueMin>3800</ValueMin>
                    <UOV>mbar</UOV>
                </Boundaries>
            </GCLObjects>
        </GasControlLine>
        </PRSData>";


        private static string StationInformationIncorrectBothXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <PRSData xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
        <PRS>
            <Route>Route 1</Route>
            <PRSCode>PDASNR=0D9015E4830E0</PRSCode>
            <PRSName>PRSName</PRSName>
            <PRSIdentification>PRSID</PRSIdentification>
            <Information> OS 1 </Information>
            <InspectionProcedure/>
            <PRSObjects>
                <ObjectName>PRS ObjectName 1</ObjectName>
                <ObjectID>PRS ObjectID 1</ObjectID>
                <MeasurePoint>PRS MP 1</MeasurePoint>
                <MeasurePointID>PRS MPID 1</MeasurePointID>
                <FieldNo>1</FieldNo>
            </PRSObjects>
            <PRSObjects>
                <ObjectName>PRS ObjectName 2</ObjectName>
                <ObjectID>PRS ObjectID 2</ObjectID>
                <MeasurePoint>PRS MP 2</MeasurePoint>
                <MeasurePointID>PRS MPID 2</MeasurePointID>
                <FieldNo>2</FieldNo>
            </PRSObjects>
        </PRS>
        <GasControlLine>
            <PRSName>PRSName wrong</PRSName>
            <PRSIdentification>PRSID wrong</PRSIdentification>
            <GasControlLineName>5 007 230.1.1 OS LS</GasControlLineName>
            <PeMin/>
            <PeMax/>
            <VolumeVA>150dm3</VolumeVA>
            <VolumeVAK>150dm3</VolumeVAK>
            <PaRangeDM>0..17bar</PaRangeDM>
            <PeRangeDM>0..17bar</PeRangeDM>
            <GCLIdentification>23.05.2012 5 007 230.1.1</GCLIdentification>
            <InspectionProcedure>B-insp MON-VA LS (Pd &gt; 300 mbar)</InspectionProcedure>
            <FSDStart>900</FSDStart>
            <GCLObjects>
                <ObjectName/>
                <ObjectID/>
                <MeasurePoint>00249</MeasurePoint>
                <MeasurePointID/>
                <FieldNo>15</FieldNo>
                <Boundaries>
                    <ValueMax>4200</ValueMax>
                    <ValueMin>3800</ValueMin>
                    <UOV>mbar</UOV>
                </Boundaries>
            </GCLObjects>
        </GasControlLine>
        </PRSData>";

        #endregion Test data

        #region XML Loader tests
        [Test]
        public void StationInfoReadConfigTest()
        {
            StationInformationLoader stationInformation = new StationInformationLoader(StationInformationXML);

            Assert.AreEqual(1, stationInformation.PRSEntities.Count);
            PRSEntity prs = stationInformation.PRSEntities[0];
            ValidatePRS(prs);

            Assert.AreEqual(1, prs.GasControlLines.Count);
            ValidateGasControlLine(prs.GasControlLines[0]);
        }

        [Test]
        public void StationInfoReadConfigIncorrectPRSNameTest()
        {
            StationInformationLoader stationInformation = new StationInformationLoader(StationInformationIncorrectPRSNameXML);
            Assert.AreEqual(1, stationInformation.PRSEntities.Count);
            PRSEntity prs = stationInformation.PRSEntities[0];
            Assert.AreEqual(0, prs.GasControlLines.Count);
        }

        [Test]
        public void StationInfoReadConfigIncorrectPRSIdTest()
        {
            StationInformationLoader stationInformation = new StationInformationLoader(StationInformationIncorrectPRSIDXML);
            Assert.AreEqual(1, stationInformation.PRSEntities.Count);
            PRSEntity prs = stationInformation.PRSEntities[0];
            Assert.AreEqual(0, prs.GasControlLines.Count);
        }


        [Test]
        public void StationInfoReadConfigIncorrectBothTest()
        {
            StationInformationLoader stationInformation = new StationInformationLoader(StationInformationIncorrectBothXML);
            Assert.AreEqual(1, stationInformation.PRSEntities.Count);
            PRSEntity prs = stationInformation.PRSEntities[0];
            Assert.AreEqual(0, prs.GasControlLines.Count);
        }

        #region Validators for test StationInfoReadConfigTest
        /// <summary>
        /// Validates the gas control line for test StationInfoReadConfigTest.
        /// </summary>
        /// <param name="gasControlLine">The gas control line.</param>
        private void ValidateGasControlLine(GasControlLineEntity gasControlLine)
        {
            Assert.AreEqual("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", gasControlLine.PRSName);
            Assert.AreEqual("5 007 230.1", gasControlLine.PRSIdentification);
            Assert.AreEqual("5 007 230.1.1 OS LS", gasControlLine.GasControlLineName);
            Assert.AreEqual(String.Empty, gasControlLine.PeMin);
            Assert.AreEqual(String.Empty, gasControlLine.PeMax);
            Assert.AreEqual("150dm3", gasControlLine.VolumeVA);
            Assert.AreEqual("150dm3", gasControlLine.VolumeVAK);
            Assert.AreEqual(TypeRangeDM.Item017bar, gasControlLine.PaRangeDM);
            Assert.AreEqual(TypeRangeDM.Item017bar, gasControlLine.PeRangeDM);
            Assert.AreEqual("23.05.2012 5 007 230.1.1", gasControlLine.GCLIdentification);
            Assert.AreEqual(String.Empty, gasControlLine.GCLCode);
            Assert.AreEqual("B-insp MON-VA LS (Pd > 300 mbar)", gasControlLine.InspectionProcedure);
            Assert.AreEqual(900, gasControlLine.FSDStart);

            Assert.AreEqual(1, gasControlLine.GCLObjects.Count);
            ValidateGCLObjects(gasControlLine.GCLObjects[0]);
        }

        /// <summary>
        /// Validates the GCL objects for test StationInfoReadConfigTest.
        /// </summary>
        /// <param name="typeObjectID">The type object ID.</param>
        private void ValidateGCLObjects(GCLObject typeObjectID)
        {
            Assert.AreEqual(String.Empty, typeObjectID.ObjectName);
            Assert.AreEqual(String.Empty, typeObjectID.ObjectID);
            Assert.AreEqual("00249", typeObjectID.MeasurePoint);
            Assert.AreEqual(String.Empty, typeObjectID.MeasurePointID);
            Assert.AreEqual(15, typeObjectID.FieldNo);

            TypeObjectIDBoundaries boundaries = typeObjectID.Boundaries;
            Assert.AreEqual(4200, boundaries.ValueMax);
            Assert.AreEqual(3800, boundaries.ValueMin);
            Assert.AreEqual(UnitOfMeasurement.ItemMbar, boundaries.UOV);
        }

        /// <summary>
        /// Validates the PRS for test StationInfoReadConfigTest.
        /// </summary>
        /// <param name="prs">The PRS.</param>
        private static void ValidatePRS(PRSEntity prs)
        {
            Assert.AreEqual("PDASNR=0D9015E4830E0", prs.PRSCode);
            Assert.AreEqual("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", prs.PRSName);
            Assert.AreEqual("5 007 230.1", prs.PRSIdentification);
            Assert.AreEqual(" OS 1 ", prs.Information);
            Assert.AreEqual(String.Empty, prs.InspectionProcedure);
            Assert.AreEqual("Route 1", prs.Route);
            ValidatePRSObjects(prs);
        }

        /// <summary>
        /// Validates the PRS objects.
        /// </summary>
        /// <param name="prs">The PRS.</param>
        private static void ValidatePRSObjects(PRSEntity prs)
        {
            Assert.AreEqual(2, prs.PRSObjects.Count);
            int index = 1;
            foreach (PRSObject prsObject in prs.PRSObjects)
            {
                Assert.AreEqual(String.Format("PRS ObjectName {0}", index), prsObject.ObjectName);
                Assert.AreEqual(String.Format("PRS ObjectID {0}", index), prsObject.ObjectID);
                Assert.AreEqual(String.Format("PRS MP {0}", index), prsObject.MeasurePoint);
                Assert.AreEqual(String.Format("PRS MPID {0}", index), prsObject.MeasurePointID);
                Assert.AreEqual(index, prsObject.FieldNo);
                index++;
            }
        }
        #endregion Validators for test StationInfoReadConfigTest
        #endregion XML Loader tests

        #region Station information manager tests
        [Test, Sequential]
        public void OverallInspectionStatusTest([Values(InspectionStatus.Completed, InspectionStatus.CompletedValueOutOfLimits, InspectionStatus.GclOrPrsDeletedByUser, InspectionStatus.NoInspectionFound, InspectionStatus.StartNotCompleted, InspectionStatus.NoInspection)] InspectionStatus inspectionStatus)
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            StationInformation stationInfo = stationInfoManager.StationsInformation[0];
            foreach (GasControlLineInformation gclInfo in stationInfo.GasControlLines)
            {
                stationInfoManager.SetInspectionStatus(inspectionStatus, stationInfo.PRSName, gclInfo.GasControlLineName);
            }
            Assert.AreEqual(inspectionStatus, stationInfo.OverallGasControlLineStatus);
        }

        [Test]
        public void InspectionStatusDifferenceCorrectTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            StationInformation stationInfo = stationInfoManager.StationsInformation[0];
            GasControlLineInformation gclInfo0 = stationInfo.GasControlLines[0];
            GasControlLineInformation gclInfo1 = stationInfo.GasControlLines[1];
            stationInfoManager.SetInspectionStatus(InspectionStatus.Completed, stationInfo.PRSName, gclInfo0.GasControlLineName);
            stationInfoManager.SetInspectionStatus(InspectionStatus.NoInspectionFound, stationInfo.PRSName, gclInfo1.GasControlLineName);

            Assert.AreEqual(InspectionStatus.Completed, stationInfo.OverallGasControlLineStatus);
        }

        [Test]
        public void InspectionStatusDifferenceWarningTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            StationInformation stationInfo = stationInfoManager.StationsInformation[0];
            GasControlLineInformation gclInfo0 = stationInfo.GasControlLines[0];
            GasControlLineInformation gclInfo1 = stationInfo.GasControlLines[1];
            stationInfoManager.SetInspectionStatus(InspectionStatus.Completed, stationInfo.PRSName, gclInfo0.GasControlLineName);
            stationInfoManager.SetInspectionStatus(InspectionStatus.GclOrPrsDeletedByUser, stationInfo.PRSName, gclInfo1.GasControlLineName);

            Assert.AreEqual(InspectionStatus.Warning, stationInfo.OverallGasControlLineStatus);
        }

        [Test]
        public void LookupInspectionProcedureNameByPrsTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            string inspectionProcedureName = stationInfoManager.LookupInspectionProcedureName("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1");

            Assert.IsNullOrEmpty(inspectionProcedureName);
        }

        [Test]
        public void LookupInspectionProcedureNameInvalidPrsTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupInspectionProcedureName("NONEXISTING_PRS_NAME"));
            Assert.AreEqual("Failed to lookup the 'NONEXISTING_PRS_NAME' station information.", exception.Message);
        }

        [Test]
        public void LookupInspectionProcedureNameByPrsAndGclTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            string inspectionProcedureName = stationInfoManager.LookupInspectionProcedureName("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", "5 007 230.1.1 OS LS");

            Assert.AreEqual("B-insp MON-VA LS (Pd > 300 mbar)", inspectionProcedureName);
        }

        [Test]
        public void LookupInspectionProcedureNameInvalidGclTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupInspectionProcedureName("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", "NONEXISTING_GCL_NAME"));
            Assert.AreEqual("Failed to lookup the 'NONEXISTING_GCL_NAME' gascontrolline information of station 'APELDOORN ORDERMOLENWEG 1 | 5 007 230.1'.", exception.Message);
        }

        [Test]
        public void StationInformationManagerTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            ValidateFirstStationAndGasControlLineOfXMLFile(stationInfoManager);
        }

        [Test]
        public void StationInformationManagerRefreshTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            ValidateFirstStationAndGasControlLineOfXMLFile(stationInfoManager);
            stationInfoManager.Refresh();
            ValidateFirstStationAndGasControlLineOfXMLFile(stationInfoManager);
        }

        [Test]
        public void LookupGclObjectWithoutGasControlLineNameWithDuplicatesThrowsExceptionTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string measurePoint = "00169";
            string objectName = String.Empty;
            int fieldNumber = 16;

            Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupGasControlLineObject(prsName, measurePoint, objectName, fieldNumber));
        }

        [Test]
        public void LookupGclObjectWithoutGasControlLineNameTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();

            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string measurePoint = "12345MP";
            string objectName = "ObjectName";
            int fieldNumber = 666;

            GclObject gclObject = null;
            Assert.DoesNotThrow(() => gclObject = stationInfoManager.LookupGasControlLineObject(prsName, measurePoint, objectName, fieldNumber));
            Assert.IsNotNull(gclObject);
            Assert.AreEqual(measurePoint, gclObject.MeasurePoint);
            Assert.AreEqual(objectName, gclObject.ObjectName);
            Assert.AreEqual("MeasurePointId", gclObject.MeasurePointId);
            Assert.AreEqual("ObjectId", gclObject.ObjectId);
            Assert.IsNotNull(gclObject.GclBoundary);
            Assert.AreEqual(999, gclObject.GclBoundary.ValueMax);
            Assert.AreEqual(1000, gclObject.GclBoundary.ValueMin);
            Assert.AreEqual("mbar", gclObject.GclBoundary.UOV);
        }

        [Test]
        public void LookupNonExistingGclObjectWithoutGasControlLineNameThrowsExceptionTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();

            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string measurePoint = "12345MP";
            string objectName = "ObjectName";
            int fieldNumber = 333; // non-existing value

            Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupGasControlLineObject(prsName, measurePoint, objectName, fieldNumber));
        }

        [Test]
        public void LookupGclObjectWithGasControlLineNameWithDuplicatesThrowsExceptionTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string measurePoint = "00169";
            string objectName = String.Empty;
            int fieldNumber = 16;

            Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupGasControlLineObject(prsName, gclName, measurePoint, objectName, fieldNumber));
        }

        [Test]
        public void LookupGclObjectWithGasControlLineNameTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();

            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string measurePoint = "12345MP";
            string objectName = "ObjectName";
            int fieldNumber = 666;

            GclObject gclObject = null;
            Assert.DoesNotThrow(() => gclObject = stationInfoManager.LookupGasControlLineObject(prsName, gclName, measurePoint, objectName, fieldNumber));
            Assert.IsNotNull(gclObject);
            Assert.AreEqual(measurePoint, gclObject.MeasurePoint);
            Assert.AreEqual(objectName, gclObject.ObjectName);
            Assert.AreEqual("MeasurePointId", gclObject.MeasurePointId);
            Assert.AreEqual("ObjectId", gclObject.ObjectId);
            Assert.IsNotNull(gclObject.GclBoundary);
            Assert.AreEqual(999, gclObject.GclBoundary.ValueMax);
            Assert.AreEqual(1000, gclObject.GclBoundary.ValueMin);
            Assert.AreEqual("mbar", gclObject.GclBoundary.UOV);
        }

        [Test]
        public void LookupNonExistingGclObjectWithGasControlLineNameThrowsExceptionTest()
        {
            StationInformationManager stationInfoManager = new StationInformationManager();

            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            string measurePoint = "12345MP";
            string objectName = "ObjectName";
            int fieldNumber = 333; // non-existing value

            Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupGasControlLineObject(prsName, gclName, measurePoint, objectName, fieldNumber));
        }

        /// <summary>
        /// Validates the first station and gas control line of XML file.
        /// </summary>
        /// <param name="stationInfoManager">The station info manager.</param>
        private static void ValidateFirstStationAndGasControlLineOfXMLFile(StationInformationManager stationInfoManager)
        {
            Assert.AreEqual(4, stationInfoManager.StationsInformation.Count);

            StationInformation firstStationInfo = stationInfoManager.StationsInformation[0];
            Assert.AreEqual(String.Empty, firstStationInfo.Route);
            Assert.AreEqual("PDASNR=0D9015E4830E0", firstStationInfo.PRSCode);
            Assert.AreEqual("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", firstStationInfo.PRSName);
            Assert.AreEqual("5 007 230.1", firstStationInfo.PRSIdentification);
            Assert.AreEqual(" OS 1 ", firstStationInfo.Information);
            Assert.AreEqual(String.Empty, firstStationInfo.InspectionProcedure);
            Assert.AreEqual(InspectionStatus.NoInspectionFound, firstStationInfo.InspectionStatus);
            Assert.AreEqual(1, firstStationInfo.PrsObjects.Count);

            PrsObject firstPrsObject = firstStationInfo.PrsObjects[0];
            Assert.AreEqual(String.Empty, firstPrsObject.ObjectName);
            Assert.AreEqual(String.Empty, firstPrsObject.ObjectId);
            Assert.AreEqual("00001", firstPrsObject.MeasurePoint);
            Assert.AreEqual(String.Empty, firstPrsObject.MeasurePointId);
            Assert.AreEqual(172, firstPrsObject.FieldNo.Value);

            foreach (StationInformation stationInfo in stationInfoManager.StationsInformation)
            {
                Assert.AreEqual(2, stationInfo.GasControlLines.Count);
            }

            GasControlLineInformation firstGasControlLineInfo = firstStationInfo.GasControlLines[0];
            Assert.AreEqual("5 007 230.1.1 OS LS", firstGasControlLineInfo.GasControlLineName);
            Assert.AreEqual(String.Empty, firstGasControlLineInfo.PeMin);
            Assert.AreEqual(String.Empty, firstGasControlLineInfo.PeMax);
            Assert.AreEqual("150dm3", firstGasControlLineInfo.VolumeVA);
            Assert.AreEqual("300dm3", firstGasControlLineInfo.VolumeVAK);
            Assert.AreEqual("0..17 bar", firstGasControlLineInfo.PaRangeDM);
            Assert.AreEqual("0..17 bar", firstGasControlLineInfo.PeRangeDM);
            Assert.AreEqual("23.05.2012 5 007 230.1.1", firstGasControlLineInfo.GCLIdentification);
            Assert.AreEqual("B-insp MON-VA LS (Pd > 300 mbar)", firstGasControlLineInfo.InspectionProcedure);
            Assert.AreEqual(InspectionStatus.NoInspection, firstGasControlLineInfo.InspectionStatus);
            Assert.AreEqual(900, firstGasControlLineInfo.FSDStart);
            Assert.AreEqual(16, firstGasControlLineInfo.GclObjects.Count);

            GclObject firstGclObject = firstGasControlLineInfo.GclObjects[0];
            Assert.AreEqual(String.Empty, firstGclObject.ObjectName);
            Assert.AreEqual(String.Empty, firstGclObject.ObjectId);
            Assert.AreEqual("00249", firstGclObject.MeasurePoint);
            Assert.AreEqual(String.Empty, firstGclObject.MeasurePointId);
            Assert.AreEqual(15, firstGclObject.FieldNo.Value);
            Assert.AreEqual(4200, firstGclObject.GclBoundary.ValueMax);
            Assert.AreEqual(3800, firstGclObject.GclBoundary.ValueMin);
            Assert.AreEqual("mbar", firstGclObject.GclBoundary.UOV);
        }

        [Test]
        public void LookupExistingStationInformationTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            StationInformationManager stationInfoManager = new StationInformationManager();
            StationInformation stationInfo = null;
            Assert.DoesNotThrow(() => stationInfo = stationInfoManager.LookupStationInformation(prsName));
            Assert.AreEqual(prsName, stationInfo.PRSName);
        }

        [Test]
        public void LookupNonExistingStationInformationThrowExceptionTest()
        {
            string prsName = "NONEXISTING_PRS_NAME";
            StationInformationManager stationInfoManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupStationInformation(prsName));
            Assert.AreEqual("Failed to lookup the 'NONEXISTING_PRS_NAME' station information.", exception.Message);
        }

        [Test]
        public void LookupExistingGasControlLineInformationTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            StationInformationManager stationInfoManager = new StationInformationManager();
            GasControlLineInformation gasControlLineInfo = null;
            Assert.DoesNotThrow(() => gasControlLineInfo = stationInfoManager.LookupGasControlLineInformation(prsName, gclName));
            Assert.AreEqual(gclName, gasControlLineInfo.GasControlLineName);
        }

        [Test]
        public void LookupNonExistingGasControlLineInformationThrowExceptionTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "NONEXISTING_GCL_NAME";
            StationInformationManager stationInfoManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupGasControlLineInformation(prsName, gclName));
            Assert.AreEqual("Failed to lookup the 'NONEXISTING_GCL_NAME' gascontrolline information of station 'APELDOORN ORDERMOLENWEG 1 | 5 007 230.1'.", exception.Message);
        }

        [Test]
        public void LookupNonExistingGasControlLineInformationWithNonExistingStationInformationThrowExceptionTest()
        {
            string prsName = "NONEXISTING_PRS_NAME";
            string gclName = "NONEXISTING_GCL_NAME";
            StationInformationManager stationInfoManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInfoManager.LookupGasControlLineInformation(prsName, gclName));
            Assert.AreEqual("Failed to lookup the 'NONEXISTING_PRS_NAME' station information.", exception.Message);
        }

        [Test]
        public void LookupGasControlLineInformationWithStationInfoObjectTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            StationInformationManager stationInfoManager = new StationInformationManager();
            StationInformation stationInfo = null;
            Assert.DoesNotThrow(() => stationInfo = stationInfoManager.LookupStationInformation(prsName));
            Assert.DoesNotThrow(() => stationInfoManager.LookupGasControlLineInformation(stationInfo, gclName));
        }
        #endregion Station information manager tests

        #region Inspection Status Tests
        [Test]
        public void AddInspectionStatusTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            Assert.DoesNotThrow(() => stationInformationManager.SetInspectionStatus(InspectionStatus.StartNotCompleted, prsName, gclName));

            stationInformationManager.Refresh(); // reload the xml file to see if the new inspection status is added

            InspectionStatusInformation inspectionStatusInfo = null;
            Assert.DoesNotThrow(() => inspectionStatusInfo = stationInformationManager.GetInspectionStatus(prsName, gclName));

            Assert.AreEqual(InspectionStatus.StartNotCompleted, inspectionStatusInfo.Status);
            Assert.AreEqual("PDASNR=0D9015E4830E0", inspectionStatusInfo.PRSCode);
            Assert.AreEqual("5 007 230.1", inspectionStatusInfo.PRSIdentification);
            Assert.AreEqual(prsName, inspectionStatusInfo.PRSName);
            Assert.AreEqual(String.Empty, inspectionStatusInfo.GCLCode);
            Assert.AreEqual("23.05.2012 5 007 230.1.2", inspectionStatusInfo.GCLIdentification);
            Assert.AreEqual(gclName, inspectionStatusInfo.GCLName);
            Assert.AreEqual(null, inspectionStatusInfo.OverallGasControlLineStatus);
        }

        [Test]
        public void ChangeInspectionStatusTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            stationInformationManager.SetInspectionStatus(InspectionStatus.StartNotCompleted, prsName, gclName);
            stationInformationManager.SetInspectionStatus(InspectionStatus.Completed, prsName, gclName);

            InspectionStatusInformation inspectionStatusInfo = null;
            Assert.DoesNotThrow(() => inspectionStatusInfo = stationInformationManager.GetInspectionStatus(prsName, gclName));

            Assert.AreEqual(InspectionStatus.Completed, inspectionStatusInfo.Status);
            Assert.AreEqual("PDASNR=0D9015E4830E0", inspectionStatusInfo.PRSCode);
            Assert.AreEqual("5 007 230.1", inspectionStatusInfo.PRSIdentification);
            Assert.AreEqual(prsName, inspectionStatusInfo.PRSName);
            Assert.AreEqual(String.Empty, inspectionStatusInfo.GCLCode);
            Assert.AreEqual("23.05.2012 5 007 230.1.2", inspectionStatusInfo.GCLIdentification);
            Assert.AreEqual(gclName, inspectionStatusInfo.GCLName);
        }

        [Test]
        public void GetInspectionStatusOfPressureRugulationStationThatIsNotPresentInXMLFileTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            IStationInformationManager stationInformationManager = new StationInformationManager();

            InspectionStatusInformation inspectionStatusInfo = null;
            Assert.DoesNotThrow(() => inspectionStatusInfo = stationInformationManager.GetInspectionStatus(prsName));

            Assert.AreEqual(InspectionStatus.NoInspectionFound, inspectionStatusInfo.Status);
            Assert.AreEqual("PDASNR=0D9015E4830E0", inspectionStatusInfo.PRSCode);
            Assert.AreEqual("5 007 230.1", inspectionStatusInfo.PRSIdentification);
            Assert.AreEqual(prsName, inspectionStatusInfo.PRSName);
            Assert.AreEqual(null, inspectionStatusInfo.GCLCode);
            Assert.AreEqual(null, inspectionStatusInfo.GCLIdentification);
            Assert.AreEqual(null, inspectionStatusInfo.GCLName);
            Assert.AreEqual(InspectionStatus.NoInspection, inspectionStatusInfo.OverallGasControlLineStatus);
        }

        [Test]
        public void GetInspectionStatusOfPressureRugulationStationAndGasControlLineThatIsNotPresentInXMLFileTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            IStationInformationManager stationInformationManager = new StationInformationManager();

            InspectionStatusInformation inspectionStatusInfo = null;
            Assert.DoesNotThrow(() => inspectionStatusInfo = stationInformationManager.GetInspectionStatus(prsName, gclName));

            Assert.AreEqual(InspectionStatus.NoInspection, inspectionStatusInfo.Status);
            Assert.AreEqual("PDASNR=0D9015E4830E0", inspectionStatusInfo.PRSCode);
            Assert.AreEqual("5 007 230.1", inspectionStatusInfo.PRSIdentification);
            Assert.AreEqual(prsName, inspectionStatusInfo.PRSName);
            Assert.AreEqual(String.Empty, inspectionStatusInfo.GCLCode);
            Assert.AreEqual("23.05.2012 5 007 230.1.2", inspectionStatusInfo.GCLIdentification);
            Assert.AreEqual(gclName, inspectionStatusInfo.GCLName);
        }

        [Test]
        public void GetInspectionStatusIsStoredCorrectlyOverMultipleSessionsTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.1 OS LS";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            stationInformationManager.GetInspectionStatus(prsName, gclName);

            stationInformationManager = new StationInformationManager();
            InspectionStatusInformation inspectionStatusInfo = null;
            Assert.DoesNotThrow(() => inspectionStatusInfo = stationInformationManager.GetInspectionStatus(prsName, gclName));

            Assert.AreEqual(InspectionStatus.NoInspection, inspectionStatusInfo.Status);
            Assert.AreEqual("PDASNR=0D9015E4830E0", inspectionStatusInfo.PRSCode);
            Assert.AreEqual("5 007 230.1", inspectionStatusInfo.PRSIdentification);
            Assert.AreEqual(prsName, inspectionStatusInfo.PRSName);
            Assert.AreEqual(String.Empty, inspectionStatusInfo.GCLCode);
            Assert.AreEqual("23.05.2012 5 007 230.1.1", inspectionStatusInfo.GCLIdentification);
            Assert.AreEqual(gclName, inspectionStatusInfo.GCLName);
        }

        [Test]
        public void SetInspectionStatusIsStoredCorrectlyOverMultipleSessionsTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            stationInformationManager.SetInspectionStatus(InspectionStatus.GclOrPrsDeletedByUser, prsName, gclName);

            stationInformationManager = new StationInformationManager();
            InspectionStatusInformation inspectionStatusInfo = null;
            Assert.DoesNotThrow(() => inspectionStatusInfo = stationInformationManager.GetInspectionStatus(prsName, gclName));

            Assert.AreEqual(InspectionStatus.GclOrPrsDeletedByUser, inspectionStatusInfo.Status);
            Assert.AreEqual("PDASNR=0D9015E4830E0", inspectionStatusInfo.PRSCode);
            Assert.AreEqual("5 007 230.1", inspectionStatusInfo.PRSIdentification);
            Assert.AreEqual(prsName, inspectionStatusInfo.PRSName);
            Assert.AreEqual(String.Empty, inspectionStatusInfo.GCLCode);
            Assert.AreEqual("23.05.2012 5 007 230.1.2", inspectionStatusInfo.GCLIdentification);
            Assert.AreEqual(gclName, inspectionStatusInfo.GCLName);
        }

        [Test]
        public void SetInspectionStatusOfUnknownPressureRegulationStationTest()
        {
            string prsName = "NONEXISTING_PRS_NAME";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInformationManager.SetInspectionStatus(InspectionStatus.GclOrPrsDeletedByUser, prsName));
            string expectedErrorMessage = "Could not create a new inspection with status 'GCL or PRS deleted by user' because the PRS 'NONEXISTING_PRS_NAME' is not available in the station information.";
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        [Test]
        public void SetInspectionStatusOfUnknownPressureRegulationStationAndUnknownGasControlLineTest()
        {
            string prsName = "NONEXISTING_PRS_NAME";
            string gclName = "NONEXISTING_GCL_NAME";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInformationManager.SetInspectionStatus(InspectionStatus.GclOrPrsDeletedByUser, prsName, gclName));
            string expectedErrorMessage = "Could not create a new inspection with status 'GCL or PRS deleted by user' because the PRS 'NONEXISTING_PRS_NAME' is not available in the station information.";
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        [Test]
        public void SetInspectionStatusOfUnknownGasControlLineTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "NONEXISTING_GCL_NAME";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInformationManager.SetInspectionStatus(InspectionStatus.GclOrPrsDeletedByUser, prsName, gclName));
            string expectedErrorMessage = "Could not create a new inspection with status 'GCL or PRS deleted by user' because the GCL 'NONEXISTING_GCL_NAME' for PRS 'APELDOORN ORDERMOLENWEG 1 | 5 007 230.1' is not available in the station information.";
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        [Test]
        public void GetInspectionStatusOfUnknownPressureRegulationStationTest()
        {
            string prsName = "NONEXISTING_PRS_NAME";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInformationManager.GetInspectionStatus(prsName));
            string expectedErrorMessage = "Could not create a new inspection with status 'No inspection found' because the PRS 'NONEXISTING_PRS_NAME' is not available in the station information.";
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        [Test]
        public void GetInspectionStatusOfUnknownPressureRegulationStationAndUnknownGasControlLineTest()
        {
            string prsName = "NONEXISTING_PRS_NAME";
            string gclName = "NONEXISTING_GCL_NAME";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInformationManager.GetInspectionStatus(prsName, gclName));
            string expectedErrorMessage = "Could not create a new inspection with status 'No inspection found' because the PRS 'NONEXISTING_PRS_NAME' is not available in the station information.";
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        [Test]
        public void GetInspectionStatusOfknownGasControlLineTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            IStationInformationManager stationInformationManager = new StationInformationManager();
            stationInformationManager.SetInspectionStatus(InspectionStatus.NoInspection, prsName, gclName);

            gclName = "NONEXISTING_GCL_NAME";
            stationInformationManager = new StationInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => stationInformationManager.GetInspectionStatus(prsName, gclName));
            string expectedErrorMessage = "Could not create a new inspection with status 'No inspection found' because the GCL 'NONEXISTING_GCL_NAME' for PRS 'APELDOORN ORDERMOLENWEG 1 | 5 007 230.1' is not available in the station information.";
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        #endregion Inspection Status Tests
    }
}
