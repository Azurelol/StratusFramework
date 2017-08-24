
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
    public ClassTypeReference Type;
    [Tooltip("The scope of the event")]
    public Event.Scope Scope;

    protected override void OnInitialize()
    {
      if (Type.Type == null)
      {
        Trace.Error("Type not set. Please select the Stratus.Event type to connect to!", this);
        return;
      }

      switch (this.Scope)
      {
        case Event.Scope.GameObject:
          this.gameObject.Connect(this.OnEvent, this.Type);
          break;
        case Event.Scope.Scene:
          Scene.Connect(this.OnEvent, this.Type);
          break;
      }
    }

    protected override void OnEnabled()
    {      
    }

    void OnEvent<T>(T e) where T : Stratus.Event
    {
      Trace.Script("Triggered by " + Type.Type.Name, this);
      //if (e.GetType() == this.Type.Type) {
      //}      
    }

    //protected virtual T GetDerivedEvent<T>()
    //{
    //  return 
    //}


  }
}