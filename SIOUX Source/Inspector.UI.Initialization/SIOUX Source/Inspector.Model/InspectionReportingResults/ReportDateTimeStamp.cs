/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionReportingResults
{
    /// <summary>
    /// ReportDateTimeStamp
    /// </summary>
    public class ReportDateTimeStamp
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public string StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public string StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        public string EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time settings.
        /// </summary>
        /// <value>
        /// The time settings.
        /// </value>
        public ReportTimeSetting TimeSettings { get; set; }
    }
}
