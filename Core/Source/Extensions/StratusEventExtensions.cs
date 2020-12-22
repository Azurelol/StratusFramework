using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace Stratus
{
	public static partial class Extensions
	{
		public static void AddListener(this UnityEvent unityEvent, System.Action action)
		{
			unityEvent.AddListener(() => action());
		}

		public static void AddEventTrigger(this Selectable selectable, EventTriggerType trigger, Action<BaseEventData> onTrigger) 
		{
			var eventTrigger = selectable.gameObject.GetOrAddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry();
			entry.eventID = trigger;
			entry.callback.AddListener((x) => onTrigger(x));
			eventTrigger.triggers.Add(entry);

		}
	}
}