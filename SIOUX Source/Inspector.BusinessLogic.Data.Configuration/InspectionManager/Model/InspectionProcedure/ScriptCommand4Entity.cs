/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Xml.Serialization;
using Inspector.Model;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure
{
    /// <summary>
    /// ScriptCommand4Entity
    /// </summary>
    [XmlRoot("Scriptcommand_4")]
    public class ScriptCommand4Entity : ScriptCommandEntityDescriptions
    {
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>The question.</value>
        [XmlElement(ElementName = "Question")]
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets the type question.
        /// </summary>
        /// <value>The type question.</value>
        [XmlElement(ElementName = "TypeQuestion")]
        public TypeQuestion TypeQuestion { get; set; }

        /// <summary>
        /// Gets or sets the text options.
        /// </summary>
        /// <value>The text options.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [XmlElement(ElementName = "TextOptions")]
        public List<string> TextOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand4Entity"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "Required")]
        public bool Required { get; set; }
    }
}
