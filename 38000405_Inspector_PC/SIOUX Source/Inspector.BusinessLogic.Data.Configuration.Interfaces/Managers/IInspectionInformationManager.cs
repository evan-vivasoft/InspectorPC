/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
{
    /// <summary>
    /// InspectionInformationManager interface.
    /// The inspection information manager loads the inspection procedure xml file. This interface contains function to search for inspection steps and procedures.
    /// </summary>
    public interface IInspectionInformationManager : IInformationManager
    {
        /// <summary>
        /// Gets the inspection procedure names.
        /// </summary>
        /// <returns>
        /// A collection of Inspection Procedure Names</returns>
        ICollection<string> InspectionProcedureNames { get; }

        /// <summary>
        /// Looks up the available inspection procedure sections that can be selected to start a partial inspection.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <returns>The InspectionProcedure sections</returns>
        /// <exception cref="InspectorLookupException">Thrown when the InspectionProcedure sections lookup fails</exception>
        SectionSelection LookupInspectionProcedureSections(string inspectionProcedureName);

        /// <summary>
        /// Looks up the inspection procedure based on the procedurename, prsName and optional gclName.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gasControlLineName">Name of the gas control line. (optional)</param>
        /// <returns>
        /// The inspection procedure information, includes the scriptcommand sequence and the related procedure information.
        /// </returns>
        /// <exception cref="InspectorLookupException">Thrown when the InspectionProcedure sections lookup fails</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        InspectionProcedureInformation LookupInspectionProcedure(string inspectionProcedureName, string prsName, string gasControlLineName = null);

        /// <summary>
        /// Looks up the inspection procedure based on selected sections, prsName, and optional gclName.
        /// </summary>
        /// <param name="sectionSelection">The section selection.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gasControlLineName">Name of the gas control line.</param>
        /// <returns></returns>
        /// <exception cref="InspectorLookupException">Thrown when the InspectionProcedure lookup fails</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        InspectionProcedureInformation LookupPartialInspectionProcedure(SectionSelection sectionSelection, string prsName, string gasControlLineName = null);

        /// <summary>
        /// Determines if the inspection procedure with the name <paramref name="inspectionProcedureName"/> is defined.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <returns>True if the inspection procedure is defined, otherwise false.</returns>
        bool InspectionProcedureExists(string inspectionProcedureName);


        /// <summary>
        /// Finds the object name description and measure point description based on the objectName, measurePoint and fieldNumber.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="fieldNo">The field number.</param>
        /// <param name="objectNameDescription">The object name description.</param>
        /// <param name="measurePointDescription">The measure point description.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#")]
        void FindObjectNameDescriptionAndMeasurePointDescription(string objectName, string measurePoint,
            int? fieldNo, out string objectNameDescription, out string measurePointDescription);
    }
}
