/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Xml.Serialization;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure
{
    /// <summary>
    /// ScriptCommand42Entity
    /// </summary>
    [XmlRoot("Scriptcommand_42")]
    public class ScriptCommand42Entity : ScriptCommandEntityBase
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
    }
}
