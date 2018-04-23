using System.Collections;
using System.Collections.Generic;
using Stratus.Dependencies.TypeReferences;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Stratus
{
  public class EventProxy : Proxy
  {
    public delegate void OnTriggerMessage(Stratus.Event e);

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Event")]
    public Event.Scope scope;
    [ClassExtends(typeof(Stratus.Event), Grouping = ClassGrouping.ByNamespace)]
    [Tooltip("What type of event this trigger will activate on")]
    public ClassTypeReference type = new ClassTypeReference();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Subscribes to collision events on this proxy
    /// </summary>
    public Event.EventCallback onTrigger = new Event.EventCallback();

    public bool hasType => type.Type != null;
    private bool connected { get; set; } 

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      if (hasType)      
        Subscribe();      
    }

    private void Start()
    {
      if (!connected)
        Subscribe();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Constructs a proxy in order to observe another GameObject's collision messages
    /// </summary>
    /// <param name="target"></param>
    /// <param name="type"></param>
    /// <param name="onCollision"></param>
    /// <param name="persistent"></param>
    /// <returns></returns>
    public static EventProxy Construct(GameObject target, Event.Scope scope, Type type, System.Action<Stratus.Event> onTrigger, bool persistent = true, bool debug = false)
    {
      var proxy = target.gameObject.AddComponent<EventProxy>();
      proxy.scope = scope;
      proxy.type = type;
      proxy.onTrigger.AddListener(new UnityAction<Event>(onTrigger));
      proxy.persistent = persistent;
      proxy.debug = debug;
      return proxy;
    }

    void OnEvent(Stratus.Event e)
    {
      if (!e.GetType().Equals(type.Type))
        return;

      //if (debug)
      //  Trace.Script("Triggered by " + type.Type.Name, this);

      onTrigger.Invoke(e);
    }

    private void Subscribe()
    {
      switch (scope)
      {
        case Event.Scope.GameObject:
          this.gameObject.Connect(this.OnEvent, this.type);
          break;
        case Event.Scope.Scene:
          Scene.Connect(this.OnEvent, this.type);
          break;
      }

      connected = true;
    }


  }

}