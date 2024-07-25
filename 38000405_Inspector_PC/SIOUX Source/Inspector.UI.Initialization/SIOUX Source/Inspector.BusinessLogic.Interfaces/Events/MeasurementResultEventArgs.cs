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
        /// Initializes a new instance of the <see cref="MeasurementResultEventArgs"/> class.
        /// </summary>
        /// <param name="measurementValue">The measurement value.</param>
        /// <param name="measurementValueOutOfLimits">if set to <c>true</c> [measurement value out of limits].</param>
        public MeasurementResultEventArgs(double measurementValue, bool measurementValueOutOfLimits)
        {
            MeasurementValue = measurementValue;
            MeasurementValueOutOfLimits = measurementValueOutOfLimits;
        }
    }
}
