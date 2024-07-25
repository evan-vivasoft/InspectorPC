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
    /// ScriptCommand70Entity
    /// </summary>
    [XmlRoot(ElementName = "Scriptcommand_70")]
    public class ScriptCommand70Entity : ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand70Entity"/> is mode.
        /// </summary>
        /// <value><c>true</c> if mode; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "Mode")]
        public bool Mode { get; set; }
    }
}
