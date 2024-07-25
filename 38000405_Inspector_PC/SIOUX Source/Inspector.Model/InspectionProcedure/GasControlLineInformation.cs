/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Inspector.Model.InspectionProcedure
{
    /// <summary>
    /// GasControlLineInformation
    /// </summary>
    public class GasControlLineInformation
    {
        #region Members
        private List<GclObject> m_GclObjects;
        private ReadOnlyCollection<GclObject> m_ReadOnlyGclObjects;
        #endregion Members

        /// <summary>
        /// Initializes a new instance of the <see cref="GasControlLineInformation"/> class.
        /// </summary>
        internal GasControlLineInformation(string gclName, string peMin, string peMax, string volumeVa, string volumeVak,
                                         string paRangeDM, string peRangeDM, string gclIdentification, string gclCode, int fsdStart,
                                         string inspectionProcedure, InspectionStatus inspectionStatus)
        {
            GasControlLineName = gclName;
            PeMin = peMin;
            PeMax = peMax;
            VolumeVA = volumeVa;
            VolumeVAK = volumeVak;
            PaRangeDM = paRangeDM;
            PeRangeDM = peRangeDM;
            GCLIdentification = gclIdentification;
            GCLCode = gclCode;
            FSDStart = fsdStart;
            InspectionProcedure = inspectionProcedure;
            InspectionStatus = inspectionStatus;
            m_GclObjects = new List<GclObject>();
        }

        /// <summary>
        /// Gets or sets the name of the gas control line.
        /// </summary>
        /// <value>The name of the gas control line.</value>
        public string GasControlLineName { get; private set; }

        /// <summary>
        /// Gets or sets the pe min.
        /// </summary>
        /// <value>The pe min.</value>
        public string PeMin { get; private set; }

        /// <summary>
        /// Gets or sets the pe max.
        /// </summary>
        /// <value>The pe max.</value>
        public string PeMax { get; private set; }

        /// <summary>
        /// Gets or sets the volume VA.
        /// </summary>
        /// <value>The volume VA.</value>
        public string VolumeVA { get; private set; }

        /// <summary>
        /// Gets or sets the volume VAK.
        /// </summary>
        /// <value>The volume VAK.</value>
        public string VolumeVAK { get; private set; }

        /// <summary>
        /// Gets or sets the pa range DM.
        /// </summary>
        /// <value>The pa range DM.</value>
        public string PaRangeDM { get; private set; }

        /// <summary>
        /// Gets or sets the pe range DM.
        /// </summary>
        /// <value>The pe range DM.</value>
        public string PeRangeDM { get; private set; }

        /// <summary>
        /// Gets or sets the GCL identification.
        /// </summary>
        /// <value>The GCL identification.</value>
        public string GCLIdentification { get; private set; }

        /// <summary>
        /// Gets or sets the GCL code.
        /// </summary>
        /// <value>The GCL code.</value>
        public string GCLCode { get; private set; }

        /// <summary>
        /// Gets or sets the FSD start.
        /// </summary>
        /// <value>The FSD start.</value>
        public int FSDStart { get; private set; }

        /// <summary>
        /// Gets the inspection procedure.
        /// </summary>
        public string InspectionProcedure { get; private set; }

        /// <summary>
        /// Gets the inspection status.
        /// </summary>
        public InspectionStatus InspectionStatus { get; internal set; }

        /// <summary>
        /// Gets the GCL objects.
        /// </summary>
        public ReadOnlyCollection<GclObject> GclObjects
        {
            get
            {
                if (m_ReadOnlyGclObjects == null)
                {
                    m_ReadOnlyGclObjects = new ReadOnlyCollection<GclObject>(m_GclObjects);
                }
                return m_ReadOnlyGclObjects;
            }
        }

        #region Internal
        /// <summary>
        /// Adds the GCL object.
        /// </summary>
        /// <param name="gclObject">The GCL object.</param>
        internal void AddGclObject(GclObject gclObject)
        {
            m_GclObjects.Add(gclObject);
        }
        #endregion Internal
    }
}
