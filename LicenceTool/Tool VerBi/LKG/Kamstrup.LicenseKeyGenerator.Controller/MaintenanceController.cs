using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Kamstrup.LicenseKeyGenerator.LicenseServer;
using Kamstrup.LicenseKeyGenerator.Model;

namespace Kamstrup.LicenseKeyGenerator.Controller
{
  public static class MaintenanceController
  {
    public static List<License> FindKeys(string orderNo, string licenseKey, string computerName, string computerKey)
    {
      return
        datasetMapper(ServerCommunication.Instance.FindKeys(orderNo, licenseKey, computerName, computerKey,
                                                            UrlLicenseWebService));
    }

    public static void DeleteLicense(string activationKey, string computerID, string orderNo, string comment)
    {
      comment = cleanComment(comment);
      DatabaseCommunication.LogDeletion(activationKey, computerID, orderNo, comment, connectionString);
      DatabaseCommunication.DeleteLicense(activationKey, computerID, connectionString);
    }

    public static void ReleaseLicense(string activationKey, string computerID, string orderNo, string comment)
    {
      comment = cleanComment(comment);
      DatabaseCommunication.LogReleaseLicense(activationKey, computerID, orderNo, comment,
                                              connectionString);
      ServerCommunication.Instance.ReleaseLicense(UrlLicenseWebService, activationKey, computerID);
    }

    public static string GetLog()
    {
      return DatabaseCommunication.GetLog(connectionString);
    }

    #region properties

    private static string url;

    private static string db;

    private static string UrlLicenseWebService
    {
      get
      {
        if (string.IsNullOrEmpty(url))
          url = SettingsController.GetConfigFileSettings().licenseServerUrl;
        return url;
      }
    }

    private static string connectionString
    {
      get
      {
        if (string.IsNullOrEmpty(db))
          db = SettingsController.GetConfigFileSettings().licenseServerDatabase;
        return db;
      }
    }

    public static string CreateTrialKey(ProductItem productItem, int numberOfDays, string orderNo)
    {
      const int pcBaseId = 3, pcBaseWaterId = 17;
      string feature = "0:0,1:0,2:0,3:0";
      switch (productItem.ProductId)
      {
        case pcBaseId:
          {
            CatalogItemLookupCollection list = CatalogItemController.Instance.GetFeaturesForProducts(productItem);
            feature =
              (from item in list where item.Feature.FeatureId == "0:128,1:0,2:0,3:0" select item.Feature.FeatureId).
                FirstOrDefault();
          }
          break;
        case pcBaseWaterId:
          {
            CatalogItemLookupCollection list = CatalogItemController.Instance.GetFeaturesForProducts(productItem);
            feature =
              (from item in list where item.Feature.FeatureId == "0:2,1:0,2:0,3:0" select item.Feature.FeatureId).
                FirstOrDefault();
          }
          break;
      }

      string key = ServerCommunication.Instance.CreateTrialKey(numberOfDays, feature);
      DatabaseCommunication.LogTrialKeyGeneration(key, orderNo, numberOfDays, productItem.ProductName, connectionString);
      return key;
    }

    #endregion

    #region private helper methods

    private static string cleanComment(string comment)
    {
      comment = comment.Replace(Environment.NewLine, " ");
      comment = comment.Replace("\n", " ");
      comment = comment.Replace("\t", " ");
      comment = comment.Replace("'", "");
      return comment;
    }

    private static List<License> datasetMapper(string dataSet)
    {
      List<License> licenseList = new List<License>();
      XmlDocument doc = new XmlDocument();
      doc.Load(new StringReader(dataSet));
      XmlNodeList nodeList = doc.SelectNodes("NewDataSet");
      if (nodeList != null)
        foreach (XmlNode newDataSetNode in nodeList)
        {
          if (newDataSetNode.HasChildNodes)
          {
            foreach (XmlNode tableNode in newDataSetNode.ChildNodes)
            {
              if (tableNode.Name.Equals("Table"))
              {
                License license = new License();
                foreach (XmlNode node in tableNode.ChildNodes)
                {
                  if (node.Name.Equals("ActivationKey"))
                    license.ActivationKey = node.InnerText;
                  if (node.Name.Equals("ComputerKey"))
                    license.ComputerKey = node.InnerText;
                  if (node.Name.Equals("ComputerName"))
                    license.ComputerName = node.InnerText;
                  if (node.Name.Equals("ReleaseCount"))
                    license.ReleaseCount = node.InnerText;
                  if (node.Name.Equals("ComputerID"))
                    license.ComputerID = node.InnerText;
                  if (node.Name.Equals("UserData1"))
                  {
                    license.UserData = node.InnerText;
                    if (node.InnerText.Contains(";"))
                      license.OrderNo = node.InnerText.Split(';')[1];
                  }
                  if (node.Name.Equals("ProductID"))
                  {
                    int id;
                    int.TryParse(node.InnerText, out id);
                    license.ProductName = ProductController.Instance.GetSpecificProduct(id).ProductName;
                  }
                }
                licenseList.Add(license);
              }
            }
          }
        }
      return licenseList;
    }

    #endregion
  }
}