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
  /// Triggers an event when its collider collides with a GameObject with the
  /// given specified tag.
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class CollisionTrigger : EventTrigger
  {    
    public enum CollisionType { Collision, Trigger }
    public enum TriggerType { Enter, Exit }

    [Header("Collision")]
    public TriggerType Type;
    [Tooltip("What targets we are allowed to collide with")] [Tag] 
    public string Tag;  

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
      if (Type != TriggerType.Enter)
        return;

      if (other.gameObject.CompareTag(this.Tag))      
        this.Trigger();
      
    }
       

    /// <summary>
    /// If its activated when it detects a collision with a target..
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider other)
    {
      if (Type != TriggerType.Exit)
        return;

      if (other.gameObject.CompareTag(this.Tag))
        this.Trigger();
    }


  }

}