/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.AutMapper;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedure;
using Inspector.Infra.Utils;
using Inspector.POService.InformationManager;
namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders
{
    /// <summary>
    /// InspectionProcedureInformationLoader
    /// </summary>
    internal class InspectionProcedureInformationLoader
    {
        #region Constants
        private const string XML_FILE = "InspectionProcedure.xml";
        private const string XSD_FILE = "InspectionProcedure.xsd";
        #endregion Constants

        #region Members
        private List<InspectionProcedureEntity> m_InspectionProcedures;

        private string m_XmlFilePath;
        private string m_XsdFilePath;
        #endregion Members

        #region Properties
        /// <summary>
        /// Gets or sets the plexors.
        /// </summary>
        /// <value>The plexors.</value>
        public IList<InspectionProcedureEntity> InspectionProcedures
        {
            private set
            {
                m_InspectionProcedures = new List<InspectionProcedureEntity>(value);
            }
            get
            {
                if (m_InspectionProcedures == null)
                {
                    m_InspectionProcedures = new List<InspectionProcedureEntity>();
                }
                return m_InspectionProcedures;
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PlexorInformationLoader"/> class.
        /// </summary>
        public InspectionProcedureInformationLoader()
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
        internal InspectionProcedureInformationLoader(string xml)
        {
            SetFilePaths();
            InspectionProcedures = ReadInspectionProcedureInformation(xml);
        }
        #endregion Constructors

        #region Public
        /// <summary>
        /// Reloads the plexor information from the XML file.
        /// </summary>
        public void Reload()
        {
            InspectionProcedures =
                MapperClass.Instance.Mapper.Map<IList<InspectionProcedureEntity>>(InformationManager.Instance.GetInspectionProcedure);
            Console.WriteLine(m_XmlFilePath);
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
        /// Reads the plexor information.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>List of Inspection procedures</returns>
        private IList<InspectionProcedureEntity> ReadInspectionProcedureInformation(string xml)
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

            return ReadInspectionProcedureInformation(tempXMLFileName, m_XsdFilePath);
        }

        /// <summary>
        /// Reads the plexor information.
        /// </summary>
        /// <param name="xmlFile">The XML file.</param>
        /// <param name="xsdFile">The XSD file.</param>
        /// <returns>List of Inspection procedures</returns>
        private static IList<InspectionProcedureEntity> ReadInspectionProcedureInformation(string xmlFile, string xsdFile)
        {
            XMLUtils.ValidateXmlFile(xmlFile, xsdFile);
            InspectionProcedureFileEntity inspectionProcedureFile = XMLUtils.Load<InspectionProcedureFileEntity>(xmlFile);
            return inspectionProcedureFile.InspectionProcedures;
        }
        #endregion Private
    }
}
