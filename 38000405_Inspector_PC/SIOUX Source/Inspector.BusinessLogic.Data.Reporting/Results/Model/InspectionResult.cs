/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    public class InspectionResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [XmlIgnore]
        public int? Status { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [XmlElement("Status")]
        public string StatusAsText
        {
            get { return (Status.HasValue) ? Status.ToString() : null; }
            set { Status = !string.IsNullOrWhiteSpace(value) ? int.Parse(value, CultureInfo.InvariantCulture) : default(int?); }
        }

        /// <summary>
        /// Gets or sets the PRS identification.
        /// </summary>
        /// <value>
        /// The PRS identification.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PRS")]
        public string PRSIdentification { get; set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>
        /// The name of the PRS.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PRS")]
        public string PRSName { get; set; }

        /// <summary>
        /// Gets or sets the PRS code.
        /// </summary>
        /// <value>
        /// The PRS code.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PRS")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GCL")]
        public string GCLIdentification { get; set; }

        /// <summary>
        /// Gets or sets the GCL code.
        /// </summary>
        /// <value>
        /// The GCL code.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GCL")]
        public string GCLCode { get; set; }

        /// <summary>
        /// Gets or sets the CRC.
        /// </summary>
        /// <value>
        /// The CRC.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CRC")]
        public string CRC { get; set; }

        /// <summary>
        /// Gets or sets the measurement_ equipment.
        /// </summary>
        /// <value>The measurement_ equipment.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        public MeasurementEquipment Measurement_Equipment { get; set; }

        /// <summary>
        /// Gets or sets the inspection procedure.
        /// </summary>
        /// <value>
        /// The inspection procedure.
        /// </value>
        public InspectionProcedure InspectionProcedure { get; set; }

        /// <summary>
        /// Gets or sets the date time stamp.
        /// </summary>
        /// <value>
        /// The date time stamp.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TimeStamp")]
        public DateTimeStamp DateTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), XmlElement("Result")]
        public List<Result> Results { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="InspectionResult"/> class from being created.
        /// </summary>
        private InspectionResult()
        {
            PRSIdentification = string.Empty;
            PRSName = string.Empty;
            PRSCode = string.Empty;
            CRC = string.Empty;
            InspectionProcedure = new Model.InspectionProcedure();
            Measurement_Equipment = new MeasurementEquipment();
            Results = new List<Result>();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="InspectionResult"/> class from being created.
        /// Required for XML Serialization
        /// </summary>
        /// <param name="startDate">The start date.</param>
        public InspectionResult(DateTime startDate)
            : this()
        {
            DateTimeStamp = new Model.DateTimeStamp(startDate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="startDate">The start date.</param>
        public InspectionResult(InspectionStatus status, DateTime startDate)
            : this(startDate)
        {
            SetInspectionStatus(status);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsIdentification">The PRS identification.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="prsCode">The PRS code.</param>
        /// <param name="gasControlLineName">Name of the gas control line.</param>
        /// <param name="gclIdentification">The GCL identification.</param>
        /// <param name="gclCode">The GCL code.</param>
        /// <param name="crc">The CRC.</param>
        /// <param name="measurementEquipment">The measurement equipment.</param>
        /// <param name="inspectionProcedure">The inspection procedure.</param>
        /// <param name="dateTimestamp">The date time stamp.</param>
        public InspectionResult(InspectionStatus status, string prsIdentification, string prsName, string prsCode, string gasControlLineName, string gclIdentification, string gclCode, string crc, MeasurementEquipment measurementEquipment, InspectionProcedure inspectionProcedure, DateTimeStamp dateTimestamp)
            : this()
        {
            Status = (int)status;
            PRSIdentification = prsIdentification;
            PRSName = prsName;
            PRSCode = prsCode;
            CRC = crc;
            GCLCode = gclCode;
            GasControlLineName = gasControlLineName;
            GCLIdentification = gclIdentification;
            Measurement_Equipment = measurementEquipment;
            InspectionProcedure = inspectionProcedure;
            DateTimeStamp = dateTimestamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsIdentification">The PRS identification.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="prsCode">The PRS code.</param>
        /// <param name="crc">The CRC.</param>
        /// <param name="inspectionProcedure">The inspection procedure.</param>
        /// <param name="dateTimestamp">The date time stamp.</param>
        public InspectionResult(InspectionStatus status, string prsIdentification, string prsName, string prsCode, string crc, InspectionProcedure inspectionProcedure, DateTimeStamp dateTimestamp)
            : this(status, prsIdentification, prsName, prsCode, null, null, null, crc, null, inspectionProcedure, dateTimestamp)
        {
        }

        /// <summary>
        /// Sets the inspection status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void SetInspectionStatus(InspectionStatus status)
        {
            Status = (int)status;
        }


        /// <summary>
        /// Sets the inspection result values.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsIdentification">The PRS identification.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="prsCode">The PRS code.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="gclCode">The GCL code.</param>
        /// <param name="gclIdentification">The GCL identification.</param>
        /// <param name="crc">The CRC.</param>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <param name="inspectionProcedureValue">The inspection procedure value.</param>
        public void SetInspectionResultValues(InspectionStatus status, string prsIdentification, string prsName, string prsCode, string gclName, string gclCode, string gclIdentification, string crc, string inspectionProcedureName, string inspectionProcedureValue)
        {
            SetInspectionStatus(status);
            PRSIdentification = prsIdentification;
            PRSName = prsName;
            PRSCode = prsCode;
            GasControlLineName = gclName;
            GCLIdentification = gclIdentification;
            GCLCode = gclCode;
            CRC = crc;
            InspectionProcedure = new InspectionProcedure(inspectionProcedureName, inspectionProcedureValue);
        }
    }
}
