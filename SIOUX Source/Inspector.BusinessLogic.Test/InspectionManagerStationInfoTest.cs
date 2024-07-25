using System;
using Inspector.BusinessLogic.InspectionManager;
using Inspector.BusinessLogic.InspectionManager.Model;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Test
{
    [TestFixture]
    class InspectionManagerStationInfoTest
    {
        #region Test data
        private static string StationInformationXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
<PRSData xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <PRS>
        <PRSCode>PDASNR=0D9015E4830E0</PRSCode>
        <PRSName>APELDOORN ORDERMOLENWEG 1 | 5 007 230.1</PRSName>
        <PRSIdentification>5 007 230.1</PRSIdentification>
        <Information> OS 1 </Information>
        <InspectionProcedure/>
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
        #endregion Test data
        
        [Test]
        public void StationInfoReadConfigTest()
        {
            StationInformation stationInformation = new StationInformation(StationInformationXML);

            Assert.AreEqual(1, stationInformation.PRSEntities.Count);
            PRSEntitiy prs = stationInformation.PRSEntities[0];
            ValidatePRS(prs);

            Assert.AreEqual(1, prs.GasControlLines.Count);
            ValidateGasControlLine(prs.GasControlLines[0]);
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
        private void ValidateGCLObjects(TypeObjectID typeObjectID)
        {
            Assert.AreEqual(String.Empty, typeObjectID.ObjectName);
            Assert.AreEqual(String.Empty, typeObjectID.ObjectID);
            Assert.AreEqual("00249", typeObjectID.MeasurePoint);
            Assert.AreEqual(String.Empty, typeObjectID.MeasurePointID);
            Assert.AreEqual(15, typeObjectID.FieldNo);

            Assert.AreEqual(1, typeObjectID.Boundaries.Count);
            TypeObjectIDBoundaries boundary = typeObjectID.Boundaries[0];
            Assert.AreEqual(4200, boundary.ValueMax);
            Assert.AreEqual(3800, boundary.ValueMin);
            Assert.AreEqual(TypeUnitsValue.ItemMbar, boundary.UOV);
        }

        /// <summary>
        /// Validates the PRS for test StationInfoReadConfigTest.
        /// </summary>
        /// <param name="prs">The PRS.</param>
        private static void ValidatePRS(PRSEntitiy prs)
        {
            Assert.AreEqual("PDASNR=0D9015E4830E0", prs.PRSCode);
            Assert.AreEqual("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", prs.PRSName);
            Assert.AreEqual("5 007 230.1", prs.PRSIdentification);
            Assert.AreEqual(" OS 1 ", prs.Information);
            Assert.AreEqual(String.Empty, prs.InspectionProcedure);
            Assert.AreEqual(null, prs.Route);
        }
        #endregion Validators for test StationInfoReadConfigTest
    }
}
