/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers
{
    /// <summary>
    /// InspectionInformationManager, manages the inspection procedure information
    /// </summary>
    public class InspectionInformationManager : IInspectionInformationManager
    {
        #region Helpers
        /// <summary>
        /// StationStepIdentifier, used by InspectionInformationManager to lookup the required station information
        /// </summary>
        private class StationStepIdentifier
        {
            public int? FieldNo = null;
            public Guid InspectionPointId = Guid.Empty;
            public string ObjectName = null;
            public string MeasurePoint = null;
        }
        #endregion Helpers

        #region Constants
        private const string LOOKUP_STATION_INFO_FOR_PRS_AND_GCL_FAILED = "Failed to lookup the additional station information for PRS '{0}' and GCL '{1}'";
        private const string STATION_INFO_COULD_NOT_BE_RETRIEVED_FOR_PRS_AND_GCL = "Unknown failure while retrieving the station information for PRS '{0}' and GCL '{1}'";
        private const string LOOKUP_INSPECTION_PROCEDURE_FAILED = "Could not lookup the inspection procedure '{0}'.";
        #endregion Constants

        #region Members
        private InspectionProcedureInformationLoader m_InspectionProcedureInfoLoader;
        private StationInformationLoader m_StationInfoLoader;
        #endregion Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionInformationManager"/> class.
        /// </summary>
        public InspectionInformationManager()
        {
            m_InspectionProcedureInfoLoader = new InspectionProcedureInformationLoader();
            m_StationInfoLoader = new StationInformationLoader();
        }
        #endregion Constructors

        #region IInspectionInformationManager Members
        /// <summary>
        /// Gets the inspection procedure names.
        /// </summary>
        public ICollection<string> InspectionProcedureNames
        {
            get
            {
                var inspectionProcedureNames = m_InspectionProcedureInfoLoader.InspectionProcedures.Select(inspection => inspection.Name).ToList();
                return inspectionProcedureNames;
            }
        }

        /// <summary>
        /// Lookups the inspection procedure steps.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <returns>The InspectionProcedure sections</returns>
        /// <exception cref="InspectorLookupException">Thrown when the InspectionProcedure sections lookup fails</exception>
        public SectionSelection LookupInspectionProcedureSections(string inspectionProcedureName)
        {
            var sectionSelection = new SectionSelection();
            sectionSelection.InspectionProcedureName = inspectionProcedureName;

            try
            {
                var inspectionProcedureEntity =
                    m_InspectionProcedureInfoLoader.InspectionProcedures.Single(ip => ip.Name.Equals(inspectionProcedureName, StringComparison.OrdinalIgnoreCase));

                var scriptCommandSections = inspectionProcedureEntity.InspectionSequence.OfType<ScriptCommand2Entity>();

                foreach (var scriptCommand2 in scriptCommandSections)
                {
                    var sectionSelectionEntity = new SectionSelectionEntity(scriptCommand2.SequenceNumber, scriptCommand2.Section, scriptCommand2.SubSection);
                    sectionSelection.AddSectionSelectionEntity(sectionSelectionEntity);
                }
            }
            catch (InvalidOperationException exception)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture, LOOKUP_INSPECTION_PROCEDURE_FAILED, inspectionProcedureName);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return sectionSelection;
        }

        /// <summary>
        /// Lookups the inspection procedure.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gasControlLineName">Name of the gas control line.</param>
        /// <returns>
        /// The sequence of scriptcommands.
        /// </returns>
        /// <exception cref="InspectorLookupException">Thrown when the InspectionProcedure lookup fails</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public InspectionProcedureInformation LookupInspectionProcedure(string inspectionProcedureName, string prsName, string gasControlLineName = null)
        {
            InspectionProcedureInformation inspectionProcedureInfo = null;

            try
            {
                var inspectionProcedureEntity = m_InspectionProcedureInfoLoader.InspectionProcedures.Single(ip => ip.Name.Equals(inspectionProcedureName, StringComparison.OrdinalIgnoreCase));

                inspectionProcedureInfo = new InspectionProcedureInformation
                {
                    ScriptCommandSequence = CreateScriptCommandSequence(prsName, gasControlLineName, inspectionProcedureEntity),
                    Name = inspectionProcedureEntity.Name,
                    Version = inspectionProcedureEntity.Version
                };
            }
            catch (InvalidOperationException exception)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture, LOOKUP_INSPECTION_PROCEDURE_FAILED, inspectionProcedureName);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return inspectionProcedureInfo;
        }

        /// <summary>
        /// Determines if the inspection procedure with the name <paramref name="inspectionProcedureName"/> is defined.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <returns>True if the inspection procedure is defined, otherwise false.</returns>
        public bool InspectionProcedureExists(string inspectionProcedureName)
        {
            return m_InspectionProcedureInfoLoader.InspectionProcedures.Any(inspectionProcedure => inspectionProcedure.Name.Equals(inspectionProcedureName, StringComparison.Ordinal));
        }

        public void FindObjectNameDescriptionAndMeasurePointDescription(string objectName, string measurePoint,
            int? fieldNo, out string objectNameDescription, out string measurePointDescription)
        {
            objectNameDescription = "";
            measurePointDescription = "";
            foreach (var procedure in m_InspectionProcedureInfoLoader.InspectionProcedures)
            {
                foreach (ScriptCommandEntityDescriptions commandEntity in procedure.InspectionSequence.Where(i => i is ScriptCommandEntityDescriptions))
                {
                    if ((!string.IsNullOrEmpty(measurePoint) && !string.IsNullOrEmpty(objectName) && commandEntity.MeasurePoint != null && commandEntity.MeasurePoint.Equals(measurePoint) && commandEntity.ObjectName.Equals(objectName)) || 
                        (fieldNo != null && commandEntity.FieldNo.Equals(fieldNo)))
                    {
                        objectNameDescription = commandEntity.ObjectNameDescription;
                        measurePointDescription = commandEntity.MeasurePointDescription;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Lookups the inspection procedure selection.
        /// </summary>
        /// <param name="sectionSelection">The section selection.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gasControlLineName">Name of the gas control line.</param>
        /// <returns></returns>
        /// <exception cref="InspectorLookupException">Thrown when the InspectionProcedure lookup fails</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public InspectionProcedureInformation LookupPartialInspectionProcedure(SectionSelection sectionSelection, string prsName, string gasControlLineName = null)
        {
            InspectionProcedureInformation inspectionProcedureInfo = null;

            try
            {
                var inspectionProcedureEntity =
                    m_InspectionProcedureInfoLoader.InspectionProcedures.Single(ip => ip.Name.Equals(sectionSelection.InspectionProcedureName, StringComparison.OrdinalIgnoreCase));

                inspectionProcedureInfo = new InspectionProcedureInformation
                {
                    ScriptCommandSequence = CreatePartialScriptSequence(sectionSelection, inspectionProcedureEntity, prsName, gasControlLineName),
                    Name = inspectionProcedureEntity.Name,
                    Version = inspectionProcedureEntity.Version,
                };
            }
            catch (InvalidOperationException exception)
            {
                var exceptionMessage = String.Format(CultureInfo.InvariantCulture, LOOKUP_INSPECTION_PROCEDURE_FAILED, sectionSelection.InspectionProcedureName);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return inspectionProcedureInfo;

        }


        /// <summary>
        /// Refresh the data by reloading the storage backend.
        /// </summary>
        public async Task Refresh()
        {
            m_StationInfoLoader.Reload();
            m_InspectionProcedureInfoLoader.Reload();
        }
        #endregion IInspectionInformationManager Members

        #region Scriptcommand creation
        /// <summary>
        /// Creates the script command partial sequence.
        /// </summary>
        /// <param name="sectionSelection">The session entity.</param>
        /// <param name="inspectionProcedureEntity">The inspection procedure entity.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        private Stack<ScriptCommandBase> CreatePartialScriptSequence(SectionSelection sectionSelection, InspectionProcedureEntity inspectionProcedureEntity, string prsName, string gclName)
        {
            var sequence = new Stack<ScriptCommandBase>();
            var addCurrentScriptCommand = true;

            foreach (var scriptCommand in inspectionProcedureEntity.InspectionSequence)
            {
                // First: Check if the script command is a 2, and set the addCurrentScriptCommand accordingly
                if (scriptCommand is ScriptCommand2Entity)
                {
                    var sectionSelectionEntity = sectionSelection.SectionSelectionEntities.First(sc2 => sc2.SequenceNumber == scriptCommand.SequenceNumber);
                    addCurrentScriptCommand = sectionSelectionEntity.IsSelected;
                }

                // Then: if the addCurrentScriptCommand = true, add the current ScriptCommand
                if (addCurrentScriptCommand)
                {
                    var scriptCommandResult = CreateScriptCommand(prsName, gclName, scriptCommand);
                    sequence.Push(scriptCommandResult);
                }
            }

            // By creating a new stack with the sequence as construction parameter, the stack is reversed, which is exactly what we want here.
            var result = new Stack<ScriptCommandBase>(sequence);

            return result;
        }

        /// <summary>
        /// Creates the script command sequence.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="inspectionProcedureEntity">The inspection procedure entity.</param>
        /// <returns>
        /// The sequence of ScriptCommands
        /// </returns>
        private Stack<ScriptCommandBase> CreateScriptCommandSequence(string prsName, string gclName, InspectionProcedureEntity inspectionProcedureEntity)
        {
            var sequence = new Stack<ScriptCommandBase>();

            foreach (var scriptCommand in inspectionProcedureEntity.InspectionSequence.Reverse<ScriptCommandEntityBase>())
            {
                var scriptCommandResult = CreateScriptCommand(prsName, gclName, scriptCommand);
                sequence.Push(scriptCommandResult);
            }

            return sequence;
        }

        /// <summary>
        /// Creates the script command.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private ScriptCommandBase CreateScriptCommand(string prsName, string gclName, ScriptCommandEntityBase scriptCommand)
        {
            ScriptCommandBase scriptCommandResult = null;

            if (scriptCommand is ScriptCommand1Entity)
            {
                scriptCommandResult = CreateScriptCommand1(scriptCommand);
            }
            else if (scriptCommand is ScriptCommand2Entity)
            {
                scriptCommandResult = CreateScriptCommand2(scriptCommand);
            }
            else if (scriptCommand is ScriptCommand3Entity)
            {
                scriptCommandResult = CreateScriptCommand3(scriptCommand);
            }
            else if (scriptCommand is ScriptCommand4Entity)
            {
                scriptCommandResult = CreateScriptCommand4(scriptCommand, prsName, gclName);
            }
            else if (scriptCommand is ScriptCommand41Entity)
            {
                scriptCommandResult = CreateScriptCommand41(scriptCommand, prsName, gclName);
            }
            else if (scriptCommand is ScriptCommand42Entity)
            {
                scriptCommandResult = CreateScriptCommand42(scriptCommand, prsName, gclName);
            }
            else if (scriptCommand is ScriptCommand43Entity)
            {
                scriptCommandResult = CreateScriptCommand43(scriptCommand, prsName, gclName);
            }
            else if (scriptCommand is ScriptCommand5XEntity)
            {
                scriptCommandResult = CreateScriptCommand5X(scriptCommand, prsName, gclName);
            }
            else if (scriptCommand is ScriptCommand70Entity)
            {
                scriptCommandResult = CreateScriptCommand70(scriptCommand);
            }

            scriptCommandResult.LinkId = Guid.NewGuid();

            return scriptCommandResult;
        }

        /// <summary>
        /// Creates the script command1.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private static ScriptCommandBase CreateScriptCommand1(ScriptCommandEntityBase scriptCommand)
        {
            var scriptCommand1Entity = scriptCommand as ScriptCommand1Entity;
            var scriptCommand1 = new ScriptCommand1
            {
                SequenceNumber = scriptCommand1Entity.SequenceNumber,
                Text = scriptCommand1Entity.Text,
            };
            return scriptCommand1;
        }

        /// <summary>
        /// Creates the script command2.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private static ScriptCommandBase CreateScriptCommand2(ScriptCommandEntityBase scriptCommand)
        {
            var scriptCommand2Entity = scriptCommand as ScriptCommand2Entity;
            var scriptCommand2 = new ScriptCommand2
            {
                SequenceNumber = scriptCommand2Entity.SequenceNumber,
                Section = scriptCommand2Entity.Section,
                SubSection = scriptCommand2Entity.SubSection,
            };
            return scriptCommand2;
        }

        /// <summary>
        /// Creates the script command3.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private static ScriptCommandBase CreateScriptCommand3(ScriptCommandEntityBase scriptCommand)
        {
            var scriptCommand3Entity = scriptCommand as ScriptCommand3Entity;
            var scriptCommand3 = new ScriptCommand3
            {
                SequenceNumber = scriptCommand3Entity.SequenceNumber,
                Text = scriptCommand3Entity.Text,
                Duration = scriptCommand3Entity.Duration,
            };
            return scriptCommand3;
        }

        /// <summary>
        /// Creates the script command4.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        private ScriptCommandBase CreateScriptCommand4(ScriptCommandEntityBase scriptCommand, string prsName, string gclName)
        {
            var scriptCommand4Entity = scriptCommand as ScriptCommand4Entity;
            var scriptCommand4 = new ScriptCommand4
            {
                SequenceNumber = scriptCommand4Entity.SequenceNumber,
                Question = scriptCommand4Entity.Question,
                TypeQuestion = scriptCommand4Entity.TypeQuestion,
                TextOptions = new List<string>(scriptCommand4Entity.TextOptions),
                Required = scriptCommand4Entity.Required,
                StationStepObject = LookUpStationStepObject(scriptCommand, prsName, gclName),
                ObjectNameDescription = scriptCommand4Entity.ObjectNameDescription,
                MeasurePointDescription = scriptCommand4Entity.MeasurePointDescription,
                FieldNo = scriptCommand4Entity.FieldNo,
                InspectionPointId = scriptCommand4Entity.InspectionPointId
            };

            return scriptCommand4;
        }

        /// <summary>
        /// Creates the script command41.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        private ScriptCommandBase CreateScriptCommand41(ScriptCommandEntityBase scriptCommand, string prsName, string gclName)
        {
            var scriptCommand41Entity = scriptCommand as ScriptCommand41Entity;
            var scriptCommand41 = new ScriptCommand41
            {
                SequenceNumber = scriptCommand41Entity.SequenceNumber,
                ShowNextListImmediately = scriptCommand41Entity.ShowNextListImmediatly,
                ScriptCommandList = ConvertToScriptCommandList(scriptCommand41Entity.ScriptCommandList),
                StationStepObject = LookUpStationStepObject(scriptCommand, prsName, gclName),
                ObjectNameDescription = scriptCommand41Entity.ObjectNameDescription,
                MeasurePointDescription = scriptCommand41Entity.MeasurePointDescription,
                FieldNo = scriptCommand41Entity.FieldNo,
                InspectionPointId = scriptCommand41Entity.InspectionPointId
            };

            return scriptCommand41;
        }

        /// <summary>
        /// Creates the script command42.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        private ScriptCommandBase CreateScriptCommand42(ScriptCommandEntityBase scriptCommand, string prsName, string gclName)
        {
            var scriptCommand42Entity = scriptCommand as ScriptCommand42Entity;
            var scriptCommand42 = new ScriptCommand42
            {
                SequenceNumber = scriptCommand42Entity.SequenceNumber,
                StationStepObject = LookUpStationStepObject(scriptCommand, prsName, gclName),
                ObjectNameDescription = scriptCommand42Entity.ObjectNameDescription,
                MeasurePointDescription = scriptCommand42Entity.MeasurePointDescription,
                FieldNo = scriptCommand42Entity.FieldNo,
                InspectionPointId= scriptCommand42Entity.InspectionPointId
            };


            return scriptCommand42;
        }

        /// <summary>
        /// Creates the script command43.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        private ScriptCommandBase CreateScriptCommand43(ScriptCommandEntityBase scriptCommand, string prsName, string gclName)
        {
            var scriptCommand43Entity = scriptCommand as ScriptCommand43Entity;
            var scriptCommand43 = new ScriptCommand43
            {
                SequenceNumber = scriptCommand43Entity.SequenceNumber,
                Instruction = scriptCommand43Entity.Instruction,
                ListItems = scriptCommand43Entity.ListItems.ToList(),
                Required = scriptCommand43Entity.Required,
                StationStepObject = LookUpStationStepObject(scriptCommand, prsName, gclName),
                ObjectNameDescription = scriptCommand43Entity.ObjectNameDescription,
                MeasurePointDescription = scriptCommand43Entity.MeasurePointDescription,
                FieldNo = scriptCommand43Entity.FieldNo,
                InspectionPointId = scriptCommand43Entity.InspectionPointId
            };

            return scriptCommand43;
        }

        /// <summary>
        /// Creates the script command5 X.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        private ScriptCommandBase CreateScriptCommand5X(ScriptCommandEntityBase scriptCommand, string prsName, string gclName)
        {
            var scriptCommand5XEntity = scriptCommand as ScriptCommand5XEntity;
            var scriptCommand5X = new ScriptCommand5X
            {
                SequenceNumber = scriptCommand5XEntity.SequenceNumber,
                ScriptCommand = scriptCommand5XEntity.ScriptCommand5X,
                Instruction = scriptCommand5XEntity.Instruction,
                DigitalManometer = scriptCommand5XEntity.DigitalManometer,
                MeasurementFrequency = scriptCommand5XEntity.MeasurementFrequency,
                MeasurementPeriod = scriptCommand5XEntity.MeasurementPeriod,
                ExtraMeasurementPeriod = scriptCommand5XEntity.ExtraMeasurementPeriod,
                Leakage = scriptCommand5XEntity.Leakage,
                StationStepObject = LookUpStationStepObject(scriptCommand, prsName, gclName),
                ObjectNameDescription = scriptCommand5XEntity.ObjectNameDescription,
                MeasurePointDescription = scriptCommand5XEntity.MeasurePointDescription,
                FieldNo = scriptCommand5XEntity.FieldNo,
                InspectionPointId = scriptCommand5XEntity.InspectionPointId,
            };

            return scriptCommand5X;
        }

        /// <summary>
        /// Creates the script command70.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private static ScriptCommandBase CreateScriptCommand70(ScriptCommandEntityBase scriptCommand)
        {
            var scriptCommand70Entity = scriptCommand as ScriptCommand70Entity;
            var scriptCommand70 = new ScriptCommand70
            {
                SequenceNumber = scriptCommand70Entity.SequenceNumber,
                Mode = scriptCommand70Entity.Mode,
            };

            return scriptCommand70;
        }
        #endregion Scriptcommand creation

        #region Private helpers


        /// <summary>
        /// Gets the station step identifiers.
        /// </summary>
        /// <param name="scriptCommandEntity">The script command entity.</param>
        /// <param name="FieldNo">The field no.</param>
        /// <param name="ObjectName">Name of the object.</param>
        /// <param name="MeasurePoint">The measure point.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static StationStepIdentifier GetStationStepIdentifiers(ScriptCommandEntityBase scriptCommandEntity)
        {
            StationStepIdentifier stationStepIdentifier = null;

            if (scriptCommandEntity is ScriptCommand4Entity)
            {
                var scriptCommand4Entity = scriptCommandEntity as ScriptCommand4Entity;
                stationStepIdentifier = new StationStepIdentifier
                {
                    FieldNo = scriptCommand4Entity.FieldNo,
                    ObjectName = scriptCommand4Entity.ObjectName,
                    MeasurePoint = scriptCommand4Entity.MeasurePoint,
                    InspectionPointId= scriptCommand4Entity.InspectionPointId,
                };
            }
            else if (scriptCommandEntity is ScriptCommand41Entity)
            {
                var scriptCommand41Entity = scriptCommandEntity as ScriptCommand41Entity;
                stationStepIdentifier = new StationStepIdentifier
                {
                    FieldNo = scriptCommand41Entity.FieldNo,
                    ObjectName = scriptCommand41Entity.ObjectName,
                    MeasurePoint = scriptCommand41Entity.MeasurePoint,
                    InspectionPointId = scriptCommand41Entity.InspectionPointId,
                };
            }
            else if (scriptCommandEntity is ScriptCommand42Entity)
            {
                var scriptCommand42Entity = scriptCommandEntity as ScriptCommand42Entity;
                stationStepIdentifier = new StationStepIdentifier
                {
                    FieldNo = scriptCommand42Entity.FieldNo,
                    ObjectName = scriptCommand42Entity.ObjectName,
                    MeasurePoint = scriptCommand42Entity.MeasurePoint,
                    InspectionPointId = scriptCommand42Entity.InspectionPointId
                };
            }
            else if (scriptCommandEntity is ScriptCommand43Entity)
            {
                var scriptCommand43Entity = scriptCommandEntity as ScriptCommand43Entity;
                stationStepIdentifier = new StationStepIdentifier
                {
                    FieldNo = scriptCommand43Entity.FieldNo,
                    ObjectName = scriptCommand43Entity.ObjectName,
                    MeasurePoint = scriptCommand43Entity.MeasurePoint,
                    InspectionPointId = scriptCommand43Entity.InspectionPointId
                };
            }
            else if (scriptCommandEntity is ScriptCommand5XEntity)
            {
                var scriptCommand5XEntity = scriptCommandEntity as ScriptCommand5XEntity;
                stationStepIdentifier = new StationStepIdentifier
                {
                    FieldNo = scriptCommand5XEntity.FieldNo,
                    ObjectName = scriptCommand5XEntity.ObjectName,
                    MeasurePoint = scriptCommand5XEntity.MeasurePoint,
                    InspectionPointId = scriptCommand5XEntity.InspectionPointId
                };
            }

            return stationStepIdentifier;
        }

        /// <summary>
        /// Looks up station step object.
        /// </summary>
        /// <param name="scriptCommandEntity">The script command entity.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        private StationStepObject LookUpStationStepObject(ScriptCommandEntityBase scriptCommandEntity, string prsName, string gclName)
        {
            var stationStepObject = new StationStepObject();

            try
            {
                var stationStepIdentifier = GetStationStepIdentifiers(scriptCommandEntity);

                var isLookUpOnFieldNo = (stationStepIdentifier.FieldNo.HasValue);
                var isLookUpOnInspectionPoint = stationStepIdentifier.InspectionPointId != Guid.Empty;
                var isLookUpOnObjectNameAndMeasurepoint = (!string.IsNullOrEmpty(stationStepIdentifier.ObjectName) || !string.IsNullOrEmpty(stationStepIdentifier.MeasurePoint));

                var isPRSInspection = (string.IsNullOrEmpty(gclName));

                stationStepObject = isPRSInspection ? CreateStationStepObject(stationStepIdentifier, prsName, isLookUpOnFieldNo, isLookUpOnObjectNameAndMeasurepoint, isLookUpOnInspectionPoint) : CreateStationStepObject(stationStepIdentifier, prsName, gclName, isLookUpOnFieldNo, isLookUpOnObjectNameAndMeasurepoint, isLookUpOnInspectionPoint);
            }
            catch (InvalidOperationException exception)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture, LOOKUP_STATION_INFO_FOR_PRS_AND_GCL_FAILED, prsName, gclName ?? string.Empty);
                throw new InspectorLookupException(exceptionMessage, exception);
            }
            catch (Exception exception)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture, STATION_INFO_COULD_NOT_BE_RETRIEVED_FOR_PRS_AND_GCL, prsName, gclName ?? string.Empty);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return stationStepObject;
        }

        /// <summary>
        /// Creates the station step object.
        /// </summary>
        /// <param name="stationStepIdentifier">The station step identifier.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="isLookUpOnFieldNo">if set to <c>true</c> [is look up on field no].</param>
        /// <param name="isLookUpOnObjectNameAndMeasurepoint">if set to <c>true</c> [is look up on object name and measurepoint].</param>
        /// <returns></returns>
        private StationStepObject CreateStationStepObject(StationStepIdentifier stationStepIdentifier, string prsName, bool isLookUpOnFieldNo, bool isLookUpOnObjectNameAndMeasurepoint, bool isLookUpOnInspectionPoint)
        {
            PRSObject prsObject = null;

            var prsEntity = m_StationInfoLoader.PRSEntities.Single(prs => prs.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase));
            //if (isLookUpOnFieldNo)
            //{
            //    prsObject = prsEntity.PRSObjects.SingleOrDefault(item => item.FieldNo.HasValue && item.FieldNo.Value == stationStepIdentifier.FieldNo.Value);
            //}

            Console.WriteLine(isLookUpOnFieldNo);

            if (isLookUpOnInspectionPoint)
            {
                prsObject = prsEntity.PRSObjects.SingleOrDefault(item => item.InspectionPointId != Guid.Empty && item.InspectionPointId == stationStepIdentifier.InspectionPointId);
            }
            else if (isLookUpOnObjectNameAndMeasurepoint)
            {
                prsObject = prsEntity.PRSObjects.SingleOrDefault(item =>
                    item.ObjectName.Equals(stationStepIdentifier.ObjectName, StringComparison.OrdinalIgnoreCase) &&
                    item.MeasurePoint.Equals(stationStepIdentifier.MeasurePoint, StringComparison.OrdinalIgnoreCase));
            }

            return CreateStationStepObject(stationStepIdentifier, prsObject);
        }

        /// <summary>
        /// Creates the station step object.
        /// </summary>
        /// <param name="stationStepIdentifier">The station step identifier.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="isLookUpOnFieldNo">if set to <c>true</c> [is look up on field no].</param>
        /// <param name="isLookUpOnObjectNameAndMeasurepoint">if set to <c>true</c> [is look up on object name and measurepoint].</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "prsEntity")]
        private StationStepObject CreateStationStepObject(StationStepIdentifier stationStepIdentifier, string prsName, string gclName, bool isLookUpOnFieldNo, bool isLookUpOnObjectNameAndMeasurepoint, bool isLookUpOnInspectionPoint)
        {
            GCLObject gclObject = null;
            var prsEntity = m_StationInfoLoader.PRSEntities.Single(prs => prs.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase));
            var gclEntity = prsEntity.GasControlLines.Single(gcl => gcl.GasControlLineName.Equals(gclName, StringComparison.OrdinalIgnoreCase));

            //if (isLookUpOnFieldNo)
            //{
            //    gclObject = gclEntity.GCLObjects.SingleOrDefault(item => item.FieldNo.HasValue && item.FieldNo.Value == stationStepIdentifier.FieldNo.Value);
            //}

            Console.WriteLine(isLookUpOnFieldNo);

            if (isLookUpOnInspectionPoint)
            {
                gclObject = gclEntity.GCLObjects.SingleOrDefault(item => item.InspectionPointId != Guid.Empty && item.InspectionPointId == stationStepIdentifier.InspectionPointId);
            }
            else if (isLookUpOnObjectNameAndMeasurepoint)
            {
                gclObject = gclEntity.GCLObjects.SingleOrDefault(item =>
                    item.ObjectName.Equals(stationStepIdentifier.ObjectName, StringComparison.OrdinalIgnoreCase) &&
                    item.MeasurePoint.Equals(stationStepIdentifier.MeasurePoint, StringComparison.OrdinalIgnoreCase));
            }

            return CreateStationStepObject(stationStepIdentifier, gclObject);
        }

        /// <summary>
        /// Creates the station step object.
        /// </summary>
        /// <param name="stationStepIdentifier">The station step identifier.</param>
        /// <param name="gclObject">The GCL object.</param>
        /// <returns></returns>
        private static StationStepObject CreateStationStepObject(StationStepIdentifier stationStepIdentifier, GCLObject gclObject)
        {
            var stationStepObject = new StationStepObject
            {
                FieldNo = stationStepIdentifier.FieldNo,
                ObjectName = stationStepIdentifier.ObjectName,
                MeasurePoint = stationStepIdentifier.MeasurePoint,
                InspectionPointId = stationStepIdentifier.InspectionPointId
            };

            if (gclObject != null)
            {
                stationStepObject.ObjectID = gclObject.ObjectID;
                stationStepObject.MeasurePointID = gclObject.MeasurePointID;

                if (gclObject.Boundaries != null)
                {
                    stationStepObject.Boundaries = new StationStepObjectBoundaries
                    {
                        ValueMin = gclObject.Boundaries.ValueMin,
                        ValueMax = gclObject.Boundaries.ValueMax,
                        UOV = gclObject.Boundaries.UOV,
                        ScriptCommandId = gclObject.Boundaries.ScriptCommandId,
                        Offset = gclObject.Boundaries.Offset
                    };
                }
            }
            return stationStepObject;
        }

        /// <summary>
        /// Creates the station step object.
        /// </summary>
        /// <param name="stationStepIdentifier">The station step identifier.</param>
        /// <param name="prsObject">The PRS object.</param>
        /// <returns></returns>
        private static StationStepObject CreateStationStepObject(StationStepIdentifier stationStepIdentifier, PRSObject prsObject)
        {
            var stationStepObject = new StationStepObject
            {
                FieldNo = stationStepIdentifier.FieldNo,
                ObjectName = stationStepIdentifier.ObjectName,
                MeasurePoint = stationStepIdentifier.MeasurePoint,
                InspectionPointId = stationStepIdentifier.InspectionPointId
            };

            if (prsObject != null)
            {
                stationStepObject.ObjectID = prsObject.ObjectID;
                stationStepObject.MeasurePointID = prsObject.MeasurePointID;
            }
            return stationStepObject;
        }

        /// <summary>
        /// Converts to script command 41 list entity to script command 41 list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private static List<ScriptCommand41List> ConvertToScriptCommandList(List<ScriptCommand41ListEntity> scriptCommandlist)
        {
            var result = new List<ScriptCommand41List>();

            foreach (var scriptCommand41Entity in scriptCommandlist)
            {
                var scriptCommand41List = new ScriptCommand41List
                {
                    CheckListResult = scriptCommand41Entity.CheckListResult,
                    ListQuestion = scriptCommand41Entity.ListQuestion,
                    ListType = scriptCommand41Entity.ListType,
                    OneSelectionAllowed = scriptCommand41Entity.OneSelectionAllowed,
                    SelectionRequired = scriptCommand41Entity.SelectionRequired,
                    SequenceNumberList = scriptCommand41Entity.SequenceNumberList,
                    ListConditionCodes = ConvertToListConditionCodes(scriptCommand41Entity.ListConditionCodes),
                };
                result.Add(scriptCommand41List);
            }

            return result;
        }

        /// <summary>
        /// Converts to list condition codes.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private static List<ListConditionCode> ConvertToListConditionCodes(List<ListConditionCodeEntity> conditionCodeList)
        {
            var result = new List<ListConditionCode>();

            foreach (var listConditionCodeEntity in conditionCodeList)
            {
                var listConditionCode = new ListConditionCode
                {
                    ConditionCode = listConditionCodeEntity.ConditionCode,
                    ConditionCodeDescription = listConditionCodeEntity.ConditionCodeDescription,
                    DisplayNextList = listConditionCodeEntity.DisplayNextList,
                };
                result.Add(listConditionCode);
            }

            return result;
        }
        #endregion Private helpers
    }
}
