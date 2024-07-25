/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/
using System;

namespace Inspector.Model.Plexor
{
    /// <summary>
    /// PlexorInformation
    /// </summary>
    public class PlexorInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlexorInformation"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="blueToothAddress">The blue tooth address.</param>
        /// <param name="pn">The pn.</param>
        /// <param name="calibrationDate">The calibration date.</param>
        internal PlexorInformation(string name, string serialNumber, string blueToothAddress, string pn, DateTime calibrationDate)
        {
            Name = name;
            SerialNumber = serialNumber;
            BlueToothAddress = blueToothAddress;
            PN = pn;
            CalibrationDate = calibrationDate;
        }

        /// <summary>
        /// The Plexor's Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The Plexor's Serial Number
        /// </summary>
        public string SerialNumber { get; private set; }

        /// <summary>
        /// The Plexor's BlueTooth Address
        /// </summary>
        public string BlueToothAddress { get; private set; }

        /// <summary>
        /// The Plexor's PN
        /// </summary>
        public string PN { get; private set; }

        /// <summary>
        /// The Plexor's Calibration Date
        /// </summary>
        public DateTime CalibrationDate { get; private set; }
    }
}
