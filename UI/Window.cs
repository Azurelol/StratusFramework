/******************************************************************************/
/*!
@file   Window.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Base window class for the LinkInterfaceSystem.
    /// </summary>
    public abstract class Window : LinkInterface
    {
      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/
      public class RedirectInputEvent : Stratus.Event { public Window Window; }
      public class OpenEvent : Stratus.Event { }
      public class CloseEvent : Stratus.Event { }
      public class OpenedEvent : Stratus.Event { }
      public class ClosedEvent : Stratus.Event { }
      public class TransitionEvent : Stratus.Event { }

      public class DescriptionEvent : Stratus.Event
      {
        public string Title;
        public string Description;
        public DescriptionEvent(string title, string description)
        {
          Title = title;
          Description = description;
        }
      }
      //------------------------------------------------------------------------/
      WindowTransition Transition;
      //------------------------------------------------------------------------/
      protected abstract void OnWindowInitialize();
      protected abstract void OnWindowOpen();
      protected abstract void OnWindowClose();
      // Default behavior for a window when cancelled is to close
      protected virtual void OnWindowCancel() { this.Close(); }
      //------------------------------------------------------------------------/    

      /**************************************************************************/
      /*!
      @brief  Initializes the Script.
      */
      /**************************************************************************/
      protected override void OnInterfaceInitialize()
      {
        // Check whether the window object has a transition add-on component.
        // If it does, we will be using it to transition in and out 
        this.Transition = GetComponent<WindowTransition>();
        // If there's no transition component, start this faded out!
        //if (this.Transition == null)
        //Trace.Script("Fading window out!");
        //Fade(0.0f, 0.0f);

        // Subscribe to window-specific events
        this.Subscribe();

        // Initialize the window subclass
        this.OnWindowInitialize();
      }

      void Subscribe()
      {
        //Trace.Script("Subsribing to events", this);
        this.gameObject.Connect<OpenEvent>(this.OnOpenEvent);
        this.gameObject.Connect<CloseEvent>(this.OnCloseEvent);
        this.gameObject.Connect<TransitionEvent>(this.OnTransitionEvent);
      }

      void OnOpenEvent(OpenEvent e)
      {
        Open();
      }

      void OnOpenedEvent(OpenedEvent e)
      {
        // Once the window has done opening, enable input
        this.SelectFirstLink();
      }

      void OnCloseEvent(CloseEvent e)
      {
        Close();
      }

      void OnTransitionEvent(TransitionEvent e)
      {

      }

      /// <summary>
      /// Opens this Window.
      /// </summary>
      //protected override void Open()
      //{
      //  base.Open();
      //
      //  float delay = 0.0f;      
      //  // If  there's a transition component, wait for its transition before enabling input
      //  if (this.Transition)
      //    delay = this.Transition.Duration + 0.05f;      
      //
      //  var seq = Actions.Sequence(this);
      //  Actions.Delay(seq, delay);
      //  Actions.Call(seq, this.SelectFirstLink);
      //}

      /// <summary>
      /// Closes this Window.
      /// </summary>
      protected override void Close()
      {
        //Trace.Script("Close called!", this);
        float delay = 0.0f;
        // If there's a transition component, wait for its transition to finish before closing
        if (this.Transition)
          delay = this.Transition.Duration + 0.05f;

        var seq = Actions.Sequence(this);
        Actions.Delay(seq, delay);
        Actions.Call(seq, base.Close);
      }

      //void DelayOnTransition(System.Delegate func, float delay = 0.0f)
      //{
      //  // If  there's a transition component, wait for its transition before enabling input
      //  if (this.Transition)
      //  {
      //    delay = this.Transition.Duration + 0.1f;
      //  }
      //
      //  var seq = Actions.Sequence(this);
      //  Actions.Delay(seq, delay);
      //  Actions.Call(seq, func);
      //}

      protected override void OnInterfaceOpen()
      {
        this.Fit();
        // If there's no traFade in instantly
        //this.Fade(1.0f, 0.0f);
        this.OnWindowOpen();
        this.gameObject.Dispatch<OpenedEvent>(new OpenedEvent());
      }

      protected override void OnInterfaceClose()
      {
        // Fade out
        //this.Fade(0.0f, 0.0f);
        this.OnWindowClose();
        this.gameObject.Dispatch<ClosedEvent>(new ClosedEvent());
      }

      protected override void OnInterfaceConfirm()
      {
      }

      protected override void OnInterfaceCancel()
      {
        this.OnWindowCancel();
      }



      /// <summary>
      /// Resizes and repositions this Window to fit on the canvas.
      /// </summary>
      protected void Fit()
      {
        // Center this window      
        this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
      }

      /// <summary>
      /// Requests this Window to open.
      /// </summary>
      public void RequestOpen()
      {
        this.gameObject.Dispatch<OpenEvent>(new OpenEvent());
      }

      /// <summary>
      /// Requests this Window to close.
      /// </summary>
      public void RequestClose()
      {
        this.gameObject.Dispatch<CloseEvent>(new CloseEvent());
      }

      /// <summary>
      /// Tweens the alpha of this window and all its children.
      /// </summary>
      /// <param name="alpha"></param>
      /// <param name="duration"></param>
      void Fade(float alpha, float duration)
      {
        foreach (var graphical in GetComponentsInChildren<Graphic>())
        {
          graphical.CrossFadeAlpha(alpha, duration, true);
        }
      }



    }

  } 
}