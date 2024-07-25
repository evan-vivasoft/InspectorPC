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
    /// ScriptCommand41Entity
    /// </summary>
    [XmlRoot("Scriptcommand_41")]
    public class ScriptCommand41Entity : ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        [XmlElement(ElementName = "ObjectName")]
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the measure point.
        /// </summary>
        /// <value>The measure point.</value>
        [XmlElement(ElementName = "MeasurePoint")]
        public string MeasurePoint { get; set; }

        /// <summary>
        /// Gets or sets the field no.
        /// </summary>
        /// <value>The field no.</value>
        [XmlElement(ElementName = "FieldNo")]
        public int? FieldNo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show next list immediatly].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show next list immediatly]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement(ElementName = "ShowNextListImmediatly")]
        public bool ShowNextListImmediatly { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [XmlElement(ElementName = "List")]
        public List<ScriptCommand41ListEntity> ScriptCommandList { get; set; }
    }

    /// <summary>
    /// ScriptCommand41ListEntity
    /// </summary>
    public class ScriptCommand41ListEntity
    {
        /// <summary>
        /// Gets or sets the sequence number list.
        /// </summary>
        /// <value>The sequence number list.</value>
        [XmlElement(ElementName = "SequenceNumberList")]
        public long SequenceNumberList { get; set; }

        /// <summary>
        /// Gets or sets the type of the list.
        /// </summary>
        /// <value>The type of the list.</value>
        [XmlElement(ElementName = "ListType")]
        public ListType ListType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [selection required].
        /// </summary>
        /// <value><c>true</c> if [selection required]; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "SelectionRequired")]
        public bool SelectionRequired { get; set; }

        /// <summary>
        /// Gets or sets the list question.
        /// </summary>
        /// <value>The list question.</value>
        [XmlElement(ElementName = "ListQuestion")]
        public string ListQuestion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [one selection allowed].
        /// </summary>
        /// <value><c>true</c> if [one selection allowed]; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "OneSelectionAllowed")]
        public bool OneSelectionAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [check list result].
        /// </summary>
        /// <value><c>true</c> if [check list result]; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CheckList")]
        [XmlElement(ElementName = "CheckListResult")]
        public bool CheckListResult { get; set; }

        /// <summary>
        /// Gets or sets the list condition codes.
        /// </summary>
        /// <value>The list condition codes.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [XmlElement(ElementName = "ListConditionCode")]
        public List<ListConditionCodeEntity> ListConditionCodes { get; set; }
    }

    /// <summary>
    /// ListConditionCodeEntity
    /// </summary>
    public class ListConditionCodeEntity
    {
        /// <summary>
        /// Gets or sets the condition code.
        /// </summary>
        /// <value>The condition code.</value>
        [XmlElement(ElementName = "ConditionCode")]
        public string ConditionCode { get; set; }

        /// <summary>
        /// Gets or sets the condition code description.
        /// </summary>
        /// <value>The condition code description.</value>
        [XmlElement(ElementName = "ConditionCodeDescription")]
        public string ConditionCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [display next list].
        /// </summary>
        /// <value><c>true</c> if [display next list]; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "DisplayNextList")]
        public bool DisplayNextList { get; set; }
    }
}
