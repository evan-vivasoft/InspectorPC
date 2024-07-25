/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.BusinessLogic.Data.Reporting.Results
{
    /// <summary>
    /// Report constants for report control and reader
    /// </summary>
    internal static class ReportConstants
    {
        #region Constants
        /// <summary>
        /// Temporary file to store the report results by inspector before writing it to Results.xml 
        /// This prevents potential data-loss on a crash as it still available in the tmpResult.xml file
        /// </summary>
        internal const string TMPRESULTS_FILENAME = @"tmpResults.xml";
        /// <summary>
        /// All results that are collected with inspector are stored in here
        /// </summary>
        internal const string INSPECTOR_RESULTS_FILENAME = @"Results.xml";
        /// <summary>
        /// Externally provided file that also contains inspection results in the same format as inspector stored it (share XSD_FILENAME schema)
        /// </summary>
        internal const string RESULTS_LAST_FILENAME = @"ResultsLast.xml";
        /// <summary>
        /// The validation schema of the result XML format
        /// </summary>
        internal const string XSD_FILENAME = "InspectionResultsData.xsd";
        #endregion Constants
    }
}
