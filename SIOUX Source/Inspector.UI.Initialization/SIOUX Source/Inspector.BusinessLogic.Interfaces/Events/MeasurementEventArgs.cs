/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using Inspector.Model.InspectionMeasurement;

namespace Inspector.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// MeasurementEventArgs
    /// </summary>
    public class MeasurementEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the measurements.
        /// </summary>
        /// <value>The measurements.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<ScriptCommand5XMeasurement> Measurements { get; set; }
    }
}
