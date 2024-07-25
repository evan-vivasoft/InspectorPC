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
    /// ScriptCommandEntityBase
    /// </summary>
    [XmlInclude(typeof(ScriptCommand1Entity))]
    [XmlInclude(typeof(ScriptCommand2Entity))]
    [XmlInclude(typeof(ScriptCommand3Entity))]
    [XmlInclude(typeof(ScriptCommand4Entity))]
    [XmlInclude(typeof(ScriptCommand41Entity))]
    [XmlInclude(typeof(ScriptCommand42Entity))]
    [XmlInclude(typeof(ScriptCommand43Entity))]
    [XmlInclude(typeof(ScriptCommand5XEntity))]
    [XmlInclude(typeof(ScriptCommand70Entity))]
    public abstract class ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets the sequence number of the scriptcommand.
        /// </summary>
        /// <value>The sequence number.</value>
        [XmlElement(ElementName = "SequenceNumber")]
        public long SequenceNumber { get; set; }
    }
}
