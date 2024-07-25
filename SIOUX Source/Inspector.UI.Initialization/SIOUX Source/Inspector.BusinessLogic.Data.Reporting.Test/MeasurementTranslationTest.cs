/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.BusinessLogic.Data.Reporting.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Reporting.Measurements;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Reporting.Test
{
    [TestFixture]
    public class MeasurementTranslationTest
    {
        [Test]
        public void InitializeTest()
        {
            MeasurementTranslations translations = null;
            Assert.DoesNotThrow(() => translations = MeasurementTranslations.Instance);
        }

        [Test]
        public void WrongKeyThrowsProperExceptionTest()
        {
            MeasurementTranslations translations = null;
            Assert.DoesNotThrow(() => translations = MeasurementTranslations.Instance);
            Assert.Throws<MeasurementTranslationException>(() => translations.GetValueByKey("INVALID_KEY_NOT_PRESENT"));
        }

        [Test]
        public void GetExistingKeysTest()
        {
            MeasurementTranslations translations = null;
            Assert.DoesNotThrow(() => translations = MeasurementTranslations.Instance);

            string value = String.Empty;
            Assert.DoesNotThrow(() => value = translations.Interval);
            Assert.AreEqual("!Interval", value);
            Assert.DoesNotThrow(() => value = translations.PRSName);
            Assert.AreEqual("!Station", value);
        }
    }
}
