using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Analytics
{
  [CustomEditor(typeof(AnalyticsCollector))]
  public class AnalyticsCollectorEditor : BehaviourEditor<AnalyticsCollector>
  {
    protected override void OnStratusEditorEnable()
    {
      // Events
      AddConstraint(nameof(AnalyticsCollector.eventType), () => target.condition == Condition.Event);
      AddConstraint(nameof(AnalyticsCollector.eventScope), () => target.condition == Condition.Event);
      // Timer
      AddConstraint(nameof(AnalyticsCollector.onTimer), () => target.condition == Condition.Timer);
      // Messages
      AddConstraint(nameof(AnalyticsCollector.onMessage), () => target.condition == Condition.Nessage);
      AddConstraint(nameof(AnalyticsCollector.onLifecycleEvent), () => target.condition == Condition.Nessage && target.onMessage == MessageType.LifecycleEvent);
      AddConstraint(nameof(AnalyticsCollector.onEventTriggerType), () => target.condition == Condition.Nessage && target.onMessage == MessageType.EventTriggerType);

    }




  }

}