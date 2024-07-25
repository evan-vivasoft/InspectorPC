/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using Inspector.Model.InspectionProcedure;

namespace Inspector.Model.InspectionProcedureStatus
{
    /// <summary>
    /// InspectionStatusInformation
    /// </summary>
    public class InspectionStatusInformation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStatusInformation"/> class.
        /// </summary>
        /// <param name="prsIdentification">The PRS identification.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="prsCode">The PRS code.</param>
        /// <param name="gclIdentification">The GCL identification.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="gclCode">The GCL code.</param>
        /// <param name="status">The status.</param>
        internal InspectionStatusInformation(string prsIdentification, string prsName, string prsCode, string gclIdentification, string gclName, string gclCode, InspectionStatus status)
        {
            PRSIdentification = prsIdentification;
            PRSName = prsName;
            PRSCode = prsCode;
            GCLIdentification = gclIdentification;
            GCLName = gclName;
            GCLCode = gclCode;
            Status = status;
            OverallGasControlLineStatus = null;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the PRS identification.
        /// </summary>
        /// <value>The PRS identification.</value>
        public string PRSIdentification { get; private set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>The name of the PRS.</value>
        public string PRSName { get; private set; }

        /// <summary>
        /// Gets or sets the PRS code.
        /// </summary>
        /// <value>The PRS code.</value>
        public string PRSCode { get; private set; }

        /// <summary>
        /// Gets or sets the GCL identification.
        /// </summary>
        /// <value>The GCL identification.</value>
        public string GCLIdentification { get; private set; }

        /// <summary>
        /// Gets or sets the name of the GCL.
        /// </summary>
        /// <value>The name of the GCL.</value>
        public string GCLName { get; private set; }

        /// <summary>
        /// Gets or sets the GCL code.
        /// </summary>
        /// <value>The GCL code.</value>
        public string GCLCode { get; private set; }

        /// <summary>
        /// Gets the overall gas control line status.
        /// </summary>
        /// <value>
        /// The overall gas control line status.
        /// </value>
        public InspectionStatus? OverallGasControlLineStatus { get; internal set; }

        /// <summary>
        /// Gets or sets the inspection status.
        /// </summary>
        /// <value>The inspection status.</value>
        public InspectionStatus Status { get; private set; }
        #endregion Properties
    }
}
