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
  public class InvokeDispatcher : EventDispatcher
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public UnityEvent Method = new UnityEvent();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected override void OnInitialize()
    {      
    }

    protected override void OnTrigger()
    {
      Method.Invoke();
    }
  }

}