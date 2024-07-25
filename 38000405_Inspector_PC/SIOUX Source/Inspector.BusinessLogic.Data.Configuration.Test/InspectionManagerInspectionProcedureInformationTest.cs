using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.Model;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionProcedureStatus;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Data.Configuration.Test
{
    [TestFixture]
    public class InspectionManagerInspectionProcedureInformationTest
    {
        #region Test Data
        private static string InspectionProcedureBaseXML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand1XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_1>
                        <SequenceNumber>4</SequenceNumber>
                        <Text>Hartelijk welkom bij de inspectie</Text>
                    </Scriptcommand_1>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand2XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_2>
                        <SequenceNumber>1</SequenceNumber>
                        <Section>Algemene opmerkingen aan section</Section>
                        <SubSection>Algemene opmerkingen aan subsection</SubSection>
                    </Scriptcommand_2>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand3XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_3>
                        <SequenceNumber>81</SequenceNumber>
                        <Text>Wachttijd</Text>
                        <Duration>30</Duration>
                    </Scriptcommand_3>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand4XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_4>
                        <SequenceNumber>94</SequenceNumber>
                        <ObjectName/>
                        <MeasurePoint>00046</MeasurePoint>
                        <FieldNo>30</FieldNo>
                        <Question>1e veiligheid VAK opnieuw afgesteld?</Question>
                        <TypeQuestion>3; 3 options</TypeQuestion>
                        <TextOptions>Ja</TextOptions>
                        <TextOptions>Nee</TextOptions>
                        <TextOptions>nvt</TextOptions>
                        <Required>true</Required>
                    </Scriptcommand_4>    
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand41XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_41>
                        <SequenceNumber>92</SequenceNumber>
                        <ObjectName/>
                        <MeasurePoint>00033</MeasurePoint>
                        <FieldNo>13</FieldNo>
                        <ShowNextListImmediately>false</ShowNextListImmediately>
                        <List>
                            <SequenceNumberList>1</SequenceNumberList>
                            <ListType>0;OptionList</ListType>
                            <SelectionRequired>true</SelectionRequired>
                            <ListQuestion>in orde?</ListQuestion>
                            <OneSelectionAllowed>true</OneSelectionAllowed>
                            <CheckListResult>true</CheckListResult>
                            <ListConditionCode>
                                <ConditionCode>Ja</ConditionCode>
                                <ConditionCodeDescription/>
                                <DisplayNextList>true</DisplayNextList>
                            </ListConditionCode>
                            <ListConditionCode>
                                <ConditionCode>Nee</ConditionCode>
                                <ConditionCodeDescription/>
                                <DisplayNextList>false</DisplayNextList>
                            </ListConditionCode>
                        </List>
                        <List>
                            <SequenceNumberList>2</SequenceNumberList>
                            <ListType>0;OptionList</ListType>
                            <SelectionRequired>true</SelectionRequired>
                            <ListQuestion>in orde?</ListQuestion>
                            <OneSelectionAllowed>false</OneSelectionAllowed>
                            <CheckListResult>true</CheckListResult>
                            <ListConditionCode>
                                <ConditionCode>A - Lekkage uitwendig</ConditionCode>
                                <ConditionCodeDescription>CondDescr</ConditionCodeDescription>
                                <DisplayNextList>false</DisplayNextList>
                            </ListConditionCode>
                        </List>
                    </Scriptcommand_41> 
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand42XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_42>
                        <SequenceNumber>2</SequenceNumber>
                        <ObjectName>ObjectNameTest</ObjectName>
                        <MeasurePoint>00134</MeasurePoint>
                        <FieldNo>49</FieldNo>
                    </Scriptcommand_42>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand43XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_43>
                        <SequenceNumber>8</SequenceNumber>
                        <ObjectName/>
                        <MeasurePoint>00179</MeasurePoint>
                        <FieldNo>2</FieldNo>
                        <Instruction>Type Installatie</Instruction>
                        <ListItem>AS</ListItem>
                        <ListItem>DS</ListItem>
                        <ListItem>GOS</ListItem>
                        <ListItem>HAS</ListItem>
                        <ListItem>IS</ListItem>
                        <ListItem>OEL</ListItem>
                        <ListItem>OS</ListItem>
                        <ListItem>NB</ListItem>
                        <Required>true</Required>
                    </Scriptcommand_43>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand5xXML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_5x>
                        <SequenceNumber>27</SequenceNumber>
                        <ObjectName/>
                        <MeasurePoint>00169</MeasurePoint>
                        <FieldNo>16</FieldNo>
                        <Scriptcommand>55</Scriptcommand>
                        <Instruction>Geregelde waarde (Inregelkraan)</Instruction>
                        <DigitalManometer>TH2</DigitalManometer>
                        <MeasurementFrequency>10</MeasurementFrequency>
                        <MeasurementPeriod>30</MeasurementPeriod>
                        <ExtraMeasurementPeriod>0</ExtraMeasurementPeriod>
                        <Leakage>-</Leakage>
                    </Scriptcommand_5x>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommand70XML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_70>
                        <SequenceNumber>427</SequenceNumber>
                        <Mode>false</Mode>
                    </Scriptcommand_70>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";

        private static string InspectionProcedureScriptCommandsMixedXML =
        @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!--Created by Liquid XML Data Binding Libraries (www.liquid-technologies.com) for Kamstrup b.v.-->
        <InspectionProcedureFile xmlns:xs=""http://www.w3.org/2001/XMLSchema-instance"">
            <InspectionProcedure>
                <Name>B-insp VAK-VA LS (Pd &lt;= 300 mbar)</Name>
                <Version>1.0</Version>
                <InspectionSequence>
                    <Scriptcommand_1>
                        <SequenceNumber>4</SequenceNumber>
                        <Text>Hartelijk welkom bij de inspectie</Text>
                    </Scriptcommand_1>
                    <Scriptcommand_70>
                        <SequenceNumber>427</SequenceNumber>
                        <Mode>false</Mode>
                    </Scriptcommand_70>
                </InspectionSequence>
            </InspectionProcedure>
        </InspectionProcedureFile>";
        #endregion Test Data

        #region XML Loader tests
        [Test]
        public void LoadInspectionProcedureBaseFromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureBaseXML);

            Assert.AreEqual("B-insp VAK-VA LS (Pd <= 300 mbar)", inspectionProcedure.Name);
            Assert.AreEqual("1.0", inspectionProcedure.Version);
            Assert.AreEqual(0, inspectionProcedure.InspectionSequence.Count);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand1FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand1XML);
            ScriptCommand1Entity scriptCommand1 = GetFirstScriptCommand<ScriptCommand1Entity>(inspectionProcedure);

            Assert.AreEqual(4, scriptCommand1.SequenceNumber);
            Assert.AreEqual("Hartelijk welkom bij de inspectie", scriptCommand1.Text);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand2FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand2XML);
            ScriptCommand2Entity scriptCommand2 = GetFirstScriptCommand<ScriptCommand2Entity>(inspectionProcedure);

            Assert.AreEqual(1, scriptCommand2.SequenceNumber);
            Assert.AreEqual("Algemene opmerkingen aan section", scriptCommand2.Section);
            Assert.AreEqual("Algemene opmerkingen aan subsection", scriptCommand2.SubSection);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand3FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand3XML);
            ScriptCommand3Entity scriptCommand3 = GetFirstScriptCommand<ScriptCommand3Entity>(inspectionProcedure);

            Assert.AreEqual(81, scriptCommand3.SequenceNumber);
            Assert.AreEqual("Wachttijd", scriptCommand3.Text);
            Assert.AreEqual(30, scriptCommand3.Duration);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand4FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand4XML);
            ScriptCommand4Entity scriptCommand4 = GetFirstScriptCommand<ScriptCommand4Entity>(inspectionProcedure);

            Assert.AreEqual(94, scriptCommand4.SequenceNumber);
            Assert.AreEqual(String.Empty, scriptCommand4.ObjectName);
            Assert.AreEqual("00046", scriptCommand4.MeasurePoint);
            Assert.AreEqual(30, scriptCommand4.FieldNo);
            Assert.AreEqual("1e veiligheid VAK opnieuw afgesteld?", scriptCommand4.Question);
            Assert.AreEqual(TypeQuestion.ThreeOptions, scriptCommand4.TypeQuestion);
            Assert.AreEqual(3, scriptCommand4.TextOptions.Count);
            Assert.AreEqual("Ja", scriptCommand4.TextOptions[0]);
            Assert.AreEqual("Nee", scriptCommand4.TextOptions[1]);
            Assert.AreEqual("nvt", scriptCommand4.TextOptions[2]);
            Assert.IsTrue(scriptCommand4.Required);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand41FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand41XML);
            ScriptCommand41Entity scriptCommand41 = GetFirstScriptCommand<ScriptCommand41Entity>(inspectionProcedure);

            Assert.AreEqual(92, scriptCommand41.SequenceNumber);
            Assert.AreEqual(String.Empty, scriptCommand41.ObjectName);
            Assert.AreEqual("00033", scriptCommand41.MeasurePoint);
            Assert.AreEqual(13, scriptCommand41.FieldNo);
            Assert.IsFalse(scriptCommand41.ShowNextListImmediatly);

            // Check 'List' nodes
            Assert.AreEqual(2, scriptCommand41.ScriptCommandList.Count);

            // 'List' node 1
            ScriptCommand41ListEntity scriptCommand41List = scriptCommand41.ScriptCommandList[0];
            Assert.AreEqual(1, scriptCommand41List.SequenceNumberList);
            Assert.AreEqual(ListType.OptionList, scriptCommand41List.ListType);
            Assert.IsTrue(scriptCommand41List.SelectionRequired);
            Assert.AreEqual("in orde?", scriptCommand41List.ListQuestion);
            Assert.IsTrue(scriptCommand41List.OneSelectionAllowed);
            Assert.IsTrue(scriptCommand41List.CheckListResult);
            // Check 'ListConditionCode' nodes
            Assert.AreEqual(2, scriptCommand41List.ListConditionCodes.Count);
            // 'ListConditionCode' node 1
            ListConditionCodeEntity listConditionCode = scriptCommand41List.ListConditionCodes[0];
            Assert.AreEqual("Ja", listConditionCode.ConditionCode);
            Assert.AreEqual(String.Empty, listConditionCode.ConditionCodeDescription);
            Assert.IsTrue(listConditionCode.DisplayNextList);
            // 'ListConditionCode' node 2
            listConditionCode = scriptCommand41List.ListConditionCodes[1];
            Assert.AreEqual("Nee", listConditionCode.ConditionCode);
            Assert.AreEqual(String.Empty, listConditionCode.ConditionCodeDescription);
            Assert.IsFalse(listConditionCode.DisplayNextList);

            // 'List' node 2
            scriptCommand41List = scriptCommand41.ScriptCommandList[1];
            Assert.AreEqual(2, scriptCommand41List.SequenceNumberList);
            Assert.AreEqual(ListType.OptionList, scriptCommand41List.ListType);
            Assert.IsTrue(scriptCommand41List.SelectionRequired);
            Assert.AreEqual("in orde?", scriptCommand41List.ListQuestion);
            Assert.IsFalse(scriptCommand41List.OneSelectionAllowed);
            Assert.IsTrue(scriptCommand41List.CheckListResult);

            // Check 'ListConditionCode' node
            Assert.AreEqual(1, scriptCommand41List.ListConditionCodes.Count);
            // 'ListConditionCode' node 
            listConditionCode = scriptCommand41List.ListConditionCodes[0];
            Assert.AreEqual("A - Lekkage uitwendig", listConditionCode.ConditionCode);
            Assert.AreEqual("CondDescr", listConditionCode.ConditionCodeDescription);
            Assert.IsFalse(listConditionCode.DisplayNextList);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand42FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand42XML);
            ScriptCommand42Entity scriptCommand42 = GetFirstScriptCommand<ScriptCommand42Entity>(inspectionProcedure);

            Assert.AreEqual(2, scriptCommand42.SequenceNumber);
            Assert.AreEqual("ObjectNameTest", scriptCommand42.ObjectName);
            Assert.AreEqual("00134", scriptCommand42.MeasurePoint);
            Assert.AreEqual(49, scriptCommand42.FieldNo);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand43FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand43XML);
            ScriptCommand43Entity scriptCommand43 = GetFirstScriptCommand<ScriptCommand43Entity>(inspectionProcedure);

            Assert.AreEqual(8, scriptCommand43.SequenceNumber);
            Assert.AreEqual(String.Empty, scriptCommand43.ObjectName);
            Assert.AreEqual("00179", scriptCommand43.MeasurePoint);
            Assert.AreEqual(2, scriptCommand43.FieldNo);
            Assert.AreEqual("Type Installatie", scriptCommand43.Instruction);
            Assert.AreEqual(8, scriptCommand43.ListItems.Count);
            Assert.AreEqual("AS", scriptCommand43.ListItems[0]);
            Assert.AreEqual("DS", scriptCommand43.ListItems[1]);
            Assert.AreEqual("GOS", scriptCommand43.ListItems[2]);
            Assert.AreEqual("HAS", scriptCommand43.ListItems[3]);
            Assert.AreEqual("IS", scriptCommand43.ListItems[4]);
            Assert.AreEqual("OEL", scriptCommand43.ListItems[5]);
            Assert.AreEqual("OS", scriptCommand43.ListItems[6]);
            Assert.AreEqual("NB", scriptCommand43.ListItems[7]);
            Assert.IsTrue(scriptCommand43.Required);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand5xFromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand5xXML);
            ScriptCommand5XEntity scriptCommand5X = GetFirstScriptCommand<ScriptCommand5XEntity>(inspectionProcedure);

            Assert.AreEqual(27, scriptCommand5X.SequenceNumber);
            Assert.AreEqual(String.Empty, scriptCommand5X.ObjectName);
            Assert.AreEqual("00169", scriptCommand5X.MeasurePoint);
            Assert.AreEqual(16, scriptCommand5X.FieldNo.Value);
            Assert.AreEqual(Inspector.Model.ScriptCommand5XType.ScriptCommand55, scriptCommand5X.ScriptCommand5X);
            Assert.AreEqual("Geregelde waarde (Inregelkraan)", scriptCommand5X.Instruction);
            Assert.AreEqual(DigitalManometer.TH2, scriptCommand5X.DigitalManometer);
            Assert.AreEqual(TypeMeasurementFrequency.Default, scriptCommand5X.MeasurementFrequency);
            Assert.AreEqual(30, scriptCommand5X.MeasurementPeriod);
            Assert.AreEqual(0, scriptCommand5X.ExtraMeasurementPeriod);
            Assert.AreEqual(Leakage.Dash, scriptCommand5X.Leakage);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommand70FromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommand70XML);
            ScriptCommand70Entity scriptCommand70 = GetFirstScriptCommand<ScriptCommand70Entity>(inspectionProcedure);

            Assert.AreEqual(427, scriptCommand70.SequenceNumber);
            Assert.IsFalse(scriptCommand70.Mode);
        }

        [Test]
        public void LoadInspectionProcedureScriptCommandsMixedFromXMLTest()
        {
            InspectionProcedureEntity inspectionProcedure = GetFirstInspectionProcedure(InspectionProcedureScriptCommandsMixedXML);
            Assert.AreEqual(2, inspectionProcedure.InspectionSequence.Count);

            // script command 1 is first
            ScriptCommand1Entity scriptCommand1 = GetScriptCommandAt<ScriptCommand1Entity>(0, inspectionProcedure);
            Assert.AreEqual(4, scriptCommand1.SequenceNumber);
            Assert.AreEqual("Hartelijk welkom bij de inspectie", scriptCommand1.Text);

            // script command 70 is second
            ScriptCommand70Entity scriptCommand70 = GetScriptCommandAt<ScriptCommand70Entity>(1, inspectionProcedure);
            Assert.AreEqual(427, scriptCommand70.SequenceNumber);
            Assert.IsFalse(scriptCommand70.Mode);

        }

        [Test]
        public void LoadInspectionProcedureScriptCommandFromFileTest()
        {
            InspectionProcedureInformationLoader inspectionProcedureInfoLoader = null;
            Assert.DoesNotThrow(() => inspectionProcedureInfoLoader = new InspectionProcedureInformationLoader());
            Assert.AreEqual(12, inspectionProcedureInfoLoader.InspectionProcedures.Count);
        }
        #endregion XML Loader tests

        #region Constants
        private const string INSPECTION_STATUS_XML_ORG_FILE = @".\InspectionManager\Data\InspectionStatus.xml";
        private const string INSPECTION_STATUS_XML_TEST_FILE = @".\InspectionStatus.xml";
        #endregion Constants

        #region Test SetUp and TearDown
        [SetUp]
        public void SetUp()
        {
            var path = Path.GetDirectoryName(typeof(InspectionManagerInspectionProcedureInformationTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);

            File.Copy(INSPECTION_STATUS_XML_ORG_FILE, INSPECTION_STATUS_XML_TEST_FILE, overwrite: true);
        }

        [TearDown]
        public void TearDown()
        {
            File.Copy(INSPECTION_STATUS_XML_ORG_FILE, INSPECTION_STATUS_XML_TEST_FILE, overwrite: true);
        }
        #endregion Test SetUp and TearDown

        #region Inspection procedure information manager tests
        [Test]
        public void InspectionProcedureNamesTest()
        {
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            List<string> inspectionProcedureNames = inspectionInfoManager.InspectionProcedureNames.ToList();

            Assert.AreEqual(12, inspectionProcedureNames.Count);
            Assert.AreEqual("Wake up Inspection", inspectionProcedureNames[0]);
            Assert.AreEqual("SC5X Inspection 0 time", inspectionProcedureNames[1]);
            Assert.AreEqual("SC5X Inspection", inspectionProcedureNames[2]);
            Assert.AreEqual("SC5XAND70 Inspection", inspectionProcedureNames[3]);
            Assert.AreEqual("B-insp VAK-VA LS (Pd <= 300 mbar)", inspectionProcedureNames[4]);
            Assert.AreEqual("B-insp VAK-VA RS (Pd <= 300 mbar)", inspectionProcedureNames[5]);
            Assert.AreEqual("B-insp VAK-VA LS (Pd > 300 mbar)", inspectionProcedureNames[6]);
            Assert.AreEqual("B-insp VAK-VA RS (Pd > 300 mbar)", inspectionProcedureNames[7]);
            Assert.AreEqual("B-insp MON-VA LS (Pd <= 300 mbar)", inspectionProcedureNames[8]);
            Assert.AreEqual("B-insp MON-VA RS (Pd <= 300 mbar)", inspectionProcedureNames[9]);
            Assert.AreEqual("B-insp MON-VA LS (Pd > 300 mbar)", inspectionProcedureNames[10]);
            Assert.AreEqual("B-insp MON-VA RS (Pd > 300 mbar)", inspectionProcedureNames[11]);
        }

        [Test]
        public void InspectionProcedureSectionsTest()
        {
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            SectionSelection sectionSelection = inspectionInfoManager.LookupInspectionProcedureSections("B-insp VAK-VA LS (Pd <= 300 mbar)");

            Assert.AreEqual(61, sectionSelection.SectionSelectionEntities.Count);
            Assert.AreEqual("B-insp VAK-VA LS (Pd <= 300 mbar)", sectionSelection.InspectionProcedureName);
            Assert.AreEqual(1, sectionSelection.SectionSelectionEntities[0].SequenceNumber);
            Assert.AreEqual(3, sectionSelection.SectionSelectionEntities[1].SequenceNumber);
            Assert.AreEqual(7, sectionSelection.SectionSelectionEntities[2].SequenceNumber);

            long currentSequenceNumber = sectionSelection.SectionSelectionEntities[0].SequenceNumber;
            for (int i = 1; i < sectionSelection.SectionSelectionEntities.Count; i++)
            {
                long newSequenceNumber = sectionSelection.SectionSelectionEntities[i].SequenceNumber;
                Assert.Greater(newSequenceNumber, currentSequenceNumber);
                currentSequenceNumber = newSequenceNumber;
            }
        }

        [Test]
        public void LookupExistingInspectionProcedureSectionsTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";

            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            SectionSelection sectionSelection = inspectionInfoManager.LookupInspectionProcedureSections("B-insp MON-VA RS (Pd > 300 mbar)");

            sectionSelection.SectionSelectionEntities[0].IsSelected = true;
            sectionSelection.SectionSelectionEntities[1].IsSelected = true;
            sectionSelection.SectionSelectionEntities[12].IsSelected = true;
            sectionSelection.SectionSelectionEntities[38].IsSelected = true;

            InspectionProcedureInformation inspectionProcedureInfo = null;
            inspectionProcedureInfo = inspectionInfoManager.LookupPartialInspectionProcedure(sectionSelection, prsName, gclName);

            Assert.AreEqual("B-insp MON-VA RS (Pd > 300 mbar)", inspectionProcedureInfo.Name);
            Assert.AreEqual("1.0", inspectionProcedureInfo.Version);
            Assert.AreEqual(26, inspectionProcedureInfo.ScriptCommandSequence.Count);
            Assert.AreEqual(1, inspectionProcedureInfo.ScriptCommandSequence.Peek().SequenceNumber);

            long currentSequenceNumber = inspectionProcedureInfo.ScriptCommandSequence.Pop().SequenceNumber;
            while (inspectionProcedureInfo.ScriptCommandSequence.Count > 0)
            {
                long newSequenceNumer = inspectionProcedureInfo.ScriptCommandSequence.Pop().SequenceNumber;
                Assert.Greater(newSequenceNumer, currentSequenceNumber);
                currentSequenceNumber = newSequenceNumer;
            }
        }

        [Test]
        public void InspectionProcedureSectionsUnknownProcedureTest()
        {
            string inspectionProcedureName = "NONEXISTING_PROCEDURE_NAME";
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => inspectionInfoManager.LookupInspectionProcedureSections(inspectionProcedureName));
            Assert.AreEqual("Could not lookup the inspection procedure 'NONEXISTING_PROCEDURE_NAME'.", exception.Message);
        }

        [Test]
        public void InspectionInformationManagerCreationTest()
        {
            Assert.DoesNotThrow(() => new InspectionInformationManager());
        }

        [Test]
        public void InspectionInformationManagerRefreshTest()
        {
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            Assert.DoesNotThrow(() => inspectionInfoManager.Refresh());
        }

        [Test]
        public void LookupNonExistingInspectionProcedureInformationOnStationNameTest()
        {
            string prsName = "NONEXISTING_PRS_NAME";
            string inspectionProcedureName = "NONEXISTING_PROCEDURE_NAME";
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => inspectionInfoManager.LookupInspectionProcedure(inspectionProcedureName, prsName));
            Assert.AreEqual("Could not lookup the inspection procedure 'NONEXISTING_PROCEDURE_NAME'.", exception.Message);
        }

        [Test]
        public void LookupNonExistingInspectionProcedureInformationOnNullStationNameTest()
        {
            string prsName = null;
            string inspectionProcedureName = "NONEXISTING_PROCEDURE_NAME";
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => inspectionInfoManager.LookupInspectionProcedure(inspectionProcedureName, prsName));
            Assert.AreEqual("Could not lookup the inspection procedure 'NONEXISTING_PROCEDURE_NAME'.", exception.Message);
        }

        [Test]
        public void LookupNonExistingInspectionProcedureInformationOnGasControlLineTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "NONEXISTING_GCL_NAME";
            string inspectionProcedureName = "NONEXISTING_PROCEDURE_NAME";
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            InspectorLookupException exception = Assert.Throws<InspectorLookupException>(() => inspectionInfoManager.LookupInspectionProcedure(inspectionProcedureName, prsName, gclName));
            Assert.AreEqual("Could not lookup the inspection procedure 'NONEXISTING_PROCEDURE_NAME'.", exception.Message);
        }

        [Test]
        public void LookupExistingPrsInspectionProcedureInformationTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = null;
            string inspectionProcedureName = "B-insp MON-VA RS (Pd > 300 mbar)";
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            InspectionProcedureInformation inspectionProcedureInfo = null;
            Assert.DoesNotThrow(() => inspectionProcedureInfo = inspectionInfoManager.LookupInspectionProcedure(inspectionProcedureName, prsName, gclName));

            Assert.AreEqual("B-insp MON-VA RS (Pd > 300 mbar)", inspectionProcedureInfo.Name);
            Assert.AreEqual("1.0", inspectionProcedureInfo.Version);
            Assert.AreEqual(147, inspectionProcedureInfo.ScriptCommandSequence.Count);
        }


        [Test]
        public void LookupExistingGclInspectionProcedureInformationTest()
        {
            string prsName = "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1";
            string gclName = "5 007 230.1.2 OS RS";
            string inspectionProcedureName = "B-insp MON-VA RS (Pd > 300 mbar)";
            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            InspectionProcedureInformation inspectionProcedureInfo = null;
            Assert.DoesNotThrow(() => inspectionProcedureInfo = inspectionInfoManager.LookupInspectionProcedure(inspectionProcedureName, prsName, gclName));

            Assert.AreEqual("B-insp MON-VA RS (Pd > 300 mbar)", inspectionProcedureInfo.Name);
            Assert.AreEqual("1.0", inspectionProcedureInfo.Version);
            Assert.AreEqual(147, inspectionProcedureInfo.ScriptCommandSequence.Count);
        }

        #endregion Inspection procedure information manager tests

        #region Test helpers
        /// <summary>
        /// Gets the first inspection procedure.
        /// </summary>
        /// <param name="inspectionProcedureXML">The inspection procedure XML.</param>
        /// <returns></returns>
        private static InspectionProcedureEntity GetFirstInspectionProcedure(string inspectionProcedureXML)
        {
            InspectionProcedureInformationLoader inspectionProcedureInformation = new InspectionProcedureInformationLoader(inspectionProcedureXML);

            Assert.AreEqual(1, inspectionProcedureInformation.InspectionProcedures.Count);
            InspectionProcedureEntity inspectionProcedure = inspectionProcedureInformation.InspectionProcedures.First();
            return inspectionProcedure;
        }

        /// <summary>
        /// Gets the script command.
        /// </summary>
        /// <typeparam name="T">The script command specialization</typeparam>
        /// <param name="inspectionProcedure">The inspection procedure.</param>
        /// <returns></returns>
        private static T GetFirstScriptCommand<T>(InspectionProcedureEntity inspectionProcedure) where T : ScriptCommandEntityBase
        {
            return GetScriptCommandAt<T>(0, inspectionProcedure);
        }

        /// <summary>
        /// Gets the script command element at the given index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">The index.</param>
        /// <param name="inspectionProcedure">The inspection procedure.</param>
        /// <returns></returns>
        private static T GetScriptCommandAt<T>(int index, InspectionProcedureEntity inspectionProcedure) where T : ScriptCommandEntityBase
        {
            Assert.LessOrEqual(index - 1, inspectionProcedure.InspectionSequence.Count);
            Assert.IsInstanceOf<T>(inspectionProcedure.InspectionSequence[index]);
            return inspectionProcedure.InspectionSequence[index] as T;
        }
        #endregion Test helpers
    }
}
