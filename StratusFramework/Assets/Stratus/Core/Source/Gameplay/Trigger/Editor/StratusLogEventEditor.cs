using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusLogEvent))]
  public class LogEventEditor : TriggerableEditor<StratusLogEvent>
  {
    //protected override void OnBaseEditorEnable()
    //{
    //}

    protected override void OnTriggerableEditorEnable()
    {
      SerializedProperty descriptionProperty = propertyMap[nameof(StratusTriggerBase.description)];
      propertyDrawOverrides.Remove(descriptionProperty);
    }

  }

}