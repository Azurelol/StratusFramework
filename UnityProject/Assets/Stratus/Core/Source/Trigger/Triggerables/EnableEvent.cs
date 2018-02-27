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
    public enum Action { Enable, Disable, Toggle }

    [Tooltip("The GameObject being toggled")]
    public GameObject target;
    [Tooltip("Whether the target is being enabled or disabled")]
    public Action action = Action.Enable;

    protected override void OnAwake()
    {      
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      switch (this.action)
      {
        case Action.Enable:
          this.target.SetActive(true);
          break;
        case Action.Disable:
          this.target.SetActive(false);
          break;
        case Action.Toggle:
          this.target.SetActive(!this.target.activeSelf);
          break;
      }
    }
  }

}