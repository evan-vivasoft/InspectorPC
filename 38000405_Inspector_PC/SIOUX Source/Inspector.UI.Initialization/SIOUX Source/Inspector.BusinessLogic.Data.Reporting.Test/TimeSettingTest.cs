/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.BusinessLogic.Data.Reporting.Results.Model;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Reporting.Test
{
    [TestFixture]
    public class TimeSettingTest
    {
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

        [Test]
        public void TimeSettingTimeZoneWinterTest()
        {
            TimeSetting timeSetting = new TimeSetting(new DateTime(2012, 1, 1, 13, 0, 0));

            Assert.AreEqual("GMT+01:00", timeSetting.TimeZone);
            Assert.AreEqual("No", timeSetting.DST);
        }

        [Test]
        public void TimeSettingTimeSummerTest()
        {
            TimeSetting timeSetting = new TimeSetting(new DateTime(2012, 7, 1, 13, 0, 0));

            Assert.AreEqual("GMT+02:00", timeSetting.TimeZone);
            Assert.AreEqual("Yes", timeSetting.DST);
        }

        [Test]
        public void TimeSettingTimeSummerHalfHourTest()
        {
            TimeSetting timeSetting = new TimeSetting(new DateTime(2012, 7, 1, 13, 30, 0));

            Assert.AreEqual("GMT+02:00", timeSetting.TimeZone);
            Assert.AreEqual("Yes", timeSetting.DST);
        }

        [Test]
        public void TimeSettingTimeSummerHourTest()
        {
            TimeSetting timeSetting = new TimeSetting(new DateTime(2012, 7, 1, 13, 30, 0));

            Assert.AreEqual("GMT+02:00", timeSetting.TimeZone);
            Assert.AreEqual("Yes", timeSetting.DST);
        }
    }
}
