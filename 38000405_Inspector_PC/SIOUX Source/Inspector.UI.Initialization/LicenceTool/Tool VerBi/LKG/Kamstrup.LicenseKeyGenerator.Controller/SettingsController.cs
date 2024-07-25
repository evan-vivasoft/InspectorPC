using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Kamstrup.LicenseKeyGenerator.Controller.Tools;
using Kamstrup.LicenseKeyGenerator.Model;

namespace Kamstrup.LicenseKeyGenerator.Controller
{
  public class SettingsController
  {
#if DEBUGNL ||RELEASENL
    private const string fileNameProducts = "products_NL.xml";
    private const string fileNameCatalogItems = "CatalogItems_NL.xml";
#else
      private const string fileNameProducts = "products_NL.xml";
      private const string fileNameCatalogItems = "CatalogItems_NL.xml";
#endif
    private const string networkPath = @"\\kamstrup_bv\Data\KAMSTRUP\Licenses\LKGenerator\";
    private const string licenseServerUrl = "http://quicklicensemanager.com/kamstrup/qlm/qlmservice.asmx";
    private const string licenseServerDB = @"Data Source=172.31.1.19\ETOOLS;Initial Catalog=qlm;User ID={0};Password={1}";
    private const string user = "qlm";
    private const string password = "qlmweb30";
    private const string testLicenseServerUrl = "http://swtestserver38/qlm/qlmservice.asmx";
    private const string testLicenseServerDB = @"Data Source=swtestserver38\TESTSERVER1;Initial Catalog=qlm;User ID={0};Password={1}";    

    public static ConfigFileSettings GetConfigFileSettings()
    {
      ConfigFileSettings config = XmlHandler.GetConfigFileSettings();

      if (!string.IsNullOrEmpty(config.path) && !config.path.EndsWith("\\"))
      {
        config.path += "\\";
      }

      if (string.IsNullOrEmpty(config.fileNameProducts))
      {
        config.fileNameProducts = fileNameProducts;
        XmlHandler.StoreSettings(config, false);
      }

      if (string.IsNullOrEmpty(config.fileNameCatalogItems))
      {
        config.fileNameCatalogItems = fileNameCatalogItems;
        XmlHandler.StoreSettings(config, false);
      }

      if (string.IsNullOrEmpty(config.networkPath))
      {
        config.networkPath = networkPath;
        XmlHandler.StoreSettings(config, false);
      }

      if (string.IsNullOrEmpty(config.licenseServerUrl))
      {
        config.licenseServerUrl = licenseServerUrl;
        XmlHandler.StoreSettings(config, false);
      }

      if (string.IsNullOrEmpty(config.licenseServerDatabase))
      {
        config.licenseServerDatabase = licenseServerDB;
        XmlHandler.StoreSettings(config, false);
      }

//#if DEBUG || DEBUGNL || DEBUGMAINTENANCE
//      config.licenseServerUrl = testLicenseServerUrl;
//      config.licenseServerDatabase = testLicenseServerDB;
//      config.networkPath = Path.Combine(networkPath, "Test");
//#endif

      config.licenseServerDatabase = string.Format(config.licenseServerDatabase, user, password);

      return config;
    }

    public static void SaveBaseSettings()
    {
      ConfigFileSettings config = new ConfigFileSettings
                                    {
                                      path = string.Empty,
                                      licenseServerUrl = licenseServerUrl,
                                      licenseServerDatabase = licenseServerDB,
                                      networkPath = networkPath,
                                      fileNameCatalogItems = fileNameCatalogItems,
                                      fileNameProducts = fileNameProducts,
                                    };
#if DEBUG || DEBUGNL || DEBUGMAINTENANCE
      config.licenseServerUrl = testLicenseServerUrl;
      config.licenseServerDatabase = testLicenseServerDB;
      config.networkPath = Path.Combine(networkPath, "Test");
#endif
      XmlHandler.StoreSettings(config, true);
    }

    public static void SetLicenseFile()
    {
      const string licenseFileName = "IsLicense40.dll";
      const string dll_32 = "IsLicense40_32.dll";
      const string dll_64 = "IsLicense40_64.dll";

      if (Is64Bit)
      {
        if (Is64Bit)
        {
          if (File.Exists(dll_64))
          {
            if (File.Exists(licenseFileName))
              File.Delete(licenseFileName);

            File.Copy(dll_64, licenseFileName);
          }
        }
      }
      else //Default 32 bit
      {
        if (File.Exists(dll_32))
        {
          if (File.Exists(licenseFileName))
            File.Delete(licenseFileName);

          File.Copy(dll_32, licenseFileName);
        }
      }
    }

    /*
     * Borrowed from:
     * http://stackoverflow.com/questions/336633/how-to-detect-windows-64-bit-platform-with-net
     */
    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

    private static bool Is64Bit
    {
      get { return IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()); }
    }

    private static bool Is32BitProcessOn64BitProcessor()
    {
      bool retVal;

      IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);

      return retVal;
    }
  }
}

