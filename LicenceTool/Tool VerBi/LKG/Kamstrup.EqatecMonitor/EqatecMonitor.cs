using System;
using EQATEC.Analytics.Monitor;

namespace Kamstrup.EqatecMonitor
{
  public class EqatecMonitor
  {
    private static bool  _loggingDeactivated = true;
    private static IAnalyticsMonitor _monitor;
    public static EqatecMonitorConstants.MonitorApplication InternalMonitorApplication = EqatecMonitorConstants.MonitorApplication.PcBase;

    #region Properties
    private static IAnalyticsMonitor AnalyticsMonitorInstance
    {
      get
      {
        if(_monitor == null)
          InitializeMonitor(EqatecMonitorConstants.MonitorApplication.PcBase);

        return _monitor;
      }
    }

    private static string _appversion = "0.0.0.0";
    public static string ApplicationVersion
    {
      set
      {
        _appversion = value;
        InitializeMonitor(InternalMonitorApplication, _appversion, _utilityDesc, _computername);
      }
    }

    private static string _utilityDesc = string.Empty;
    public static string UtilityDesc
    {
      set
      {
        _utilityDesc = value;
        InitializeMonitor(InternalMonitorApplication, _appversion, _utilityDesc, _computername);
      }
    }

    private static string _computername = string.Empty;
    public static string ComputerName
    {
      set
      {
        _computername = value;
        InitializeMonitor(InternalMonitorApplication, _appversion, _utilityDesc, _computername);
      }
    }
    #endregion

    #region Private
    protected EqatecMonitor() { }

    #endregion

    #region Public

    public static bool DeactivateLogging
    {
      set
      {
        _loggingDeactivated = value;
      }
    }

    public static void InitializeMonitor(EqatecMonitorConstants.MonitorApplication app)
    {
      InitializeMonitor(app, string.Empty, string.Empty, string.Empty);
    }

    /// <summary>
    /// </summary>
    /// <param name="app"></param>
    /// <param name="Version"></param>
    /// <param name="utilityDesc"></param>
    /// <param name="computerName"></param>
    public static void InitializeMonitor(EqatecMonitorConstants.MonitorApplication app, string Version, string utilityDesc, string computerName)
    {
      AnalyticsMonitorSettings settings;
      InternalMonitorApplication = app;
      switch(app)
      {
        case (EqatecMonitorConstants.MonitorApplication.PcBase):
          settings = new AnalyticsMonitorSettings(EqatecMonitorConstants.PcBaseAppId);
          break;
        case (EqatecMonitorConstants.MonitorApplication.PcTermPro):
          settings = new AnalyticsMonitorSettings(EqatecMonitorConstants.PcTermProAppId);
          break;
        case EqatecMonitorConstants.MonitorApplication.LicenseKeyGenerator:
          settings = new AnalyticsMonitorSettings(EqatecMonitorConstants.LicenseKeyGeneratorAppId);
          break;
        default:
          settings = new AnalyticsMonitorSettings(EqatecMonitorConstants.PcBaseAppId);
          break;
      }
      if (!Version.Equals(string.Empty))
        settings.Version = new Version(Version);
      _monitor = AnalyticsMonitorFactory.Create(settings);
      _monitor.SetInstallationInfo(utilityDesc + "; " + computerName);
    }


    public static void Start()
    {
      try
      {
        if(_loggingDeactivated) { return; }

        AnalyticsMonitorInstance.Start();
      }
      catch(Exception x)
      {
        ErrorInEqatec(x);
      }
    }

    public static void Stop()
    {
      try
      {
        if(_loggingDeactivated) { return; }

        AnalyticsMonitorInstance.Stop();
      }
      catch(Exception x)
      {

        ErrorInEqatec(x);
      }
    }

    public static void SendLog(string logMessage)
    {
      try
      {
        if(_loggingDeactivated) { return; }

        if(AnalyticsMonitorInstance != null)
          AnalyticsMonitorInstance.Stop();
      }
      catch(Exception x)
      {
        ErrorInEqatec(x);
      }
    }


    public static void SendFeatureUse(string featureName, string value)
    {
      if(_loggingDeactivated) { return; }

      string version;
#if DEBUGNL || RELEASENL
      version = "Full_NL";
#elif DEBUGMAINTENANCE|| RELEASEMAINTENANCE
      version = "Maintenance";
#else
      version = "Full";
#endif

      AnalyticsMonitorInstance.TrackFeature(version + "_" + featureName + "." + value);
    }

    public static void SendFeatureUseClean(string featureName, string value)
    {
      if(_loggingDeactivated) { return; }
      AnalyticsMonitorInstance.TrackFeature(featureName + "." + value);
    }

    public static void SendException(Exception x)
    {
      if(_loggingDeactivated) { return; }

      try
      {
        AnalyticsMonitorInstance.TrackException(x);
      }
      catch(Exception ex)
      {
        ErrorInEqatec(ex);
      }
    }

    public static void ErrorInEqatec(Exception x)
    {
      try
      {
        AnalyticsMonitorInstance.SendLog("Error in Monitoring: " + x.StackTrace);
      }
      catch(Exception) {/*Do nothing*/}
    }
    #endregion
  }
}
