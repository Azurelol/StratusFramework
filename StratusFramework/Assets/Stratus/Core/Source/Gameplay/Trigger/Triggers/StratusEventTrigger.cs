
using System;
using Stratus.Dependencies.TypeReferences;
using UnityEngine;

namespace Stratus.Gameplay
{
  /// <summary>
  /// A trigger that is activated when a specified stratus event is received.
  /// </summary>
  public class StratusEventTrigger : Trigger
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Event")]
    [Tooltip("The scope of the event")]
    public StratusEvent.Scope eventScope;
    [DrawIf(nameof(StratusEventTrigger.eventScope), StratusEvent.Scope.GameObject, ComparisonType.Equals)]
    [Tooltip("The source GameObject which we want to listen the event on")]
    public GameObject source;
    [ClassExtends(typeof(Stratus.StratusEvent), Grouping = ClassGrouping.ByNamespace)]
    [Tooltip("What type of event this trigger will activate on")]
    public ClassTypeReference type;

    //[SerializeField]
    //private string eventData;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public override string automaticDescription
    {
      get
      {
        if (hasType)
          return $"On {type.Type.Name}";
        return string.Empty;
      }
    }

    public StratusEventProxy proxy { get; private set; }
    public bool hasType => type.Type != null;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      if (type.Type == null)
      {
        Trace.Error("Type not set. Please select the Stratus.Event type to connect to!", this);
        return;
      }

      proxy = StratusEventProxy.Construct(source, eventScope, type, OnEvent, persistent, debug);

    }

    void OnEvent<T>(T e) where T : Stratus.StratusEvent
    {
      Activate();
      //Trace.Script($"Triggered on {e.GetType()}");
    }


    protected override void OnReset()
    {      
      source = this.gameObject;
    }


  }
}