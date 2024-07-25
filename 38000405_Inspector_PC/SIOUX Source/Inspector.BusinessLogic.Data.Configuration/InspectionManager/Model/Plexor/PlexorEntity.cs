/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Plexor
{
    /// <summary>
    /// Contains a collection of plexor information
    /// </summary>
    [XmlRoot(ElementName = "PLEXORS")]
    public class PlexorsEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [XmlElement(ElementName = "PLEXOR")]
        public List<PlexorEntity> Plexors { get; set; }
    }

    /// <summary>
    /// PlexorEntity
    /// </summary>
    [XmlRoot(ElementName = "PLEXOR")]
    public class PlexorEntity
    {
        /// <summary>
        /// The Plexor's Name
        /// </summary>
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// The Plexor's Serial Number
        /// </summary>
        [XmlElement(ElementName = "SerialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The Plexor's BlueTooth Address
        /// </summary>
        [XmlElement(ElementName = "BTAddress")]
        public string BlueToothAddress { get; set; }

        /// <summary>
        /// The Plexor's PN
        /// </summary>
        [XmlElement(ElementName = "PN")]
        public string PN { get; set; }

        /// <summary>
        /// The Plexor's Calibration Date
        /// </summary>
        [XmlElement(ElementName = "CalibrationDate")]
        public DateTime CalibrationDate { get; set; }
    }
}
