/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Linq;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic
{
    /// <summary>
    /// Utility functions for Inspection.
    /// </summary>
    internal static class InspectionUtils
    {
        /// <summary>
        /// Copies the specified script command.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns>A deep copy of the script command or null if the scriptcommand type is unknown.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static ScriptCommandBase CopyScriptCommand(this ScriptCommandBase scriptCommand)
        {
            ScriptCommandBase result = null;

            if (scriptCommand is ScriptCommand1)
            {
                result = CopyScriptCommand1(scriptCommand as ScriptCommand1);
            }
            else if (scriptCommand is ScriptCommand2)
            {
                result = CopyScriptCommand2(scriptCommand as ScriptCommand2);
            }
            else if (scriptCommand is ScriptCommand3)
            {
                result = CopyScriptCommand3(scriptCommand as ScriptCommand3);
            }
            else if (scriptCommand is ScriptCommand4)
            {
                result = CopyScriptCommand4(scriptCommand as ScriptCommand4);
            }
            else if (scriptCommand is ScriptCommand41)
            {
                result = CopyScriptCommand41(scriptCommand as ScriptCommand41);
            }
            else if (scriptCommand is ScriptCommand42)
            {
                result = CopyScriptCommand42(scriptCommand as ScriptCommand42);
            }
            else if (scriptCommand is ScriptCommand43)
            {
                result = CopyScriptCommand43(scriptCommand as ScriptCommand43);
            }
            else if (scriptCommand is ScriptCommand5X)
            {
                result = CopyScriptCommand5X(scriptCommand as ScriptCommand5X);
            }
            else if (scriptCommand is ScriptCommand70)
            {
                result = CopyScriptCommand70(scriptCommand as ScriptCommand70);
            }

            return result;
        }

        #region Script command copiers
        /// <summary>
        /// Copies the script command 1.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 1</returns>
        private static ScriptCommandBase CopyScriptCommand1(ScriptCommand1 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand1
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                Text = scriptCommand.Text,
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 2.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 2</returns>
        private static ScriptCommandBase CopyScriptCommand2(ScriptCommand2 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand2
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                Section = scriptCommand.Section,
                SubSection = scriptCommand.SubSection,
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 3.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 3</returns>
        private static ScriptCommandBase CopyScriptCommand3(ScriptCommand3 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand3
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                Text = scriptCommand.Text,
                Duration = scriptCommand.Duration,
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 4.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 4</returns>
        private static ScriptCommandBase CopyScriptCommand4(ScriptCommand4 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand4
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                StationStepObject = CopyStationStepObject(scriptCommand.StationStepObject),
                Question = scriptCommand.Question,
                TypeQuestion = scriptCommand.TypeQuestion,
                TextOptions = scriptCommand.TextOptions.ToList(),
                Required = scriptCommand.Required,
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 41.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 41</returns>
        private static ScriptCommandBase CopyScriptCommand41(ScriptCommand41 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand41
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                StationStepObject = CopyStationStepObject(scriptCommand.StationStepObject),
                ShowNextListImmediately = scriptCommand.ShowNextListImmediately,
                ScriptCommandList = CopyScriptCommand41List(scriptCommand.ScriptCommandList),
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 42.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 42</returns>
        private static ScriptCommandBase CopyScriptCommand42(ScriptCommand42 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand42
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                StationStepObject = CopyStationStepObject(scriptCommand.StationStepObject),
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 43.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 43</returns>
        private static ScriptCommandBase CopyScriptCommand43(ScriptCommand43 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand43
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                StationStepObject = CopyStationStepObject(scriptCommand.StationStepObject),
                Instruction = scriptCommand.Instruction,
                ListItems = scriptCommand.ListItems.ToList(),
                Required = scriptCommand.Required,
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 5X.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 5X</returns>
        private static ScriptCommandBase CopyScriptCommand5X(ScriptCommand5X scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand5X
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                StationStepObject = CopyStationStepObject(scriptCommand.StationStepObject),
                ScriptCommand = scriptCommand.ScriptCommand,
                Instruction = scriptCommand.Instruction,
                DigitalManometer = scriptCommand.DigitalManometer,
                MeasurePointDescription = scriptCommand.MeasurePointDescription,
                ObjectNameDescription = scriptCommand.ObjectNameDescription,
                MeasurementFrequency = scriptCommand.MeasurementFrequency,
                MeasurementPeriod = scriptCommand.MeasurementPeriod,
                ExtraMeasurementPeriod = scriptCommand.ExtraMeasurementPeriod,
                Leakage = scriptCommand.Leakage,
            };
            return result;
        }

        /// <summary>
        /// Copies the script command 70.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="result">The result.</param>
        /// <returns>Deep copy of script command 70</returns>
        private static ScriptCommandBase CopyScriptCommand70(ScriptCommand70 scriptCommand)
        {
            ScriptCommandBase result = new ScriptCommand70
            {
                SequenceNumber = scriptCommand.SequenceNumber,
                Mode = scriptCommand.Mode,
            };
            return result;
        }
        #endregion Script command copiers

        #region Script command copy helpers
        /// <summary>
        /// Copies the script command41 list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private static List<ScriptCommand41List> CopyScriptCommand41List(List<ScriptCommand41List> list)
        {
            List<ScriptCommand41List> results = new List<ScriptCommand41List>();

            foreach (ScriptCommand41List scriptCommand41List in list)
            {
                results.Add(new ScriptCommand41List
                {
                    SequenceNumberList = scriptCommand41List.SequenceNumberList,
                    ListType = scriptCommand41List.ListType,
                    SelectionRequired = scriptCommand41List.SelectionRequired,
                    ListQuestion = scriptCommand41List.ListQuestion,
                    OneSelectionAllowed = scriptCommand41List.OneSelectionAllowed,
                    CheckListResult = scriptCommand41List.CheckListResult,
                    ListConditionCodes = CopyListConditionCodes(scriptCommand41List.ListConditionCodes),
                });
            }
            return results;
        }

        /// <summary>
        /// Copies the list condition codes.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private static List<ListConditionCode> CopyListConditionCodes(List<ListConditionCode> list)
        {
            List<ListConditionCode> results = new List<ListConditionCode>();

            foreach (ListConditionCode listConditionCode in list)
            {
                results.Add(new ListConditionCode
                {
                    ConditionCode = listConditionCode.ConditionCode,
                    ConditionCodeDescription = listConditionCode.ConditionCodeDescription,
                    DisplayNextList = listConditionCode.DisplayNextList,
                });
            }

            return results;
        }

        /// <summary>
        /// Copies the station step object.
        /// </summary>
        /// <param name="stationStepObject">The station step object.</param>
        /// <returns>Deep copy of the stationstepobject</returns>
        private static StationStepObject CopyStationStepObject(StationStepObject stationStepObject)
        {
            StationStepObject result = new StationStepObject
            {
                FieldNo = stationStepObject.FieldNo,
                MeasurePoint = stationStepObject.MeasurePoint,
                MeasurePointID = stationStepObject.MeasurePointID,
                ObjectName = stationStepObject.ObjectName,
                ObjectID = stationStepObject.ObjectID,
            };

            if (stationStepObject.Boundaries != null)
            {
                result.Boundaries = new StationStepObjectBoundaries
                {
                    ValueMin = stationStepObject.Boundaries.ValueMin,
                    ValueMax = stationStepObject.Boundaries.ValueMax,
                    UOV = stationStepObject.Boundaries.UOV,
                };
            }

            return result;
        }
        #endregion Script command copy helpers

    }
}
