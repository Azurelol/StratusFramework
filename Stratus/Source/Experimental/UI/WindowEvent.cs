/******************************************************************************/
/*!
@file   WindowEvent.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// An event for windows
    /// </summary>
    public class WindowEvent : Triggerable
    {
      public Window.EventType Type = Window.EventType.Open;
      public Window Target;

      protected override void OnAwake()
      {

      }

      protected override void OnTrigger()
      {
        switch (Type)
        {
          case Window.EventType.Open:
            Target.gameObject.Dispatch<Window.OpenEvent>(new Window.OpenEvent());
            break;
          case Window.EventType.Close:
            Target.gameObject.Dispatch<Window.CloseEvent>(new Window.CloseEvent());
            break;
        }
      }

    } 
  }
}
