using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using System;

namespace Stratus.Analytics
{
  /// <summary>
  /// Retrieves the current value of a given member among a GameObject's components
  /// </summary>
  [Serializable]
  public class AnalyticsDataProvider
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/   
    public enum DataType
    {
      Member,
      Event
    }

    public enum Analysis
    {
      Statistical,
      Spatial
    }

    public enum SpatialAnalysis
    {
      Heatmap
    }

    public enum StatisticalAnalysis
    {
      Histogram
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("How to analyze this data")]
    public Analysis analysis;
    [Tooltip("An identifier for this data")]
    public string label;
    [Tooltip("An optional description of this data")]
    public string description;
    [Tooltip("The member that will be inspected")]
    [Filter(Methods = false, Properties = true, NonPublic = true, ReadOnly = true, Static = true, Inherited = true, Fields = true)]
    public UnityMember member;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public GameObject gameObject { get; set; }
    public Transform transform { get; set; }
    public object latestValue { get; set; }
    public bool hasValue => latestValue != null;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes this data for collection
    /// </summary>
    /// <returns></returns>
    public bool Initialize()
    {
      gameObject = member.target as GameObject;
      if (!gameObject)
        return false;
      transform = gameObject.transform;
      return true;
    }

    /// <summary>
    /// Records the current value of the given member
    /// </summary>
    /// <returns></returns>
    public bool Collect()
    {
      if (!member.isAssigned)
        return false;

      latestValue = member.Get();
      return true;
    }



  }

}