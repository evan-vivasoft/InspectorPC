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
    /// ScriptCommand3Entity
    /// </summary>
    [XmlRoot(ElementName = "Scriptcommand_3")]
    public class ScriptCommand3Entity : ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        [XmlElement(ElementName = "Duration")]
        public int Duration { get; set; }
    }
}
