/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionMeasurement
{
    /// <summary>
    /// Measurement result that contains all measured and calculated values
    /// </summary>
    public class ScriptCommand5XMeasurement
    {
        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>The sequence number.</value>
        public long SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the measurement.
        /// </summary>
        /// <value>The measurement.</value>
        public double Measurement { get; set; }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public double Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public double Maximum { get; set; }

        /// <summary>
        /// Gets or sets the average.
        /// </summary>
        /// <value>The average.</value>
        public double Average { get; set; }

        /// <summary>
        /// Gets or sets the leakage value.
        /// </summary>
        /// <value>
        /// The leakage value.
        /// </value>
        public double LeakageValue { get; set; }

        /// <summary>
        /// Gets or sets the leakage v1.
        /// </summary>
        /// <value>The leakage v1.</value>
        public double LeakageV1 { get; set; }

        /// <summary>
        /// Gets or sets the leakage v2.
        /// </summary>
        /// <value>The leakage v2.</value>
        public double LeakageV2 { get; set; }

        /// <summary>
        /// Gets or sets the leakage membrane.
        /// </summary>
        /// <value>The leakage membrane.</value>
        public double LeakageMembrane { get; set; }

        /// <summary>
        /// Gets or sets the io status.
        /// </summary>
        /// <value>
        /// The io status.
        /// </value>
        public int IoStatus { get; set; }
    }
}
