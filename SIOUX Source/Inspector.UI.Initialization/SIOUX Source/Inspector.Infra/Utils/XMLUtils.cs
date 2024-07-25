using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using log4net;

namespace Inspector.Infra.Utils
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1052:StaticHolderTypesShouldBeSealed")]
    public class XMLUtils
    {
        #region properties
        private static readonly ILog cLogger = LogManager.GetLogger(typeof(XMLUtils).FullName);
        #endregion

        /// <summary>
        /// Prevents a default instance of the <see cref="XMLUtils"/> class from being created.
        /// </summary>
        private XMLUtils()
        {

        }

        /// <summary>
        /// Validates the XML file.
        /// </summary>
        /// <param name="xmlFile">The XML file.</param>
        /// <param name="xsdFile">The XSD file.</param>
        /// <exception cref="ReportControlException">Thrown when XMLto XSD validation fails</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void ValidateXmlFile(string xmlFile, string xsdFile)
        {
            //LogHelper.Log(cLogger, LogHelper.LogLevel.Info, string.Format(CultureInfo.InvariantCulture, "Validating file: {0} to XSD", xmlFile));

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, xsdFile);
                settings.ValidationType = ValidationType.Schema;
                XmlDocument document = new XmlDocument();
                document.Load(xmlFile);

                using (StringReader stringReader = new StringReader(document.InnerXml))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
                    {
                        while (xmlReader.Read()) { }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Failed to validate XML File: {0} to XSD file: {1}. Exception: {2}", xmlFile, xsdFile, ex.Message), ex);
            }
        }

        /// <summary>
        /// Save an object serialized to a file on the harddisk
        /// </summary>
        /// <param name="path">string path to store the serialized object</param>
        /// <param name="obj">object to serialize</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void Save(string path, Object obj)
        {
            Type type = obj.GetType();
            XmlSerializer xmlserializer = new XmlSerializer(type);

            bool containsDirectory = (path.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) > -1);
            if (containsDirectory)
            {
                string directoryPath = path.Substring(0, path.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase));

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            using (Stream fileStream = new FileStream(path, FileMode.Create))
            {
                using (XmlWriter xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8))
                {
                    xmlserializer.Serialize(xmlWriter, obj);
                }
            }
        }

        /// <summary>
        /// Saves the file atomical by using Move operation, this ensures that the file is always written completely.
        /// Not supported by all filesystems such as FAT32, works on NTFS.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void AtomicSave<T>(T objectToSerialize, string path) where T : class
        {
            string tempFileName = Path.GetTempFileName();
            Save(tempFileName, objectToSerialize);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Move(tempFileName, path); // atomic action on NTFS

            try
            {
                File.Delete(tempFileName);
            }
            catch { /* failed deletion of temporary files can safely be ignored */ }
        }

        /// <summary>
        /// Save an object serialized to a file on the harddisk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">string path to store the serialized object T</param>
        public static void Save<T>(T objectToSerialize, string path) where T : class
        {
            Save(path, objectToSerialize);
        }

        /// <summary>
        /// Save an object serialized to memory stream
        /// </summary>        
        /// <param name="obj">object to serialize</param>
        /// <returns>Stream with serialized xml</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static Stream Save(Object obj)
        {
            Type type = obj.GetType();
            XmlSerializer xmlserializer = new XmlSerializer(type);

            using (Stream memoryStream = new MemoryStream())
            {
                using (XmlWriter xmlWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {
                    xmlserializer.Serialize(xmlWriter, obj);
                }

                return memoryStream;
            }
        }

        /// <summary>
        /// Load a serialized object from a file on harddisk
        /// </summary>
        /// <param name="path">string path to file</param>
        /// <param name="type">typeof object to desirialize</param>
        /// <returns>desirialized object</returns>
        public static object Load(string path, Type type)
        {
            object obj = null;
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    obj = serializer.Deserialize(fs);
                }
            }

            return obj;
        }

        /// <summary>
        /// Load a serialized object T from a file on harddisk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">string path to file</param>
        /// <returns>desirialized object T</returns>
        public static T Load<T>(string path)
        {
            return (T)Load(path, typeof(T));
        }
    }
}
