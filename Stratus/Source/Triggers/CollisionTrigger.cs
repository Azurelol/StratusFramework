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
  /**************************************************************************/
  /*!
  @class CollisionTrigger 
  */
  /**************************************************************************/
  public class CollisionTrigger : EventTrigger
  {
    public enum TriggerType { Enter, Exit }
    public TriggerType Type;
    public Transform CollisionTarget;  

    /**************************************************************************/
    /*!
    @brief  Initializes the CollisionTrigger.
    */
    /**************************************************************************/
    protected override void OnInitialize()
    {      
    }

    protected override void OnEnabled()
    {
    }

    /**************************************************************************/
    /*!
    @brief If its activated when it detects a collision with a target..
    */
    /**************************************************************************/
    void OnCollisionEnter(Collision collision)
    {
      if (Type != TriggerType.Enter)
        return;      
      
      if (collision.collider.gameObject.transform == this.CollisionTarget)
      {
        this.Trigger();
      }
    }

    /**************************************************************************/
    /*!
    @brief If its activated when it detects a collision with a target..
    */
    /**************************************************************************/
    void OnCollisionExit(Collision collision)
    {
      if (Type != TriggerType.Exit)
        return;

      if (collision.collider.gameObject == this.CollisionTarget)
        this.Trigger();
    }


  }

}