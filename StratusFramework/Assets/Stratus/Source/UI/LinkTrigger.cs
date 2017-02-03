/******************************************************************************/
/*!
@file   LinkEventTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using Stratus.UI;
using System;

namespace Stratus
{
  /// <summary>
  /// An event for links
  /// </summary>
  public class LinkTrigger : EventTrigger
  {
    public Link.EventType Type = Link.EventType.Confirm;
    
    protected override void OnInitialize()
    {
      switch (Type)
      {
        case Link.EventType.Select:
          this.gameObject.Connect<Link.SelectEvent>(this.OnSelectEvent);
          break;
        case Link.EventType.Deselect:
          this.gameObject.Connect<Link.DeselectEvent>(this.OnDeselectEvent);
          break;
        case Link.EventType.Confirm:          
          this.gameObject.Connect<Link.ConfirmEvent>(this.OnConfirmEvent);
          break;
        case Link.EventType.Cancel:
          this.gameObject.Connect<Link.CancelEvent>(this.OnCancelEvent);
          break;
      }
    }

    void OnSelectEvent(Link.SelectEvent e)
    {
      this.Trigger();
    }

    void OnDeselectEvent(Link.DeselectEvent e)
    {
      this.Trigger();
    }

    void OnConfirmEvent(Link.ConfirmEvent e)
    {      
      this.Trigger();
    }

    void OnCancelEvent(Link.CancelEvent e)
    {
      this.Trigger();
    }

    protected override void OnEnabled()
    {
      
    }
  }

}