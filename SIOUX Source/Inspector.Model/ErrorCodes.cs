/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Net;

namespace Inspector.Model
{
    /// <summary>
    /// ErrorCodes
    /// </summary>
    public static class ErrorCodes
    {
        /* 
         * Error codes are divided in ranges. 
         * 0 - 700: BT Framework
         * 1000-1099: HAL
         * 1100-1199: Communication Control
         * 1200-1299: Initialization 
         * 1300-1399: Inspection
         * 1400-1599: General errors
         * 1600-1699: Manometer errors
         */

        #region BT Framework
        // No error codes are defined here

        // Some of the known BT Framework error codes:
        // 14: Specified Bluetooth API is not available
        // 44: Specified service was not found (Could not connect)
        #endregion BT Framework

        #region HAl
        /// <summary>
        /// An unexpected error occurred in the HAL
        /// </summary>
        public static int HAL_UNEXPECTED_ERROR = 1000;
        /// <summary>
        /// The connection properties were not found
        /// </summary>
        public static int HAL_CONNECTION_PROPERTIES_EMPTY = 1001;
        /// <summary>
        /// A connection property was not found
        /// </summary>
        public static int HAL_CONNECTION_PROPERTY_NOT_FOUND = 1002;
        /// <summary>
        /// There has been tried to make a connection, but the connection was already active
        /// </summary>
        public static int HAL_CONNECTION_ALREADY_ACTIVE = 1003;
        /// <summary>
        /// The specified bluetooth api is not supported by the BT Framework
        /// </summary>
        public static int HAL_BLUETOOTH_API_NOT_SUPPORTED = 1004;
        /// <summary>
        /// The bluetooth dongle(s) that are connected to the PC are not found in the allowed dongle list.
        /// </summary>
        public static int HAL_BLUETOOTH_DONGLE_NOT_ALLOWED = 1005;
        /// <summary>
        /// No Bluetooth dongles found on the pc.
        /// </summary>
        public static int HAL_BLUETOOTH_DONGLE_NOT_FOUND = 1006;
        /// <summary>
        /// No Bluetooth api found in the settings file
        /// </summary>
        public static int HAL_BLUETOOTH_API_NOT_FOUND = 1007;
        /// <summary>
        /// This error occurs if the last bit is not a stop bit. This generally  
        /// happens because of synchronization error. You may face this 
        /// error when connecting two computers with the help of null 
        /// modem, if the baud rates of the transmitting computer 
        /// differs from the baud rates of the receiving computer.
        /// </summary>
        public static int HAL_SERIAL_ERROR_FRAME = 1010;
        /// <summary>
        /// This error usually occurs when the data are read from the port slower 
        /// than they are received. If you don't read the incoming bytes fast enough 
        /// the last byte can be overwritten with the byte which was received last, 
        /// in this case the last byte may be lost which will cause overrun error.
        /// </summary>
        public static int HAL_SERIAL_ERROR_OVERRUN = 1011;
        /// <summary>
        /// An input buffer overflow has occurred. There is either no room in the 
        /// input buffer, or a character was received after the end-of-file 
        /// (EOF) character. 
        /// </summary>
        public static int HAL_SERIAL_ERROR_RXOVER = 1012;
        /// <summary>
        /// This error occurs if the parity doesn't coincide with the 
        /// parameters set when the byte is received.
        /// </summary>
        public static int HAL_SERIAL_ERROR_RXPARITY = 1013;
        /// <summary>
        /// The application tried to transmit a character, but the output buffer 
        /// was full. 
        /// </summary>
        public static int HAL_SERIAL_ERROR_TXFULL = 1014;
        /// <summary>
        /// An undefined error occurred on the serial port
        /// </summary>
        public static int HAL_SERIAL_ERROR_UNDEFINED = 1015;
        /// <summary>
        /// Could not connect to the serial port
        /// </summary>
        public static int HAL_SERIAL_ERROR_CREATING_CONNECTION = 1020;
        /// <summary>
        /// Could not close the serial port
        /// </summary>
        public static int HAL_SERIAL_ERROR_CLOSING_CONNECTION = 1021;
        /// <summary>
        /// Could not send the command
        /// </summary>
        public static int HAL_SERIAL_ERROR_SEND_COMMAND = 1022;
        /// <summary>
        /// The message that was received over the serial port had an incorrect checksum
        /// </summary>
        public static int HAL_MESSAGE_RECEIVED_INCORRECT_CHECKSUM = 1030;
        /// <summary>
        /// The message received over the serial port had an incorrect format
        /// </summary>
        public static int HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT = 1031;
        /// <summary>
        /// There has been no reaction in a timely manner
        /// </summary>
        public static int HAL_COMMAND_TIMEOUT_RECEIVED = 1040;
        /// <summary>
        /// There has been a timeout during measurement
        /// </summary>
        public static int HAL_MEASUREMENT_TIMEOUT_RECEIVED = 1041;
        /// <summary>
        /// Could not set the DTR on the serial port.
        /// </summary>
        public static int HAL_SERIAL_ERROR_DTR = 1042;
        /// <summary>
        /// Could not initialize the serial port.
        /// </summary>
        public static int HAL_SERIAL_ERROR_PORT_CREATION = 1043;
        /// <summary>
        /// Could not start the continuous measurement.
        /// </summary>
        public static int HAL_CONTINUOUS_MEASUREMENT_START_FAILED = 1044;
        /// <summary>
        /// Could not stop the continuous measurement.
        /// </summary>
        public static int HAL_CONTINUOUS_MEASUREMENT_STOP_FAILED = 1045;
        /// <summary>
        /// Could not compute the checksum.
        /// </summary>
        public static int HAL_FAILED_CHECKSUM_COMPUTATION = 1046;
        #endregion HAL

        #region Communication Control
        /// <summary>
        /// It was tried to send a command while the communication control was not yet claimed
        /// </summary>
        public static int COMMUNICATIONCONTROL_NOT_YET_CLAIMED = 1101;
        /// <summary>
        /// A second attempt was made to claim the communication control without releasing the previous claim first.
        /// </summary>
        public static int COMMUNICATIONCONTROL_ALREADY_CLAIMED = 1102;
        /// <summary>
        /// The call to the statemachine was invalid for the current state of the connection statemachine.
        /// </summary>
        public static int COMMUNICATIONCONTROL_STATEMACHINE_ERROR = 1103;
        #endregion Communication Control

        #region Initialization
        /// <summary>
        /// The Initialization process finished succesfully
        /// </summary>
        public static int INITIALIZATION_FINISHED_SUCCESSFULLY = 1200;
        /// <summary>
        /// The SCPI interface reported at least one, but at most 5 errors.
        /// The errors encountered are in the first messageParameter
        /// </summary>
        public static int INITIALIZATION_STEP_SCPI_WARNING = 1201;
        /// <summary>
        /// The SCPI interface reported too many errors (6)
        /// The errors encountered are in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_SCPI_ERROR = 1202;
        /// <summary>
        /// The Set Pressure Unit call reported an error
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_SET_PRESSURE_UNIT_ERROR = 1203;
        /// <summary>
        /// The Check Manometer Range call reported an error
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_MANOMETER_RANGE_ERROR = 1204;
        /// <summary>
        /// The Initiate Self Test call reported an error
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_SELF_TEST_ERROR = 1205;
        /// <summary>
        /// Reading the battery limit failed
        /// </summary>
        public static int INITIALIZATION_STEP_BATTERYLIMIT_CONFIG_ERROR = 1206;
        /// <summary>
        /// The Battery Level was in an inccorrect format
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_BATTERYLIMIT_FORMAT_ERROR = 1207;
        /// <summary>
        /// The Battery Level is below the Battery level limit
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_BATTERYLIMIT_LEVEL_WARNING = 1208;
        /// <summary>
        /// Switching the manometer has failed.
        /// </summary>
        public static int INITIALIZATION_STEP_SWITCH_MANOMETER_ERROR = 1209;
        /// <summary>
        /// Entering Remote Local command mode has failed.
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_ENTER_REMOTE_ERROR = 1210;
        /// <summary>
        /// Switching the Initialization led has failed.
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_SWITCH_INITIALIZATION_LED_ERROR = 1211;
        /// <summary>
        /// Exiting the Remote Local command mode has failed.
        /// The reply from the device is in the first message parameter
        /// </summary>
        public static int INITIALIZATION_STEP_EXIT_REMOTE_ERROR = 1212;
        /// <summary>
        /// Reading the Initialization led address from configuration failed.
        /// </summary>
        public static int INITIALIZATION_STEP_SWITCH_INITIALIZATION_LED_CONFIG_ERROR = 1213;
        /// <summary>
        /// Tried to execute an unsupported command.
        /// </summary>
        public static int INITIALIZATION_UNSUPPORTED_COMMAND = 1214;
        /// <summary>
        /// The initialization step was executed successfully.
        /// </summary>
        public static int INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY = 1215;
        /// <summary>
        /// The pressure range retrieved from the device is not according to the defined range in the station information for manometer TH1 
        /// </summary>
        public static int INITIALIZATION_PRESSURE_RANGE_MANOMETER_TH1_INCORRECT = 1216;
        /// <summary>
        /// The pressure range retrieved from the device is not according to the defined range in the station information for manometer TH2
        /// </summary>
        public static int INITIALIZATION_PRESSURE_RANGE_MANOMETER_TH2_INCORRECT = 1217;
        /// <summary>
        /// The pressure range could not be retrieved from the station information
        /// </summary>
        public static int INITIALIZATION_COULD_NOT_RETRIEVE_PRESSURE_RANGE = 1218;
        /// <summary>
        /// Manometer is not present
        /// </summary>
        public static int INITIALIZATION_MANOMETER_NOT_PRESENT = 1219;
        /// <summary>
        /// Manometer is present
        /// </summary>
        public static int INITIALIZATION_MANOMETER_PRESENT = 1220;
        /// <summary>
        /// There are no subscribers to the UIRequest event
        /// </summary>
        public static int INITIALIZATION_UIREQUEST_UNATTACHED = 1221;
        /// <summary>
        /// The user has aborted the initialization
        /// </summary>
        public static int INITIALIZATION_FINISHED_USER_ABORTED = 1296;
        ///<summary>
        /// The initialization process encountered a timeout on one or more of the manometers
        /// </summary>
        public static int INITIALIZATION_FINISHED_TIMEOUT = 1297;
        /// <summary>
        /// The initialization process finished with an error. 
        /// </summary>
        public static int INITIALIZATION_FINISHED_ERROR = 1298;
        /// <summary>
        /// The initialization process finished with a warning. 
        /// </summary>
        public static int INITIALIZATION_FINISHED_WARNING = 1299;
        #endregion Initialization

        #region Inspection
        /// <summary>
        /// The Inspection process finished succesfully
        /// </summary>
        public static int INSPECTION_FINISHED_SUCCESSFULLY = 1300;
        /// <summary>
        /// Could not create the inspection report
        /// </summary>
        public static int INSPECTION_COULD_NOT_CREATE_INSPECTION_REPORT = 1301;
        /// <summary>
        /// Could not retrieve the station information needed for the result file.
        /// </summary>
        public static int INSPECTION_COULD_NOT_RETRIEVE_STATION_INFORMATION = 1302;
        /// <summary>
        /// Could not update the inspection report
        /// </summary>
        public static int INSPECTION_COULD_NOT_UPDATE_REPORT = 1303;
        /// <summary>
        /// The retrieved StepResult had a sequence number that was different from the step that was currently executed.
        /// </summary>
        public static int INSPECTION_INCORRECT_SEQUENCE_NUMBER = 1304;
        /// <summary>
        /// The retrieved StepResult was of incorrect result type.
        /// </summary>
        public static int INSPECTION_INCORRECT_RESULT_TYPE = 1305;
        /// <summary>
        /// Could not retrieve the inspection procedure
        /// </summary>
        public static int INSPECTION_COULD_NOT_RETRIEVE_INSPECTION = 1306;
        /// <summary>
        /// The inspection process was aborted by the user
        /// </summary>
        public static int INSPECTION_ABORTED_BY_USER = 1307;
        /// <summary>
        /// Could not retrieve the inspection procedure name
        /// </summary>
        public static int INSPECTION_COULD_NOT_RETRIEVE_INSPECTION_PROCEDURE_NAME = 1308;
        /// <summary>
        /// The inspection process could not continue because the volumeVak or volumeVa was not unavailable or could not be parsed.
        /// </summary>
        public static int INSPECTION_COULD_NOT_READ_VOLUMEVAK_OR_VOLUME_VA = 1309;
        /// <summary>
        /// The inspection process encountered an invalid procedure step.
        /// </summary>
        public static int INSPECTION_INVALID_PROCEDURE_STEP = 1310;
        /// <summary>
        /// The inspection process encoutnered an invalid manometer unit. Supported units are 'bar' and 'mbar'.
        /// </summary>
        public static int INSPECTION_INVALID_MANOMETER_UNIT = 1311;
        /// <summary>
        /// The inspection process could not update the measurement file.
        /// </summary>
        public static int INSPECTION_COULD_NOT_UPDATE_MEASUREMENT_RESULT_FILE = 1312;
        /// <summary>
        /// The inspection process could not retrieve the required DTR value from the settings file.
        /// </summary>
        public static int INSPECTION_COULD_NOT_RETRIEVE_MANOMETER_DTR = 1313;
        /// <summary>
        /// The inspection process could not retrieve the connection properties from file.
        /// </summary>
        public static int INSPECTION_COULD_NOT_RETRIEVE_CONNECTION_PROPERTIES = 1314;
        /// <summary>
        /// The inspection process could not prepare the required manometers
        /// </summary>
        public static int INSPECTION_COULD_NOT_PREPARE_REQUIRED_MANOMETERS = 1315;
        /// <summary>
        /// The inspection process failed to initialize properly.
        /// </summary>
        public static int INSPECTION_ACTIVITY_CONTROL_INIT_FAILED = 1316;
        /// <summary>
        /// This inspection process could not creat the measurement report.
        /// </summary>
        public static int INSPECTION_COULD_NOT_START_MEASUREMENT_REPORT = 1317;
        /// <summary>
        /// The pressure range could not be retrieved from the station information
        /// </summary>
        public static int INSPECTION_COULD_NOT_RETRIEVE_PRESSURE_RANGE = 1320;
        /// <summary>
        /// The continuous measurement could not be stopped.
        /// </summary>
        public static int INSPECTION_COULD_NOT_STOP_CONTINUOUS_MEASUREMENT = 1321;
        /// <summary>
        /// The resolution for the given Pressure Range for manometer TH1 could not be found.
        /// </summary>
        public static int INSPECTION_MANOMETER_TH1_RESOLUTION_COULD_NOT_BE_FOUND = 1322;
        /// <summary>
        /// The resolution for the given Pressure Range for manometer TH2 could not be found.
        /// </summary>
        public static int INSPECTION_MANOMETER_TH2_RESOLUTION_COULD_NOT_BE_FOUND = 1323;
        /// <summary>
        /// The configured bluetooth address is undefined in the plexor information (xml file).
        /// </summary>
        public static int INSPECTION_PLEXOR_BLUETOOTH_ADDRESS_UNDEFINED = 1324;
        /// <summary>
        /// There are no subscribers to the UIRequest event.
        /// </summary>
        public static int INSPECTION_UIREQUEST_UNATTACHED = 1325;
        /// <summary>
        /// IO status is incorrect.
        /// </summary>
        public static int INSPECTION_INCORRECT_IO_STATUS = 1326;

        #endregion Inspection

        #region General errors
        /// <summary>
        /// Inspector could not find the InspectorSettings.xml file.
        /// </summary>
        public static int GENERAL_COULD_NOT_FIND_INSPECTOR_SETTINGS = 1400;
        /// <summary>
        /// Inspector could not load a required setting from the InspectorSettings.xml file.
        /// </summary>
        public static int GENERAL_COULD_NOT_READ_PROPERTY_INSPECTOR_SETTINGS = 1401;

        public static int GENERAL_INVALID_UIRESPONSE = 1402;
        #endregion General errros

        #region Manometer errors
        /// <summary>
        /// The manometer replied with er-001:
        ///  "RS232 Protocol checksum Error"
        /// </summary>
        public static int MANOMETER_RS232_PROTOCOL_CHECKSUM_ERROR = 1600;

        /// <summary>
        /// The manometer replied with er-110:
        ///  "Header Error: Too short"
        ///  "Header Error: Too many subnodes"
        ///  "Header Error: Query not at leaf node"
        ///  "Header Error: Multiple queries"
        ///  "Header Error: Characters after query"
        ///  "Header Error: Too long"
        /// </summary>
        public static int MANOMETER_HEADER_ERROR = 1601;

        /// <summary>
        /// The manometer replied with er-113:
        ///  "Undefined Header; Undefined command"
        /// </summary>
        public static int MANOMETER_UNDEFINED_HEADER_UNDEFINED_COMMAND = 1602;

        /// <summary>
        /// The manometer replied with er-101:
        ///  "Invalid character": Terminator expected"
        /// </summary>
        public static int MANOMETER_INVALID_CHARACTER_TERMINATOR_EXPECTED = 1603;

        /// <summary>
        /// The manometer replied with er-108:
        ///  "Invalid parameter: Out of bounds"
        ///  "Invalid parameter: Too long"
        /// </summary>
        public static int MANOMETER_INVALID_PARAMETER = 1604;

        /// <summary>
        /// The manometer replied with er-203:
        ///  "Command protected"
        /// </summary>
        public static int MANOMETER_COMMAND_PROTECTED = 1605;

        /// <summary>
        /// The manometer replied with er-999:
        ///  "EEProm Read/Write Error"
        /// </summary>
        public static int MANOMETER_EEPROM_READ_WRITE_ERROR = 1606;

        /// <summary>
        /// The manometer replied with er-002:
        ///  "Fatal Command Execution Error"
        /// </summary>
        public static int MANOMETER_FATAL_COMMAND_EXECUTION_ERROR = 1607;

        /// <summary>
        /// The manometer replied with an unknown error (any potential error which is not described in the specification).
        /// </summary>
        public static int MANOMETER_UNKNOWN_ERROR = 1608;

        /// <summary>
        /// The manometer replied with er-109:
        ///  "Missing parameter
        ///  "Missing parameter: Boolean expected"
        ///  "Missing parameter: String expected"
        ///  "Missing parameter: Discrete expected"
        ///  "Missing parameter: Not of expected type"
        /// </summary>
        public static int MANOMETER_MISSING_PARAMETER = 1609;
        #endregion Manometer errors
    }
}
