/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using Inspector.Model;

namespace Inspector.Hal.Interfaces.Events
{
    /// <summary>
    /// MeasurementsReceivedEventArgs
    /// </summary>
    public class MeasurementsReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the measurements.
        /// </summary>
        public IList<Measurement> Measurements { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementsReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        public MeasurementsReceivedEventArgs(IList<Measurement> measurements)
        {
            this.Measurements = measurements;
        }
    }
}
