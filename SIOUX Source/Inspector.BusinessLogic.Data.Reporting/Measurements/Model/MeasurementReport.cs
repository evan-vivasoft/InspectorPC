/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using Inspector.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements.Model
{
    /// <summary>
    /// Model representing a measurement result file
    /// </summary>
    internal class MeasurementReport
    {
        #region Properties
        /// <summary>
        /// Gets or sets the file header.
        /// </summary>
        /// <value>
        /// The file header.
        /// </value>
        public FileHeader FileHeader { get; set; }

        /// <summary>
        /// Gets or sets Sample Rate
        /// </summary>
        /// <value>
        /// Sample Rate
        /// </value>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets Interval
        /// </summary>
        /// <value>
        /// Interval
        /// </value>
        public int Interval { get; set; }

        /// <summary>
        /// Gets or sets Measure Count
        /// </summary>
        /// <value>
        /// Total number of measurements
        /// </value>
        public int MeasureCount { get; set; }

        /// <summary>
        /// Gets or sets UOM
        /// </summary>
        /// <value>
        /// Unit of Measurement
        /// </value>
        public UnitOfMeasurement UOM { get; set; }

        /// <summary>
        /// Gets or sets PRSId
        /// </summary>
        /// <value>
        /// PRS id
        /// </value>
        public Guid PRSId { get; set; }

        /// <summary>
        /// Gets or sets GCLId
        /// </summary>
        /// <value>
        /// GCL id
        /// </value>
        public Guid GCLId { get; set; }


        /// <summary>
        /// Gets or sets the measurements.
        /// </summary>
        /// <value>
        /// The measurements.
        /// </value>
        public List<Measurement> Measurements { get; set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>
        /// The name of the PRS.
        /// </value>
        public string PrsName { get; set; }

        /// <summary>
        /// Gets or sets the name of the GCL.
        /// </summary>
        /// <value>
        /// The name of the GCL.
        /// </value>
        public string GclName { get; set; }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        /// <value>
        /// The start date time.
        /// </value>
        public string StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date time.
        /// </summary>
        /// <value>
        /// The end date time.
        /// </value>
        public DateTime? EndDateTime { get; set; }
        #endregion Properties

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            if (FileHeader != null)
            {
                result.AppendLine(FileHeader.ToString());
            }

            if (Measurements != null)
            {
                foreach (Measurement measurement in Measurements)
                {
                    result.AppendLine(measurement.ToString());
                }
            }
            return result.ToString();
        }
    }
}
