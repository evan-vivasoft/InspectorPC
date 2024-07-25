/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    public class TimeSetting
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DST")]
        public string DST
        {
            get;
            set;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="TimeSetting"/> class from being created.
        /// Required for XML Serialization
        /// </summary>
        private TimeSetting()
        {
            TimeZone = "GMT";
            DST = BoolToYesNo(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSetting"/> class.
        /// </summary>
        /// <param name="offset">The off set.</param>
        public TimeSetting(DateTime offset)
        {
            DST = BoolToYesNo(offset.IsDaylightSavingTime());
            TimeZone = SetTimeZoneFormat(offset);
        }

        /// <summary>
        /// Sets the time zone format in GMT+/- on whole hours
        /// </summary>
        /// <param name="offSet">The off set.</param>
        /// <returns></returns>
        private static string SetTimeZoneFormat(DateTime offSet)
        {
            StringBuilder gmtTimeFormat = new StringBuilder("GMT");
            {
                TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(offSet);
                if (utcOffset.Ticks != 0)
                {
                    if (utcOffset.Hours < 0)
                    {
                        gmtTimeFormat.Append("-");
                    }
                    else
                    {
                        gmtTimeFormat.Append("+");
                    }

                    int hourAbsolute = Math.Abs(utcOffset.Hours);
                    if (hourAbsolute < 10)
                    {
                        gmtTimeFormat.Append("0");
                    }
                    gmtTimeFormat.Append(hourAbsolute);
                    gmtTimeFormat.Append(":00");
                }
            }

            return gmtTimeFormat.ToString();
        }

        /// <summary>
        /// Bools to yes no.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>Yes No</returns>
        private static string BoolToYesNo(bool value)
        {
            string retval = "No";

            if (value)
            {
                retval = "Yes";
            }

            return retval;
        }
    }
}
