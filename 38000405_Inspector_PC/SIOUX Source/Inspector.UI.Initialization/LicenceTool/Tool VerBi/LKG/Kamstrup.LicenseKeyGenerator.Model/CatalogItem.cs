using System.Collections.Generic;

namespace Kamstrup.LicenseKeyGenerator.Model
{
  public class CatalogItem
  {
    /// <summary>
    /// 
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Name of the product shown in the user interface (PcTermPro and PcImportExport is two different things)
    /// </summary>
    public string ProductDescription { get; set; }

    /// <summary>
    /// The numbers the item has in the catalog. Aka. Stock/item number (da: varenummer). Aka. Order no. 
    /// </summary>
    List<string> catalogNumbers = new List<string>();
    public List<string> CatalogNumbers
    {
      get { return catalogNumbers ?? (catalogNumbers = new List<string>()); }
      set { catalogNumbers = value; }
    }

    public override bool Equals(object obj)
    {
      return ProductId == ((CatalogItem) obj).ProductId &&
             ProductDescription == ((CatalogItem) obj).ProductDescription &&
             CatalogNumbers.Equals(((CatalogItem) obj).CatalogNumbers);
    }

    public override string ToString()
    {
      return string.Format("desc = {0}, id = {1}", ProductDescription, ProductId);
    }
  }
}
