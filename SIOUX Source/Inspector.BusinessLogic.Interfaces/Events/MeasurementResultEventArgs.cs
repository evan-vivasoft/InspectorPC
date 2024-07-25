/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;

namespace Inspector.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// MeasurementResultEventArgs
    /// </summary>
    public class MeasurementResultEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the measurement value.
        /// </summary>
        /// <value>
        /// The measurement value.
        /// </value>
        public double MeasurementValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [measurement value out of limits].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [measurement value out of limits]; otherwise, <c>false</c>.
        /// </value>
        public bool MeasurementValueOutOfLimits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the measurement has a value.
        /// When the user manually stops the measurement, while there was a valsensor, this value will be true.
        /// </summary>
        public bool HasMeasurementValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementResultEventArgs"/> class.
        /// </summary>
        /// <param name="measurementValue">The measurement value.</param>
        /// <param name="measurementValueOutOfLimits">if set to <c>true</c> [measurement value out of limits].</param>
        /// <param name="hasMeasurementValue">if set to <c>true</c> [measurement has a value].</param>
        public MeasurementResultEventArgs(double measurementValue, bool measurementValueOutOfLimits, bool hasMeasurementValue)
        {
            MeasurementValue = measurementValue;
            MeasurementValueOutOfLimits = measurementValueOutOfLimits;
            HasMeasurementValue = hasMeasurementValue;
        }
    }
}
