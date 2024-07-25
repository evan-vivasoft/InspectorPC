/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security;
using Inspector.BusinessLogic.Data.Reporting.Interfaces.Exceptions;

namespace Inspector.BusinessLogic.Data.Reporting.Measurements
{
    /// <summary>
    /// Access the measurement translations defined in KAM.Inspector.Infra MeasurementDataFileResx resource
    /// </summary>
    /// <remarks>
    /// Multithread safe singleton implemeted according to http://msdn.microsoft.com/en-us/library/ff650316.aspx
    /// </remarks>
    internal sealed class MeasurementTranslations
    {
        #region Constants
        private static string RESOURCE_ASSEMBLY_NAME = "KAM.INSPECTOR.Infra.dll";
        private static string RESOURCE_RESX_NAME = "KAM.INSPECTOR.Infra.MeasurementDataFileResx";

        internal static string PLEXOR_NAME = "PLEXOR name";
        internal static string PLEXOR_BT_ADDRESS = "PLEXOR BT address";
        internal static string TH1_SERIAL_NUMBER = "TH1 serial number";
        internal static string TH2_SERIAL_NUMBER = "TH2 serial number";
        internal static string PRS_NAME = "Station";
        internal static string PSR_CODE = "StationCode";
        internal static string GCL_NAME = "Gas control line";
        internal static string GCL_IDENTIFICATION = "Gas control line identification code";
        internal static string INSPECTION_PROCEDURE_NAME = "Test program";
        internal static string INSPECTOR_VERSION = "InspectorVersion";
        internal static string FSD_START = "FSDStart";
        internal static string S_DATE = "DATE";
        internal static string S_TIME = "TIME";
        internal static string INSPECTION_PROCEDURE_VERSION = "InspectionProcedureVersion";
        internal static string SEPERATOR_SIGN = "=:";
        internal static string SCRIPTCOMMMAND = "ScriptCommand";
        internal static string START_OF_MEASUREMENT = "Start of measurement";
        internal static string END_OF_MEASUREMENT = "End of measurement";
        internal static string COUNT_TOTAL = "Count total";
        internal static string INTERVAL = "Interval";
        internal static string FIELD_IN_ACCESS_DATABASE = "Field in Access database";
        internal static string OBJECT_NAME = "ObjectName";
        internal static string MEASURE_POINT = "Measurepoint";
        internal static string VALUE = "Value";
        #endregion Constants

        #region Class members
        private static volatile MeasurementTranslations s_Instance;
        private static object s_SyncRoot = new object();

        private ResourceManager m_ResourceManager;
        #endregion Class members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementTranslations"/> class.
        /// </summary>
        /// <exception cref="MeasurementTranslationException">Thrown when the translation resources could not be loaded.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
        private MeasurementTranslations()
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(Path.GetFullPath(RESOURCE_ASSEMBLY_NAME));
                m_ResourceManager = new ResourceManager(RESOURCE_RESX_NAME, assembly);
            }
            catch (ArgumentException argumentException)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Could not locate the full path of the assembly '{0}' with the measurementdatafile resources.", RESOURCE_ASSEMBLY_NAME);
                throw new MeasurementTranslationException(message, argumentException);
            }
            catch (SecurityException securityException)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Could not access the full path of the assembly '{0}' with the measurementdatafile resources.", RESOURCE_ASSEMBLY_NAME);
                throw new MeasurementTranslationException(message, securityException);
            }
            catch (Exception exception)
            {
                throw new MeasurementTranslationException("Failed to load measurement translations", exception);
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        /// <exception cref="MeasurementTranslationException">Thrown when the translation resources could not be loaded.</exception>
        public static MeasurementTranslations Instance
        {
            get
            {
                lock (s_SyncRoot)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new MeasurementTranslations();
                    }
                }
                return s_Instance;
            }
        }
        #endregion Constructors

        #region Public
        /// <summary>
        /// Gets the value by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value belonging to the key.</returns>
        /// <exception cref="MeasurementTranslationException">Thrown when the value of the given key could not be retrieved.</exception>
        public string GetValueByKey(string key)
        {
            string value = m_ResourceManager.GetString(key);
            if (value == null)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Failed to retrieve the value for the key '{0}'", key);
                throw new MeasurementTranslationException(message);
            }

            return value;
        }

        /// <summary>
        /// Gets the name of the plexor.
        /// </summary>
        /// <value>The name of the plexor.</value>
        public string PlexorName
        {
            get
            {
                return GetValueByKeyOrDefault("PLEXOR_Name", PLEXOR_NAME);
            }
        }

        /// <summary>
        /// Gets the plexor bluetooth address.
        /// </summary>
        /// <value>The plexor bluetooth address.</value>
        public string PlexorBluetoothAddress
        {
            get
            {
                return GetValueByKeyOrDefault("PLEXOR_BT_Address", PLEXOR_BT_ADDRESS);
            }
        }

        /// <summary>
        /// Gets the TH1 serial number.
        /// </summary>
        /// <value>The TH1 serial number.</value>
        public string TH1SerialNumber
        {
            get
            {
                return GetValueByKeyOrDefault("TH1_Serial_Number", TH1_SERIAL_NUMBER);
            }
        }

        /// <summary>
        /// Gets the TH2 serial number.
        /// </summary>
        /// <value>The TH2 serial number.</value>
        public string TH2SerialNumber
        {
            get
            {
                return GetValueByKeyOrDefault("TH2_Serial_Number", TH2_SERIAL_NUMBER);
            }
        }

        /// <summary>
        /// Gets the name of the PRS.
        /// </summary>
        /// <value>The name of the PRS.</value>
        public string PRSName
        {
            get
            {
                return GetValueByKeyOrDefault("PRS_Name", PRS_NAME);
            }
        }

        /// <summary>
        /// Gets the PRS code.
        /// </summary>
        /// <value>The PRS code.</value>
        public string PRSCode
        {
            get
            {
                return GetValueByKeyOrDefault("PRS_Code", PSR_CODE);
            }
        }

        /// <summary>
        /// Gets the name of the GCL.
        /// </summary>
        /// <value>The name of the GCL.</value>
        public string GCLName
        {
            get
            {
                return GetValueByKeyOrDefault("GCL_Name", GCL_NAME);
            }
        }

        /// <summary>
        /// Gets the GCL identification.
        /// </summary>
        /// <value>The GCL identification.</value>
        public string GCLIdentification
        {
            get
            {
                return GetValueByKeyOrDefault("GCL_Identification", GCL_IDENTIFICATION);
            }
        }

        /// <summary>
        /// Gets the name of the inspection procedure.
        /// </summary>
        /// <value>The name of the inspection procedure.</value>
        public string InspectionProcedureName
        {
            get
            {
                return GetValueByKeyOrDefault("Inspection_Procedure_Name", INSPECTION_PROCEDURE_NAME);
            }
        }

        /// <summary>
        /// Gets the inspector version.
        /// </summary>
        /// <value>The inspector version.</value>
        public string InspectorVersion
        {
            get
            {
                return GetValueByKeyOrDefault("Inspector_Version", INSPECTOR_VERSION);
            }
        }

        /// <summary>
        /// Gets the FSD start.
        /// </summary>
        /// <value>The FSD start.</value>
        public string FSDStart
        {
            get
            {
                return GetValueByKeyOrDefault("FSDStart", FSD_START);
            }
        }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public string StartDate
        {
            get
            {
                return GetValueByKeyOrDefault("sDate", S_DATE);
            }
        }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public string StartTime
        {
            get
            {
                return GetValueByKeyOrDefault("sTime", S_TIME);
            }
        }

        /// <summary>
        /// Gets the inspection procedure version.
        /// </summary>
        /// <value>The inspection procedure version.</value>
        public string InspectionProcedureVersion
        {
            get
            {
                return GetValueByKeyOrDefault("Inspection_Procedure_Version", INSPECTION_PROCEDURE_VERSION);
            }
        }

        /// <summary>
        /// Gets the seperator sign.
        /// </summary>
        /// <value>The seperator sign.</value>
        public string SeperatorSign
        {
            get
            {
                return GetValueByKeyOrDefault("Seperator_Sign", SEPERATOR_SIGN);
            }
        }

        /// <summary>
        /// Gets the script command.
        /// </summary>
        /// <value>The script command.</value>
        public string ScriptCommand
        {
            get
            {
                return GetValueByKeyOrDefault("Scriptcommand", SCRIPTCOMMMAND);
            }
        }

        /// <summary>
        /// Gets the start of measurement.
        /// </summary>
        /// <value>The start of measurement.</value>
        public string StartOfMeasurement
        {
            get
            {
                return GetValueByKeyOrDefault("Start_Of_Measurement", START_OF_MEASUREMENT);
            }
        }

        /// <summary>
        /// Gets the end of measurement.
        /// </summary>
        /// <value>The end of measurement.</value>
        public string EndOfMeasurement
        {
            get
            {
                return GetValueByKeyOrDefault("End_Of_Measurement", END_OF_MEASUREMENT);
            }
        }

        /// <summary>
        /// Gets the count total.
        /// </summary>
        /// <value>The count total.</value>
        public string CountTotal
        {
            get
            {
                return GetValueByKeyOrDefault("Count_Total", COUNT_TOTAL);
            }
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public string Interval
        {
            get
            {
                return GetValueByKeyOrDefault("Interval", INTERVAL);
            }
        }

        /// <summary>
        /// Gets the field in access database.
        /// </summary>
        /// <value>The field in access database.</value>
        public string FieldInAccessDatabase
        {
            get
            {
                return GetValueByKeyOrDefault("Field_In_Access_Database", FIELD_IN_ACCESS_DATABASE);
            }
        }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        public string ObjectName
        {
            get
            {
                return GetValueByKeyOrDefault("Object_Name", OBJECT_NAME);
            }
        }

        /// <summary>
        /// Gets the measure point.
        /// </summary>
        /// <value>The measure point.</value>
        public string MeasurePoint
        {
            get
            {
                return GetValueByKeyOrDefault("Measure_Point", MEASURE_POINT);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return GetValueByKeyOrDefault("Value", VALUE);
            }
        }
        #endregion Public

        #region Private
        /// <summary>
        /// Gets the value by key or default.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value from the resource file that belongs to the given key or the defaultValue if retrieval of the value fails.</returns>
        private string GetValueByKeyOrDefault(string key, string defaultValue)
        {
            string value = String.Empty;
            try
            {
                value = GetValueByKey(key);
            }
            catch (MeasurementTranslationException)
            {
                value = defaultValue;
            }
            return value;
        }
        #endregion Private
    }
}
