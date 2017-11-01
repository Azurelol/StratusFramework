/******************************************************************************/
/*!
@file   LogEvent.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /// <summary>
  /// Simple event that logs a message to the console when triggered.
  /// </summary>
  public class LogEvent : Triggerable
  {
    public string message;

    protected override void OnAwake()
    {
      
    }

    protected override void OnTrigger()
    {
      Trace.Script(this.message, this);      
    }
    
  }
}
