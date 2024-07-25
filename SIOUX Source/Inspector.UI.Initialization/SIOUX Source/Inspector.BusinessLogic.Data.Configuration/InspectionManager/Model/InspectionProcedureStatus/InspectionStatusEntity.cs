/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Xml.Serialization;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedureStatus
{
    /// <summary>
    /// Contains a collection of inspection status information
    /// </summary>
    [XmlRoot(ElementName = "Status")]
    public class InspectionStatusesEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        [XmlElement(ElementName = "InspectionStatus")]
        public List<InspectionStatusEntity> InspectionStatuses { get; set; }
    }

    /// <summary>
    /// InspectionStatusEntity, represents the XML data structure of an inspection status
    /// </summary>
    [XmlRoot(ElementName = "InspectionStatus")]
    public class InspectionStatusEntity
    {
        /// <summary>
        /// Gets or sets the PRS identification.
        /// </summary>
        /// <value>The PRS identification.</value>
        [XmlElement(ElementName = "PRSIdentification")]
        public string PRSIdentification { get; set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>The name of the PRS.</value>
        [XmlElement(ElementName = "PRSName")]
        public string PRSName { get; set; }

        /// <summary>
        /// Gets or sets the PRS code.
        /// </summary>
        /// <value>The PRS code.</value>
        [XmlElement(ElementName = "PRSCode")]
        public string PRSCode { get; set; }

        /// <summary>
        /// Gets or sets the GCL identification.
        /// </summary>
        /// <value>The GCL identification.</value>
        [XmlElement(ElementName = "GCLIdentification")]
        public string GCLIdentification { get; set; }

        /// <summary>
        /// Gets or sets the name of the GCL.
        /// </summary>
        /// <value>The name of the GCL.</value>
        [XmlElement(ElementName = "GasControlLineName")]
        public string GCLName { get; set; }

        /// <summary>
        /// Gets or sets the GCL code.
        /// </summary>
        /// <value>The GCL code.</value>
        [XmlElement(ElementName = "GCLCode")]
        public string GCLCode { get; set; }

        /// <summary>
        /// Gets or sets the status ID.
        /// </summary>
        /// <value>The status ID.</value>
        [XmlElement(ElementName = "StatusID")]
        public InspectionStatus StatusID { get; set; }
    }
}
