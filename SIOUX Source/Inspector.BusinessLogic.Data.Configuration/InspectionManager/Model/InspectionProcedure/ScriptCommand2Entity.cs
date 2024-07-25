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
    /// ScriptCommand2Entity
    /// </summary>
    [XmlRoot(ElementName = "Scriptcommand_2")]
    public class ScriptCommand2Entity : ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets the section.
        /// </summary>
        /// <value>The section.</value>
        [XmlElement(ElementName = "Section")]
        public string Section { get; set; }

        /// <summary>
        /// Gets or sets the sub section.
        /// </summary>
        /// <value>The sub section.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SubSection")]
        [XmlElement(ElementName = "SubSection")]
        public string SubSection { get; set; }
    }
}
