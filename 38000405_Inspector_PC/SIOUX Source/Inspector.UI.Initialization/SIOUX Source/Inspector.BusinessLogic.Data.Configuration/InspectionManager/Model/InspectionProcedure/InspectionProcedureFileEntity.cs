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
    /// InspectionProcedureFileEntity
    /// </summary>
    [XmlRoot(ElementName = "InspectionProcedureFile")]
    public class InspectionProcedureFileEntity
    {
        /// <summary>
        /// Gets or sets the inspection procedures.
        /// </summary>
        /// <value>The inspection procedures.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), XmlElement(ElementName = "InspectionProcedure")]
        public List<InspectionProcedureEntity> InspectionProcedures { get; set; }
    }
}
