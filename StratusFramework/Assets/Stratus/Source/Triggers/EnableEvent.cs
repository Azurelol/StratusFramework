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
  public class EnableEvent : EventDispatcher
  {
    public GameObject Target;
    public bool Enabling = true;

    protected override void OnInitialize()
    {
      
    }

    protected override void OnTrigger()
    {
      //Trace.Script("Enabling!");
      this.Target.SetActive(this.Enabling);
    }
  }

}