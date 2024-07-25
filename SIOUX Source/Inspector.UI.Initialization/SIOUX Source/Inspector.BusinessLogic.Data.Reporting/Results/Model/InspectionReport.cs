/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    [XmlType("InspectionResultsData")]
    public class InspectionReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionReport"/> class.
        /// </summary>
        public InspectionReport()
        {
            InspectionResults = new List<InspectionResult>();
        }

        /// <summary>
        /// Gets or sets the inspection results.
        /// </summary>
        /// <value>
        /// The inspection results.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), XmlElement("InspectionResult")]
        public List<InspectionResult> InspectionResults { get; set; }
    }
}
