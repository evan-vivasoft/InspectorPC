/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Collections.Generic;
using Inspector.Model;
namespace Inspector.Hal
{
    /// <summary>
    /// Contains the error message that a manometer in an erroneous state sends
    /// </summary>
    internal static class ManometerErrorReply
    {
        public const string ERRORMESSAGE_FORMAT_REGEX = @"^er-\d{3}$";

        public const string RS232_PROTOCOL_CHECKSUM_ERROR = "er-001";
        public const string HEADER_ERROR = "er-110";
        public const string UNDEFINED_HEADER_UNDEFINED_COMMAND = "er-113";
        public const string MISSING_PARAMETER = "er-109";
        public const string INVALID_CHARACTER_TERMINATOR_EXPECTED = "er-101";
        public const string INVALID_PARAMETER = "er-108";
        public const string COMMAND_PROTECTED = "er-203";
        public const string EEPROM_READ_WRITE_ERROR = "er-999";
        public const string FATAL_COMMAND_EXECUTION_ERROR = "er-002";

        /// <summary>
        /// Lookup has from manometer error string to integer error (ErrorCodes constant).
        /// </summary>
        public static readonly Dictionary<string, int> ErrorCodeLookup = new Dictionary<string, int>
        {
            { RS232_PROTOCOL_CHECKSUM_ERROR, ErrorCodes.MANOMETER_RS232_PROTOCOL_CHECKSUM_ERROR },
            { HEADER_ERROR, ErrorCodes.MANOMETER_HEADER_ERROR },
            { UNDEFINED_HEADER_UNDEFINED_COMMAND, ErrorCodes.MANOMETER_UNDEFINED_HEADER_UNDEFINED_COMMAND },
            { MISSING_PARAMETER, ErrorCodes.MANOMETER_MISSING_PARAMETER},
            { INVALID_CHARACTER_TERMINATOR_EXPECTED, ErrorCodes.MANOMETER_INVALID_CHARACTER_TERMINATOR_EXPECTED },
            { INVALID_PARAMETER, ErrorCodes.MANOMETER_INVALID_PARAMETER },
            { COMMAND_PROTECTED, ErrorCodes.MANOMETER_COMMAND_PROTECTED },
            { EEPROM_READ_WRITE_ERROR, ErrorCodes.MANOMETER_EEPROM_READ_WRITE_ERROR },
            { FATAL_COMMAND_EXECUTION_ERROR, ErrorCodes.MANOMETER_FATAL_COMMAND_EXECUTION_ERROR },
        };
    }
}
