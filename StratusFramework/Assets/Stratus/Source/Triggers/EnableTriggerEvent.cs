/******************************************************************************/
/*!
@file   EnableTriggerEvent.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Stratus
{
  /// <summary>
  /// An event which enables another trigger.
  /// </summary>
  public class EnableTriggerEvent : EventDispatcher
  {
    public EventTrigger Target;
    public bool Enabled = true;

    protected override void OnInitialize()
    {
      
    }

    protected override void OnTrigger()
    {
      Target.gameObject.Dispatch<EventTrigger.EnableEvent>(new EventTrigger.EnableEvent(this.Enabled));
    }
  }

}