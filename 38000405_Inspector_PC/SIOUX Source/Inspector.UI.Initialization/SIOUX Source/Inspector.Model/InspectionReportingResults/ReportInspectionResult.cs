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
    /// ReportInspectionResult, contains the results of an inspection procedure
    /// </summary>
    public class ReportInspectionResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the PRS identification.
        /// </summary>
        /// <value>
        /// The PRS identification.
        /// </value>
        public string PRSIdentification { get; set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>
        /// The name of the PRS.
        /// </value>
        public string PRSName { get; set; }

        /// <summary>
        /// Gets or sets the PRS code.
        /// </summary>
        /// <value>
        /// The PRS code.
        /// </value>
        public string PRSCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the gas control line.
        /// </summary>
        /// <value>
        /// The name of the gas control line.
        /// </value>
        public string GasControlLineName { get; set; }

        /// <summary>
        /// Gets or sets the GCL identification.
        /// </summary>
        /// <value>
        /// The GCL identification.
        /// </value>
        public string GCLIdentification { get; set; }

        /// <summary>
        /// Gets or sets the GCL code.
        /// </summary>
        /// <value>
        /// The GCL code.
        /// </value>
        public string GCLCode { get; set; }

        /// <summary>
        /// Gets or sets the CRC.
        /// </summary>
        /// <value>
        /// The CRC.
        /// </value>
        public string CRC { get; set; }

        /// <summary>
        /// Gets or sets the measurement_ equipment.
        /// </summary>
        /// <value>The measurement_ equipment.</value>
        public ReportMeasurementEquipment Measurement_Equipment { get; set; }

        /// <summary>
        /// Gets or sets the name of the inspection procedure.
        /// </summary>
        /// <value>The name of the inspection procedure.</value>
        public string InspectionProcedureName { get; set; }

        /// <summary>
        /// Gets or sets the inspection procedure version.
        /// </summary>
        /// <value>The inspection procedure version.</value>
        public string InspectionProcedureVersion { get; set; }

        /// <summary>
        /// Gets or sets the date time stamp.
        /// </summary>
        /// <value>
        /// The date time stamp.
        /// </value>
        public ReportDateTimeStamp DateTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public List<ReportResult> Results { get; set; }
    }
}
