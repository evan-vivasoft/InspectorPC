using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// ScriptCommand 42, 56,57 not found

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders.InspectionProcedure
{
    public class InspectionProcedureJson
    {
        public string name { get; set; }
        public string version { get; set; }
        public List<ProcedureSection> sections { get; set; }
    }

    public class ProcedureSection
    {
        public int sequence { get; set; }
        public string name { get; set; }
        public List<ProcedureSubSection> sub_sections { get; set; }
    }

    public class ProcedureSubSection
    {
        public int sequence { get; set; }
        public string name { get; set; }
        public string command_type { get; set; }
        public List<List<InspectionScriptCommand>> script_commands { get; set; }
    }

    public class InspectionScriptCommand
    {
        public int sequence { set; get; }
        public Guid script_command_id { set; get; }
        public string command_type { set; get; }
        public ScriptCommandType data { set; get; }
    }

    public enum CommandType
    {
        ScriptCommand_1,
        ScriptCommand_2,
        ScriptCommand_3,
        ScriptCommand_4,
        ScriptCommand_41,
        ScriptCommand_42,
        ScriptCommand_43,
        ScriptCommand_51,
        ScriptCommand_52,
        ScriptCommand_53,
        ScriptCommand_54,
        ScriptCommand_55,
        ScriptCommand_56,
        ScriptCommand_57
    }
    public class ScriptCommandType 
    {
        private int _MeasureFrequency;
        private string _LeakageValue;
        private string _DigitalManometer;
        private string[] _AllowedLeakageValues = ["V1", "V2", "Membrane", "-"];
        private string[] _AllowedDigitalManometerValues = ["TH1", "TH2"];

        public string object_name { set; get; }
        public string measure_point { set; get; }
        public Guid inspection_point_id { set; get; }
        public string instruction { set; get; }
        public string digital_manometer
        {
            get => _DigitalManometer;
            set
            {
                if (Array.IndexOf(_AllowedDigitalManometerValues, value) == -1)
                {
                    throw new ArgumentException($"Allowed manometer values are TH1, TH2 but provided ${value}");
                }
                _DigitalManometer = value;
            }
        }
        public int measuring_frequency
        {
            get => _MeasureFrequency;
            set
            {
                if (value != 10 && value != 15)
                {
                    throw new ArgumentException("Measure frequency must be 10 or 15");
                }
                _MeasureFrequency = value;
            }
        }
        public uint measuring_period { set; get; }
        public uint extra_measuring_period { set; get; }
        public string leakage_amount
        {
            get => _LeakageValue;
            set
            {
                if (Array.IndexOf(_AllowedLeakageValues, value) == -1)
                {
                    throw new ArgumentException($"Allowed leakage values are [V1, V2, Membrane, -] but provided {value}");
                }
                _LeakageValue = value;
            }
        }
        public bool show_next_list_immediately { get; set; }
        public string list_type { get; set; }
        public ListType[] lists { get; set; }
        public bool required { get; set; }
        public string[] items { get; set; }
        public string question { get; set; }
        public string type_of_question { get; set; }
        public string? option_1 { get; set; }
        public string? option_2 { get; set; }
        public string? option_3 { get; set; }
        public int? duration {  get; set; } 
    }
    public class scriptcommandbase : ScriptCommandType
    {
        private int _measurefrequency;
        private string _leakagevalue;
        private string _digitalmanometer;
        private string[] _allowedleakagevalues = ["v1", "v2", "membrane", "-"];
        private string[] _alloweddigitalmanometervalues = ["th1", "th2"];

        public string object_name { set; get; }
        public string measure_point { set; get; }
        public Guid inspection_point_id { set; get; }
        public string instruction { set; get; }
        public string digital_manometer
        {
            get => _digitalmanometer;
            set
            {
                if (Array.IndexOf(_alloweddigitalmanometervalues, value) == -1)
                {
                    throw new ArgumentException($"allowed manometer values are th1, th2 but provided ${value}");
                }
                _digitalmanometer = value;
            }
        }
        public int measuring_frequency
        {
            get => _measurefrequency;
            set
            {
                if (value != 10 && value != 15)
                {
                    throw new ArgumentException("measure frequency must be 10 or 15");
                }
                _measurefrequency = value;
            }
        }
        public uint measuring_period { set; get; }
        public uint extra_measuring_period { set; get; }
        public string leakage_amount
        {
            get => _leakagevalue;
            set
            {
                if (Array.IndexOf(_allowedleakagevalues, value) == -1)
                {
                    throw new ArgumentException($"allowed leakage values are [v1, v2, membrane, -] but provided {value}");
                }
                _leakagevalue = value;
            }
        }
    }

    public class scriptcommand1 : ScriptCommandType
    {
        public string instruction { get; set; }
    }

    public class scriptcommand41 : scriptcommandbase
    {
        public bool show_next_list_immediately { get; set; }
        public string list_type { get; set; }
        public ListType[] lists { get; set; }
    }

    public class ListType
    {
        public string list_question { get; set; }
        public bool one_selection { get; set; }
        public ConditionCodesType[] condition_codes { get; set; }
        public bool selection_required { get; set; }
        public bool saving_all_condition_codes { get; set; }
    }

    public class ConditionCodesType
    {
        public bool do_not_display_next_list { get; set; }
        public string condition_code { get; set; }
        public string condition_code_description { get; set; }
    }

    public class Scriptcommand43 : ScriptCommandType
    {
        public string? object_name { get; set; }
        public string? measure_point { get; set; }
        public Guid inspection_point_id { get; set; }
        public string instruction { get; set; }
        public bool required { get; set; }
        public string[] items { get; set; }
    }

    public class ScriptCommand42 : ScriptCommandType
    {
        public string? object_name { get; set; }
        public string? measure_point { get; set; }
        public Guid inspection_point_id { get; set; }

    }
    public class ScriptCommand4 : ScriptCommandType
    {
       public string? object_name {  set; get; }
       public string? measure_point { get; set; }
       public Guid inspection_point_id { get; set; }
       public string question { get; set; }
       public TypeQuestion type_of_question { get; set; }
       public string? option_1 { get; set; }
       public string? option_2 { get; set; }
       public string? option_3 { get; set; }
       public bool required {  set; get; }

    }

    public class ScriptCommand5x : ScriptCommandType
    {
        private string _leakageValue;
        private string _manometerValue;
        private int _measureFreq;
        public string? object_name { get; set; }
        public string? measure_point { get; set; }
        public Guid inspection_point_id { get; set; }
        public string instruction { get; set; }
        public string digital_manometer 
        {
            get => _manometerValue; 
            set
            {
                switch (value)
                {
                    case "Unknown":
                        _manometerValue = "-1";
                        break;
                    case "TH1":
                        _manometerValue = "0";
                        break;
                    case "TH2":
                        _manometerValue = "1";
                        break;
                    default:
                        throw new ArgumentException($"Invalid manometer value provided: {value}");
                }
            }
        }
        public int measuring_frequency 
        {
            get => _measureFreq;
            set
            {
                if (value == 10 || value == 15)
                {
                    _measureFreq = value;
                }
                else
                {
                    throw new ArgumentException($"Invalid frequency value provided: {value}");
                }
            }
        }
        public int measuring_period { get; set; }
        public int extra_measuring_period { get; set; }
        public string? leakage_amount 
        {
            get => _leakageValue;
            set
            {
                switch (value)
                {
                    case "V1":
                        _leakageValue = "0";
                        break;
                    case "V2":
                        _leakageValue = "1";
                        break;
                    case "Membrane":
                        _leakageValue = "2";
                        break;
                    case "-":
                        _leakageValue = "3";
                        break;
                    default:
                        throw new ArgumentException($"Invalid leakage amount provided: {value}");
                }
            }
        }
    }
}


/**
 * inspectionProcedure.name    => InspectionProcedureEntity.Name
 * inspectionProcedure.version => InspectionProcedureEntity.Version
 * inspectionProcedure.section => InspectionProcedureEntity.InspectionSequence
 * inspectionProcedure.section.subsection contains `InspectionProcedureEntity.InspectionSequence` of various types
 * **/