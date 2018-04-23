using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Stratus
{
  [CustomEditor(typeof(CompositeTrigger))]
  public class CompositeTriggerEditor : TriggerEditor<CompositeTrigger>
  {
    //CompositeTrigger composite => target as CompositeTrigger;
    //
    //private SerializedProperty typeProp => declaredProperties.Item2[0];
    //private SerializedProperty criteriaProp => declaredProperties.Item2[1];
    //private SerializedProperty neededProp => declaredProperties.Item2[2];
    //private SerializedProperty triggersProp => declaredProperties.Item2[3];
    //private SerializedProperty triggerablesProp => declaredProperties.Item2[4];

    protected override void OnTriggerEditorEnable()
    {
      //compositeTrigger= (CompositeTrigger)target.
      propertyConstraints.Add(propertyMap["triggers"], () => trigger.type == CompositeTrigger.Type.Trigger);
      propertyConstraints.Add(propertyMap["triggerables"], () => trigger.type == CompositeTrigger.Type.Triggerable);
      //propertyGroupDrawOverrides.Add(typeof(CompositeTrigger), DrawCompositeTriggerProperties);

    }

    protected override bool DrawDeclaredProperties()
    {
      bool changed = false;

      EditorGUI.BeginChangeCheck();
      DrawSerializedProperty(nameof(CompositeTrigger.type));
      DrawSerializedProperty(nameof(CompositeTrigger.criteria));

      if (trigger.criteria == CompositeTrigger.Criteria.Subset)
        trigger.needed = EditorGUILayout.IntSlider(trigger.needed, 1, trigger.count);

      if (trigger.type == CompositeTrigger.Type.Trigger)
      {
        DrawSerializedProperty(nameof(CompositeTrigger.triggers));
      }
      else if (trigger.type == CompositeTrigger.Type.Triggerable)
      {
        DrawSerializedProperty(nameof(CompositeTrigger.triggerables));
      }
      changed = EditorGUI.EndChangeCheck();
      return changed;
    }

  }

}