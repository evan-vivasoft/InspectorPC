namespace Kamstrup.EqatecMonitor
{
  public static class EqatecMonitorConstants
  {
    #region MonitorApplication enum

    public enum MonitorApplication
    {
      PcBase,
      PcTermPro,
      LicenseKeyGenerator
    }

    #endregion

    public const string PcTermPro_ProgramName = "PcTermProIII";
    public const string PcBase_ProgramName = "PcBaseIII";

    public const string PcBaseAppId = "AAFE7F0821A7419A87687C6301D7ACB8";
    public const string PcTermProAppId = "DAC5999392944323A9166BAFF7B5CB4A";
    public const string LicenseKeyGeneratorAppId = "45745401AA564AB3938E394A12038632";
    
    public static string LKG_Version = "1.4.0";
  }
}