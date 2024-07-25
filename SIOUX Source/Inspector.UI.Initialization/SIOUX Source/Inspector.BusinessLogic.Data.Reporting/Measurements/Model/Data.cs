/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements.Model
{
    /// <summary>
    /// Model representing the measurement values
    /// </summary>
    internal class Data
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        /// <value>
        /// The measurement values.
        /// </value>
        public List<double> MeasurementValues { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="measurementUnit">The measurement unit.</param>
        public Data(string measurementUnit)
        {
            MeasurementValues = new List<double>();
            Unit = measurementUnit;
        }
        #endregion Constructors

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("[DATA]");
            foreach (double measurementValue in MeasurementValues)
            {
                result.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", measurementValue, Unit));
            }

            return result.ToString();
        }
    }
}
