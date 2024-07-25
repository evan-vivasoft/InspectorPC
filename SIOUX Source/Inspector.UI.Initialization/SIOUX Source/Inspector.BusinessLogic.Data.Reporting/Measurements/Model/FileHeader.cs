/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Globalization;
using System.Text;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements.Model
{
    /// <summary>
    /// FileHeader
    /// </summary>
    internal class FileHeader
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

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        /// <value>
        /// The start date time.
        /// </value>
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public string StartTime { get; set; }
        #endregion Properties

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <exception cref="MeasurementTranslationException">Throw when the measurement translation resources failed to load.</exception>
        public override string ToString()
        {
            MeasurementTranslations translation = MeasurementTranslations.Instance;
            StringBuilder result = new StringBuilder();

            string seperator = translation.SeperatorSign;
            string baseFormat = "{0} " + seperator + " {1}";

            result.AppendLine("[FHEADER]");
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.PlexorName, PlexorName));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.PlexorBluetoothAddress, PlexorBtAddress));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.TH1SerialNumber, TH1SerialNumber));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.TH2SerialNumber, TH2SerialNumber));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.PRSName, Station));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.PRSCode, StationCode));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.GCLName, GasControlLine));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.GCLIdentification, GasControlLineIdentificationCode));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.InspectionProcedureName, TestProgram));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.InspectionProcedureVersion, InspectionProcedureVersion));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.InspectorVersion, InspectorVersion));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.FSDStart, FSDStart));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.StartDate, StartDate));
            result.AppendLine(String.Format(CultureInfo.InvariantCulture, baseFormat, translation.StartTime, StartTime));

            return result.ToString();
        }
    }
}
