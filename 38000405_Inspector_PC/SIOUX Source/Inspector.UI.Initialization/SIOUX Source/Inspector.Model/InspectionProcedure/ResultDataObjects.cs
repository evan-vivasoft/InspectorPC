/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Inspector.Model.InspectionProcedure
{

    /// <summary>
    /// enum for available MeterNumbers
    /// </summary>
    public enum MeterNumber
    {
        ID_DM1,
        ID_DM2
    }

    /// <summary>
    /// inspection statuses
    /// </summary>
    public enum InspectionStatus
    {
        [Description("Inspection status is unknown")]
        [XmlEnum(Name = "0")]
        Unset = 0,

        [Description("No inspection")]
        [XmlEnum(Name = "1")]
        NoInspection = 1,

        [Description("Inspection started but not completed")]
        [XmlEnum(Name = "2")]
        StartNotCompleted = 2,

        [Description("Inspection completed")]
        [XmlEnum(Name = "3")]
        Completed = 3,

        [Description("GCL or PRS deleted by user")]
        [XmlEnum(Name = "4")]
        GclOrPrsDeletedByUser = 4,

        [Description("Inspection completed, values out of limits")]
        [XmlEnum(Name = "5")]
        CompletedValueOutOfLimits = 5,

        [Description("No inspection found")]
        [XmlEnum(Name = "6")]
        NoInspectionFound = 6,

        [Description("Warning")]
        [XmlEnum(Name = "7")]
        Warning = 7,
    }

    /// <summary>
    /// Structure containing generic inspection procedure information
    /// </summary>
    public struct InspectionProcedureGenericInformation
    {
        public InspectionStatus InpectionStatus;
        public string PrsIdentification;
        public string PrsName;
        public string PrsCode;
        public string GclName;
        public string GclIdentification;
        public string GclCode;
        public string Crc;
        public string ProcedureName;
        public string ProcedureVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionProcedureGenericInformation"/> struct.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsIdentification">The PRS identification.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="prsCode">The PRS code.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="gclIdentification">The GCL identification.</param>
        /// <param name="gclCode">The GCL code.</param>
        /// <param name="crc">The CRC.</param>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <param name="inspectionProcedureVersion">The inspection procedure version.</param>
        public InspectionProcedureGenericInformation(InspectionStatus status, string prsIdentification, string prsName, string prsCode, string gclName, string gclIdentification, string gclCode, string crc, string inspectionProcedureName, string inspectionProcedureVersion)
        {
            InpectionStatus = status;
            PrsIdentification = prsIdentification;
            PrsName = prsName;
            PrsCode = prsCode;
            Crc = crc;
            ProcedureName = inspectionProcedureName;
            ProcedureVersion = inspectionProcedureVersion;
            GclName = gclName;
            GclIdentification = gclIdentification;
            GclCode = gclCode;
        }
    }

    /// <summary>
    /// InspectionProcedureStepResultMeasureValue
    /// </summary>
    public struct InspectionProcedureStepResultMeasureValue
    {
        public double Value;
        public UnitOfMeasurement UOM;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionProcedureStepResultMeasureValue"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="uom">The uom.</param>
        public InspectionProcedureStepResultMeasureValue(double value, UnitOfMeasurement uom)
        {
            Value = value;
            UOM = uom;
        }
    }


    /// <summary>
    /// InspectionProcedureStepResult
    /// </summary>
    public struct InspectionProcedureStepResult
    {
        public string ObjectName;

        public string ObjectId;

        public string MeasurePoint;

        public string MeasurePointId;

        public int? FieldNumber;

        public DateTime Executed;

        public InspectionProcedureStepResultMeasureValue? MeasureValue;

        public string Text;

        public List<string> Options;

        public long SequenceNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionProcedureStepResult"/> struct.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="executed">The executed.</param>
        /// <param name="fieldNo">The field no.</param>
        public InspectionProcedureStepResult(long sequenceNumber, string measurePoint, DateTime executed, int? fieldNo)
        {
            SequenceNumber = sequenceNumber;
            FieldNumber = fieldNo;
            Executed = executed;
            MeasurePoint = measurePoint;

            ObjectName = string.Empty;
            ObjectId = string.Empty;
            MeasurePointId = string.Empty;
            Text = string.Empty;
            Options = null;
            MeasureValue = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionProcedureStepResult"/> struct.
        /// </summary>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="executed">The executed.</param>
        /// <param name="fieldNo">The field no.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="measurePointId">The measure point id.</param>
        /// <param name="text">The text.</param>
        /// <param name="options">The options.</param>
        /// <param name="measureValue">The measure value.</param>
        public InspectionProcedureStepResult(long sequenceNumber, string measurePoint, DateTime executed, int? fieldNo, string objectName, string objectId, string measurePointId, string text, List<string> options = null, InspectionProcedureStepResultMeasureValue? measureValue = null)
        {
            SequenceNumber = sequenceNumber;
            FieldNumber = fieldNo;
            Executed = executed;
            MeasurePoint = measurePoint;
            ObjectName = objectName;
            ObjectId = objectId;
            MeasurePointId = measurePointId;
            Text = text;
            Options = options;
            MeasureValue = measureValue;
        }
    }
}
