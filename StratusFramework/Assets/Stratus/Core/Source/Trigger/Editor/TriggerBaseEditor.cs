using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(TriggerBase), true), CanEditMultipleObjects]
  public abstract class TriggerBaseEditor<T> : BehaviourEditor<T> where T : TriggerBase
  {
    abstract internal void OnTriggerBaseEditorEnable();

    protected override void OnStratusEditorEnable()
    {
      // Custom description support
      SerializedProperty descriptionModeProperty = propertyMap[nameof(TriggerBase.descriptionMode)];
      propertyConstraints.Add(descriptionModeProperty, False);
      SerializedProperty descriptionProperty = propertyMap[nameof(TriggerBase.description)];

      propertyDrawOverrides.Add(descriptionProperty, (SerializedProperty property) =>
      {
        EditorGUILayout.BeginHorizontal();
        if (target.descriptionMode == TriggerBase.DescriptionMode.Automatic)
        {
          //EditorGUILayout.SelectableLabel(target.automaticDescription, GUILayout.ExpandWidth(true));
          GUI.enabled = false;
          EditorGUILayout.PropertyField(property, true, GUILayout.ExpandWidth(true));
          GUI.enabled = true;
        }
        else if (target.descriptionMode == TriggerBase.DescriptionMode.Manual)
        {
          EditorGUILayout.PropertyField(property, true, GUILayout.ExpandWidth(true));
        }
        EditorGUILayout.PropertyField(descriptionModeProperty, GUIContent.none, true, GUILayout.Width(85));
        EditorGUILayout.EndHorizontal();
      });

      UpdateDescription();

      OnTriggerBaseEditorEnable();
    }

    protected override void OnBehaviourEditorValidate()
    {
      if (target == null)
        Trace.Script($"Null at {GetType().Name}");

      UpdateDescription();
    }

    private void UpdateDescription()
    {
      if (target.descriptionMode == TriggerBase.DescriptionMode.Automatic)
        target.description = target.automaticDescription;
    }

  }

}