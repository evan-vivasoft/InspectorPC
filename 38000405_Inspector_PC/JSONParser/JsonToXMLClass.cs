using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders.InspectionProcedure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace JsonParser
{
    public class JsonToXMLClass(InspectionProcedureJson[] inspectionData)
    {   
       public InspectionProcedureJson[] InspectionProcedureJson = inspectionData;

        public List<InspectionProcedureEntity> GetJsonToXml()
        {
            List<InspectionProcedureEntity> XmlData = [];
            foreach (var item in InspectionProcedureJson)
            {
                var xmlItem = GetXMLItemFromJsonItem(item);
                XmlData.Add(xmlItem);
            }

            return XmlData;
        }

        public InspectionProcedureEntity GetXMLItemFromJsonItem(InspectionProcedureJson jsonData)
        {
            InspectionProcedureEntity item = new InspectionProcedureEntity
            {
                Name = jsonData.name,
                Version = jsonData.version,
                InspectionSequence = []
            };

            foreach (var section in jsonData.sections)
            {
                var sectionName = section.name;
                var sequenceNumber = section.sequence;

                if (section.sub_sections.Count > 0)
                {
                    // then we will get a list of script command list inside script_commands property
                    foreach (var subSectionList in section.sub_sections)
                    {
                        var scriptCommand2 = new ScriptCommand2Entity
                        {
                            SequenceNumber = sequenceNumber,
                            Section = sectionName,
                            SubSection = subSectionList.name
                        };

                        if (subSectionList.script_commands.Count > 0)
                        {
                            item.InspectionSequence.Add(scriptCommand2);
                            // it will be a list of script command list
                            foreach (var commands in subSectionList.script_commands)
                            {
                                foreach (var command in commands)
                                {
                                    switch (command.command_type)
                                    {
                                        case "Scriptcommand_1":
                                            var scriptCommand1 = new ScriptCommand1Entity
                                            {
                                                SequenceNumber = command.sequence,
                                                Text = (command.data).instruction
                                            };
                                            item.InspectionSequence.Add(scriptCommand1);
                                            break;
                                        case "Scriptcommand_3":
                                            var scriptCommand3 = new ScriptCommand3Entity
                                            {
                                                SequenceNumber = command.sequence,
                                                Text = (command.data).instruction,
                                                Duration = (command.data).duration ?? 0
                                            };
                                            item.InspectionSequence.Add(scriptCommand3);
                                            break;
                                        case "Scriptcommand_4":
                                            var scriptCommand4Json = (command.data);
                                            var scriptCommand4 = new ScriptCommand4Entity
                                            {
                                                SequenceNumber = command.sequence,
                                                ObjectName = scriptCommand4Json.object_name,
                                                MeasurePoint = scriptCommand4Json.measure_point,
                                                Question = scriptCommand4Json.question,
                                                TypeQuestion = Helper.GetEnumValueFromDescription<TypeQuestion>("1; Input single line"),//scriptCommand4Json.type_of_question),
                                                TextOptions = [],
                                                Required = scriptCommand4Json.required
                                            };

                                            if (!string.IsNullOrEmpty(scriptCommand4Json.option_1))
                                            {
                                                scriptCommand4.TextOptions.Add(scriptCommand4Json.option_1);
                                            }

                                            if (!string.IsNullOrEmpty(scriptCommand4Json.option_2))
                                            {
                                                scriptCommand4.TextOptions.Add(scriptCommand4Json.option_2);
                                            }

                                            if (!string.IsNullOrEmpty(scriptCommand4Json.option_3))
                                            {
                                                scriptCommand4.TextOptions.Add(scriptCommand4Json.option_3);
                                            }

                                            item.InspectionSequence.Add(scriptCommand4);
                                            break;
                                        case "Scriptcommand_41":
                                            var scriptCommand41Json = (command.data);
                                            var scriptCommand41 = new ScriptCommand41Entity
                                            {
                                                SequenceNumber = command.sequence,
                                                ObjectName = scriptCommand41Json.object_name,
                                                MeasurePoint = scriptCommand41Json.measure_point,
                                                ShowNextListImmediatly = scriptCommand41Json.show_next_list_immediately,
                                                ScriptCommandList = []
                                            };
                                            var listType = scriptCommand41Json.list_type;
                                            var index = 1;

                                            foreach (var listItem in scriptCommand41Json.lists)
                                            {
                                                List<ListConditionCodeEntity> listConditionCode = [];

                                                foreach (var conditionCode in listItem.condition_codes)
                                                {
                                                    var conditionCodeEntity = new ListConditionCodeEntity
                                                    {
                                                        ConditionCode = conditionCode.condition_code,
                                                        ConditionCodeDescription = conditionCode.condition_code_description,
                                                        DisplayNextList = !conditionCode.do_not_display_next_list
                                                    };
                                                    listConditionCode.Add(conditionCodeEntity);
                                                }

                                                ScriptCommand41ListEntity listEntity = new ScriptCommand41ListEntity
                                                {
                                                    SequenceNumberList = index++,
                                                    ListType = listType,
                                                    SelectionRequired = listItem.selection_required,
                                                    ListQuestion = listItem.list_question,
                                                    OneSelectionAllowed = listItem.one_selection,
                                                    CheckListResult = true, // to be fixed
                                                    ListConditionCodes = listConditionCode
                                                };
                                            }
                                            item.InspectionSequence.Add(scriptCommand41);
                                            break;
                                        case "Scriptcommand_42":
                                            var scriptCommand42Json = (command.data);
                                            var scriptCommand42 = new ScriptCommand42Entity
                                            {
                                                ObjectName = scriptCommand42Json.object_name,
                                                MeasurePoint = scriptCommand42Json.measure_point,
                                                SequenceNumber = command.sequence
                                            };
                                            item.InspectionSequence.Add(scriptCommand42);
                                            break;
                                        case "Scriptcommand_43":
                                            var scriptCommand43Json = command.data;
                                            var scriptCommand43 = new ScriptCommand43Entity
                                            {
                                                SequenceNumber = command.sequence,
                                                ObjectName = scriptCommand43Json.object_name,
                                                MeasurePoint = scriptCommand43Json.measure_point,
                                                Instruction = scriptCommand43Json.instruction,
                                                ListItems = scriptCommand43Json.items.ToList(),
                                                Required = scriptCommand43Json.required
                                            };
                                            item.InspectionSequence.Add(scriptCommand43);
                                            break;
                                        case "Scriptcommand_51":
                                        case "Scriptcommand_52":
                                        case "Scriptcommand_53":
                                        case "Scriptcommand_54":
                                        case "Scriptcommand_55":
                                        case "Scriptcommand_56":
                                        case "Scriptcommand_57":
                                            var scriptCommand5xJson = (command.data);
                                            var scriptCommand5x = new ScriptCommand5XEntity
                                            {
                                                SequenceNumber = command.sequence,
                                                ObjectName = scriptCommand5xJson.object_name,
                                                MeasurePoint = scriptCommand5xJson.measure_point,
                                                ScriptCommand5X = ScriptCommand5XType.ScriptCommand51, // To be fixed
                                                Instruction = scriptCommand5xJson.instruction,
                                                DigitalManometer = Helper.GetEnumValueFromDescription<DigitalManometer>(scriptCommand5xJson.digital_manometer),
                                                MeasurementFrequency = Helper.GetEnumValueFromDescription<TypeMeasurementFrequency>(scriptCommand5xJson.measuring_frequency.ToString()),
                                                MeasurementPeriod = (int)(scriptCommand5xJson.measuring_period),
                                                ExtraMeasurementPeriod = (int)scriptCommand5xJson.extra_measuring_period,
                                                Leakage =Helper.GetEnumValueFromDescription<Leakage>(!string.IsNullOrEmpty(scriptCommand5xJson.leakage_amount) ? scriptCommand5xJson.leakage_amount : "-")
                                            };
                                            item.InspectionSequence.Add(scriptCommand5x);
                                            break;
                                        default:
                                            throw new InvalidDataException($"Command type not known {command.command_type}");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return item;
        }

        public void ValidateXmlFile(string xmlFile, string xsdFile)
        {
            //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Validating file: {0} to XSD", xmlFile));

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, xsdFile);
                settings.ValidationType = ValidationType.Schema;
                XmlDocument document = new XmlDocument();
                document.Load(xmlFile);

                using (StringReader stringReader = new StringReader(document.InnerXml))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
                    {
                        while (xmlReader.Read()) { }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Failed to validate XML File: {0} to XSD file: {1}. Exception: {2}", xmlFile, xsdFile, ex.Message), ex);
            }
        }

        public void Save(string path, Object obj)
        {
            Type type = obj.GetType();
            XmlSerializer xmlserializer = new XmlSerializer(type);

            bool containsDirectory = (path.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) > -1);
            if (containsDirectory)
            {
                string directoryPath = path.Substring(0, path.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase));

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            using (Stream fileStream = new FileStream(path, FileMode.Create))
            {
                using (XmlWriter xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8))
                {
                    xmlserializer.Serialize(xmlWriter, obj);
                }
            }
        }
    }
}
