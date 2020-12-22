using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace Stratus.Gameplay
{
  [CustomEditor(typeof(StratusDispatchEvent))]
  public class DispatchEventEditor : TriggerableEditor<StratusDispatchEvent>
  {
    private Stratus.StratusEvent eventObject;
    private StratusSerializedEditorObject serializedEvent;
    private Type type => triggerable.type.Type;
    private SerializedProperty eventDataProperty;

    protected override void OnTriggerableEditorEnable()
    {
      AddConstraint(() => triggerable.eventScope == StratusEvent.Scope.GameObject, nameof(StratusDispatchEvent.targets));
      eventDataProperty = serializedObject.FindProperty("eventData");
      drawGroupRequests.Add(new DrawGroupRequest(SetMembers, () => triggerable.hasType && serializedEvent != null && serializedEvent.drawer.isDrawable));
      propertyChangeCallbacks.Add(propertyMap[nameof(StratusDispatchEvent.type)], OnEventChanged);

      if (triggerable.hasType)
        OnEventChanged();
    }

    private void SetMembers(Rect rect)
    {
      EditorGUILayout.Space();
      EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);

      if (serializedEvent.DrawEditorGUILayout())
        serializedEvent.Serialize(target, eventDataProperty);
    }

    private void OnEventChanged()
    {
      endOfFrameRequests.Add(UpdateEventObject);
    }

    void UpdateEventObject()
    {
      if (!triggerable.hasType)
        return;

      eventObject = (Stratus.StratusEvent)Utilities.StratusReflection.Instantiate(type);
      serializedEvent = new StratusSerializedEditorObject(eventObject);
      serializedEvent.Deserialize(eventDataProperty);
    }

  }
}