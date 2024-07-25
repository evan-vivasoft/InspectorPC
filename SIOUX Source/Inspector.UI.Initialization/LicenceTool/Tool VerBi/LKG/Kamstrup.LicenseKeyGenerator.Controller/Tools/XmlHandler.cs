using System;
using System.Collections.Generic;
using System.Xml;
using Kamstrup.LicenseKeyGenerator.Model;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32.SafeHandles;

namespace Kamstrup.LicenseKeyGenerator.Controller.Tools
{
  public class XmlHandler
  {
    private static readonly string pathConfigFile = AppDomain.CurrentDomain.BaseDirectory + "ConfigFile.xml";

    public static CatalogItemLookupCollection GetStoredItems()
    {
      FileStream fs = null;
      try
      {
        string pathCatalogItems = Path.Combine(SettingsController.GetConfigFileSettings().networkPath, SettingsController.GetConfigFileSettings().fileNameCatalogItems);
        fs = new FileStream(pathCatalogItems, FileMode.OpenOrCreate);

        XmlSerializer xs = new XmlSerializer(typeof(CatalogItemLookupCollection));

        return (CatalogItemLookupCollection)xs.Deserialize(fs);
      }
      finally
      {
        if (fs != null) fs.Close();
      }
    }

    public static ConfigFileSettings GetConfigFileSettings()
    {
      FileStream fs = null;
      try
      {
        fs = new FileStream(pathConfigFile, FileMode.OpenOrCreate);

        XmlSerializer xs = new XmlSerializer(typeof(ConfigFileSettings));

        return (ConfigFileSettings)xs.Deserialize(fs);
      }
      finally
      {
        if (fs != null) fs.Close();
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="editationDate">Last time a catalog item was added</param>
    public static void StoreItems(CatalogItemLookupCollection collection, DateTime editationDate)
    {
      string pathCatalogItems = Path.Combine(SettingsController.GetConfigFileSettings().networkPath, SettingsController.GetConfigFileSettings().fileNameCatalogItems);
      if (!File.Exists(pathCatalogItems) || File.GetLastWriteTime(pathCatalogItems) < editationDate)
      {
        FileStream fs = null;
        try
        {
          fs = new FileStream(pathCatalogItems, FileMode.Create);
          XmlSerializer xs = new XmlSerializer(typeof(CatalogItemLookupCollection));
          xs.Serialize(fs, collection);
        }
        finally
        {
          if (fs != null) fs.Close();
        }
      }
    }

    /// <summary>
    /// Saves the settings if the settings file does not exist
    /// </summary>
    /// <param name="config"></param>
    /// <param name="checkForExistingFile"></param>
    public static void StoreSettings(ConfigFileSettings config, bool checkForExistingFile)
    {
      if (checkForExistingFile)
      {
        if (!File.Exists(pathConfigFile))
        {
          using (FileStream fs = new FileStream(pathConfigFile, FileMode.Create))
          {
            XmlSerializer xs = new XmlSerializer(typeof(ConfigFileSettings));
            xs.Serialize(fs, config);
          }
        }
      }
      else
      {
        using (FileStream fs = new FileStream(pathConfigFile, FileMode.Create))
        {
          XmlSerializer xs = new XmlSerializer(typeof(ConfigFileSettings));
          xs.Serialize(fs, config);
        }
      }
    }

    public static List<ProductItem> GetStoredProducts()
    {
      List<ProductItem> productItems = new List<ProductItem>();

      string pathLicenseProducts = Path.Combine(SettingsController.GetConfigFileSettings().networkPath, SettingsController.GetConfigFileSettings().fileNameProducts);
      XmlTextReader reader = new XmlTextReader(pathLicenseProducts);
      
      reader.WhitespaceHandling = WhitespaceHandling.None;
      
      XmlDocument xmlDoc = new XmlDocument();

      //Load the file into the XmlDocument
      xmlDoc.Load(reader);
      //Close off the connection to the file.

      reader.Close();

      XmlNodeList memberNodes = xmlDoc.SelectNodes("//PRODUCT");

      foreach (XmlNode node in memberNodes)
      {
        if (node.Attributes != null)
        {
          ProductItem productItem = new ProductItem
                                      {
                                        ProductId = Convert.ToInt32(node.Attributes["ID"].Value),
                                        ProductName = node.Attributes["Name"].Value,
                                        EncryptionKey = node.Attributes["Key"].Value,
                                        MajorVersion = Convert.ToInt32(node.Attributes["Major"].Value),
                                        MinorVersion = Convert.ToInt32(node.Attributes["Minor"].Value),
                                        PersistenceKey = node.Attributes["GUID"].Value,
                                        PublicKey = node.Attributes["PublicKey"].Value,
                                      };
          productItems.Add(productItem);
        }
      }

      return productItems;
    }
  }
}
