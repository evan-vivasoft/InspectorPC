using System;
using System.Collections.Generic;
using System.Linq;
using Kamstrup.LicenseKeyGenerator.Controller.Tools;
using Kamstrup.LicenseKeyGenerator.Model;
using System.Security.Cryptography;
using System.IO;
using Kamstrup.LicenseKeyGenerator.LicenseServer;

namespace Kamstrup.LicenseKeyGenerator.Controller
{
  public class CatalogItemController
  {
    private static readonly DateTime editationDate = DateTime.Parse("08-07-2011 00:00:00");//dd-mm-yyyy HH:MM:SS

    #region singleton
    private CatalogItemController(){ }
    private static CatalogItemController _instance;
    public static CatalogItemController Instance
    {
      get { return _instance ?? (_instance = new CatalogItemController()); }
    }
    #endregion

    public List<CatalogItem> GetCatalogItems()
    {
      List<CatalogItem> results = new List<CatalogItem>();
      CatalogItemLookupCollection collection = XmlHandler.GetStoredItems();

      foreach (CatalogItemLookupObject lookupObject in collection)
      {
        CatalogItemLookupObject o = lookupObject;
        IEnumerable<CatalogItem> res = results.Where(r => r.ProductDescription == o.Catalog.ProductDescription);
        if (res.Count() == 0)
          results.Add(lookupObject.Catalog);
      }

      return results;
    }

    public CatalogItemLookupObject LookupStoredItem(string catalogNumber)
    {
      CatalogItemLookupCollection collection = XmlHandler.GetStoredItems();

      var result = from c in collection
                   where c.Feature.CatalogNumbers.Contains(catalogNumber)
                   select c;

      return result.FirstOrDefault();
    }

    public CatalogItemLookupCollection GetFeaturesForProducts(ProductItem product)
    {
      CatalogItemLookupCollection results = new CatalogItemLookupCollection();
      CatalogItemLookupCollection collection = XmlHandler.GetStoredItems();
      results.AddRange(collection.Where(item => item.Catalog.ProductId == product.ProductId));
      return results;
    }


    public void DefineProduct(ProductItem product)
    {
      ServerCommunication.Instance.DefineProduct(product);
    }

    /// <summary>
    /// When using debug license key will be generated on the test server
    /// </summary>
    /// <param name="featureItem">FeatureItem featureItem</param>
    /// <param name="orderNumber">Order number to store with the activation key</param>
    /// <returns>Returns the license key</returns>
    public string GenerateActivationKey(FeatureItem featureItem, string orderNumber)
    {
      return ServerCommunication.Instance.GenerateActivationKey(featureItem, SettingsController.GetConfigFileSettings().licenseServerUrl, orderNumber);
    }

    public string GenerateActivationKey(FeatureItem featureItem)
    {
        return ServerCommunication.Instance.GenerateActivationKey(featureItem, SettingsController.GetConfigFileSettings().licenseServerUrl);
    }

    public void ReadLicenseFile(string path, out string activationKey, out string computerID, out string computerName)
    {
      DESCryptoServiceProvider DES = new DESCryptoServiceProvider { Key = LicenseServer.Constants.Constants.FileEncryptionKey, IV = LicenseServer.Constants.Constants.FileEncryptionKey };
      //Create crypto stream set to read and do a DES decryption transform on incoming bytes.
      FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

      CryptoStream cryptostreamDecr = new CryptoStream(file, DES.CreateDecryptor(), CryptoStreamMode.Read);

      StreamReader reader = new StreamReader(cryptostreamDecr);
      computerID = reader.ReadLine();
      computerName = reader.ReadLine();
      activationKey = reader.ReadLine();

      reader.Close();
      cryptostreamDecr.Close();
    }

    public string GenerateComputerKey(string activationKey, string computerID, string computerName)
    {
      if(!ServerCommunication.Instance.ValidateKey(activationKey))
      {
        throw new Exception("The activation key could not be validated");
      }
      return ServerCommunication.Instance.GenerateComputerKey(activationKey, computerID, computerName, SettingsController.GetConfigFileSettings().licenseServerUrl);
    }

    public void saveFile(string key)
    {
        string path = SettingsController.GetConfigFileSettings().path;

        //if no path is given don't save the activation key
        if (string.IsNullOrEmpty(path))
            return;

        //Append file name and save
        string filePath = Path.Combine(path, "Activation key.txt");

        saveFile(key, filePath);
    }

    public void saveFile(string key, string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        TextWriter writer = File.CreateText(filePath);
        writer.WriteLine(key);
        writer.Close();
    }
  }
}

