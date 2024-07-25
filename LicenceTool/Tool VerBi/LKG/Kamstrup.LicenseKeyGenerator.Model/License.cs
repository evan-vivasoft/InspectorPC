namespace Kamstrup.LicenseKeyGenerator.Model
{
  public class License
  {
    public string ActivationKey { get; set; }
    public string ComputerKey { get; set; }
    public string ComputerName { get; set; }
    public string ComputerID { get; set; }

    /// <summary>
    /// Contains user name of who created the license + order number
    /// <para>UserName;OrderNo</para>
    /// </summary>
    public string UserData { get; set; }

    public string OrderNo { get; set; }

    /// <summary>
    /// Number of times this license has been released i.e. moved from old computer to a new one
    /// </summary>
    public string ReleaseCount { get; set; }

    public string ProductName { get; set; }
  }
}
