/******************************************************************************/
/*!
@file   EnableEvent.cs
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
  /// Enables a specified GameObject.
  /// </summary>
  public class EnableEvent : Triggerable
  {
    public enum TargetType { Component, GameObject }

    public Behaviour Target;
    public TargetType Type = TargetType.Component;
    [Tooltip("Whether the target is being enabled or disabled")]
    public bool Enabled = true;

    protected override void OnAwake()
    {      
    }

    protected override void OnTrigger()
    {
      //Trace.Script("Enabling!");
      switch (this.Type)
      {
        case TargetType.Component:
          this.Target.enabled = this.Enabled;
          break;
        case TargetType.GameObject:
          this.Target.gameObject.SetActive(this.Enabled);
          break;
      }
    }
  }

}