using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using System;using Stratus.Dependencies.TypeReferences;


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



  /// <summary>
  /// Retrieves the current value of a given member among a GameObject's components
  /// </summary>
  //[Serializable]
  public struct AnalyticsPayload
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The attribute this payload is for
    /// </summary>
    public Analysis.Attribute attribute;
    /// <summary>
    /// The value for this attribute
    /// </summary>
    public object value;
    /// <summary>
    /// When this attribute was collected
    /// </summary>
    public float timeStamp;

    public AnalyticsPayload(Analysis.Attribute attribute, object value, float timeStamp)
    {
      this.attribute = attribute;
      this.value = value;
      this.timeStamp = timeStamp;
    }

    //public GameObject gameObject { get; set; }
    //public Transform transform { get; set; }
    //public object latestValue { get; set; }
    //public bool hasValue => latestValue != null;

    ////------------------------------------------------------------------------/
    //// Methods
    ////------------------------------------------------------------------------/
    ///// <summary>
    ///// Initializes this data for collection
    ///// </summary>
    ///// <returns></returns>
    //public bool Initialize()
    //{
    //  gameObject = member.target as GameObject;
    //  if (!gameObject)
    //    return false;
    //  transform = gameObject.transform;
    //  return true;
    //}
    //
    ///// <summary>
    ///// Records the current value of the given member
    ///// </summary>
    ///// <returns></returns>
    //public bool Collect()
    //{
    //  if (!member.isAssigned)
    //    return false;
    //
    //  latestValue = member.Get();
    //  return true;
    //}



  }

}