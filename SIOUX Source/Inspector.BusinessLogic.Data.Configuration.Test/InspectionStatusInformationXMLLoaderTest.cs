using System.IO;
using System.Linq;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedureStatus;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using Inspector.Model.InspectionProcedure;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Configuration.Test
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class InspectionStatusInformationXMLLoaderTest
    {
        [SetUp]
        public void Setup()
        {
            var path = Path.GetDirectoryName(typeof(InspectionStatusInformationXMLLoaderTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region Test data
        private static string InspectionStatusXML =
        @"<?xml version=""1.0"" standalone=""yes""?>
        <Status>
          <InspectionStatus>
            <PRSIdentification>1 013 178.1</PRSIdentification>
            <PRSName>DRACHTEN RICHTERLAAN 0 | 1 013 178.1</PRSName>
            <PRSCode>PDASNR=0D9015E4830E0</PRSCode>
            <GCLIdentification>1 013 178.1.1</GCLIdentification>
            <GasControlLineName>1 013 178.1.1 DS LS</GasControlLineName>
            <GCLCode>05.03.2012</GCLCode>
            <StatusID>2</StatusID>
          </InspectionStatus>
        </Status>";
        #endregion Test data

        #region XML Loader tests
        [Test]
        public void LoadPlexorInfoFromXMLTest()
        {
            InspectionStatusInformationLoader inspectionStatusInfo = new InspectionStatusInformationLoader(InspectionStatusXML);
            ValidateInspectionStatusXML(inspectionStatusInfo);
        }

        /// <summary>
        /// Validates the inspection status XML.
        /// </summary>
        /// <param name="inspectionStatusInfo">The inspection status info.</param>
        private void ValidateInspectionStatusXML(InspectionStatusInformationLoader inspectionStatusInfo)
        {
            Assert.AreEqual(1, inspectionStatusInfo.InspectionStatuses.Count);
            InspectionStatusEntity inspectionStatus = inspectionStatusInfo.InspectionStatuses.First();

            Assert.AreEqual("1 013 178.1", inspectionStatus.PRSIdentification);
            Assert.AreEqual("DRACHTEN RICHTERLAAN 0 | 1 013 178.1", inspectionStatus.PRSName);
            Assert.AreEqual("PDASNR=0D9015E4830E0", inspectionStatus.PRSCode);
            Assert.AreEqual("1 013 178.1.1", inspectionStatus.GCLIdentification);
            Assert.AreEqual("1 013 178.1.1 DS LS", inspectionStatus.GCLName);
            Assert.AreEqual("05.03.2012", inspectionStatus.GCLCode);
            Assert.AreEqual(InspectionStatus.StartNotCompleted, inspectionStatus.StatusID);
        }
        #endregion XML Loader tests
    }
}
