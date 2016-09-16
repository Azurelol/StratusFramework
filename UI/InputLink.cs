/******************************************************************************/
/*!
@file   InputLink.cs
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
  namespace UI
  {
    /// <summary>
    /// Redirects input either to specified callback functions or to a gameobject.
    /// </summary>
    public class InputLink : Link
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public enum RedirectionType { Events, Callbacks }

      /// <summary>
      /// The type of redirection to be used.
      /// </summary>
      public RedirectionType Type;
      // Events (target)
      public GameObject Target;
      // Callbacks
      public UnityEvent OnConfirmation = new UnityEvent();
      public UnityEvent OnCancellation = new UnityEvent();
      public NavigationCallback OnNavigation = new NavigationCallback();
      //public Direction Direction;
      //public UnityEvent OnUp = new UnityEvent();
      //public UnityEvent OnDown = new UnityEvent();
      //public UnityEvent OnLeft = new UnityEvent();
      //public UnityEvent OnRight = new UnityEvent();


      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Invoked when the link receives a navigation event.
      /// </summary>
      /// <param name="dir"></param>
      protected override void OnNavigate(Navigation dir)
      {
        //Trace.Script("Hey!");
        if (this.Type == RedirectionType.Events)
        {
          Forward<NavigateEvent>(new NavigateEvent(dir));
        }
        else if (this.Type == RedirectionType.Callbacks)
        {
          //Direction = dir;
          //OnNavigation = new 
          OnNavigation.Invoke(dir);

          //switch (dir)
          //{
          //  case Direction.Up:
          //    OnUp.Invoke();
          //    break;
          //  case Direction.Down:
          //    OnDown.Invoke();
          //    break;
          //  case Direction.Left:
          //    OnLeft.Invoke();
          //    break;
          //  case Direction.Right:
          //    OnRight.Invoke();
          //    break;
          //}
        }

      }

      protected override void OnActivate()
      {
      }

      /// <summary>
      /// Invoked when the link receives a confirm event.
      /// </summary>
      protected override void OnConfirm()
      {
        if (this.Type == RedirectionType.Events)
          Target.gameObject.Dispatch<ConfirmEvent>(new ConfirmEvent());
        else if (this.Type == RedirectionType.Callbacks)
          OnConfirmation.Invoke();

        // Close the link
        this.Deactivate();
      }

      protected override void OnCancel()
      {
        if (this.Type == RedirectionType.Events)
          Target.gameObject.Dispatch<CancelEvent>(new CancelEvent());
        else if (this.Type == RedirectionType.Callbacks)
          OnCancellation.Invoke();

        // Close the link
        //this.Deactivate();
      }

      protected override void OnDeselect()
      {
      }

      protected override void OnSelect()
      {
      }

      void Forward<T>(T forwardedEvent) where T : Stratus.Event
      {
        //Trace.Script("Redirecting event to " + this.Target.name);
        this.Target.gameObject.Dispatch<T>(forwardedEvent);
      }


    }

  } 
}