/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionReportingResults
{
    /// <summary>
    /// ReportTimeSetting
    /// </summary>
    public class ReportTimeSetting
    {
        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public string TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DST.
        /// </summary>
        /// <value>
        /// The DST.
        /// </value>
        public string DST
        {
            get;
            set;
        }
    }
}
