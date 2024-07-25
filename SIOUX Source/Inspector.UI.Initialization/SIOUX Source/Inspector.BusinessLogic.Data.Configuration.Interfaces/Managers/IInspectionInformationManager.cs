/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
{
    /// <summary>
    /// InspectionInformationManager interface
    /// </summary>
    public interface IInspectionInformationManager : IInformationManager
    {
        /// <summary>
        /// Gets the inspection procedure names.
        /// </summary>
        ICollection<string> InspectionProcedureNames { get; }

        /// <summary>
        /// Lookups the inspection procedure steps.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <returns>The InspectionProcedure sections</returns>
        /// <exception cref="InspectorLookupException">Thrown when the InspectionProcedure sections lookup fails</exception>
        SectionSelection LookupInspectionProcedureSections(string inspectionProcedureName);

        /// <summary>
        /// Looks up the inspection procedure.
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
        /// Lookups the inspection procedure selection.
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
    }
}
