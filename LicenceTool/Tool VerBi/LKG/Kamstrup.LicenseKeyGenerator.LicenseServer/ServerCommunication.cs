using System;
using Kamstrup.LicenseKeyGenerator.Model;
using QlmLicenseLib ;

namespace Kamstrup.LicenseKeyGenerator.LicenseServer
{
  public class ServerCommunication
  {
     QlmLicense qlm;
    private ServerCommunication()
    {
      try
      {
        qlm = new QlmLicense(); 
        qlm.AdminEncryptionKey = Constants.Constants.AdministrationEncryptionKey;
        qlm.CommunicationEncryptionKey = Constants.Constants.CommunicationEncryptionKey; 
      }
      catch (Exception ex)
      {
        throw new Exception("Error initialising qlm object! I need a geek and an IDE find out why." + Environment.NewLine + ex);
      }
    }

    private static ServerCommunication _instance;
    public static ServerCommunication Instance
    {
      get { return _instance ?? (_instance = new ServerCommunication()); }
    }

    public void DefineProduct(ProductItem product)
    {
      qlm.DefineProduct(product.ProductId,
                        product.ProductName,
                        product.MajorVersion,
                        product.MinorVersion,
                        product.EncryptionKey,
                        product.PersistenceKey
                         );
      qlm.PublicKey = product.PublicKey; 
    }

    public string GenerateActivationKey(FeatureItem item, string urlLicenseWebService, string orderNumber)
    {
      int[] featureSet = ParseFeatureSet(item.FeatureId);
      string response;
      qlm.AdminEncryptionKey = Constants.Constants.AdministrationEncryptionKey;
      qlm.CommunicationEncryptionKey = Constants.Constants.CommunicationEncryptionKey; 
      qlm.CreateActivationKey(urlLicenseWebService, string.Empty, 0, 1, true,"4.0.00", string.Empty,
                               Environment.UserName + ";" + orderNumber, string.Empty, out response);
      ILicenseInfo licenseInfo = new LicenseInfo();
      string message = string.Empty;
      if (qlm.ParseResults(response, ref licenseInfo, ref message))
      {
        return licenseInfo.ActivationKey;
      }

      //This is a major hack. PcImportExport and PcTermPro are treated as the same product in the license server.
      //This is just to hide that fact for the user.
      if (item.ProductDescription.Equals("PcImportExport III"))
        throw new Exception(message.Replace("PcTermPro III", "PcImportExport III"));
      throw new Exception(message);
    }

    public string GenerateActivationKey(FeatureItem item, string urlLicenseWebService)
    {
        int[] featureSet = ParseFeatureSet(item.FeatureId);
        string response;
        qlm.CreateActivationKeyEx(urlLicenseWebService, string.Empty, featureSet, 1, true, Constants.Constants.QlmVersion, string.Empty,
                                 Environment.UserName + ";" + item.ProductId.ToString(), string.Empty, out response);
        ILicenseInfo licenseInfo = new LicenseInfo();
        string message = string.Empty;
        if (qlm.ParseResults(response, ref licenseInfo, ref message))
        {
            return licenseInfo.ActivationKey;
        }

        //This is a major hack. PcImportExport and PcTermPro are treated as the same product in the license server.
        //This is just to hide that fact for the user.
        if (item.ProductDescription.Equals("PcImportExport III"))
            throw new Exception(message.Replace("PcTermPro III", "PcImportExport III"));
        throw new Exception(message);
    }

    public string GenerateComputerKey(string activationKey, string computerID, string computerName, string urlLicenseWebService)
    {
      string response;
      //user data must(!) be an empty string here
      qlm.ActivateLicenseForUser(urlLicenseWebService, activationKey, string.Empty, computerID, computerName,
                                  Constants.Constants.QlmVersion, string.Empty, string.Empty, out response);

      ILicenseInfo licenseInfo = new LicenseInfo();
      string message = string.Empty;
      if (qlm.ParseResults(response, ref licenseInfo, ref message))
      {
        return licenseInfo.ComputerKey;
      }

      //This is a major hack. PcImportExport and PcTermPro are treated as the same product in the license server.
      //This is just to hide that fact for the user.
      if (message.Contains("PcTermPro III"))
        message.Replace("PcTermPro III", "PcTermPro III/PcImportExport III");
      throw new Exception(message);
    }

    /// <summary>
    /// Creates a non-computer bound license key. You must call DefineProduct prior to calling this function.
    /// </summary>
    /// <param name="numberOfDays"></param>
    /// <param name="features"></param>
    /// <returns></returns>
    public string CreateTrialKey(int numberOfDays, string features)
    {
      DateTime expiryDate = DateTime.Now.Date.AddDays(numberOfDays).AddSeconds(-1);
      return qlm.CreateLicenseKeyEx4(expiryDate, -1, 1, ELicenseType.Evaluation, string.Empty, ParseFeatureSet(features));
    }

    /// <summary>
    /// </summary>
    /// <param name="orderNo">Order number to look for. Else null or string.Empty</param>
    /// <param name="activationKey">Activation key to look for. Else null or string.Empty</param>
    /// <param name="computerName"></param>
    /// <param name="computerKey"></param>
    /// <param name="urlLicenseWebService"></param>
    /// <returns>Returns the data set as an xml responce</returns>
    public string FindKeys(string orderNo, string activationKey, string computerName, string computerKey, string urlLicenseWebService)
    {
      string filter = string.Empty;
      if (!string.IsNullOrEmpty(orderNo))
        filter = string.Format("UserData1 LIKE '%{0}%'", orderNo);
      if (!string.IsNullOrEmpty(activationKey))
      {
        if (!filter.Equals(string.Empty))
          filter += " AND ";
        filter += string.Format("ActivationKey LIKE '%{0}%'", activationKey);
      }
      if (!string.IsNullOrEmpty(computerName))
      {
        if (!filter.Equals(string.Empty))
          filter += " AND ";
        filter += string.Format("ComputerName LIKE '%{0}%'", computerName);
      }
      if (!string.IsNullOrEmpty(computerKey))
      {
        if (!filter.Equals(string.Empty))
          filter += " AND ";
        filter += string.Format("ComputerKey LIKE '%{0}%'", computerKey);
      }

      string dataset = string.Empty;
      string response;
      qlm.GetDataSet(urlLicenseWebService, filter, ref dataset, out response);

      return dataset;
    }

    /// <summary>
    /// Releases the computer id lock on the given license
    /// </summary>
    /// <param name="urlLicenseWebService"></param>
    /// <param name="activationKey"></param>
    /// <param name="computerId"></param>
    public void ReleaseLicense(string urlLicenseWebService, string activationKey, string computerId)
    {
      string response;
      qlm.ReleaseLicense(urlLicenseWebService, activationKey, computerId, out response);

      ILicenseInfo licenseInfo = new LicenseInfo();
      string message = string.Empty;
      qlm.ParseResults(response, ref licenseInfo, ref message);
    }

    public bool ValidateKey(string activationKey)
    {
      string response = qlm.ValidateLicense(activationKey);
      return !response.Contains("invalid");
    }

    private int[] ParseFeatureSet(string featureId)
    {
      int[] featureSets = new int[4];
      string[] pairs = featureId.Split(',');
      foreach (string pair in pairs)
      {
        string[] split = pair.Split(':');
        short set = short.Parse(split[0]);
        short id = short.Parse(split[1]);
        featureSets[set] = id;
      }
      return featureSets;
    }
  }
}
