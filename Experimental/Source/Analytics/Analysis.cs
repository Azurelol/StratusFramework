using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Analytics
{
  public enum AnalysisType
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

  [Serializable]
  public class Analysis 
  {
    [Serializable]
    public class Attribute
    {
      [Tooltip("An identifier for this attribute")]
      public string label;
      [Tooltip("The type of this attribute")]
      public StratusActionProperty.Types type;
      [Tooltip("An optional description of this attribute")]
      public string description;
    }

    public string label;
    public string description;
    public AnalysisType type;
    public List<Attribute> attributes;
    //public abstract void Draw(Rect position);
  }

}