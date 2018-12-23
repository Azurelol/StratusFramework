using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Analytics
{
  [CustomEditor(typeof(AnalyticsSchema))]
  public class AnalyticsSchemaEditor : ScriptableEditor<AnalyticsSchema>
  {
    protected override void OnStratusEditorEnable()
    {
      drawReorderableLists = false;
    }


  }

}