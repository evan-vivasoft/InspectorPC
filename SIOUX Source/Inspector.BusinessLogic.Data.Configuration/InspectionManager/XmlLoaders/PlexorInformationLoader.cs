/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Plexor;
using Inspector.Infra.Utils;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders
{
    /// <summary>
    /// PlexorInformation
    /// </summary>
    internal class PlexorInformationLoader
    {
        #region Constants
        private const string XML_FILE = "Plexor.xml";
        private const string XSD_FILE = "PLEXOR.xsd";
        #endregion Constants

        #region Members
        private List<PlexorEntity> m_Plexors;
        private string m_XmlFilePath;
        private string m_XsdFilePath;
        #endregion Members

        #region Properties
        /// <summary>
        /// Gets or sets the plexors.
        /// </summary>
        /// <value>The plexors.</value>
        public IList<PlexorEntity> Plexors
        {
            private set
            {
                m_Plexors = new List<PlexorEntity>(value);
            }
            get
            {
                if (m_Plexors == null)
                {
                    m_Plexors = new List<PlexorEntity>();
                }
                return m_Plexors;
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PlexorInformationLoader"/> class.
        /// </summary>
        public PlexorInformationLoader()
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
        internal PlexorInformationLoader(string xml)
        {
            SetFilePaths();
            Plexors = ReadPlexorInformation(xml);
        }
        #endregion Constructors

        #region Public
        /// <summary>
        /// Reloads the plexor information from the XML file.
        /// </summary>
        public void Reload()
        {
            Plexors = ReadPlexorInformation(m_XmlFilePath, m_XsdFilePath);
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
        /// <returns></returns>
        private IList<PlexorEntity> ReadPlexorInformation(string xml)
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

            return ReadPlexorInformation(tempXMLFileName, m_XsdFilePath);
        }

        /// <summary>
        /// Reads the plexor information.
        /// </summary>
        /// <param name="xmlFile">The XML file.</param>
        /// <param name="xsdFile">The XSD file.</param>
        /// <returns>List of Plexor information</returns>
        private static IList<PlexorEntity> ReadPlexorInformation(string xmlFile, string xsdFile)
        {
            XMLUtils.ValidateXmlFile(xmlFile, xsdFile);
            PlexorsEntity plexorsEntity = XMLUtils.Load<PlexorsEntity>(xmlFile);
            return plexorsEntity.Plexors;
        }
        #endregion Private
    }
}
