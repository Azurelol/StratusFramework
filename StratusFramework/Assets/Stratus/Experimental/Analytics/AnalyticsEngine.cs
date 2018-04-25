using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stratus.Analytics
{
  [ExecuteInEditMode]
  public class AnalyticsEngine : Singleton<AnalyticsEngine>
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public List<AnalyticsCollector> collectors = new List<AnalyticsCollector>();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      
    }

    private void OnValidate()
    {
      
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    public static void Connect(AnalyticsCollector collector)
    {
      get.collectors.Add(collector);
    }

    public static void Disconnect(AnalyticsCollector collector)
    {
      get.collectors.Remove(collector);
    }

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/



  }

}