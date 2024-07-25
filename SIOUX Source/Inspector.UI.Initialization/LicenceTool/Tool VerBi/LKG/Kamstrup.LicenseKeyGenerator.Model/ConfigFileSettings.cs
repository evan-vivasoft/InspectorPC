using System;

namespace Kamstrup.LicenseKeyGenerator.Model
{
  public class ConfigFileSettings
  {
    public string path { get; set; }

    public string licenseServerUrl { get; set; }

    public string licenseServerDatabase { get; set; }

    public string networkPath { get; set; }

    public string fileNameCatalogItems { get; set; }

    public string fileNameProducts { get; set; }
  }
}
