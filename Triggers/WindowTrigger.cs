/******************************************************************************/
/*!
@file   WnidowTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Stratus
{
  /// <summary>
  /// Trigger for a Window-related event.
  /// </summary>
  public class WindowTrigger : EventTrigger
  {
    public enum TriggerType { Opened, Closed }
    public TriggerType Type;

    protected override void OnEnabled()
    {
      
    }

    protected override void OnInitialize()
    {
      if (this.Type == TriggerType.Opened)
        this.gameObject.Connect<Stratus.UI.Window.OpenedEvent>(this.OnWindowOpened);
      else if (this.Type == TriggerType.Closed)
        this.gameObject.Connect<Stratus.UI.Window.ClosedEvent>(this.OnWindowClosed);
    }

    void OnWindowOpened(Stratus.UI.Window.OpenedEvent e)
    {
      this.Trigger();
    }

    void OnWindowClosed(Stratus.UI.Window.ClosedEvent e)
    {
      this.Trigger();
    }

  }
}
