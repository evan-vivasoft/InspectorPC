/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/


namespace Inspector.Model.MeasurementResult
{
    /// <summary>
    /// MeasurementReportMetaData
    /// </summary>
    public class InspectionProcedureMetadata
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the plexor.
        /// </summary>
        /// <value>
        /// The name of the plexor.
        /// </value>
        public string PlexorName { get; set; }

        /// <summary>
        /// Gets or sets the plexor bt address.
        /// </summary>
        /// <value>
        /// The plexor bt address.
        /// </value>
        public string PlexorBtAddress { get; set; }

        /// <summary>
        /// Gets or sets the TH1 serial number.
        /// </summary>
        /// <value>
        /// The TH1 serial number.
        /// </value>
        public string TH1SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the TH2 serial number.
        /// </summary>
        /// <value>
        /// The TH2 serial number.
        /// </value>
        public string TH2SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the station.
        /// </summary>
        /// <value>
        /// The station.
        /// </value>
        public string Station { get; set; }

        /// <summary>
        /// Gets or sets the station code.
        /// </summary>
        /// <value>
        /// The station code.
        /// </value>
        public string StationCode { get; set; }

        /// <summary>
        /// Gets or sets the gas control line.
        /// </summary>
        /// <value>
        /// The gas control line.
        /// </value>
        public string GasControlLine { get; set; }

        /// <summary>
        /// Gets or sets the gas control line identification code.
        /// </summary>
        /// <value>
        /// The gas control line identification code.
        /// </value>
        public string GasControlLineIdentificationCode { get; set; }

        /// <summary>
        /// Gets or sets the test program.
        /// </summary>
        /// <value>
        /// The test program.
        /// </value>
        public string TestProgram { get; set; }

        /// <summary>
        /// Gets or sets the inspection procedure varsion.
        /// </summary>
        /// <value>
        /// The inspection procedure varsion.
        /// </value>
        public string InspectionProcedureVersion { get; set; }

        /// <summary>
        /// Gets or sets the inspector version.
        /// </summary>
        /// <value>
        /// The inspector version.
        /// </value>
        public string InspectorVersion { get; set; }

        /// <summary>
        /// Gets or sets the FSD start.
        /// </summary>
        /// <value>
        /// The FSD start.
        /// </value>
        public string FSDStart { get; set; }
        #endregion Properties
    }
}
