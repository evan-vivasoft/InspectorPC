using System.Collections.Generic;
using System.Linq;
using Kamstrup.LicenseKeyGenerator.Controller.Tools;
using Kamstrup.LicenseKeyGenerator.Model;

namespace Kamstrup.LicenseKeyGenerator.Controller
{
  public class ProductController
  {
    #region singleton

    private static ProductController _instance;
    public static ProductController Instance
    {
      get { return _instance ?? (_instance = new ProductController()); }
    }
    #endregion

    public List<ProductItem> GetProducts()
    {
      return XmlHandler.GetStoredProducts();
    }

    public ProductItem GetSpecificProduct(int id)
    {
      List<ProductItem> products = GetProducts();

      if (products.Where(p => p.ProductId == id).Select(p => p).Count() < 1)
      {
#if DEBUGNL || RELEASENL
        return new ProductItem { ProductName = "Kamstrup DK product" };
#else
        return new ProductItem { ProductName = "Kamstrup NL product" };
#endif
      }
      return products.Single(p => p.ProductId == id);
    }
  }
}
