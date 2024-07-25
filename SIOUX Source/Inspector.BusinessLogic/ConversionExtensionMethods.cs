/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using Inspector.Infra;
using Inspector.Model;

namespace Inspector.BusinessLogic
{
    /// <summary>
    /// Extension methods for the conversions of types within the Businesslogic
    /// </summary>
    public static class ConversionExtensionMethods
    {
        /// <summary>
        /// Convert the digital nanometer to the corresponding devicecommand for manometer switching.
        /// </summary>
        /// <param name="digitalManometer">The digital manometer.</param>
        /// <returns></returns>
        public static DeviceCommand ToDeviceCommand(this DigitalManometer digitalManometer)
        {
            DeviceCommand deviceCommand = DeviceCommand.None;
            switch (digitalManometer)
            {
                case DigitalManometer.TH1:
                    deviceCommand = DeviceCommand.SwitchToManometerTH1;
                    break;
                case DigitalManometer.TH2:
                    deviceCommand = DeviceCommand.SwitchToManometerTH2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("digitalManometer");
            }

            return deviceCommand;
        }
    }
}
