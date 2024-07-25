/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.Model.MeasurementResult
{
    public class MeasurementMetadata
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
        /// Gets or sets the end of measurement.
        /// </summary>
        /// <value>
        /// The end of measurement.
        /// </value>
        public DateTime? EndOfMeasurement { get; set; }

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

    }
}
