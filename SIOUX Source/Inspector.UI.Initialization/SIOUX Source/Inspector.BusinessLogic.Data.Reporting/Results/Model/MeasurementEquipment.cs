/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Globalization;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Model
{
    /// <summary>
    /// This Class represents part XML model used to create the InspectionResultsData Report.
    /// Do Not set properties via the setters!
    /// Use the constructor or a specific Set function to ensure proper setting of a value.
    /// </summary>
    public class MeasurementEquipment
    {
        /// <summary>
        /// Gets or sets the I d_ D m1.
        /// </summary>
        /// <value>
        /// The I d_ D m1.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string ID_DM1 { get; set; }

        /// <summary>
        /// Gets or sets the I d_ D m2.
        /// </summary>
        /// <value>
        /// The I d_ D m2.
        /// </value>        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string ID_DM2 { get; set; }

        /// <summary>
        /// Gets or sets the B t_ address.
        /// </summary>
        /// <value>
        /// The B t_ address.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        public string BT_Address { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementEquipment"/> class.
        /// </summary>
        public MeasurementEquipment()
        {
            ID_DM1 = string.Empty;
            ID_DM2 = string.Empty;
            BT_Address = string.Empty;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementEquipment"/> class.
        /// </summary>
        /// <param name="idDM1">The id_dm1.</param>
        /// <param name="idDM2">The id_dm2.</param>
        /// <param name="btAddress">The bt_address.</param>
        public MeasurementEquipment(string idDM1, string idDM2, string btAddress)
            : this()
        {
            ID_DM1 = idDM1;
            ID_DM2 = idDM2;
            BT_Address = btAddress;
        }

        /// <summary>
        /// Sets the meter value.
        /// </summary>
        /// <param name="meter">The meter.</param>
        /// <param name="value">The value.</param>
        public void SetMeterValue(MeterNumber meter, string value)
        {
            switch (meter)
            {
                case MeterNumber.ID_DM1:
                    ID_DM1 = value;
                    break;

                case MeterNumber.ID_DM2:
                    ID_DM2 = value;
                    break;

                default:
                    throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "MeterNumber: {0} does not exist", meter));
            }
        }

        /// <summary>
        /// Sets the blue tooth address.
        /// </summary>
        /// <param name="btAddress">The bt address.</param>
        public void SetBlueToothAddress(string btAddress)
        {
            BT_Address = btAddress;
        }
    }
}
