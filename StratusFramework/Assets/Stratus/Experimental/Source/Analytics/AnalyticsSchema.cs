using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Analytics
{
  [CreateAssetMenu(fileName = "Analytics Schema", menuName = "Stratus/Analytics Schema")]
  public class AnalyticsSchema : StratusScriptable
  {
    public List<Analysis.Attribute> attributes = new List<Analysis.Attribute>();
    public List<Analysis> analyses = new List<Analysis>();
  }

}