using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusCompositeTrigger))]
  public class CompositeTriggerEditor : TriggerEditor<StratusCompositeTrigger>
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
      propertyConstraints.Add(propertyMap["triggers"], () => trigger.type == StratusCompositeTrigger.Type.Trigger);
      propertyConstraints.Add(propertyMap["triggerables"], () => trigger.type == StratusCompositeTrigger.Type.Triggerable);
      //propertyGroupDrawOverrides.Add(typeof(CompositeTrigger), DrawCompositeTriggerProperties);

    }

    protected override bool DrawDeclaredProperties()
    {
      bool changed = false;

      EditorGUI.BeginChangeCheck();
      DrawSerializedProperty(nameof(StratusCompositeTrigger.type));
      DrawSerializedProperty(nameof(StratusCompositeTrigger.criteria));

      if (trigger.criteria == StratusCompositeTrigger.Criteria.Subset)
        trigger.needed = EditorGUILayout.IntSlider(trigger.needed, 1, trigger.count);

      if (trigger.type == StratusCompositeTrigger.Type.Trigger)
      {
        DrawSerializedProperty(nameof(StratusCompositeTrigger.triggers));
      }
      else if (trigger.type == StratusCompositeTrigger.Type.Triggerable)
      {
        DrawSerializedProperty(nameof(StratusCompositeTrigger.triggerables));
      }
      changed = EditorGUI.EndChangeCheck();
      return changed;
    }

  }

}