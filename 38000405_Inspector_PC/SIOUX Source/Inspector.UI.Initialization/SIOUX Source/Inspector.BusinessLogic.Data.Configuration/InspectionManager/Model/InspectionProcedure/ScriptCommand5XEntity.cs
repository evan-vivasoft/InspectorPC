/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Xml.Serialization;
using Inspector.Model;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure
{
    /// <summary>
    /// ScriptCommand5XEntity
    /// </summary>
    [XmlRoot(ElementName = "ScriptCommand_5x")]
    public class ScriptCommand5XEntity : ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        [XmlElement(ElementName = "ObjectName")]
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the measure point.
        /// </summary>
        /// <value>The measure point.</value>
        [XmlElement(ElementName = "MeasurePoint")]
        public string MeasurePoint { get; set; }

        /// <summary>
        /// Gets or sets the field no.
        /// </summary>
        /// <value>The field no.</value>
        [XmlElement(ElementName = "FieldNo")]
        public int? FieldNo { get; set; }

        /// <summary>
        /// Gets or sets the scriptcommand5X.
        /// </summary>
        /// <value>The scriptcommand5X.</value>
        [XmlElement(ElementName = "Scriptcommand")]
        public ScriptCommand5XType ScriptCommand5X { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        /// <value>The instruction.</value>
        [XmlElement(ElementName = "Instruction")]
        public string Instruction { get; set; }

        [XmlElement(ElementName = "DigitalManometer")]
        public DigitalManometer DigitalManometer { get; set; }

        [XmlElement(ElementName = "MeasurementFrequency")]
        public TypeMeasurementFrequency MeasurementFrequency { get; set; }

        [XmlElement(ElementName = "MeasurementPeriod")]
        public int MeasurementPeriod { get; set; }

        [XmlElement(ElementName = "ExtraMeasurementPeriod")]
        public int ExtraMeasurementPeriod { get; set; }

        [XmlElement(ElementName = "Leakage")]
        public Leakage Leakage { get; set; }
    }
}
