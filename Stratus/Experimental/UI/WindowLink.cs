/******************************************************************************/
/*!
@file   WindowLink.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  namespace UI
  {
    /**************************************************************************/
    /*!
    @class WindowLink 
    */
    /**************************************************************************/
    public class WindowLink : Link
    {
      public Window Target;

      protected override void OnActivate()
      {
        // Request the window to open
        Trace.Script("Opening window!", this);
        Target.gameObject.Dispatch<Window.OpenEvent>(new Window.OpenEvent());
      }

      void OnWindowDescriptionEvent(Window.DescriptionEvent e)
      {
        // Redirect to the parent
        Interface.gameObject.Dispatch<Window.DescriptionEvent>(e);
      }

      protected override void OnSelect()
      {
      }

      protected override void OnDeselect()
      {
      }

      protected override void OnConfirm()
      {
        //Trace.Script("Forwarding ", this);
        Forward<ConfirmEvent>(new ConfirmEvent());
      }

      protected override void OnNavigate(Navigation dir)
      {
        Forward<NavigateEvent>(new NavigateEvent(dir));
      }

      protected override void OnCancel()
      {
        Forward<CancelEvent>(new CancelEvent());

        // If the window has been deactivated, deactivate this link as well
        if (Target.Active == false)
        {
          this.Deactivate();
        }
      }

      void Forward<T>(T forwardedEvent) where T : Stratus.Event
      {
        this.Target.gameObject.Dispatch<T>(forwardedEvent);
      }


    }

  } 
}