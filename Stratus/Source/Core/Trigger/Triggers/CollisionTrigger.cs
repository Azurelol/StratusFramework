/******************************************************************************/
/*!
@file   CollisionTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  /// <summary>
  /// Triggers an event when its (trigger) collider collides with a GameObject with the
  /// given specified tag.
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class CollisionTrigger : Trigger
  {    
    public enum TriggerType { Enter, Exit }

    [Header("Collision")]
    public TriggerType type;
    [Tooltip("What targets we are allowed to collide with")]
    public TagField targetType = new TagField();
    private Collider trigger { get; set; }

    protected override void OnInitialize()
    {
    }

    protected override void OnEnabled()
    {
    }

    /// <summary>
    /// If its activated when it detects a collision with a target.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider other)
    {
      if (type != TriggerType.Enter)
        return;

      if (other.gameObject.CompareTag(this.targetType))      
        this.Activate();      
    }       

    /// <summary>
    /// If its activated when it detects a collision with a target..
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider other)
    {
      if (type != TriggerType.Exit)
        return;

      if (other.gameObject.CompareTag(this.targetType))
        this.Activate();
    }


  }

}