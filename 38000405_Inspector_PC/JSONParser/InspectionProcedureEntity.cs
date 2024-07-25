using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders.InspectionProcedure
{
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

    public abstract class ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets the sequence number of the scriptcommand.
        /// </summary>
        /// <value>The sequence number.</value>
        [XmlElement(ElementName = "SequenceNumber")]
        public long SequenceNumber { get; set; }
    }

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

    [XmlRoot("Scriptcommand_4")]
    public class ScriptCommand4Entity : ScriptCommandEntityDescriptions
    {
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>The question.</value>
        [XmlElement(ElementName = "Question")]
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets the type question.
        /// </summary>
        /// <value>The type question.</value>
        [XmlElement(ElementName = "TypeQuestion")]
        public TypeQuestion TypeQuestion { get; set; }

        /// <summary>
        /// Gets or sets the text options.
        /// </summary>
        /// <value>The text options.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [XmlElement(ElementName = "TextOptions")]
        public List<string> TextOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand4Entity"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "Required")]
        public bool Required { get; set; }
    }

    public class ScriptCommandEntityDescriptions : ScriptCommandEntityBase
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
        /// Gets or sets the object name description.
        /// </summary>
        /// <value>
        /// The object name description.
        /// </value>
        [XmlElement(ElementName = "ObjectNameDescription")]
        public string ObjectNameDescription { get; set; }

        /// <summary>
        /// Gets or sets the measure point description.
        /// </summary>
        /// <value>
        /// The measure point description.
        /// </value>
        [XmlElement(ElementName = "MeasurePointDescription")]
        public string MeasurePointDescription { get; set; }
    }

    public enum TypeQuestion
    {
        /// <summary>
        /// Question is in multiline format
        /// </summary>
        [Description("Multiline input")]
        [XmlEnum(Name = "0; Input multi lines")]
        InputMultiLines = 0,
        /// <summary>
        /// Question is in singleline format
        /// </summary>
        [Description("Singleline input")]
        [XmlEnum(Name = "1; Input single line")]
        InputSingleLine = 1,
        /// <summary>
        /// Question provides a choice between 2 options
        /// </summary>
        [Description("2 options input")]
        [XmlEnum(Name = "2; 2 options")]
        TwoOptions = 2,
        /// <summary>
        /// Question provides a choice between 3 options
        /// </summary>
        [Description("3 options input")]
        [XmlEnum(Name = "3; 3 options")]
        ThreeOptions = 3,
    }

    public class ScriptCommand41Entity : ScriptCommandEntityDescriptions
    {

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
        public string ListType { get; set; }

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

    public class ScriptCommand42Entity : ScriptCommandEntityDescriptions
    {

    }

    public class ScriptCommand43Entity : ScriptCommandEntityDescriptions
    {
        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        /// <value>The instruction.</value>
        [XmlElement(ElementName = "Instruction")]
        public string Instruction { get; set; }

        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        /// <value>The list items.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [XmlElement(ElementName = "ListItem")]
        public List<string> ListItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand43Entity"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "Required")]
        public bool Required { get; set; }
    }

    public class ScriptCommand5XEntity : ScriptCommandEntityDescriptions
    {
        /// <summary>
        /// Gets or sets the scriptcommand5X.
        /// </summary>
        /// <value>The scriptcommand5X.</value>
        [XmlElement(ElementName = "Scriptcommand")]
        public ScriptCommand5XType ScriptCommand5X { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        /// <value>The instruction.</value>
        [XmlElement(ElementName = "Instruction")]
        public string Instruction { get; set; }

        [XmlElement(ElementName = "DigitalManometer")]
        public DigitalManometer DigitalManometer { get; set; }

        [XmlElement(ElementName = "MeasurementFrequency")]
        public TypeMeasurementFrequency MeasurementFrequency { get; set; }

        [XmlElement(ElementName = "MeasurementPeriod")]
        public int MeasurementPeriod { get; set; }

        [XmlElement(ElementName = "ExtraMeasurementPeriod")]
        public int ExtraMeasurementPeriod { get; set; }

        [XmlElement(ElementName = "Leakage")]
        public Leakage Leakage { get; set; }
    }

    public enum ScriptCommand5XType
    {
        /// <summary>
        /// ScriptCommand51
        /// </summary>
        [Description("51")]
        [XmlEnum(Name = "51")]
        ScriptCommand51 = 0,

        /// <summary>
        /// ScriptCommand52
        /// </summary>
        [Description("52")]
        [XmlEnum(Name = "52")]
        ScriptCommand52 = 1,

        /// <summary>
        /// ScriptCommand53
        /// </summary>
        [Description("53")]
        [XmlEnum(Name = "53")]
        ScriptCommand53 = 2,

        /// <summary>
        /// ScriptCommand54
        /// </summary>
        [Description("54")]
        [XmlEnum(Name = "54")]
        ScriptCommand54 = 3,

        /// <summary>
        /// ScriptCommand55
        /// </summary>
        [Description("55")]
        [XmlEnum(Name = "55")]
        ScriptCommand55 = 4,

        /// <summary>
        /// ScriptCommand56
        /// </summary>
        [Description("56")]
        [XmlEnum(Name = "56")]
        ScriptCommand56 = 5,

        /// <summary>
        /// ScriptCommand57
        /// </summary>
        [Description("57")]
        [XmlEnum(Name = "57")]
        ScriptCommand57 = 6,
    }

    public class ScriptCommand70Entity : ScriptCommandEntityBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptCommand70Entity"/> is mode.
        /// </summary>
        /// <value><c>true</c> if mode; otherwise, <c>false</c>.</value>
        [XmlElement(ElementName = "Mode")]
        public bool Mode { get; set; }
    }

    public enum DigitalManometer
    {
        /// <summary>
        /// Unknown device
        /// </summary>
        [Description("Unknown digital manometer")]
        [XmlEnum(Name = "Unknown")]
        Unknown = -1,

        /// <summary>
        /// TH1, left placed digital manometer
        /// </summary>
        [Description("TH1, left placed digital manometer")]
        [XmlEnum(Name = "TH1")]
        TH1 = 0,

        /// <summary>
        /// TH2, right placed digital manometer
        /// </summary>
        [Description("TH2, right placed digital manometer")]
        [XmlEnum(Name = "TH2")]
        TH2 = 1
    }

    /// <summary>
    /// Select the measurement frequency of the digital manometer. Default 10, set 25 only for fingerprint use only.
    /// The assigned constant integer represents the actual frequency value (10 or 25).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum TypeMeasurementFrequency
    {
        /// <summary>
        /// Default measurement frequency (10)
        /// </summary>
        [Description("Default measurement frequency (10)")]
        [XmlEnum(Name = "10")]
        Default = 10,

        /// <summary>
        /// Fingerprint only measure frequency (25)
        /// </summary>
        [Description("Fingerprint only measure frequency (25)")]
        [XmlEnum(Name = "25")]
        Fingerprint = 25,
    }

    /// <summary>
    /// Select the type of leakage. More information in the manual of CONNEXION
    /// </summary>
    public enum Leakage
    {
        /// <summary>
        /// V1
        /// </summary>
        [Description("V1 leakage")]
        [XmlEnum(Name = "V1")]
        V1 = 0,

        /// <summary>
        /// V2
        /// </summary>
        [Description("V2 leakage")]
        [XmlEnum(Name = "V2")]
        V2 = 1,

        /// <summary>
        /// Membrane
        /// </summary>
        [Description("Membrane leakage")]
        [XmlEnum(Name = "Membrane")]
        Membrane = 2,

        /// <summary>
        /// Dash (-)
        /// </summary>
        [Description("-")]
        [XmlEnum(Name = "-")]
        Dash = 3,
    }
}
