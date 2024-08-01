/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Globalization;
using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements.Model
{
    /// <summary>
    /// Model representing the DataHeader for a measurement
    /// </summary>
    internal class DataHeader
    {
        #region Properties
        /// <summary>
        /// Gets or sets the script command.
        /// </summary>
        /// <value>
        /// The script command.
        /// </value>
        public string ScriptCommand { get; set; }

        /// <summary>
        /// Gets or sets the start of measurement.
        /// </summary>
        /// <value>
        /// The start of measurement.
        /// </value>
        public string StartOfMeasurement { get; set; }

        /// <summary>
        /// Gets or sets the end of measurement.
        /// </summary>
        /// <value>
        /// The end of measurement.
        /// </value>
        public DateTime? EndOfMeasurement { get; set; }

        /// <summary>
        /// Gets or sets the count total.
        /// </summary>
        /// <value>
        /// The count total.
        /// </value>
        public int CountTotal { get; set; }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public int Interval { get; set; }

        /// <summary>
        /// Gets or sets the field in access database.
        /// </summary>
        /// <value>
        /// The field in access database.
        /// </value>
        public string FieldInAccessDatabase { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the measurepoint.
        /// </summary>
        /// <value>
        /// The measurepoint.
        /// </value>
        public string Measurepoint { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
        #endregion Properties

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            MeasurementTranslations translation = MeasurementTranslations.Instance;
            StringBuilder result = new StringBuilder();

            string seperator = translation.SeperatorSign;
            string baseFormat = "{0} " + seperator + " {1}";

            result.AppendLine("[DHEADER]");
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.ScriptCommand, ScriptCommand));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.StartOfMeasurement, StartOfMeasurement));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.EndOfMeasurement, EndOfMeasurement));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.CountTotal, CountTotal));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.Interval, Interval));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.FieldInAccessDatabase, FieldInAccessDatabase));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.ObjectName, ObjectName));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.MeasurePoint, Measurepoint));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.Value, Value));

            return result.ToString();
        }

    }
}
