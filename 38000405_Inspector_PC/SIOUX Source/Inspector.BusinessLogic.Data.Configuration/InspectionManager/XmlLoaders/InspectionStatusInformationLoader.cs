/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.InspectionProcedureStatus;
using Inspector.Infra.Utils;
using Inspector.Model.InspectionProcedure;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders
{
    /// <summary>
    /// Information of inspection statuses
    /// </summary>
    internal class InspectionStatusInformationLoader
    {
        #region Constants
        private const string XML_FILE = "InspectionStatus.xml";
        private const string XSD_FILE = "InspectionStatus.xsd";
        #endregion Constants

        #region Members
        private InspectionStatusesEntity m_InspectionStatusesEntity = new InspectionStatusesEntity();
        private string m_XmlFilePath;
        private string m_XsdFilePath;
        #endregion Members

        #region Properties
        /// <summary>
        /// Gets the inspection statuses.
        /// </summary>
        /// <value>The inspection statuses.</value>
        public IList<InspectionStatusEntity> InspectionStatuses
        {
            private set
            {
                m_InspectionStatusesEntity.InspectionStatuses = new List<InspectionStatusEntity>(value);
            }
            get
            {
                if (m_InspectionStatusesEntity.InspectionStatuses == null)
                {
                    m_InspectionStatusesEntity.InspectionStatuses = new List<InspectionStatusEntity>();
                }
                return m_InspectionStatusesEntity.InspectionStatuses;
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStatusInformationLoader"/> class.
        /// </summary>
        public InspectionStatusInformationLoader()
        {
            SetFilePaths();
            Reload();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionStatusInformationLoader"/> class.
        /// For unit testing.
        /// </summary>
        internal InspectionStatusInformationLoader(string xml)
        {
            SetFilePaths();
            InspectionStatuses = ReadInspectionStatuses(xml);
        }
        #endregion Constructors

        #region Public
        /// <summary>
        /// Reloads the inspection statuses information from the XML file.
        /// </summary>
        public void Reload()
        {
            InspectionStatuses = ReadInspectionStatuses(m_XmlFilePath, m_XsdFilePath);
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
        /// Reads the inpection statuses.
        /// </summary>
        /// <param name="xmlFile">The XML file.</param>
        /// <param name="xsdFile">The XSD file.</param>
        /// <returns>List of inspection status</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static IList<InspectionStatusEntity> ReadInspectionStatuses(string xmlFile, string xsdFile)
        {
            InspectionStatusesEntity inspectionStatusesEntity;
            try
            {
                XMLUtils.ValidateXmlFile(xmlFile, xsdFile);
                inspectionStatusesEntity = XMLUtils.Load<InspectionStatusesEntity>(xmlFile);
            }
            catch
            {
                inspectionStatusesEntity = new InspectionStatusesEntity();
                inspectionStatusesEntity.InspectionStatuses = new List<InspectionStatusEntity>();
            }
            return inspectionStatusesEntity.InspectionStatuses;
        }

        /// <summary>
        /// Reads the inspection statuses.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>List of inspection status</returns>
        private IList<InspectionStatusEntity> ReadInspectionStatuses(string xml)
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

            return ReadInspectionStatuses(tempXMLFileName, m_XsdFilePath);
        }
        #endregion Private

        #region Internal
        /// <summary>
        /// Save the inspection status to XML file, note that the statuses 'No Inspection' and 'No Inspection Found' are not saved to disk.
        /// </summary>
        internal void Save()
        {
            List<InspectionStatusEntity> originalList = m_InspectionStatusesEntity.InspectionStatuses;
            try
            {
                Func<InspectionStatusEntity, bool> criteria =
                    inspectionStatus => inspectionStatus.StatusID != InspectionStatus.NoInspection &&
                                        inspectionStatus.StatusID != InspectionStatus.NoInspectionFound;
                IEnumerable<InspectionStatusEntity> filteredList = m_InspectionStatusesEntity.InspectionStatuses.Where(criteria);
                m_InspectionStatusesEntity.InspectionStatuses = new List<InspectionStatusEntity>(filteredList);
                XMLUtils.AtomicSave<InspectionStatusesEntity>(m_InspectionStatusesEntity, m_XmlFilePath);
            }
            finally
            {
                m_InspectionStatusesEntity.InspectionStatuses = originalList;
            }
        }
        #endregion Internal
    }
}
