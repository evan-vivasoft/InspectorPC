/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// StationInformation
    /// </summary>
    public class StationInformation
    {
        #region Members
        private List<GasControlLineInformation> m_GasControLines;
        private ReadOnlyCollection<GasControlLineInformation> m_ReadOnlyGasControLines;

        private List<PrsObject> m_PrsObjects;
        private ReadOnlyCollection<PrsObject> m_ReadOnlyPrsObjects;
        #endregion Members

        #region Contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StationInformation"/> class.
        /// </summary>
        internal StationInformation(string route, string prsCode, string prsName, string prsIdentification, string information, string inspectionProcedure, InspectionStatus inspectionStatus)
        {
            m_GasControLines = new List<GasControlLineInformation>();
            m_PrsObjects = new List<PrsObject>();
            Route = route;
            PRSCode = prsCode;
            PRSName = prsName;
            PRSIdentification = prsIdentification;
            Information = information;
            InspectionProcedure = inspectionProcedure;
            InspectionStatus = inspectionStatus;
        }
        #endregion Contructors

        #region Properties
        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        /// <value>The route.</value>
        public string Route { get; private set; }

        /// <summary>
        /// Gets or sets the PRS code.
        /// </summary>
        /// <value>The PRS code.</value>
        public string PRSCode { get; private set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>The name of the PRS.</value>
        public string PRSName { get; private set; }

        /// <summary>
        /// Gets or sets the PRS identification.
        /// </summary>
        /// <value>The PRS identification.</value>
        public string PRSIdentification { get; private set; }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        public string Information { get; private set; }

        /// <summary>
        /// Gets the inspection procedure.
        /// </summary>
        public string InspectionProcedure { get; private set; }

        /// <summary>
        /// Gets the inspection status.
        /// </summary>
        public InspectionStatus InspectionStatus { get; internal set; }

        /// <summary>
        /// Gets the overall gas control line status.
        /// </summary>
        /// <value>
        /// The overall gas control line status.
        /// </value>
        public InspectionStatus OverallGasControlLineStatus
        {
            get
            {
                InspectionStatus overallGasControlStatus = InspectionStatus.Completed;
                if (GasControlLines.Count > 0)
                {
                    overallGasControlStatus = GasControlLines[0].InspectionStatus;

                    foreach (GasControlLineInformation glcInformation in GasControlLines)
                    {
                        if (glcInformation.InspectionStatus != overallGasControlStatus)
                        {
                            bool hasIncorrectStatus = GasControlLines.Any(gcl => gcl.InspectionStatus != InspectionStatus.Completed &&
                                                                                 gcl.InspectionStatus != InspectionStatus.NoInspectionFound);

                            if (hasIncorrectStatus)
                            {
                                overallGasControlStatus = InspectionStatus.Warning;
                            }
                            else
                            {
                                overallGasControlStatus = InspectionStatus.Completed;
                            }
                            break;
                        }
                    }
                }
                return overallGasControlStatus;
            }
        }

        /// <summary>
        /// Gets the gas control lines.
        /// </summary>
        /// <value>The gas control lines.</value>
        public ReadOnlyCollection<GasControlLineInformation> GasControlLines
        {
            get
            {
                if (m_ReadOnlyGasControLines == null)
                {
                    m_ReadOnlyGasControLines = new ReadOnlyCollection<GasControlLineInformation>(m_GasControLines);
                }
                return m_ReadOnlyGasControLines;
            }
        }

        /// <summary>
        /// Gets the PRS objects.
        /// </summary>
        public ReadOnlyCollection<PrsObject> PrsObjects
        {
            get
            {
                if (m_ReadOnlyPrsObjects == null)
                {
                    m_ReadOnlyPrsObjects = new ReadOnlyCollection<PrsObject>(m_PrsObjects);
                }
                return m_ReadOnlyPrsObjects;
            }
        }
        #endregion Properties

        #region Internal
        /// <summary>
        /// Adds the gas control line.
        /// </summary>
        /// <param name="gasControlLineInformation">The gas control line information.</param>
        internal void AddGasControlLine(GasControlLineInformation gasControlLineInformation)
        {
            m_GasControLines.Add(gasControlLineInformation);
        }

        /// <summary>
        /// Adds the PRS object.
        /// </summary>
        /// <param name="prsObject">The PRS object.</param>
        internal void AddPrsObject(PrsObject prsObject)
        {
            m_PrsObjects.Add(prsObject);
        }
        #endregion Internal
    }
}
