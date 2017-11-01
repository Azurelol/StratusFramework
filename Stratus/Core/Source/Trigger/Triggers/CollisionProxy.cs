using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
  /// <summary>
  /// Acts a proxy for collision events, observing another GameObject's collision messages
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class CollisionProxy : Proxy
  {
    public enum TriggerType { Enter, Exit }
    public delegate void OnTriggerMessage(Collider collider);

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Collision")]
    public TriggerType type;


    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Subscribes to collision events on this proxy
    /// </summary>
    public OnTriggerMessage onTrigger { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    /// <summary>
    /// If its activated when it detects a collision with a target.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider other)
    {
      OnTrigger(other, TriggerType.Enter);
    }

    /// <summary>
    /// If its activated when it detects a collision with a target..
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider other)
    {
      OnTrigger(other, TriggerType.Exit);
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
    public static CollisionProxy Construct(Collider target, TriggerType type, OnTriggerMessage onCollision, bool persistent = true)
    {
      var proxy = target.gameObject.AddComponent<CollisionProxy>();
      proxy.type = type;
      proxy.onTrigger += onCollision;
      proxy.persistent = persistent;
      return proxy;
    }

    //------------------------------------------------------------------------/
    // Procedures
    //------------------------------------------------------------------------/
    private void OnTrigger(Collider other, TriggerType type)
    {
      if (this.type != type)
        return;

      onTrigger(other);

      if (!persistent)
      {
        Destroy(this);
      }
    }




  }

}