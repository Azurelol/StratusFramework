using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Analytics
{
  [CustomEditor(typeof(AnalyticsCollector))]
  public class AnalyticsCollectorEditor : StratusBehaviourEditor<AnalyticsCollector>
  {
    private DropdownList<Analysis.Attribute> attributes;

    protected override void OnStratusEditorEnable()
    {
      // Events
      //AddConstraint(nameof(AnalyticsCollector.eventType), () => target.condition == Condition.Event);
      //AddConstraint(nameof(AnalyticsCollector.eventScope), () => target.condition == Condition.Event);
      //// Timer
      //AddConstraint(nameof(AnalyticsCollector.onTimer), () => target.condition == Condition.Timer);
      //// Messages
      //AddConstraint(nameof(AnalyticsCollector.onMessage), () => target.condition == Condition.Nessage);
      //AddConstraint(nameof(AnalyticsCollector.onLifecycleEvent), () => target.condition == Condition.Nessage && target.onMessage == MessageType.LifecycleEvent);
      //AddConstraint(nameof(AnalyticsCollector.onEventTriggerType), () => target.condition == Condition.Nessage && target.onMessage == MessageType.EventTriggerType);
      SerializedProperty schemaAttribute = propertyMap[nameof(AnalyticsCollector.schema)];
      propertyChangeCallbacks.Add(schemaAttribute, MakeAttributeList);
      //AddSection(SelectAttribute);
      //AddSection(SelectCondition);
      MakeAttributeList();
    }


    protected override bool DrawDeclaredProperties()
    {
      bool changed = false;

      changed |= DrawSerializedProperty(nameof(AnalyticsCollector.schema));
      EditorGUI.BeginChangeCheck();
      attributes.selectedIndex = EditorGUILayout.Popup("Attribute", attributes.selectedIndex, attributes.displayedOptions);
      if (EditorGUI.EndChangeCheck())
      {
        changed = true;
        target.attribute = attributes.selected;
      }
      changed |= DrawSerializedProperty(nameof(AnalyticsCollector.member));

      changed |= DrawSerializedProperty(nameof(AnalyticsCollector.condition));
      switch (target.condition)
      {
        case Condition.Timer:
          changed |= DrawSerializedProperty(nameof(AnalyticsCollector.onTimer));
          break;

        case Condition.Nessage:
          changed |= DrawSerializedProperty(nameof(AnalyticsCollector.onMessage));
          switch (target.onMessage)
          {
            case MessageType.LifecycleEvent:
              changed |= DrawSerializedProperty(nameof(AnalyticsCollector.onLifecycleEvent));
              break;
            case MessageType.EventTriggerType:
              changed |= DrawSerializedProperty(nameof(AnalyticsCollector.onEventTriggerType));
              break;
          }
          break;

        case Condition.Event:
          changed |= DrawSerializedProperty(nameof(AnalyticsCollector.eventScope));
          changed |= DrawSerializedProperty(nameof(AnalyticsCollector.eventType));
          break;
      }

      return changed;
    }


    private void SelectAttribute(Rect position)
    {

    }

    private void SelectCondition(Rect position)
    {

    }

    private void MakeAttributeList()
    {
      if (!target.schema)
      {
        attributes = null;
      }

      attributes = new DropdownList<Analysis.Attribute>(target.schema.attributes, (Analysis.Attribute attr) => $"{attr.label} ({attr.type}) ", target.attribute);
      //MakeCheckpointList();
      //target.attribute = null;
    }




  }

}