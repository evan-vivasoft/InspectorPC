using System;
using System.Windows.Forms;
using Kamstrup.LicenseKeyGenerator.Controller;

namespace Kamstrup.LicenseKeyGenerator.GUI
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        EqatecMonitor.EqatecMonitor.InitializeMonitor(EqatecMonitor.EqatecMonitorConstants.MonitorApplication.LicenseKeyGenerator, EqatecMonitor.EqatecMonitorConstants.LKG_Version, Environment.UserName, Environment.MachineName);
#if DEBUG || DEBUGNL || DEBUGMAINTENANCE
        EqatecMonitor.EqatecMonitor.DeactivateLogging = true;
#else
        EqatecMonitor.EqatecMonitor.DeactivateLogging = false;
#endif
        EqatecMonitor.EqatecMonitor.Start();

        SettingsController.SaveBaseSettings();
        SettingsController.SetLicenseFile();
        CatalogItemController.Instance.GetCatalogItems();
        ProductController.Instance.GetProducts();
        if(args.Length > 0 && args.Length == 1)
        {
         // Console.ConsoleApp.Main(args);
        }
        else
        {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          Application.Run(new frmKeyGenerator());
        }
        EqatecMonitor.EqatecMonitor.Stop();
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
        Environment.ExitCode = - 1;
        EqatecMonitor.EqatecMonitor.SendException(ex);
        EqatecMonitor.EqatecMonitor.Stop();
      }
    }
  }
}
