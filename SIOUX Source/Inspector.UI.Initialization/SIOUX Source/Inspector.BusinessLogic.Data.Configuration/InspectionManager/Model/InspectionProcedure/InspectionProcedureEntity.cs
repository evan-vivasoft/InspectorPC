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
    /// InspectionProcedureEntity
    /// </summary>
    [XmlRoot(ElementName = "InspectionProcedure")]
    public class InspectionProcedureEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the inspection sequence.
        /// </summary>
        /// <value>The inspection sequence.</value>
        //[XmlElement(ElementName="InspectionSequence")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        [XmlArrayItem("Scriptcommand_1", typeof(ScriptCommand1Entity))]
        [XmlArrayItem("Scriptcommand_2", typeof(ScriptCommand2Entity))]
        [XmlArrayItem("Scriptcommand_3", typeof(ScriptCommand3Entity))]
        [XmlArrayItem("Scriptcommand_4", typeof(ScriptCommand4Entity))]
        [XmlArrayItem("Scriptcommand_41", typeof(ScriptCommand41Entity))]
        [XmlArrayItem("Scriptcommand_42", typeof(ScriptCommand42Entity))]
        [XmlArrayItem("Scriptcommand_43", typeof(ScriptCommand43Entity))]
        [XmlArrayItem("Scriptcommand_5x", typeof(ScriptCommand5XEntity))]
        [XmlArrayItem("Scriptcommand_70", typeof(ScriptCommand70Entity))]
        public List<ScriptCommandEntityBase> InspectionSequence { get; set; }
    }
}
