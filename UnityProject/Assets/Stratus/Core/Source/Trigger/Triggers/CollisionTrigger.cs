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
  public class CollisionTrigger : Trigger
  {    
    [Header("Collision Type")]
    public CollisionProxy.TriggerType type;
    [Tooltip("The object whose collision messages we are listening for")]
    public Collider source;
    [Tooltip("What targets we are allowed to collide with")]
    public GameObjectField collisionTarget;
    
    protected override void OnAwake()
    {
      CollisionProxy.Construct(source, type, OnCollision, persistent);
    }    

    protected override void OnReset()
    {
      source = GetComponent<Collider>();
    }

    private void OnValidate()
    {
      ValidateLayers();
    }

    private void OnCollision(Collider other)
    {
      if (collisionTarget.IsTarget(other.gameObject) && !activated) 
      {
        Trace.Script("Activating", this);
        this.Activate();
      }
    }

    /// <summary>
    /// Verifies that the layer between the collision target and the source are compatible
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void ValidateLayers()
    {
      if (collisionTarget == null)
        return;

      int layer = 0;
      if (collisionTarget.GetLayer(ref layer))
      {
        bool ignored = Physics.GetIgnoreLayerCollision(source.gameObject.layer, layer);
        if (ignored)
        {
          string sourceLayerName = LayerMask.LayerToName(source.gameObject.layer);
          string targetLayerName = LayerMask.LayerToName(layer);
          string message = $"Collisions between the layer {sourceLayerName} on the source GameObject {source.gameObject.name} and the layer {targetLayerName} are not possible!";
          Trace.Error(message, this);
          Trace.Dialog("Ignored Layers", message);
        }

      }
    }



  }

}