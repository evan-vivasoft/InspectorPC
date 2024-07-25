/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station;
using Inspector.Infra.Utils;
using Inspector.Model;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders
{
    /// <summary>
    /// Information of PRS and contained GasControlLines
    /// </summary>
    internal class StationInformationLoader
    {
        #region Constants
        private const string XML_FILE = "StationInformation.xml";
        private const string XSD_FILE = "StationInformation.xsd";
        private const string PRS_TABLE_NAME = "PRS";
        private const string GCL_TABLE_NAME = "GasControlLine";

        private const string PRS_PRSOBJECTS_RELATION_NAME = "PRS_PRSObjects";
        private const string PRS_GCL_RELATION_NAME = "PRS_GCLs";
        private const string GCL_GCLOBJECTS_RELATION_NAME = "GasControlLine_GCLObjects";
        private const string GCLOBJECTS_BOUNDARIES_RELATION_NAME = "GCLObjects_Boundaries";

        // PRS to GasControlLine primary key columns
        private const string PRSGCL_PRIMARY_KEY_PART1_OF_2 = "PRSIdentification";
        private const string PRSGCL_PRIMARY_KEY_PART2_OF_2 = "PRSName";
        #endregion Constants

        #region Members
        private string m_XmlFilePath;
        private string m_XsdFilePath;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the PRS entities.
        /// </summary>
        /// <value>The PRS entities.</value>
        public List<PRSEntity> PRSEntities { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StationInformationLoader"/> class.
        /// </summary>
        /// <exception cref="XmlException"/>
        public StationInformationLoader()
        {
            SetFilePaths();
            Reload();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StationInformationLoader"/> class.
        /// For unit testing.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <exception cref="XmlException"/>
        internal StationInformationLoader(string xml)
        {
            SetFilePaths();
            PRSEntities = ReadStationInformation(xml);
        }
        #endregion ConstructorsF

        #region Public
        public void Reload()
        {
            PRSEntities = ReadStationInformation(m_XmlFilePath, m_XsdFilePath);
        }
        #endregion Public

        #region Private
        /// <summary>
        /// Sets the file paths.
        /// </summary>
        private void SetFilePaths()
        {
            m_XmlFilePath = Path.Combine(SettingsUtils.LookupXmlFilePath(), XML_FILE);
            m_XsdFilePath = Path.Combine(SettingsUtils.LookupXsdFilePath(), XSD_FILE);
        }

        /// <summary>
        /// Reads the station information from the configuration file.
        /// </summary>
        /// <returns>The station information</returns>
        /// <exception cref="XmlException"/>
        private static List<PRSEntity> ReadStationInformation(string xmlFile, string xsdFile)
        {
            XMLUtils.ValidateXmlFile(xmlFile, xsdFile);
            List<PRSEntity> prsResults = null;

            using (DataSet dataSet = new DataSet())
            {
                dataSet.Locale = CultureInfo.InvariantCulture;
                dataSet.ReadXmlSchema(xsdFile);
                dataSet.ReadXml(xmlFile, XmlReadMode.ReadSchema);

                RemoveGCLsWithoutPRSs(dataSet);

                InjectPRSToGasControlLineRelation(dataSet);
                DataTable prsTable = dataSet.Tables[PRS_TABLE_NAME];
                prsResults = ExtractPrsObjects(prsTable);
            }

            return prsResults;
        }

        private static void RemoveGCLsWithoutPRSs(DataSet dataSet)
        {
            DataRow[] foundRows = dataSet.Tables[GCL_TABLE_NAME].Select();
            if (foundRows != null)
            {
                foreach (var row in foundRows)
                {
                    string searchString = string.Format("PRSName like '{0}' AND PRSIdentification like '{1}'", row.ItemArray[0].ToString(), row.ItemArray[1].ToString());
                    DataRow[] FoundPRSRows = dataSet.Tables[PRS_TABLE_NAME].Select(searchString);
                    if (FoundPRSRows.Length == 0)
                    {
                        row.Delete();
                    }
                }
            }
        }

        private List<PRSEntity> ReadStationInformation(string xml)
        {
            string tempXMLFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XmlTempFile.tmp");
            if (File.Exists(tempXMLFileName))
            {
                File.Delete(tempXMLFileName);
            }

            using (StreamWriter writer = new StreamWriter(tempXMLFileName))
            {
                writer.Write(xml);
            }

            return ReadStationInformation(tempXMLFileName, m_XsdFilePath);
        }
        #endregion Private

        #region Dataset parse helpers
        /// <summary>
        /// Extracts the PRS objects.
        /// </summary>
        /// <param name="prsTable">The PRS table.</param>
        /// <returns>The PRS entities</returns>
        private static List<PRSEntity> ExtractPrsObjects(DataTable prsTable)
        {
            List<PRSEntity> prsResults = new List<PRSEntity>();

            foreach (DataRow prsRow in prsTable.Rows)
            {
                PRSEntity prs = new PRSEntity
                {
                    Route = prsRow["Route"].ToString(),
                    PRSCode = prsRow["PRSCode"].ToString(),
                    PRSName = prsRow["PRSName"].ToString(),
                    PRSIdentification = prsRow["PRSIdentification"].ToString(),
                    Information = prsRow["Information"].ToString(),
                    InspectionProcedure = prsRow["InspectionProcedure"].ToString(),
                };

                ExtractPrsObject(prsTable, prsRow, prs);
                ExtractGasControlLineObject(prsTable, prsRow, prs);
                prsResults.Add(prs);
            }

            return prsResults;
        }

        /// <summary>
        /// Injects the PRS-to-gascontrolline relation.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        private static void InjectPRSToGasControlLineRelation(DataSet dataSet)
        {
            DataColumn[] parentKeyColumns = new DataColumn[] 
            {
                dataSet.Tables[PRS_TABLE_NAME].Columns[PRSGCL_PRIMARY_KEY_PART1_OF_2], 
                dataSet.Tables[PRS_TABLE_NAME].Columns[PRSGCL_PRIMARY_KEY_PART2_OF_2] 
            };
            DataColumn[] childKeyColumns = new DataColumn[] 
            { 
                dataSet.Tables[GCL_TABLE_NAME].Columns[PRSGCL_PRIMARY_KEY_PART1_OF_2], 
                dataSet.Tables[GCL_TABLE_NAME].Columns[PRSGCL_PRIMARY_KEY_PART2_OF_2] 
            };
            DataRelation relation = new DataRelation(PRS_GCL_RELATION_NAME, parentKeyColumns, childKeyColumns);
            dataSet.Relations.Add(relation);
        }

        /// <summary>
        /// Extracts the gas control line object.
        /// </summary>
        /// <param name="prsTable">The PRS table.</param>
        /// <param name="prsRow">The PRS row.</param>
        /// <param name="prs">The PRS.</param>
        private static void ExtractGasControlLineObject(DataTable prsTable, DataRow prsRow, PRSEntity prs)
        {
            if (prsTable.ChildRelations.Contains(PRS_GCL_RELATION_NAME))
            {
                DataRelation childRelation = prsTable.ChildRelations[PRS_GCL_RELATION_NAME];
                DataRow[] prsGasControlLines = prsRow.GetChildRows(childRelation);

                foreach (DataRow prsGasControlLine in prsGasControlLines)
                {
                    GasControlLineEntity gcl = new GasControlLineEntity
                    {
                        PRSName = prsGasControlLine["PRSName"].ToString(),
                        PRSIdentification = prsGasControlLine["PRSIdentification"].ToString(),
                        GasControlLineName = prsGasControlLine["GasControlLineName"].ToString(),
                        PeMin = prsGasControlLine["PeMin"].ToString(),
                        PeMax = prsGasControlLine["PeMax"].ToString(),
                        VolumeVA = prsGasControlLine["VolumeVA"].ToString(),
                        VolumeVAK = prsGasControlLine["VolumeVAK"].ToString(),
                        PaRangeDM = CastToTypeRangeDMOrUnset(prsGasControlLine["PaRangeDM"].ToString()),
                        PeRangeDM = CastToTypeRangeDMOrUnset(prsGasControlLine["PeRangeDM"].ToString()),
                        GCLIdentification = prsGasControlLine["GCLIdentification"].ToString(),
                        GCLCode = prsGasControlLine["GCLCode"].ToString(),
                        InspectionProcedure = prsGasControlLine["InspectionProcedure"].ToString(),
                        FSDStart = CastToIntOrNull(prsGasControlLine["FSDStart"].ToString()) ?? -1,
                    };

                    ExtractGasControlLineGCLObjects(prsGasControlLine, gcl);
                    prs.GasControlLines.Add(gcl);
                }
            }
        }

        /// <summary>
        /// Extracts the gas control line GCL objects.
        /// </summary>
        /// <param name="prsGasControlLine">The PRS gas control line.</param>
        /// <param name="gcl">The GCL.</param>
        private static void ExtractGasControlLineGCLObjects(DataRow prsGasControlLine, GasControlLineEntity gcl)
        {
            if (prsGasControlLine.Table.ChildRelations.Contains(GCL_GCLOBJECTS_RELATION_NAME))
            {
                DataRelation subRelation = prsGasControlLine.Table.ChildRelations[GCL_GCLOBJECTS_RELATION_NAME];
                DataRow[] gclObjects = prsGasControlLine.GetChildRows(subRelation);

                foreach (DataRow gclObject in gclObjects)
                {
                    GCLObject typeObjectId = new GCLObject
                    {
                        ObjectName = gclObject["ObjectName"].ToString(),
                        ObjectID = gclObject["ObjectID"].ToString(),
                        MeasurePoint = gclObject["MeasurePoint"].ToString(),
                        MeasurePointID = gclObject["MeasurePointID"].ToString(),
                        FieldNo = CastToIntOrNull(gclObject["FieldNo"].ToString()),
                    };

                    ExtractGCLObjectBoundariesObject(gclObject, typeObjectId);
                    gcl.GCLObjects.Add(typeObjectId);
                }
            }
        }

        /// <summary>
        /// Extracts the GCL object boundaries object.
        /// </summary>
        /// <param name="gclObject">The GCL object.</param>
        /// <param name="typeObjectId">The type object id.</param>
        private static void ExtractGCLObjectBoundariesObject(DataRow gclObject, GCLObject typeObjectId)
        {
            if (gclObject.Table.ChildRelations.Contains(GCLOBJECTS_BOUNDARIES_RELATION_NAME))
            {
                DataRelation subSubRelation = gclObject.Table.ChildRelations[GCLOBJECTS_BOUNDARIES_RELATION_NAME];
                DataRow[] gclObjectBoundaries = gclObject.GetChildRows(subSubRelation);

                if (gclObjectBoundaries.Length > 0)
                {
                    DataRow gclObjectBoundary = gclObjectBoundaries[0];
                    typeObjectId.Boundaries = new TypeObjectIDBoundaries
                    {
                        ValueMax = CastToDoubleOrNan(gclObjectBoundary["ValueMax"].ToString()),
                        ValueMin = CastToDoubleOrNan(gclObjectBoundary["ValueMin"].ToString()),
                        UOV = CastToTypeUnitsValueOrUnset(gclObjectBoundary["UOV"].ToString()),
                    };
                }
            }
        }

        /// <summary>
        /// Extracts the PRS object.
        /// </summary>
        /// <param name="prsTable">The PRS table.</param>
        /// <param name="prsRow">The PRS row.</param>
        private static void ExtractPrsObject(DataTable prsTable, DataRow prsRow, PRSEntity prs)
        {
            if (prsTable.ChildRelations.Contains(PRS_PRSOBJECTS_RELATION_NAME))
            {
                DataRelation childRelation = prsTable.ChildRelations[PRS_PRSOBJECTS_RELATION_NAME];
                DataRow[] prsObjectRows = prsRow.GetChildRows(childRelation);

                foreach (DataRow prsObjectRow in prsObjectRows)
                {
                    PRSObject prsObject = new PRSObject
                    {
                        ObjectName = prsObjectRow["ObjectName"].ToString(),
                        ObjectID = prsObjectRow["ObjectID"].ToString(),
                        MeasurePoint = prsObjectRow["MeasurePoint"].ToString(),
                        MeasurePointID = prsObjectRow["MeasurePointID"].ToString(),
                        FieldNo = CastToIntOrNull(prsObjectRow["FieldNo"].ToString()),
                    };

                    prs.PRSObjects.Add(prsObject);
                }
            }
        }
        #endregion Dataset parse helpers

        #region Casting helpers
        /// <summary>
        /// Casts to int, defaults to -1.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>value as int if succesfull otherwise -1</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Int32.TryParse(System.String,System.Int32@)")]
        private static int? CastToIntOrNull(string value)
        {
            int? result = null;
            if (!String.IsNullOrEmpty(value))
            {
                int parsedResult = -1;
                int.TryParse(value, out parsedResult);
                result = parsedResult;
            }
            return result;
        }

        /// <summary>
        /// Casts to type range DM, defaults to unset.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>value as TypeRangeDM if successfull otherwise TypeRangeDM.UNSET</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Enum.TryParse<Inspector.Model.TypeRangeDM>(System.String,System.Boolean,Inspector.Model.TypeRangeDM@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Enum.TryParse<Inspector.BusinessLogic.InspectionManager.Model.TypeRangeDM>(System.String,System.Boolean,Inspector.BusinessLogic.InspectionManager.Model.TypeRangeDM@)")]
        private static TypeRangeDM CastToTypeRangeDMOrUnset(string value)
        {
            TypeRangeDM result = TypeRangeDM.UNSET;
            value = "Item" + value.Replace("-", String.Empty).Replace(".", String.Empty);
            Enum.TryParse<TypeRangeDM>(value, true, out result);
            return result;
        }

        /// <summary>
        /// Casts to type units value, defaults to unset.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>value as TypeUnitsValue if successfull otherwise TypeUnitsValue.UNSET</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Enum.TryParse<Inspector.Model.UnitOfMeasurement>(System.String,System.Boolean,Inspector.Model.UnitOfMeasurement@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Enum.TryParse<Inspector.BusinessLogic.InspectionManager.Model.TypeUnitsValue>(System.String,System.Boolean,Inspector.BusinessLogic.InspectionManager.Model.TypeUnitsValue@)")]
        private static UnitOfMeasurement CastToTypeUnitsValueOrUnset(string value)
        {
            UnitOfMeasurement result = UnitOfMeasurement.UNSET;
            value = "Item" + value.Replace("-", String.Empty).Replace("/", String.Empty);
            Enum.TryParse<UnitOfMeasurement>(value, true, out result);
            return result;
        }

        /// <summary>
        /// Casts to double, defaults to double.NaN.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>value as double if successfull otherwise double.NaN</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Double.TryParse(System.String,System.Double@)")]
        private static double CastToDoubleOrNan(string value)
        {
            double result = double.NaN;
            if (!String.IsNullOrEmpty(value))
            {
                double.TryParse(value, out result);
            }
            return result;
        }
        #endregion Casting helpers
    }
}
