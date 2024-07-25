/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;

namespace Inspector.Model.InspectionReportingResults
{
    /// <summary>
    /// ReportingResult, a single result of an inspection procedure
    /// </summary>
    public class ReportResult
    {
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string ObjectName { get; set; }


        /// <summary>
        /// Gets or sets the object name description.
        /// </summary>
        /// <value>
        /// The object name description.
        /// </value>
        public string ObjectNameDescription { get; set; }

        /// <summary>
        /// Gets or sets the object ID.
        /// </summary>
        /// <value>
        /// The object ID.
        /// </value>
        public string ObjectID { get; set; }

        /// <summary>
        /// Gets or sets the measure point.
        /// </summary>
        /// <value>
        /// The measure point.
        /// </value>
        public string MeasurePoint { get; set; }

        /// <summary>
        /// Gets or sets the measure point description.
        /// </summary>
        /// <value>
        /// The measure point description.
        /// </value>
        public string MeasurePointDescription { get; set; }

        /// <summary>
        /// Gets or sets the measure point ID.
        /// </summary>
        /// <value>
        /// The measure point ID.
        /// </value>
        public string MeasurePointID { get; set; }

        /// <summary>
        /// Gets or sets the field no.
        /// </summary>
        /// <value>
        /// The field no.
        /// </value>
        public int? FieldNo { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets the measure value.
        /// </summary>
        /// <value>
        /// The measure value.
        /// </value>
        public ReportMeasureValue MeasureValue { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public List<string> List { get; set; }
    }
}
