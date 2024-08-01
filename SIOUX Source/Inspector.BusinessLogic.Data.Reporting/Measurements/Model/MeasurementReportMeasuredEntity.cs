/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements.Model
{
    /// <summary>
    /// Model representing the measurement values
    /// </summary>
    /// 
    [SuppressMessage("Microsoft.Naming", "CA1702", MessageId = "TimeStamp", Justification = "Reviewed: Name is intended to be 'TimeStamp'")]
    [SuppressMessage("Microsoft.Naming", "CA1703", MessageId = "Data", Justification = "Reviewed: Data is a valid identifier in this context.")]
    [SuppressMessage("Microsoft.Naming", "CA1701", MessageId = "Data", Justification = "Reviewed: Data is a valid identifier in this context.")]
    [SuppressMessage("Microsoft.Naming", "CA2227", MessageId = "TimeStamp", Justification = "Reviewed: Name is intended to be 'TimeStamp'")]
    public class MeasurementReportMeasuredEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum value time stamp.
        /// </summary>
        /// <value>
        /// The maximum value time stamp.
        /// </value>
        /// 
        [SuppressMessage("Microsoft.Naming", "CA1702", MessageId = "TimeStamp", Justification = "Reviewed: Name is intended to be 'TimeStamp'")]
        [SuppressMessage("Microsoft.Naming", "CA1703", MessageId = "Data", Justification = "Reviewed: Data is a valid identifier in this context.")]
        [SuppressMessage("Microsoft.Naming", "CA1701", MessageId = "Data", Justification = "Reviewed: Data is a valid identifier in this context.")]

        public DateTime MaxValueTimeStamp{ get; set; }

        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        /// <value>
        /// The measurement values.
        /// </value>
        /// 
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Reviewed: List<Measurement> is appropriate for this context.")]
        public List<Inspector.Model.Measurement> MeasurementValues { get;}

        /// <summary>
        /// Gets or sets the measurement values.
        /// </summary>
        /// <value>
        /// The measurement values.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Reviewed: List<Measurement> is appropriate for this context.")]
        public List<Inspector.Model.Measurement> ExtraMeasurementValues { get;}

        /// <summary>
        /// Gets or sets a value indicating whether [report io status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [report io status]; otherwise, <c>false</c>.
        /// </value>
        public bool ReportIoStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [maximum value registered].
        /// </summary>
        /// <value>
        /// <c>true</c> if [maximum value registered]; otherwise, <c>false</c>.
        /// </value>
        public bool MaxValueRegistered { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementReportMeasuredEntity"/> class.
        /// </summary>
        /// <param name="measurementUnit">The measurement unit.</param>
        public MeasurementReportMeasuredEntity(string measurementUnit, bool reportIoStatus)
        {
            MeasurementValues = new List<Inspector.Model.Measurement>();
            ExtraMeasurementValues = new List<Inspector.Model.Measurement>();
            Unit = measurementUnit;
            ReportIoStatus = reportIoStatus;
            StartTime = DateTime.Now;
            MaxValueTimeStamp = DateTime.MinValue;
        }

        #endregion Constructors

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            MaxValueRegistered = false;
            result.AppendLine("[DATA]");
            AddValues(MeasurementValues, result);
            if (ExtraMeasurementValues.Count > 0)
            {
                result.AppendLine(string.Format(CultureInfo.InvariantCulture, "Start extra sample time {0}", StartTime));
                AddValues(ExtraMeasurementValues, result);
            }
            return result.ToString();
        }

        private void AddValues(List<Inspector.Model.Measurement> values, StringBuilder result)
        {
            var canReportMax = false;
            foreach (var measurementValue in values)
            {
                if (ReportIoStatus)
                {
                    result.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} | {2}", measurementValue.Value, Unit, measurementValue.IoStatus));
                }
                else
                {
                    result.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0} {1}", measurementValue.Value, Unit));
                }

                if (measurementValue.IoStatus != 7)
                {
                    canReportMax = true;
                }
                if (canReportMax && !MaxValueRegistered && measurementValue.IoStatus == 7 && MaxValueTimeStamp != DateTime.MinValue)
                {
                    MaxValueRegistered = true;
                    result.AppendLine(string.Format(CultureInfo.InvariantCulture, "Registered Max result {0}", MaxValueTimeStamp.ToString("HH:mm:ss", CultureInfo.InvariantCulture)));
                }
            }
        }
    }
}
