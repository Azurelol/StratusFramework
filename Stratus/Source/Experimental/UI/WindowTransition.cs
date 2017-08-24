/******************************************************************************/
/*!
@file   WindowTransition.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Add-on for the Window class that handles automated transitions.
    /// </summary>
    [RequireComponent(typeof(Window))]
    public abstract class WindowTransition : MonoBehaviour
    {
      [Tooltip("Duration of the transition")]
      public float Duration = 1.0f;
      protected abstract void OnWindowInitialize();
      protected abstract void OnWindowOpen();
      protected abstract void OnWindowClose();
      protected Window Window { get { return GetComponent<Window>(); } }

      /// <summary>
      /// Initializes the WindowTransition script.
      /// </summary>
      void Awake()
      {
        //Trace.Script("Subsribing to events", this);
        this.OnWindowInitialize();
        this.gameObject.Connect<Window.OpenEvent>(this.OnWindowOpenEvent);
        this.gameObject.Connect<Window.CloseEvent>(this.OnWindowClosedEvent);
      }

      void OnWindowOpenEvent(Window.OpenEvent e)
      {
        OnWindowOpen();
        //var seq = Actions.Sequence(this);
        //Actions.Delay(seq, this.Duration);
        //Actions.Call(seq, this.ReportTransitionEnded);
      }

      void OnWindowClosedEvent(Window.CloseEvent e)
      {
        OnWindowClose();
        //var seq = Actions.Sequence(this);
        //Actions.Delay(seq, this.Duration);
        //Actions.Call(seq, this.ReportTransitionEnded);
      }

      void ReportTransitionEnded()
      {
        this.gameObject.Dispatch<Window.TransitionEvent>(new Window.TransitionEvent());

      }





    }

  } 
}