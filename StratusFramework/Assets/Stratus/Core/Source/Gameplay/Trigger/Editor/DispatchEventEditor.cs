using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace Stratus
{
  [CustomEditor(typeof(DispatchEvent))]
  public class DispatchEventEditor : TriggerableEditor<DispatchEvent>
  {
    private Stratus.Event eventObject;
    private SerializedSystemObject serializedEvent;
    private Type type => triggerable.type.Type;
    private SerializedProperty eventDataProperty;

    protected override void OnTriggerableEditorEnable()
    {
      AddConstraint(nameof(DispatchEvent.targets), () => triggerable.eventScope == Event.Scope.GameObject);
      eventDataProperty = serializedObject.FindProperty("eventData");
      drawGroupRequests.Add(new DrawGroupRequest(SetMembers, () => triggerable.hasType && serializedEvent != null && serializedEvent.isDrawable));
      propertyChangeCallbacks.Add(propertyMap[nameof(DispatchEvent.type)], OnEventChanged);

      if (triggerable.hasType)
        OnEventChanged();
    }

    private void SetMembers(Rect rect)
    {
      EditorGUILayout.Space();
      EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);

      if (serializedEvent.DrawFields())
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

      eventObject = (Stratus.Event)Utilities.Reflection.Instantiate(type);
      serializedEvent = new SerializedSystemObject(type, eventObject);
      serializedEvent.Deserialize(eventDataProperty);
    }

  }
}