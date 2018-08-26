using System;
using UnityEngine;
using Stratus.Dependencies.TypeReferences;
using System.Collections.Generic;

namespace Stratus
{

  /// <summary>
  /// Allows you to select any registered events within the Editor
  /// </summary>
  [Serializable]
  public class EventField 
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    [Header("Event")]
    [Tooltip("The scope of the event")]
    public Event.Scope scope;
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

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Dispatches the event onto the given target
    /// </summary>
    /// <returns></returns>
    public bool Dispatch()
    {
      if (!hasType)
        return false;

      if (eventInstance == null)
        eventInstance = Event.Instantiate(type, eventData);

      switch (scope)
      {
        case Event.Scope.GameObject:
          foreach (var target in targets)
          {
            if (target)
              target.Dispatch(eventInstance, type.Type);
          }
          break;
        case Event.Scope.Scene:
          Scene.Dispatch(eventInstance, type.Type);
          break;
      }
      return true;
    }

    



  }

}