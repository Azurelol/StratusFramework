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
using UnityEngine.Serialization;

namespace Stratus
{
  /// <summary>
  /// Triggers an event when its (trigger) collider collides with a GameObject with the
  /// given specified tag.
  /// </summary>
  public class CollisionTrigger : Trigger
  {
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Header("Collision Type")]
    public CollisionProxy.CollisionMessage type;
    [Tooltip("The object whose collision messages we are listening for")]
    public Collider source;
    //[Validate(nameof(OnFilterChanged), ValidateLevel.Warning)]
    [Tooltip("What targets we are allowed to collide with")]
    [FormerlySerializedAs("collisionTarget")]
    public GameObjectField filter;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public CollisionProxy proxy { get; private set; }
    public override string automaticDescription
    {
      get
      {
        if (source)
          return $"On {type} for {source.name} against {filter}";
        return string.Empty;
      }
    }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      proxy = CollisionProxy.Construct(source, type, OnTrigger, OnCollision, persistent);
    }

    protected override void OnReset()
    {
      source = GetComponent<Collider>();
    }

    private void OnValidate()
    {
      if (debug)
        Validate();
    }

    private void OnEnable()
    {
      proxy.enabled = true;
    }

    private void OnDisable()
    {
      if (proxy)
        proxy.enabled = false;
    }

    public override Validation Validate()
    {
      Validation validation = new Validation(Validation.Level.Warning, this);
      validation.Add(Validation.NullReference(this, $"<i>{description}</i>"));
      validation.Add(ValidateLayers());
      return validation;
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    private void OnTrigger(Collider other)
    {
      if (filter.IsTarget(other.gameObject))
      {
        this.Activate();
      }
      else if (debug)
      {
        Trace.Script($"{other.name} is not a valid target for this trigger", this);
      }
    }

    private void OnCollision(Collision collision)
    {
      if (filter.IsTarget(collision.gameObject))
      {
        this.Activate();
      }
      else if (debug)
      {
        Trace.Script($"{collision.gameObject.name} is not a valid target for this trigger", this);
      }
    }

    /// <summary>
    /// Verifies that the layer between the collision target and the source are compatible
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public string ValidateLayers()
    {
      if (!source || filter == null)
        return null;

      int layer = 0;
      if (filter.GetLayer(ref layer))
      {
        bool ignored = Physics.GetIgnoreLayerCollision(source.gameObject.layer, layer);
        if (ignored)
        {
          string sourceLayerName = LayerMask.LayerToName(source.gameObject.layer);
          string targetLayerName = LayerMask.LayerToName(layer);
          string msg = $"Collisions between the layer <i>{sourceLayerName}</i> on the source <i>{source.gameObject.name}</i> and the layer <i>{targetLayerName}</i> are not possible!";
          Error(msg, this);
          //Trace.Dialog("Ignored Layers", msg);
          return msg;
        }
      }

      return null;
    }


  }

}