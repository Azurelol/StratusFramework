using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  /// <summary>
  /// An interface for selecting from Stratus.Events
  /// </summary>
  public class EventTypeSelector : TypeSelector
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private SerializedProperty eventDataProperty;
    private Stratus.StratusEvent eventObject;
    private SerializedSystemObject serializedEvent;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public EventTypeSelector() : base(typeof(Stratus.StratusEvent), false, true)
    {
      //this.eventDataProperty = eventDataProperty;
    }

    private EventTypeSelector(Type baseEventType) : base(baseEventType, false, true)
    {
    }

    public EventTypeSelector Construct<T>() where T : Stratus.StratusEvent
    {
      Type type = typeof(T);
      return new EventTypeSelector(type);
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnSelectionChanged()
    {
      base.OnSelectionChanged();
      eventObject = (Stratus.StratusEvent)Utilities.Reflection.Instantiate(selectedClass);
      serializedEvent = new SerializedSystemObject(selectedClass, eventObject);
      //serializedEvent.Deserialize(eventDataProperty);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public void Serialize(SerializedProperty stringProperty)
    {
      this.serializedEvent.Serialize(stringProperty);
    }

    public void EditorGUILayout(SerializedProperty stringProperty)
    {
      if (serializedEvent.DrawEditorGUILayout())
       this.Serialize(stringProperty);
    }



  }

}