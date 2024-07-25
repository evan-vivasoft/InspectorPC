/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionProcedureStatus;

namespace Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
{
    /// <summary>
    /// StationInformationManager interface
    /// Th stationInformationManager loads the stationInformation xml file.
    /// This interface can be used to find information about the Gas control line, and also provides methods to find information about the inspectionProcedure and inspectionStatus.
    /// </summary>
    public interface IStationInformationManager : IInformationManager
    {
        /// <summary>
        /// Looks up the name of the inspection procedure based on the prsName and optional gclName.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>the inspectionProcedureName </returns>
        /// <exception cref="InspectorLookupInspection">Thrown when the PRS or GCL cannot be found.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        string LookupInspectionProcedureName(string prsName, string gclName = null);

        /// <summary>
        /// Lookups the pe range DM.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The Pe range</returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        string LookupPeRangeDM(string prsName, string gclName);

        /// <summary>
        /// Lookups the pa range DM.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        string LookupPaRangeDM(string prsName, string gclName);

        /// <summary>
        /// Gets a list of all known stationInformations
        /// </summary>
        /// <value>The stations information.</value>
        ReadOnlyCollection<StationInformation> StationsInformation { get; }

        /// <summary>
        /// Looks up the station information for a given prsName.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <returns>The station information</returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        StationInformation LookupStationInformation(string prsName);

        /// <summary>
        /// Looks up information about a gas control line, based on a prsName and a gclName
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The gas control line information</returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        GasControlLineInformation LookupGasControlLineInformation(string prsName, string gclName);

        /// <summary>
        ///  Looks up information about a gas control line, based on the station information and gclName
        /// </summary>
        /// <param name="stationInfo">The station info.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The gas control line information</returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        GasControlLineInformation LookupGasControlLineInformation(StationInformation stationInfo, string gclName);

        /// <summary>
        /// Sets the inspection status. Creates a new inspectionstatus entry if the entry is not yet present in the InspectionStatus.xml file.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL. (optional)</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        void SetInspectionStatus(InspectionStatus status, string prsName, string gclName = null);

        /// <summary>
        /// Gets the inspection status. When the inspection status is not yet present then a new inspection status 
        /// entry is added with the initial status 'No inspection', given that the defined inspection procedure name is 
        /// available and defined  for the station <paramref name="prsName"/> and <paramref name="gclName"/>. 
        /// Otherwise the initial status is set to 'No Inspection Found'.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL. (optional)</param>
        /// <returns>The status of the corresponding prs/gcl.</returns>
        /// <exception cref="InspectorLookupException">Thrown when the prs or prs/gcl combination is not available from the station information.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        InspectionStatusInformation GetInspectionStatus(string prsName, string gclName = null);

        /// <summary>
        /// Gets the inspection statuses.
        /// </summary>
        /// <returns>
        /// The list of inspection procedure statuses
        /// </returns>
        ICollection<InspectionStatusInformation> GetInspectionStatuses { get; }

        /// <summary>
        /// Looks up the gas control line object.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="fieldNumber">The field number (optional).</param>
        /// <returns>
        /// The GclObject that matches the requested criteria.
        /// </returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the GclObject fails. The lookup fails when none or multiple matches were found.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        GclObject LookupGasControlLineObject(string prsName, string measurePoint, string objectName, int? fieldNumber = null);

        /// <summary>
        /// Looks up the gas control line object.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="fieldNumber">The field number.</param>
        /// <returns>
        /// The GclObject that matches the requested criteria.
        /// </returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the GclObject fails. The lookup fails when none or multiple matches were found.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        GclObject LookupGasControlLineObject(string prsName, string gclName, string measurePoint, string objectName, int? fieldNumber = null);
    }
}
