/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedureStatus;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.Infra.Utils;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionProcedureStatus;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers
{
    /// <summary>
    /// StationInformationManager, manages the station and corresponding gascontrollines information
    /// </summary>
    public class StationInformationManager : IStationInformationManager
    {
        #region Constants
        private const string LOOKUP_STATION_INFO_FAILED = "Failed to lookup the '{0}' station information.";
        private const string LOOKUP_GASCONTROLLINE_INFO_FAILED = "Failed to lookup the '{0}' gascontrolline information of station '{1}'.";
        private const string LOOKUP_GCLOBJECT_ON_PRS_FAILED = "Failed to lookup the PRS object for the pressure relay station '{0}.'";
        private const string LOOKUP_GCLOBJECT_ON_GCL_PRS_FAILED = "Failed to lookup the GCL object for the gas control line '{0}' of the pressure relay station '{1}.'";
        #endregion Constants

        #region Members
        private List<StationInformation> m_StationsInformation;
        private ReadOnlyCollection<StationInformation> m_ReadOnlyStationsInformation;
        private InspectionStatusInformationLoader m_InspectionStatusInformationLoader;
        private StationInformationLoader m_StationInfoLoader;
        private IInspectionInformationManager m_InspectionInformationManager;
        #endregion Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StationInformationManager"/> class.
        /// </summary>
        public StationInformationManager()
        {
            m_StationsInformation = new List<StationInformation>();
            m_InspectionStatusInformationLoader = new InspectionStatusInformationLoader();
            m_InspectionInformationManager = new InspectionInformationManager();
            m_StationInfoLoader = new StationInformationLoader();
            LoadStationsInformation();
        }
        #endregion Constructors

        #region IStationInformationManager Members
        /// <summary>
        /// Lookups the name of the inspection procedure.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>the inspectionProcedureName </returns>
        /// <exception cref="InspectorLookupInspection">Thrown when the PRS or GCL cannot be found.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public string LookupInspectionProcedureName(string prsName, string gclName = null)
        {
            string inspectionProcedureName = String.Empty;
            bool isLookupBasedOnPrs = (gclName == null);

            StationInformation stationInfo = LookupStationInformation(prsName);
            if (isLookupBasedOnPrs)
            {
                inspectionProcedureName = stationInfo.InspectionProcedure;
            }
            else
            {
                GasControlLineInformation gclInfo = LookupGasControlLineInformation(stationInfo, gclName);
                inspectionProcedureName = gclInfo.InspectionProcedure;
            }

            return inspectionProcedureName;
        }

        /// <summary>
        /// Lookups the pe range DM.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        public string LookupPeRangeDM(string prsName, string gclName)
        {
            GasControlLineInformation gcl = LookupGasControlLineInformation(prsName, gclName);
            return gcl.PeRangeDM ?? String.Empty;
        }

        /// <summary>
        /// Lookups the pa range DM.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns></returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        public string LookupPaRangeDM(string prsName, string gclName)
        {
            GasControlLineInformation gcl = LookupGasControlLineInformation(prsName, gclName);
            return gcl.PaRangeDM ?? String.Empty;
        }


        /// <summary>
        /// Gets the stations information.
        /// </summary>
        /// <value>The stations information.</value>
        public ReadOnlyCollection<StationInformation> StationsInformation
        {
            get
            {
                if (m_ReadOnlyStationsInformation == null)
                {
                    m_ReadOnlyStationsInformation = new ReadOnlyCollection<StationInformation>(m_StationsInformation);
                }
                return m_ReadOnlyStationsInformation;
            }
        }

        /// <summary>
        /// Lookups the station information.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <returns>The station information</returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        public StationInformation LookupStationInformation(string prsName)
        {
            StationInformation stationInfo = null;
            try
            {
                stationInfo = m_StationsInformation.Single(prs => prs.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase));
            }
            catch (InvalidOperationException exception)
            {
                string exceptionMessage = String.Format(CultureInfo.InvariantCulture, LOOKUP_STATION_INFO_FAILED, prsName);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return stationInfo;
        }

        /// <summary>
        /// Lookups the gas control line information.
        /// </summary>
        /// <param name="stationInfo">The station info.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The gas control line information</returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public GasControlLineInformation LookupGasControlLineInformation(StationInformation stationInfo, string gclName)
        {
            GasControlLineInformation gasControlLineInfo = null;
            try
            {
                gasControlLineInfo = stationInfo.GasControlLines.Single(gcl => gcl.GasControlLineName.Equals(gclName, StringComparison.OrdinalIgnoreCase));
            }
            catch (InvalidOperationException exception)
            {
                string exceptionMessage = String.Format(CultureInfo.InvariantCulture, LOOKUP_GASCONTROLLINE_INFO_FAILED, gclName, stationInfo.PRSName);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return gasControlLineInfo;
        }

        /// <summary>
        /// Lookups the gas control line information.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The gas control line information</returns>
        /// <exception cref="InspectorLookupException">Thrown when lookup of the StationInformation fails</exception>
        public GasControlLineInformation LookupGasControlLineInformation(string prsName, string gclName)
        {
            StationInformation stationInfo = LookupStationInformation(prsName);
            GasControlLineInformation gasControlLineInfo = LookupGasControlLineInformation(stationInfo, gclName);
            return gasControlLineInfo;
        }

        /// <summary>
        /// Refresh the data by reloading the storage backend.
        /// </summary>
        public void Refresh()
        {
            m_StationsInformation.Clear(); // remove any previous results first
            m_InspectionStatusInformationLoader.Reload();
            m_StationInfoLoader.Reload();
            LoadStationsInformation();
        }

        /// <summary>
        /// Sets the inspection status. Creates a new inspectionstatus entry if the entry is not yet present in the InspectionStatus.xml file.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL. (optional)</param>
        /// <exception cref="InspectorLookupException">Thrown when the prs or prs/gcl combination is not available from the station information</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void SetInspectionStatus(InspectionStatus status, string prsName, string gclName = null)
        {
            SetInspectionWithoutSave(status, prsName, gclName);
            m_InspectionStatusInformationLoader.Save();
        }

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
        public InspectionStatusInformation GetInspectionStatus(string prsName, string gclName = null)
        {
            InspectionStatusInformation inspectionStatusInfo = null;
            InspectionStatusEntity inspectionStatus = LookupInspectionStatus(prsName, gclName, m_InspectionStatusInformationLoader.InspectionStatuses);

            bool isInspectionStatusNew = (inspectionStatus == null);
            if (isInspectionStatusNew)
            {
                InspectionStatus initialInspectionStatus = DetermineInitialInspectionStatus(prsName, gclName);
                inspectionStatus = CreateInspectionStatus(initialInspectionStatus, prsName, gclName);
                m_InspectionStatusInformationLoader.InspectionStatuses.Add(inspectionStatus);
            }

            inspectionStatusInfo = new InspectionStatusInformation(
                    inspectionStatus.PRSIdentification, inspectionStatus.PRSName, inspectionStatus.PRSCode,
                    inspectionStatus.GCLIdentification, inspectionStatus.GCLName, inspectionStatus.GCLCode,
                    inspectionStatus.StatusID);

            if (String.IsNullOrEmpty(gclName))
            {
                StationInformation stationInformation = m_StationsInformation.Single(prs => prs.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase));
                inspectionStatusInfo.OverallGasControlLineStatus = stationInformation.OverallGasControlLineStatus;
            }

            return inspectionStatusInfo;
        }

        /// <summary>
        /// Gets the inspection statuses.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The list of already available inspection procedure statuses
        /// </returns>
        public ICollection<InspectionStatusInformation> GetInspectionStatuses
        {
            get
            {
                ICollection<InspectionStatusInformation> statuses = new List<InspectionStatusInformation>();
                foreach (InspectionStatusEntity inspectionStatusEntity in m_InspectionStatusInformationLoader.InspectionStatuses)
                {
                    InspectionStatusInformation inspectionStatusInfo = new InspectionStatusInformation(
                        inspectionStatusEntity.PRSIdentification, inspectionStatusEntity.PRSName, inspectionStatusEntity.PRSCode,
                        inspectionStatusEntity.GCLIdentification, inspectionStatusEntity.GCLName, inspectionStatusEntity.GCLCode,
                        inspectionStatusEntity.StatusID);

                    if (String.IsNullOrEmpty(inspectionStatusEntity.GCLName))
                    {
                        StationInformation stationInformation = m_StationsInformation.Single(prs => prs.PRSName.Equals(inspectionStatusEntity.PRSName, StringComparison.OrdinalIgnoreCase));
                        inspectionStatusInfo.OverallGasControlLineStatus = stationInformation.OverallGasControlLineStatus;
                    }

                    statuses.Add(inspectionStatusInfo);
                }
                return statuses;
            }
        }

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
        public GclObject LookupGasControlLineObject(string prsName, string measurePoint, string objectName, int? fieldNumber = null)
        {
            GclObject gclObject = null;
            Func<GclObject, bool> lookupCriteria = ObjectLookupPredicate<GclObject>(measurePoint, objectName, fieldNumber);

            StationInformation stationInformation = LookupStationInformation(prsName);
            try
            {
                gclObject = stationInformation.GasControlLines.SelectMany(gcl => gcl.GclObjects).Single(lookupCriteria);
            }
            catch (InvalidOperationException exception)
            {
                string exceptionMessage = String.Format(CultureInfo.InvariantCulture, LOOKUP_GCLOBJECT_ON_PRS_FAILED, prsName);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return gclObject;
        }

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
        public GclObject LookupGasControlLineObject(string prsName, string gclName, string measurePoint, string objectName, int? fieldNumber = null)
        {
            GclObject gclObject = null;
            Func<GclObject, bool> lookupCriteria = ObjectLookupPredicate<GclObject>(measurePoint, objectName, fieldNumber);

            GasControlLineInformation gclInformation = LookupGasControlLineInformation(prsName, gclName);
            try
            {
                gclObject = gclInformation.GclObjects.Single(lookupCriteria);
            }
            catch (InvalidOperationException exception)
            {
                string exceptionMessage = String.Format(CultureInfo.InvariantCulture, LOOKUP_GCLOBJECT_ON_GCL_PRS_FAILED, gclName, prsName);
                throw new InspectorLookupException(exceptionMessage, exception);
            }

            return gclObject;
        }
        #endregion IStationInformationManager Members

        #region Private
        /// <summary>
        /// Sets the inspection status without saving it to disk. Creates a new inspectionstatus entry if the entry is not yet present in the InspectionStatus.xml file.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL. (optional)</param>
        /// <exception cref="InspectorLookupException">Thrown when the prs or prs/gcl combination is not available from the station information</exception>
        private void SetInspectionWithoutSave(InspectionStatus status, string prsName, string gclName = null)
        {
            IList<InspectionStatusEntity> inspectionStatuses = m_InspectionStatusInformationLoader.InspectionStatuses;
            InspectionStatusEntity inspectionStatus = LookupInspectionStatus(prsName, gclName, inspectionStatuses);

            bool isUpdateOfExistingInspectionStatusEntry = (inspectionStatus != null);
            if (isUpdateOfExistingInspectionStatusEntry)
            {
                inspectionStatus.StatusID = status;
            }
            else // create an new entry
            {
                inspectionStatus = CreateInspectionStatus(status, prsName, gclName);
                m_InspectionStatusInformationLoader.InspectionStatuses.Add(inspectionStatus);
            }

            StationInformation stationInformation = m_StationsInformation.FirstOrDefault(prs => prs.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase));
            if (stationInformation != null)
            {
                if (String.IsNullOrEmpty(gclName))
                {
                    stationInformation.InspectionStatus = status;
                }
                else
                {
                    GasControlLineInformation gclInfo = stationInformation.GasControlLines.FirstOrDefault(gcl => !String.IsNullOrEmpty(gcl.GasControlLineName) &&
                                                                                                                 gcl.GasControlLineName == gclName);
                    if (gclInfo != null)
                    {
                        gclInfo.InspectionStatus = status;
                    }
                }
            }
        }

        /// <summary>
        /// Determines the initial inspecton status.
        /// </summary>
        /// <param name="inspectionProcedureName">Name of the inspection procedure.</param>
        /// <returns>
        /// The status 'No Inspection' is returned if the inspection procedure name for the prs/gcl is defined and available, otherwise the status 'No Inspection Found' is returned.
        /// </returns>
        private InspectionStatus DetermineInitialInspectionStatus(string inspectionProcedureName)
        {
            InspectionStatus initialInspectionStatus;
            try
            {
                bool isInspectionProcedureDefined = m_InspectionInformationManager.InspectionProcedureExists(inspectionProcedureName);
                initialInspectionStatus = isInspectionProcedureDefined ? InspectionStatus.NoInspection : InspectionStatus.NoInspectionFound;
            }
            catch (InspectorLookupException)
            {
                initialInspectionStatus = InspectionStatus.NoInspectionFound;
            }
            return initialInspectionStatus;
        }

        /// <summary>
        /// Determines the initial inspection status.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>The status 'No Inspection' is returned if the inspection procedure name for the prs/gcl is defined and available, otherwise the status 'No Inspection Found' is returned.</returns>
        private InspectionStatus DetermineInitialInspectionStatus(string prsName, string gclName = null)
        {
            InspectionStatus initialInspectionStatus;
            try
            {
                string inspectionProcedureName = LookupInspectionProcedureName(prsName, gclName);
                bool isInspectionProcedureDefined = m_InspectionInformationManager.InspectionProcedureExists(inspectionProcedureName);
                initialInspectionStatus = isInspectionProcedureDefined ? InspectionStatus.NoInspection : InspectionStatus.NoInspectionFound;
            }
            catch (InspectorLookupException)
            {
                initialInspectionStatus = InspectionStatus.NoInspectionFound;
            }
            return initialInspectionStatus;
        }

        /// <summary>
        /// Creates the inspection status by looking up the required prs and gcl information from stationinformation.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="inspectionStatus">The inspection status.</param>
        /// <returns>A filled in inspection status entity</returns>
        /// <exception cref="InspectorLookupException">Thrown when the prs or prs/gcl combination is not available from the station information</exception>
        private InspectionStatusEntity CreateInspectionStatus(InspectionStatus status, string prsName, string gclName)
        {
            PRSEntity prs = m_StationInfoLoader.PRSEntities.SingleOrDefault(item => item.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase));
            if (prs == null)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Could not create a new inspection with status '{0}' because the PRS '{1}' is not available in the station information.", status.GetDescription(), prsName);
                throw new InspectorLookupException(message);
            }

            GasControlLineEntity gcl = null;
            if (!String.IsNullOrEmpty(gclName))
            {
                gcl = prs.GasControlLines.SingleOrDefault(item => item.GasControlLineName.Equals(gclName, StringComparison.OrdinalIgnoreCase));
                if (gcl == null)
                {
                    string message = String.Format(CultureInfo.InvariantCulture, "Could not create a new inspection with status '{0}' because the GCL '{1}' for PRS '{2}' is not available in the station information.",
                                                   status.GetDescription(), gclName, prsName);
                    throw new InspectorLookupException(message);
                }
            }

            InspectionStatusEntity inspectionStatus = new InspectionStatusEntity
            {
                PRSName = prsName,
                PRSIdentification = prs.PRSIdentification,
                PRSCode = prs.PRSCode,
                StatusID = status,
            };

            if (gcl != null)
            {
                inspectionStatus.GCLName = gclName;
                inspectionStatus.GCLIdentification = gcl.GCLIdentification;
                inspectionStatus.GCLCode = gcl.GCLCode;
            }
            return inspectionStatus;
        }

        /// <summary>
        /// Lookups the inspection status.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="inspectionStatuses">The inspection statuses.</param>
        /// <returns></returns>
        private static InspectionStatusEntity LookupInspectionStatus(string prsName, string gclName, IList<InspectionStatusEntity> inspectionStatuses)
        {
            InspectionStatusEntity inspectionStatus = null;

            if (String.IsNullOrEmpty(gclName))
            {
                inspectionStatus = inspectionStatuses.SingleOrDefault(item => item.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase) &&
                                                                              String.IsNullOrEmpty(item.GCLName));
            }
            else
            {
                inspectionStatus = inspectionStatuses.SingleOrDefault(item => item.PRSName.Equals(prsName, StringComparison.OrdinalIgnoreCase) &&
                                                                              !String.IsNullOrEmpty(item.GCLName) &&
                                                                              item.GCLName.Equals(gclName, StringComparison.OrdinalIgnoreCase));
            }
            return inspectionStatus;
        }

        /// <summary>
        /// Loads the stations information.
        /// </summary>
        private void LoadStationsInformation()
        {
            InspectionStatus inspectionStatus;
            StationInformationLoader stationsInfoLoader = new StationInformationLoader();

            foreach (PRSEntity prs in stationsInfoLoader.PRSEntities)
            {
                string prsName = prs.PRSName;
                InspectionStatusEntity prsInspectionStatusEntity = LookupInspectionStatus(prsName, null, m_InspectionStatusInformationLoader.InspectionStatuses);
                if (prsInspectionStatusEntity == null)
                {
                    inspectionStatus = DetermineInitialInspectionStatus(prs.InspectionProcedure);
                    SetInspectionWithoutSave(inspectionStatus, prsName);
                }
                else
                {
                    inspectionStatus = prsInspectionStatusEntity.StatusID;
                }

                StationInformation stationInfo = new StationInformation(prs.Route, prs.PRSCode, prs.PRSName, prs.PRSIdentification, prs.Information, prs.InspectionProcedure, inspectionStatus);

                foreach (PRSObject prsObject in prs.PRSObjects)
                {
                    PrsObject prsObjectToAdd = new PrsObject(prsObject.ObjectName, prsObject.ObjectID, prsObject.MeasurePoint, prsObject.MeasurePointID, prsObject.FieldNo);
                    stationInfo.AddPrsObject(prsObjectToAdd);
                }

                foreach (GasControlLineEntity gcl in prs.GasControlLines)
                {
                    string gclName = gcl.GasControlLineName;
                    InspectionStatusEntity gclInspectionStatusEntity = LookupInspectionStatus(prsName, gclName, m_InspectionStatusInformationLoader.InspectionStatuses);
                    if (gclInspectionStatusEntity == null)
                    {
                        inspectionStatus = DetermineInitialInspectionStatus(gcl.InspectionProcedure);
                        SetInspectionWithoutSave(inspectionStatus, prsName, gclName);
                    }
                    else
                    {
                        inspectionStatus = gclInspectionStatusEntity.StatusID;
                    }

                    GasControlLineInformation gclInformation = new GasControlLineInformation(
                        gcl.GasControlLineName,
                        gcl.PeMin, gcl.PeMax,
                        gcl.VolumeVA, gcl.VolumeVAK,
                        gcl.PaRangeDM.GetDescription(), gcl.PeRangeDM.GetDescription(),
                        gcl.GCLIdentification, gcl.GCLCode, gcl.FSDStart, gcl.InspectionProcedure, inspectionStatus);

                    foreach (GCLObject gclObject in gcl.GCLObjects)
                    {
                        GclBoundaries gclBoundary = null;
                        if (gclObject.Boundaries != null)
                        {
                            gclBoundary = new GclBoundaries(gclObject.Boundaries.ValueMax, gclObject.Boundaries.ValueMin, gclObject.Boundaries.UOV.GetDescription());
                        }
                        GclObject gclObjectToAdd = new GclObject(gclObject.ObjectName, gclObject.ObjectID, gclObject.MeasurePoint, gclObject.MeasurePointID, gclObject.FieldNo, gclBoundary);
                        gclInformation.AddGclObject(gclObjectToAdd);
                    }

                    stationInfo.AddGasControlLine(gclInformation);
                }

                m_StationsInformation.Add(stationInfo);
            }
            m_InspectionStatusInformationLoader.Save();
            stationsInfoLoader = null;
        }

        /// <summary>
        /// Create the lookup criteria function.
        /// </summary>
        /// <typeparam name="T">The type of object, either PrsObject or GclObject.</typeparam>
        /// <param name="measurePoint">The measure point.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="fieldNumber">The field number.</param>
        /// <returns>The prs or gcl object lookup predicate.</returns>
        private static Func<T, bool> ObjectLookupPredicate<T>(string measurePoint, string objectName, int? fieldNumber) where T : PrsGclObjectBase
        {
            Func<T, bool> lookupCriteria = null;
            if (fieldNumber == null)
            {
                lookupCriteria = prsgclObject => prsgclObject.MeasurePoint.Equals(measurePoint, StringComparison.Ordinal) &&
                                              prsgclObject.ObjectName.Equals(objectName, StringComparison.Ordinal);
            }
            else
            {
                lookupCriteria = prsgclObject => prsgclObject.MeasurePoint.Equals(measurePoint, StringComparison.Ordinal) &&
                                              prsgclObject.ObjectName.Equals(objectName, StringComparison.Ordinal) &&
                                              prsgclObject.FieldNo == fieldNumber;
            }
            return lookupCriteria;
        }
        #endregion Private
    }
}
