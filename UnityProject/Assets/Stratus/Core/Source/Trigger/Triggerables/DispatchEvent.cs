using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.TypeReferences;
using System;

namespace Stratus
{
  public class DispatchEvent : Triggerable
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Event")]
    [Tooltip("The scope of the event")]
    public Event.Scope eventScope;
    [DrawIf(nameof(DispatchEvent.eventScope), Event.Scope.GameObject, ComparisonType.Equals)]
    [Tooltip("The GameObjects which we want to dispatch the event to")]
    public List<GameObject> targets = new List<GameObject>();
    [ClassExtends(typeof(Stratus.Event), Grouping = ClassGrouping.ByNamespace)]
    [Tooltip("What type of event this trigger will activate on")]
    public ClassTypeReference type = new ClassTypeReference();

    [SerializeField]
    private string eventData = string.Empty;    

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public bool hasType => type.Type != null;
    private Stratus.Event eventInstance { get; set; }

    public override string automaticDescription
    {
      get
      {
        if (hasType)
          return $"Dispatch {type.Type.Name} to {eventScope}";
        return string.Empty;
      }
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      eventInstance = (Stratus.Event)Utilities.Reflection.Instantiate(type);
      JsonUtility.FromJsonOverwrite(eventData, eventInstance);
    }

    protected override void OnReset()
    {
      
    }

    protected override void OnTrigger()
    {
      switch (eventScope)
      {
        case Event.Scope.GameObject:
          foreach(var target in targets)
          {
            if (target)
              target.Dispatch(eventInstance, type.Type);
          }
          break;
        case Event.Scope.Scene:
          Scene.Dispatch(eventInstance, type.Type);
          break;
      }
    }

  }

}