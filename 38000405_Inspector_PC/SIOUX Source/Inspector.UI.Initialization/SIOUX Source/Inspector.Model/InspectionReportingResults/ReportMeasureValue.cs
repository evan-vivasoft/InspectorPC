/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionReportingResults
{
    /// <summary>
    /// ReportMeasureValue, 
    /// </summary>
    public class ReportMeasureValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the UOM.
        /// </summary>
        /// <value>
        /// The UOM.
        /// </value>
        public UnitOfMeasurement UOM { get; set; }
    }
}
