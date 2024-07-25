using System;
using System.Collections.Generic;
using System.Linq;

namespace Kamstrup.EqatecMonitor
{
  public class EqatecMonitorFeature
  {
    public string Name { get; set; }
    public Guid Id { get; set; }


  }
  public class EqatecMonitorFeatures : List<EqatecMonitorFeature>
  {
    private EqatecMonitorFeatures()
    {

    }
    
    public EqatecMonitorFeature GetFeature(string FeatureName)
    {
      return FeatureList != null ? FeatureList.SingleOrDefault(f => f.Name == FeatureName) : null;
    }

    public EqatecMonitorFeature GetFeature(Guid FeatureId)
    {
      return FeatureList != null ? FeatureList.SingleOrDefault(f => f.Id == FeatureId) : null;
    }
    
    public static EqatecMonitorFeatures FeatureList;
  }
}
