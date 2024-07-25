using System;
using System.IO;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Plexor;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using Inspector.Model.Plexor;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Configuration.Test
{
    [TestFixture]
    public class InspectionManagerPlexorInformationTest
    {
        [SetUp]
        public void Setup()
        {
            var path = Path.GetDirectoryName(typeof(InspectionManagerPlexorInformationTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region Test data
        private static string PlexorsXML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <PLEXORS xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <PLEXOR>
                <Name>100156</Name>
                <SerialNumber>100156</SerialNumber>
                <BTAddress>00:80:98:C4:D4:B2</BTAddress>
                <PN>0..16 bar</PN>
                <CalibrationDate>2011-03-01</CalibrationDate>
            </PLEXOR>
            <PLEXOR>
                <Name>100157</Name>
                <SerialNumber>100157</SerialNumber>
                <BTAddress>00:80:98:C4:D2:D5</BTAddress>
                <PN>0..16 bar</PN>
                <CalibrationDate>2011-03-01</CalibrationDate>
            </PLEXOR>
        </PLEXORS>";
        #endregion Test data

        #region XML Loader tests
        [Test]
        public void LoadPlexorInfoFromXMLTest()
        {
            PlexorInformationLoader plexorInformation = new PlexorInformationLoader(PlexorsXML);
            ValidatePlexorXML(plexorInformation);
        }

        /// <summary>
        /// Validates the plexorXML against the resulting PlexorInformation
        /// </summary>
        /// <param name="plexorInformation">The plexor information.</param>
        private static void ValidatePlexorXML(PlexorInformationLoader plexorInformation)
        {
            Assert.AreEqual(2, plexorInformation.Plexors.Count);

            PlexorEntity plexor = plexorInformation.Plexors[0];
            Assert.AreEqual("100156", plexor.Name);
            Assert.AreEqual("100156", plexor.SerialNumber);
            Assert.AreEqual("00:80:98:C4:D4:B2", plexor.BlueToothAddress);
            Assert.AreEqual("0..16 bar", plexor.PN);
            Assert.AreEqual(new DateTime(2011, 3, 1), plexor.CalibrationDate);

            plexor = plexorInformation.Plexors[1];
            Assert.AreEqual("100157", plexor.Name);
            Assert.AreEqual("100157", plexor.SerialNumber);
            Assert.AreEqual("00:80:98:C4:D2:D5", plexor.BlueToothAddress);
            Assert.AreEqual("0..16 bar", plexor.PN);
            Assert.AreEqual(new DateTime(2011, 3, 1), plexor.CalibrationDate);
        }
        #endregion XML Loader tests

        #region Station information manager tests
        [Test]
        public void PlexorInformationManagerTest()
        {
            PlexorInformationManager stationInfoManager = new PlexorInformationManager();
            ValidateFirstPlexor(stationInfoManager);
        }

        [Test]
        public void StationInformationManagerRefreshTest()
        {
            PlexorInformationManager stationInfoManager = new PlexorInformationManager();
            ValidateFirstPlexor(stationInfoManager);
            stationInfoManager.Refresh();
            ValidateFirstPlexor(stationInfoManager);
        }

        /// <summary>
        /// Validates the first plexor.
        /// </summary>
        /// <param name="stationInfoManager">The station info manager.</param>
        private void ValidateFirstPlexor(PlexorInformationManager stationInfoManager)
        {
            Assert.AreEqual(8, stationInfoManager.PlexorsInformation.Count);

            PlexorInformation firstPlexorInfo = stationInfoManager.PlexorsInformation[0];
            Assert.AreEqual("100156", firstPlexorInfo.Name);
            Assert.AreEqual("100156", firstPlexorInfo.SerialNumber);
            Assert.AreEqual("00:80:98:C4:D4:B2", firstPlexorInfo.BlueToothAddress);
            Assert.AreEqual("0..16 bar", firstPlexorInfo.PN);
            Assert.AreEqual(new DateTime(2011, 3, 1), firstPlexorInfo.CalibrationDate);
        }
        #endregion Station information manager tests
    }
}
