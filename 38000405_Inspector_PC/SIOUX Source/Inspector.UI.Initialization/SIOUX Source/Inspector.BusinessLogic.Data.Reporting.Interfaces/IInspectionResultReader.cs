/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using Inspector.Model.InspectionReportingResults;

namespace Inspector.BusinessLogic.Data.Reporting.Interfaces
{
    /// <summary>
    /// InspectionResultReader interface, reads of the last two results from the result.xml and lastresult.xml
    /// </summary>
    public interface IInspectionResultReader
    {
        /// <summary>
        /// Looks up the last inspection result.
        /// </summary>
        /// <returns>The last inspection result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ReportInspectionResult LookupLastResult(string prsName, string gclName = null);

        /// <summary>
        /// Looks up the previous to last inspection result.
        /// </summary>
        /// <returns>The last to previous inspection result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ReportInspectionResult LookupPreviousToLastResult(string prsName, string gclName = null);

        /// <summary>
        /// Looks up the last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>The last report result that matches the criteria, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ReportResult LookupLastReportResult(string prsName, string measurePoint, string objectName, int? FieldNumber = null);

        /// <summary>
        /// Looks up the last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>The last report result that matches the criteria, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ReportResult LookupLastReportResult(string prsName, string gclName, string measurePoint, string objectName, int? FieldNumber = null);

        /// <summary>
        /// Looks up the previous to last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>The last report result that matches the criteria, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ReportResult LookupPreviousToLastReportResult(string prsName, string measurePoint, string objectName, int? FieldNumber = null);

        /// <summary>
        /// Looks up the previous to last report result.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="FieldNumber">The field number (optional).</param>
        /// <returns>The last report result that matches the criteria, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ReportResult LookupPreviousToLastReportResult(string prsName, string gclName, string measurePoint, string objectName, int? FieldNumber = null);
    }
}
