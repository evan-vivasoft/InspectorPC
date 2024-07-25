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
    /// ScriptCommand1Entity
    /// </summary>
    [XmlRoot(ElementName = "Scriptcommand_1")]
    public class ScriptCommand1Entity : ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }
    }
}
