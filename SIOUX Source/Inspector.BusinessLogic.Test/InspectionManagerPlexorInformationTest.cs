using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Inspector.BusinessLogic.InspectionManager;
using Inspector.BusinessLogic.InspectionManager.Model;

namespace Inspector.BusinessLogic.Test
{
    [TestFixture]
    public class InspectionManagerPlexorInformationTest
    {
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

        [Test]
        public void LoadPlexorInfoFromXMLTest()
        {
            PlexorInformation plexorInformation = new PlexorInformation(PlexorsXML);
            ValidatePlexorXML(plexorInformation);
        }

        /// <summary>
        /// Validates the plexorXML against the resulting PlexorInformation
        /// </summary>
        /// <param name="plexorInformation">The plexor information.</param>
        private static void ValidatePlexorXML(PlexorInformation plexorInformation)
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
    }
}
