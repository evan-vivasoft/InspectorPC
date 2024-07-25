/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
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

    /// <summary>
    /// Select the digital manometer to communicate with. "TH1" left placed digital manometer; "TH2" right placed digital manometer
    /// </summary>
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

        [Description("-")]
        [System.Xml.Serialization.XmlEnumAttribute("-")]
        Item,

        [Description("mbar")]
        [System.Xml.Serialization.XmlEnumAttribute("mbar")]
        ItemMbar,

        [Description("bar")]
        [System.Xml.Serialization.XmlEnumAttribute("bar")]
        ItemBar,

        [Description("Pa")]
        [System.Xml.Serialization.XmlEnumAttribute("Pa")]
        ItemPa,

        [Description("hPa")]
        [System.Xml.Serialization.XmlEnumAttribute("hPa")]
        ItemhPa,

        [Description("kPa")]
        [System.Xml.Serialization.XmlEnumAttribute("kPa")]
        ItemkPa,

        [Description("MPa")]
        [System.Xml.Serialization.XmlEnumAttribute("MPa")]
        ItemMPa,

        [Description("kg/cm2")]
        [System.Xml.Serialization.XmlEnumAttribute("kg/cm2")]
        Itemkgcm2,

        [Description("kg/m2")]
        [System.Xml.Serialization.XmlEnumAttribute("kg/m2")]
        Itemkgm2,

        [Description("mmHg")]
        [System.Xml.Serialization.XmlEnumAttribute("mmHg")]
        ItemmmHg,

        [Description("cmHg")]
        [System.Xml.Serialization.XmlEnumAttribute("cmHg")]
        ItemcmHg,

        [Description("mHg")]
        [System.Xml.Serialization.XmlEnumAttribute("mHg")]
        ItemmHg,

        [Description("inHg")]
        [System.Xml.Serialization.XmlEnumAttribute("inHg")]
        IteminHg,

        [Description("mmH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("mmH2o")]
        ItemmmH2o,

        [Description("cmH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("cmH2o")]
        ItemcmH2o,

        [Description("mH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("mH2o")]
        ItemmH2o,

        [Description("inH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("inH2o")]
        IteminH2o,

        [Description("ftH2o")]
        [System.Xml.Serialization.XmlEnumAttribute("ftH2o")]
        ItemftH2o,

        [Description("psi")]
        [System.Xml.Serialization.XmlEnumAttribute("psi")]
        Itempsi,

        [Description("lb/in2")]
        [System.Xml.Serialization.XmlEnumAttribute("lb/in2")]
        Itemlbin2,

        [Description("lb/ft2")]
        [System.Xml.Serialization.XmlEnumAttribute("lb/ft2")]
        Itemlbft2,

        [Description("torr")]
        [System.Xml.Serialization.XmlEnumAttribute("torr")]
        Itemtorr,

        [Description("atm")]
        [System.Xml.Serialization.XmlEnumAttribute("atm")]
        Itematm,

        [Description("mbar/min")]
        [System.Xml.Serialization.XmlEnumAttribute("mbar/min")]
        ItemMbarMin,

        [Description("Pa/min")]
        [System.Xml.Serialization.XmlEnumAttribute("Pa/min")]
        ItemPamin,

        [Description("(kg/cm2)/min")]
        [System.Xml.Serialization.XmlEnumAttribute("(kg/cm2)/min")]
        Itemkgcm2min,

        [Description("mmHg/min")]
        [System.Xml.Serialization.XmlEnumAttribute("mmHg/min")]
        ItemmmHgmin,

        [Description("inHg/min")]
        [System.Xml.Serialization.XmlEnumAttribute("inHg/min")]
        IteminHgmin,

        [Description("mmH2o/min")]
        [System.Xml.Serialization.XmlEnumAttribute("mmH2o/min")]
        ItemmmH2omin,

        [Description("inH2o/min")]
        [System.Xml.Serialization.XmlEnumAttribute("inH2o/min")]
        IteminH2omin,

        [Description("ftH2o/min")]
        [System.Xml.Serialization.XmlEnumAttribute("ftH2o/min")]
        ItemftH2omin,

        [Description("(lb/in2)/min")]
        [System.Xml.Serialization.XmlEnumAttribute("(lb/in2)/min")]
        Itemlbin2min,

        [Description("(lb/ft2)/min")]
        [System.Xml.Serialization.XmlEnumAttribute("(lb/ft2)/min")]
        Itemlbft2min,

        [Description("dm3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("dm3/h")]
        ItemDm3h,

        [Description("ft3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("ft3/h")]
        Itemft3h,

        [Description("m3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("m3/h")]
        Itemm3h,

        [Description("l/h")]
        [System.Xml.Serialization.XmlEnumAttribute("l/h")]
        Itemlh,

        [Description("gph")]
        [System.Xml.Serialization.XmlEnumAttribute("gph")]
        Itemgph,

        [Description("in3/h")]
        [System.Xml.Serialization.XmlEnumAttribute("in3/h")]
        Itemin3h,

        [Description("°C")]
        [System.Xml.Serialization.XmlEnumAttribute("°C")]
        ItemC,

        [Description("°F")]
        [System.Xml.Serialization.XmlEnumAttribute("°F")]
        ItemF,

        [Description("°K")]
        [System.Xml.Serialization.XmlEnumAttribute("°K")]
        ItemK,

        [Description("°Re")]
        [System.Xml.Serialization.XmlEnumAttribute("°Re")]
        ItemRe
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
