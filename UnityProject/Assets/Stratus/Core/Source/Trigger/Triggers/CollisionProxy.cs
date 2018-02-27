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
    //public enum CollisionMode { Trigger, Collision }
    public enum TriggerType
    {
      TriggerEnter,
      TriggerExit,
      TriggerStay,
      CollisionEnter,
      CollisionExit,
      CollisionStay
    }
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
    private void OnTriggerEnter(Collider other)
    {
      OnTrigger(other, TriggerType.TriggerEnter);
    }

    private void OnTriggerStay(Collider other)
    {
      OnTrigger(other, TriggerType.TriggerStay);
    }
    private void OnTriggerExit(Collider other)
    {
      OnTrigger(other, TriggerType.TriggerExit);
    }

    private void OnCollisionEnter(Collision collision)
    {
      OnTrigger(collision.collider, TriggerType.CollisionEnter);
    }

    private void OnCollisionExit(Collision collision)
    {
      OnTrigger(collision.collider, TriggerType.CollisionExit);
    }

    private void OnCollisionStay(Collision collision)
    {
      OnTrigger(collision.collider, TriggerType.CollisionStay);
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

      onTrigger?.Invoke(other);

      //if (!persistent)
      //{
      //  Destroy(this);
      //}
    }




  }

}