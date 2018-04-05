using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(LogEvent))]
  public class LogEventEditor : TriggerableEditor<LogEvent>
  {
    //protected override void OnBaseEditorEnable()
    //{
    //}

    protected override void OnTriggerableEditorEnable()
    {
      SerializedProperty descriptionProperty = propertyMap[nameof(TriggerBase.description)];
      propertyDrawOverrides.Remove(descriptionProperty);
    }

  }

}