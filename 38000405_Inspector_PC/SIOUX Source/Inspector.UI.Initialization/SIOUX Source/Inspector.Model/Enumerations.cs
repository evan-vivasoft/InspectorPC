/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.ComponentModel;
using System.Xml.Serialization;

namespace Inspector.Model
{
    #region Initialization enumerations
    /// <summary>
    /// The result of an individual Initialization step
    /// </summary>
    public enum InitializationStepResult
    {
        /// <summary>
        /// Default; The initialization step has not yet been executed
        /// </summary>
        UNSET,
        /// <summary>
        /// The initialization step has completed succesfully
        /// </summary>
        SUCCESS,
        /// <summary>
        /// The initialization step has completed with an error
        /// </summary>
        ERROR,
        /// <summary>
        /// The initialization step has completed with a warning
        /// </summary>
        WARNING,
        /// <summary>
        /// The manometer could be reached in the initialization step
        /// </summary>
        TIMEOUT,
        /// <summary>
        /// The suer aborted the initialization
        /// </summary>
        USERABORTED
    }

    /// <summary>
    /// The result of an initialization procedure.
    /// </summary>
    public enum InitializationResult
    {
        /// <summary>
        /// Default; The initialization has not yet been executed
        /// </summary>
        UNSET,
        /// <summary>
        /// The initialization has completed succesfully
        /// </summary>
        SUCCESS,
        /// <summary>
        /// The initialization has completed with an error
        /// </summary>
        ERROR,
        /// <summary>
        /// The initialization has completed with a warning
        /// </summary>
        WARNING,
        /// <summary>
        /// The initialization has completed, but one or more of the manometers could not be reached.
        /// </summary>
        TIMEOUT,
        /// <summary>
        /// The user has aborted the initialization
        /// </summary>
        USERABORTED
    }

    /// <summary>
    /// The manometer an initialization step was sent to
    /// </summary>
    public enum InitializationManometer
    {
        /// <summary>
        /// Default; The manometer has not yet been set
        /// </summary>
        UNSET,
        /// <summary>
        /// No manometer message (e.g. Connect)
        /// </summary>
        BLUETOOTH_MODULE,
        /// <summary>
        /// Manometer TH1
        /// </summary>
        TH1,
        /// <summary>
        /// Manometer TH2
        /// </summary>
        TH2
    }
    #endregion Initialization enumerations

    #region Station and procedure information enumerations
    /// <summary>
    /// TypeQuestion enumeration
    /// </summary>
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

    /// <summary>
    /// ListType
    /// </summary>
    public enum ListType
    {
        /// <summary>
        /// The list type is a option selection list
        /// </summary>
        [Description("OptionList")]
        [XmlEnum(Name = "0;OptionList")]
        OptionList = 0,
        /// <summary>
        /// The list type is a checklist
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CheckList")]
        [Description("CheckList")]
        [XmlEnum(Name = "1;CheckList")]
        CheckList = 1,
    }

    /// <summary>
    /// Defines the type of scripcommand5X
    /// </summary>
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
    }

    /// <summary>
    /// Select the digital manometer to communicate with. "TH1" left placed digital manometer; "TH2" right placed digital manometer
    /// </summary>
    public enum DigitalManometer
    {
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
        TH2 = 1,
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

    /// <summary>
    /// TypeUnitsValue
    /// </summary>
    public enum UnitOfMeasurement
    {
        /// <summary>
        /// Value not set
        /// </summary>
        [Description("Unset")]
        UNSET,

        /// <summary>
        /// No value or unknown value
        /// </summary>
        [Description("-")]
        [System.Xml.Serialization.XmlEnumAttribute("-")]
        Item,

        [Description("mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("mbar")]
        ItemMbar,

        [Description("bar")]
        [System.Xml.Serialization.XmlEnumAttribute("bar")]
        ItemBar,

        [Description("mbar/min")]
        [System.Xml.Serialization.XmlEnumAttribute("mbar/min")]
        ItemMbarMin,

        [Description("dm3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("dm3/h")]
        ItemDm3h,
    }

    /// <summary>
    /// TypeRangeDM
    /// </summary>
    public enum TypeRangeDM
    {
        /// <summary>
        /// Value not set
        /// </summary>
        [Description("Unset")]
        UNSET,

        /// <summary>
        /// No value or unknown value
        /// </summary>
        [Description("-")]
        [System.Xml.Serialization.XmlEnumAttribute("-")]
        Item,

        [Description("0..25 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..25mbar")]
        Item025mbar,

        [Description("0..70 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..70mbar")]
        Item070mbar,

        [Description("0..70 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..70mbar")]
        Item070mbar1,

        [Description("0..200 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..200mbar")]
        Item0200mbar,

        [Description("0..300 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..300mbar")]
        Item0300mbar,

        [Description("0..500 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..500mbar")]
        Item0500mbar,

        [Description("0..1000 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..1000mbar")]
        Item01000mbar,

        [Description("0..1100 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..1100mbar")]
        Item01100mbar,

        [Description("0..2000 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..2000mbar")]
        Item02000mbar,

        [Description("0..7500 mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..7500mbar")]
        Item07500mbar,

        [Description("0..10 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..10bar")]
        Item010bar,

        [Description("0..17 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..17bar")]
        Item017bar,

        [Description("0..35 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..35bar")]
        Item035bar,

        [Description("0..90 bar")]
        [System.Xml.Serialization.XmlEnumAttribute("0..90bar")]
        Item090bar,
    }
    #endregion Station and procedure information enumerations
}
