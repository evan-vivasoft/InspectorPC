/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Inspector.Model;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure
{
    /// <summary>
    /// ScriptCommand4Entity
    /// </summary>
    [XmlRoot("Scriptcommand_4")]
    public class ScriptCommandEntityDescriptions : ScriptCommandEntityBase
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

        [XmlIgnore]
        public Guid InspectionPointId { get; set; }

        /// <summary>
        /// Gets or sets the object name description.
        /// </summary>
        /// <value>
        /// The object name description.
        /// </value>
        [XmlElement(ElementName = "ObjectNameDescription")]
        public string ObjectNameDescription { get; set; }

        /// <summary>
        /// Gets or sets the measure point description.
        /// </summary>
        /// <value>
        /// The measure point description.
        /// </value>
        [XmlElement(ElementName = "MeasurePointDescription")]
        public string MeasurePointDescription { get; set; }
    }
}
