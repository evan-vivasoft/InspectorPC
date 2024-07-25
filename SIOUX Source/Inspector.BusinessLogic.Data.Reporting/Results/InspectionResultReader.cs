/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Data.Reporting.Results.Model;
using Inspector.Infra.Utils;
using Inspector.Model.InspectionReportingResults;

namespace Inspector.BusinessLogic.Data.Reporting.Results
{
    /// <summary>
    /// Reader that looks up the last or previous to last results from
    /// Result.xml and LastResult.xml
    /// </summary>
    public class InspectionResultReader : IInspectionResultReader
    {
        public ReportInspectionResult LookupLastResult(string prsName, string gclName = null)
        {
            return InspectionReportsData.LookupLastResult(prsName, gclName);
        }

        public ReportInspectionResult LookupPreviousToLastResult(string prsName, string gclName = null)
        {
            return InspectionReportsData.LookupPreviousToLastResult(prsName, gclName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<ReportInspectionResult> LookupAllResults(string prsName, string gclName = null)
        {
            return InspectionReportsData.LookupAllResults(prsName, gclName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupLastReportResult(string prsName, string measurePoint, string objectName, int? FieldNumber = null)
        {
            return InspectionReportsData.LookupLastReportResult(prsName, measurePoint, objectName, FieldNumber);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupLastReportResult(string prsName, string gclName, string measurePoint, string objectName,
            int? FieldNumber = null)
        {
            return InspectionReportsData.LookupLastReportResult(prsName, gclName, measurePoint, objectName, FieldNumber);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupPreviousToLastReportResult(string prsName, string measurePoint, string objectName,
            int? FieldNumber = null)
        {
            return InspectionReportsData.LookupPreviousToLastReportResult(prsName, measurePoint, objectName, FieldNumber);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public ReportResult LookupPreviousToLastReportResult(string prsName, string gclName, string measurePoint, string objectName,
            int? FieldNumber = null)
        {
            return InspectionReportsData.LookupPreviousToLastReportResult(prsName, gclName, measurePoint, objectName,
                FieldNumber);
        }
    }
}
