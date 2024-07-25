/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using Inspector.Model;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    public class MeasureValue
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "UOM")]
        public UnitOfMeasurement UOM { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="MeasureValue"/> class from being created.
        /// </summary>
        private MeasureValue()
        {
            Value = 0.0;
            UOM = UnitOfMeasurement.Item;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="uom">The uom.</param>
        public MeasureValue(double value, UnitOfMeasurement uom)
        {
            Value = value;
            UOM = uom;
        }
    }
}
