using System;

namespace Kamstrup.LicenseKeyGenerator.Model
{
  [Serializable]
  public class ProductItem
  {
    //public enum product { PcBase = 1, PcTermPro = 2, PcBaseIII_Loggerlicense = 3, PcBaseIII_AutomaticExport = 4, PcImportExport = 5, PcNet = 6, PcBilling = 7};
    //public product Product { get; set; }

    public int ProductId { get; set; }

    /// <summary>
    /// Name of the product shown in the user interface (PcTermPro and PcImportExport is two different things)
    /// </summary>
    //public string ProductDescription { get; set; }

    /// <summary>
    /// ProductDescription of the product (PcTermPro and PcImportExport is the same when looking only on the license part)
    /// </summary>
    public string ProductName { get; set; }
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public string EncryptionKey { get; set; }
    public string PersistenceKey { get; set; }
    public string PublicKey { get; set; }

    public override bool Equals(object obj)
    {
      if(obj.GetType() == typeof(ProductItem))
      {
        return ((ProductItem)obj).ProductId == ProductId;
      }
      return base.Equals(obj);
    }
  }
}