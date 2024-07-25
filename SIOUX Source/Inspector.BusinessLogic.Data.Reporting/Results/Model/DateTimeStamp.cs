/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Globalization;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TimeStamp")]
    public class DateTimeStamp
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DATE_FORMAT = "yyyy-MM-dd";
        /// <summary>
        /// 
        /// </summary>
        public const string TIME_FORMAT = "HH:mm:ss";

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
        public TimeSetting TimeSettings { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeStamp"/> class.
        /// </summary>
        private DateTimeStamp()
        {
            StartDate = string.Empty;
            StartTime = string.Empty;
            EndTime = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeStamp"/> class.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        public DateTimeStamp(DateTime startDate)
        {
            TimeSettings = new TimeSetting(startDate);

            StartDate = startDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
            StartTime = startDate.ToString(TIME_FORMAT, CultureInfo.InvariantCulture);
            EndTime = startDate.ToString(TIME_FORMAT, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="DateTimeStamp"/> class from being created.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endTime">The start time.</param>
        public DateTimeStamp(DateTime startDate, DateTime endTime)
            : this(startDate)
        {
            EndTime = endTime.ToString(TIME_FORMAT, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the end time.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        public void SetEndTime(DateTime endTime)
        {
            EndTime = endTime.ToString(TIME_FORMAT, CultureInfo.InvariantCulture);
        }
    }
}
