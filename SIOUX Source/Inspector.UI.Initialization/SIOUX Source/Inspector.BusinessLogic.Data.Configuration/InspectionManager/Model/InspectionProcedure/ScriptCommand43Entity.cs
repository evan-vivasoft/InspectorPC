/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure
{
    /// <summary>
    /// ScriptCommand43Entity
    /// </summary>
    [XmlRoot("Scriptcommand_43")]
    public class ScriptCommand43Entity : ScriptCommandEntityBase
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
        /// Gets or sets the instruction.
        /// </summary>
        /// <value>The instruction.</value>
        [XmlElement(ElementName = "Instruction")]
        public string Instruction { get; set; }

        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        /// <value>The list items.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [XmlElement(ElementName = "ListItem")]
        public List<string> ListItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand43Entity"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "Required")]
        public bool Required { get; set; }
    }
}
