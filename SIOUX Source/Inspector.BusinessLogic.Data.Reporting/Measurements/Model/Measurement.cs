/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Management.Instrumentation;
using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements.Model
{
    /// <summary>
    /// 
    /// </summary>
    internal class Measurement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public MeasurementReportMeasuredEntity Data { get; set; }

        /// <summary>
        /// Gets or sets a Sample Rate
        /// </summary>
        /// <value>
        /// Sample Rate
        /// </value>
        public double SampleRate { get; set; }

        /// <summary>
        /// Gets or sets a Interval
        /// </summary>
        /// <value>
        /// Interval
        /// </value>
        public double Interval { get; set; }

        /// <summary>
        /// Gets or sets the LinkId.
        /// </summary>
        /// <value>
        /// LinkId. This id is used for mapping result data and measuement data after completion of inspection
        /// </value>
        public Guid LinkId { get; set; }

        /// <summary>
        /// Gets or sets the data header.
        /// </summary>
        /// <value>
        /// The data header.
        /// </value>
        public DataHeader DataHeader { get; set; }
        #endregion Properties

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var result = new StringBuilder();

            if (Data != null)
            {
                result.Append(Data);
            }
        
            result.AppendLine();

            if (DataHeader != null)
            {
                result.AppendLine(DataHeader.ToString());
            }

            return result.ToString();
        }
    }
}
