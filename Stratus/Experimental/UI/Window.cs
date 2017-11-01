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
using System.Collections;

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
      // Declarations
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

      public enum EventType { Open, Close }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      WindowTransition Transition;
      /// <summary>
      /// How long before the window can be closed
      /// </summary>
      float ClosingThreshold = 0.25f;
      /// <summary>
      /// Whether this window can be closed currently
      /// </summary>
      [HideInInspector] public bool CanBeClosed = false;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnWindowInitialize();
      protected abstract void OnWindowOpen();
      protected abstract void OnWindowClose();
      // Default behavior for a window when cancelled is to close
      protected virtual void OnWindowCancel() {}

      protected override void OnInterfaceInitialize()
      {
        // Check whether the window object has a transition add-on component.
        // If it does, we will be using it to transition in and out 
        this.Transition = GetComponent<WindowTransition>();
        // If there's no transition component, start this faded out!
        //if (this.Transition == null)
        //Trace.Script("Fading window out!");
        //Fade(0.0f, 0.0f);

        this.Subscribe();

        // Initialize the window subclass
        this.OnWindowInitialize();
      }

      protected override void OnInterfaceOpen()
      {
        // Prevent this window from being closed immediately
        //StartCoroutine(this.EnableClosingOfWindow());

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
        //Trace.Script("Hi!", this);
      }

      protected override void OnInterfaceCancel()
      {
        //Trace.Script("Window Cancel", this);
        this.OnWindowCancel();
        this.Close();
      }

      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/
      /// <summary>
      /// Subscribes to window-specific events
      /// </summary>
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

      /// <summary>
      /// Once the window is done opening, enable input
      /// </summary>
      /// <param name="e"></param>
      void OnOpenedEvent(OpenedEvent e)
      {
        this.SelectFirstLink();
      }

      /// <summary>
      /// Closes the window after a specified transition time
      /// </summary>
      /// <param name="e"></param>
      void OnCloseEvent(CloseEvent e)
      {
        //Trace.Script("Closing", this);
        float delay = 0.0f;
        if (this.Transition)
          delay = this.Transition.Duration + 0.05f;

        var seq = Actions.Sequence(this);
        Actions.Delay(seq, delay);
        Actions.Call(seq, base.Close);
      }

      void OnTransitionEvent(TransitionEvent e)
      {

      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
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


      public IEnumerator EnableClosingOfWindow()
      {
        Trace.Script("Preventing window from being closed for " + this.ClosingThreshold + " seconds!", this);
        this.CanBeClosed = false;
        yield return StartCoroutine(Routines.WaitForRealSeconds(this.ClosingThreshold));
        this.CanBeClosed = true;
        Trace.Script("This window can now be closed", this);
      }


    }

  }
}