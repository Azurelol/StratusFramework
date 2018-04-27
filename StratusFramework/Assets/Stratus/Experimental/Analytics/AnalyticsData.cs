using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using System;
using Stratus.Dependencies.TypeReferences;

namespace Stratus.Analytics
{

  public enum DataType
  {
    Member,
    Event
  }

  public enum Condition
  {
    Timer,
    Nessage,
    Event
  }

  public enum MessageType
  {
    LifecycleEvent,
    EventTriggerType
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

  /// <summary>
  /// Retrieves the current value of a given member among a GameObject's components
  /// </summary>
  [Serializable]
  public class AnalyticsPayload
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/   


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
    [Filter(typeof(Vector2), typeof(Vector3), typeof(int), typeof(float), typeof(bool),
      Methods = false, Properties = true, NonPublic = true, ReadOnly = true, 
      Static = true, Inherited = true, Fields = true)]
    public UnityMember member;

    public float timeStamp;

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