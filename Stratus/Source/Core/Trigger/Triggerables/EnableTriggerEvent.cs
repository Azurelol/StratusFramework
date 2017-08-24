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
  public class EnableTriggerEvent : Triggerable
  {
    public Trigger Target;
    public bool Enabled = true;

    protected override void OnAwake()
    {
      
    }

    protected override void OnTrigger()
    {
      Target.gameObject.Dispatch<Trigger.EnableEvent>(new Trigger.EnableEvent(this.Enabled));
    }
  }

}