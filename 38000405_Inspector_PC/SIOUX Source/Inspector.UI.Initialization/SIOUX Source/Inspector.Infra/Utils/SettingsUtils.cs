/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using KAM.INSPECTOR.Infra;

namespace Inspector.Infra.Utils
{
    /// <summary>
    /// Used to retrieve settings from inspectorSettings.xml
    /// </summary>
    public static class SettingsUtils
    {
        private const string XML_DEFAULT_PATH = @"XML\";
        private const string XSD_DEFAULT_PATH = @"XSD\";
        private const string TEMP_DEFAULT_PATH = @"TEMP\";
        private const string MEASUREMENT_FILES_DEFAULT_PATH = @"Measurements\";
        private const string SETTINGS_CATEGORY = "APPLICATION";
        private const string SETTINGS_XML_PATH = "XmlFilesPath";
        private const string SETTINGS_XSD_PATH = "XsdFilesPath";
        private const string SETTINGS_TEMP_PATH = "TemporaryFilesPath";
        private const string SETTINGS_MEASUREMENT_FILES_PATH = "MeasurementFilesPath";
        private const string SETTINGS_RETURN_NO_VALUE = "<NO VALUE>";
        private const string SETTINGS_PLEXOR = "PLEXOR";
        private const string SETTINGS_BLUETOOTH_API = "BluetoothApi";
        private const string SETTINGS_ADDRESS = "BluetoothDestinationAddress";
        private const string SETTINGS_DTR = "DTR";

        /// <summary>
        /// Gets the XML file path.
        /// </summary>
        /// <returns></returns>
        public static string LookupXmlFilePath()
        {
            return GetFilePath(SETTINGS_XML_PATH, XML_DEFAULT_PATH);
        }

        /// <summary>
        /// Gets the XSD file path.
        /// </summary>
        /// <returns></returns>
        public static string LookupXsdFilePath()
        {
            return GetFilePath(SETTINGS_XSD_PATH, XSD_DEFAULT_PATH);
        }

        /// <summary>
        /// Gets the temporary path.
        /// </summary>
        /// <returns></returns>
        public static string LookupTemporaryPath()
        {
            return GetFilePath(SETTINGS_TEMP_PATH, TEMP_DEFAULT_PATH);
        }

        /// <summary>
        /// Lookups the measurement files path.
        /// </summary>
        /// <returns></returns>
        public static string LookupMeasurementFilesPath()
        {
            return GetFilePath(SETTINGS_MEASUREMENT_FILES_PATH, MEASUREMENT_FILES_DEFAULT_PATH);
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="settingsField">The settings field.</param>
        /// <param name="defaultPath">The default path.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetFilePath(string settingsField, string defaultPath)
        {
            string filePath = String.Empty;
            try
            {
                clsSettings settings = new clsSettings();
                string pathFromSettings = settings.get_GetSetting(SETTINGS_CATEGORY, settingsField).ToString();

                if (pathFromSettings.Equals(SETTINGS_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
                {
                    filePath = defaultPath;
                }
                else
                {
                    filePath = pathFromSettings;
                }
            }
            catch
            {
                filePath = defaultPath;
            }

            return filePath;
        }

        /// <summary>
        /// Retrieves the connection properties from the InspectorSettings.xml.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when InspectorSettings.xml cannot be found.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a required value cannot be found in InspectorSettings.xml</exception>
        public static Dictionary<string, string> RetrieveConnectionProperties()
        {
            Dictionary<string, string> connectionProperties = new Dictionary<string, string>();
            try
            {
                clsSettings settings = new clsSettings();
                string bluetoothApi = settings.get_GetSetting(SETTINGS_PLEXOR, SETTINGS_BLUETOOTH_API).ToString();
                string destinationAddress = settings.get_GetSetting(SETTINGS_PLEXOR, SETTINGS_ADDRESS).ToString();
                if (bluetoothApi.ToUpperInvariant().Equals(SETTINGS_RETURN_NO_VALUE))
                {
                    throw new ArgumentOutOfRangeException(String.Format(CultureInfo.InvariantCulture, "{0} should be available in the configuration file.", SETTINGS_BLUETOOTH_API));
                }
                if (destinationAddress.ToUpperInvariant().Equals(SETTINGS_RETURN_NO_VALUE))
                {
                    throw new ArgumentOutOfRangeException(String.Format(CultureInfo.InvariantCulture, "{0} should be available in the configuration file.", SETTINGS_ADDRESS));
                }
                connectionProperties.Add("bluetoothApi", bluetoothApi);
                connectionProperties.Add("destinationAddress", destinationAddress);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Could not find Configuration file", ex);
            }

            return connectionProperties;
        }

        /// <summary>
        /// Gets the DTR.
        /// </summary>
        /// <returns>The DTS setting.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when dts value is invalid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public static string GetDTR()
        {
            clsSettings settings = new clsSettings();
            string dtr = settings.get_GetSetting(SETTINGS_PLEXOR, SETTINGS_DTR).ToString();
            if (!(dtr.Equals("Low") || dtr.Equals("High")))
            {
                throw new ArgumentOutOfRangeException("Value for Dtr should either be 'High' or 'Low'");
            }

            return dtr;
        }
    }
}
