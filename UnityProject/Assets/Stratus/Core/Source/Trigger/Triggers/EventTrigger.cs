
using System;
using Stratus.Dependencies.TypeReferences;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// A trigger that is activated when a specified stratus event is received.
  /// </summary>
  public class EventTrigger : Trigger
  { 
    [Header("Event")]
    [ClassExtends(typeof(Stratus.Event), Grouping = ClassGrouping.ByNamespace)]
    [Tooltip("What type of event this trigger will activate on")]
    public ClassTypeReference type;
    [Tooltip("The scope of the event")]
    public Event.Scope eventScope;

    protected override void OnAwake()
    {
      if (type.Type == null)
      {
        Trace.Error("Type not set. Please select the Stratus.Event type to connect to!", this);
        return;
      }

      switch (this.eventScope)
      {
        case Event.Scope.GameObject:
          this.gameObject.Connect(this.OnEvent, this.type);
          break;
        case Event.Scope.Scene:
          Scene.Connect(this.OnEvent, this.type);
          break;
      }
    }

    protected override void OnReset()
    {

    }

    void OnEvent<T>(T e) where T : Stratus.Event
    {
      Trace.Script("Triggered by " + type.Type.Name, this);
      //if (e.GetType() == this.Type.Type) {
      //}      
    }
    

  }
}