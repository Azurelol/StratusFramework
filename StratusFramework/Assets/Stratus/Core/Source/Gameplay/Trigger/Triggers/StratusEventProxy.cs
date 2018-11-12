using System.Collections;
using System.Collections.Generic;
using Stratus.Dependencies.TypeReferences;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Stratus
{
  public class StratusEventProxy : StratusProxy
  {
    public delegate void OnTriggerMessage(Stratus.StratusEvent e);

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Event")]
    public StratusEvent.Scope scope;
    [ClassExtends(typeof(Stratus.StratusEvent), Grouping = ClassGrouping.ByNamespace)]
    [Tooltip("What type of event this trigger will activate on")]
    public ClassTypeReference type = new ClassTypeReference();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Subscribes to collision events on this proxy
    /// </summary>
    public StratusEvent.EventCallback onTrigger = new StratusEvent.EventCallback();

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
    public static StratusEventProxy Construct(GameObject target, StratusEvent.Scope scope, Type type, System.Action<Stratus.StratusEvent> onTrigger, bool persistent = true, bool debug = false)
    {
      var proxy = target.gameObject.AddComponent<StratusEventProxy>();
      proxy.scope = scope;
      proxy.type = type;
      proxy.onTrigger.AddListener(new UnityAction<StratusEvent>(onTrigger));
      proxy.persistent = persistent;
      proxy.debug = debug;
      return proxy;
    }

    void OnEvent(Stratus.StratusEvent e)
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
        case StratusEvent.Scope.GameObject:
          this.gameObject.Connect(this.OnEvent, this.type);
          break;
        case StratusEvent.Scope.Scene:
          Scene.Connect(this.OnEvent, this.type);
          break;
      }

      connected = true;
    }


  }

}