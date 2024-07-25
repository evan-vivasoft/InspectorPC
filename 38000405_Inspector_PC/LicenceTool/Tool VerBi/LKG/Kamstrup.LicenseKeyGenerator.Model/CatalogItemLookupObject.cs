using System;
using System.Collections.Generic;

namespace Kamstrup.LicenseKeyGenerator.Model
{
  [Serializable]
  public class CatalogItemLookupObject
  {
    public CatalogItem Catalog { get; set; }
    public FeatureItem Feature { get; set; }
  }

  [Serializable]
  public class CatalogItemLookupCollection : List<CatalogItemLookupObject>
  {
  }
}

