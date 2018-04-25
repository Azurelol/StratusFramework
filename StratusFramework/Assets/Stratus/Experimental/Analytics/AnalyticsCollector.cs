using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Analytics
{
  [ExecuteInEditMode]
  public class AnalyticsCollector : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public AnalyticsDataProvider target;
    [Header("Settings")]
    [Tooltip("How often the members are polled for their current value")]
    [Range(0f, 3f)]
    public float pollFrequency = 1f;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      
    }

    private void OnEnable()
    {
      
    }

    private void OnDisable()
    {
      
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Submits the current data point of the target
    /// </summary>
    public void Submit()
    {

    }

  }

}