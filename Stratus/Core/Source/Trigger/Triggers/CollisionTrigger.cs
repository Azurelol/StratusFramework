/******************************************************************************/
/*!
@file   CollisionTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Stratus
{
  /// <summary>
  /// Triggers an event when its (trigger) collider collides with a GameObject with the
  /// given specified tag.
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class CollisionTrigger : Trigger
  {    
    [Header("Collision Type")]
    public CollisionProxy.TriggerType type;
    [Tooltip("The object whose collision messages we are listening for")]
    public Collider source;
    [Tooltip("What targets we are allowed to collide with")]
    public TargetField collisionTarget;
    
    protected override void OnAwake()
    {
      CollisionProxy.Construct(source, type, OnCollision, false);
    }
    
    private void OnCollision(Collider other)
    {
      if (collisionTarget.IsTarget(other.gameObject))
      {
        this.Activate();
      }
    }

    private void Reset()
    {
      // Attempt to use self as a target first
      source = GetComponent<Collider>();
    }

    //public static CollisionTrigger Construct(Transform transform, Triggerable target, Instruction instruction, string tag, TriggerType type, bool persistent)
    //{
    //  var collisionTrigger = transform.GetOrAddComponent<CollisionTrigger>();
    //  collisionTrigger.target = target;
    //  collisionTrigger.instruction = instruction;
    //  collisionTrigger.targetType.value = tag;
    //  collisionTrigger.type = type;
    //  collisionTrigger.persistent = persistent;
    //  return collisionTrigger;
    //}


  }

}