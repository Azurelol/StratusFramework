/******************************************************************************/
/*!
@file   InvokeDispatcher.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using UnityEngine.Events;

namespace Stratus
{
  /// <summary>
  /// Invokes a method when triggered.
  /// </summary>
  public class InvokeEvent : Triggerable
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public UnityEvent callbacks = new UnityEvent();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {      
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      callbacks.Invoke();
    }
  }

}