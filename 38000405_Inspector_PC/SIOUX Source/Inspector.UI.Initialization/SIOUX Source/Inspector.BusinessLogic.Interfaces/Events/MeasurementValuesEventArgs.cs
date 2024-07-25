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
    /// MeasurementValuesEventArgs
    /// </summary>
    public class MeasurementValuesEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        /// <value>The measurement values.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<ScriptCommand5XMeasurement> MeasurementValues { get; set; }
    }
}