/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Model.InspectionReportingResults
{
    /// <summary>
    /// ReportMeasurementEquipment, equipment container which is part of a ReportInspectionResult
    /// </summary>
    public class ReportMeasurementEquipment
    {
        /// <summary>
        /// Gets or sets the identity D m1.
        /// </summary>
        /// <value>The identity D m1.</value>
        public string IdentityDM1 { get; set; }

        /// <summary>
        /// Gets or sets the identity D m2.
        /// </summary>
        /// <value>The identity D m2.</value>
        public string IdentityDM2 { get; set; }

        /// <summary>
        /// Gets or sets the bluetooth address.
        /// </summary>
        /// <value>The bluetooth address.</value>
        public string BluetoothAddress { get; set; }
    }
}
