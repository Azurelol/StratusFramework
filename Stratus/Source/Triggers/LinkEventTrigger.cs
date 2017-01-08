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
  /**************************************************************************/
  /*!
  @class LinkEventTrigger 
  */
  /**************************************************************************/
  public class LinkEventTrigger : EventTrigger
  {
    public enum TriggerType { Select, Deselect, Confirm, Cancel }
    public TriggerType Type = TriggerType.Confirm;

    /**************************************************************************/
    /*!
    @brief  Initializes the LinkEventDispatcher.
    */
    /**************************************************************************/
    protected override void OnInitialize()
    {
      switch (Type)
      {
        case TriggerType.Select:
          this.gameObject.Connect<Link.SelectEvent>(this.OnSelectEvent);
          break;
        case TriggerType.Deselect:
          this.gameObject.Connect<Link.DeselectEvent>(this.OnDeselectEvent);
          break;
        case TriggerType.Confirm:          
          this.gameObject.Connect<Link.ConfirmEvent>(this.OnConfirmEvent);
          break;
        case TriggerType.Cancel:
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