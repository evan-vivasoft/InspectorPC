/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

namespace Inspector.Infra
{
    /// <summary>
    /// 
    /// </summary>
    public enum DeviceCommand
    {
        /// <summary>
        /// No action, used as "empty" command
        /// </summary>
        None,

        /// <summary>
        /// Connect to the device
        /// </summary>
        Connect,
        /// <summary>
        /// Disconnect from the device
        /// </summary>
        Disconnect,

        /// <summary>
        /// Enter remote local command mode on the device
        /// </summary>
        EnterRemoteLocalCommandMode,
        /// <summary>
        /// Exit remote local command mode on device
        /// </summary>
        ExitRemoteLocalCommandMode,

        /// <summary>
        /// Flushes the manometer cache
        /// </summary>
        FlushManometerCache,

        /// <summary>
        /// Switch the active manometer to TH1
        /// </summary>
        SwitchToManometerTH1,
        /// <summary>
        /// Switch the active manometer to TH2
        /// </summary>
        SwitchToManometerTH2,

        /// <summary>
        /// Check the battery status from the device
        /// </summary>
        CheckBatteryStatus,
        /// <summary>
        /// Check the SCPI interface
        /// </summary>
        CheckSCPIInterface,
        /// <summary>
        /// Initiate a Self Test
        /// </summary>
        InitiateSelfTest,
        /// <summary>
        /// Get the identification from the device
        /// </summary>
        CheckIdentification,
        /// <summary>
        /// Get the range from the device
        /// </summary>
        CheckRange,
        /// <summary>
        /// Set the pressure unit
        /// </summary>
        SetPressureUnit,
        /// <summary>
        /// Get the pressure unit from the device
        /// </summary>
        CheckPressureUnit,

        /// <summary>
        /// Send a measurement command, with a frequency of 0, 10 or 25 measurements per second
        /// </summary>
        MeasureContinuously,

        /// <summary>
        /// Clears the cache of the bluetooth adapter in the device
        /// </summary>
        FlushBluetoothCache,

        /// <summary>
        /// Checks if the manometer is present
        /// </summary>
        CheckManometerPresent,

        /// <summary>
        /// Makes sure the infra red of the Mano Meters stays on
        /// </summary>
        IRAlwaysOn,

        /// <summary>
        /// Checks the system software version
        /// </summary>
        CheckSystemSoftwareVersion,

        /// <summary>
        /// Checks the calibartion data
        /// </summary>
        CheckCalibrationDate,

        /// <summary>
        /// Enable the IO status information
        /// </summary>
        EnableIOStatus,

        /// <summary>
        /// Disable the IO status information
        /// </summary>
        DisableIOStatus,

        CheckIo3Status,

        ActivatePortSensor,

        ReadSensorId,

        ActivatePortIrDa,

        StopSensorRun,

        MeasureSingleValue
    }
}
