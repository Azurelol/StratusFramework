using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.TypeReferences;
using System;

namespace Stratus.Gameplay
{
  public class StratusDispatchEvent : StratusTriggerable
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Event")]
    [Tooltip("The scope of the event")]
    public StratusEvent.Scope eventScope;
    [DrawIf(nameof(StratusDispatchEvent.eventScope), StratusEvent.Scope.GameObject, ComparisonType.Equals)]
    [Tooltip("The GameObjects which we want to dispatch the event to")]
    public List<GameObject> targets = new List<GameObject>();
    [ClassExtends(typeof(Stratus.StratusEvent), Grouping = ClassGrouping.ByNamespace)]
    [Tooltip("What type of event this trigger will activate on")]
    public ClassTypeReference type = new ClassTypeReference();

    [SerializeField]
    private string eventData = string.Empty;    

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public bool hasType => type.Type != null;
    private Stratus.StratusEvent eventInstance { get; set; }

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
      eventInstance = StratusEvent.Instantiate(type, eventData);
    }

    protected override void OnReset()
    {
      
    }

    protected override void OnTrigger()
    {
      switch (eventScope)
      {
        case StratusEvent.Scope.GameObject:
          foreach(var target in targets)
          {
            if (target)
              target.Dispatch(eventInstance, type.Type);
          }
          break;
        case StratusEvent.Scope.Scene:
          StratusScene.Dispatch(eventInstance, type.Type);
          break;
      }
    }

  }

}