/******************************************************************************/
/*!
@file   LinkInterface.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Base class for the LinkInterfaceSystem.
    /// </summary>
    public abstract class LinkInterface : StratusBehaviour
    {
      // Signature of toggle functions that will get called?
      //public delegate void ToggleFunc(bool toggled);
      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/
      public class ToggleInputEvent : Stratus.Event
      {
        public bool Enabled;
        public ToggleInputEvent(bool enabled) { Enabled = enabled; }
      }
      //------------------------------------------------------------------------/
      public bool Tracing = false;
      [Tooltip("Whether this LinKInterface is active")]
      public bool Active;
      [Tooltip("Time before input is accepted after opening")]
      public float InputDelay = 0.25f;
      [HideInInspector]
      public bool IsAcceptingInput = false;
      protected LinkController Links;
      [ReadOnly]
      public Link CurrentLink;
      protected LinkLayer CurrentLayer { get { return Links.CurrentLayer; } }
      //Stack<Action> QueuedEvents = new Stack<Action>();
      //------------------------------------------------------------------------/
      // Inheritance
      //------------------------------------------------------------------------/
      protected abstract void OnInterfaceInitialize();
      protected abstract void OnInterfaceOpen();
      protected abstract void OnInterfaceClose();
      protected abstract void OnInterfaceConfirm();
      protected abstract void OnInterfaceCancel();
      protected virtual void OnLinkOpened() { }
      protected virtual void OnLinkClosed() { }
      protected virtual void OnLinkSelect() { }

      //------------------------------------------------------------------------/
      // Initialization
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes the LinkInterface, subscribing to all link navigation
      /// events, registering the link controller, etc...
      /// </summary>
      void Awake()
      {
        this.gameObject.Connect<Link.NavigateEvent>(this.OnNavigateEvent);
        this.gameObject.Connect<Link.ConfirmEvent>(this.OnConfirmEvent);
        this.gameObject.Connect<Link.CancelEvent>(this.OnCancelEvent);
        this.gameObject.Connect<Link.OpenedEvent>(this.OnLinkOpenedEvent);
        this.gameObject.Connect<Link.ClosedEvent>(this.OnLinkClosedEvent);
        this.gameObject.Connect<Link.SelectEvent>(this.OnLinkSelectEvent);

        // Look for the LinkController among the children
        Links = GetComponentInChildren<LinkController>();
        if (Links) Links.Connect(this);

        // Initialize the interface subclass
        this.OnInterfaceInitialize();

        // If it's already active, open it
        if (Active)
        {
          this.Open();
        }
        // Otherwise hide everyhting
        else
        {
          ToggleGraphics(false);
          ToggleInput(false, true);
        }

      }

      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------//
      /// <summary>
      /// Invoked upon receiving 'Navigational' input
      /// </summary>
      /// <param name="e"></param>
      void OnNavigateEvent(Link.NavigateEvent e)
      {
        if (!IsAcceptingInput)
        {
          //if (Tracing) Trace.Script("Not accepting input!", this);
          return;
        }

        if (!CurrentLink)
        {
          //if (Tracing) Trace.Script("No link available!", this);
          return;
        }

        //CurrentLink.Navigate 

        // If there is a link active, redirect input
        RedirectInput<Link.NavigateEvent>(e);
      }

      /// <summary>
      /// Invoked upon receiving a 'Confirm' input
      /// </summary>
      /// <param name="e"></param>
      void OnConfirmEvent(Link.ConfirmEvent e)
      {
        if (!IsAcceptingInput)
          return;

        // If there's no link to redirect output to,
        // send it directly to the interface
        if (!RedirectInput<Link.ConfirmEvent>(e))
          this.OnInterfaceConfirm();
      }

      /// <summary>
      /// Invoked upon receiving a 'Cancel' input
      /// </summary>
      /// <param name="e"></param>
      void OnCancelEvent(Link.CancelEvent e)
      {
        if (!IsAcceptingInput)
          return;

        // If there's no active link or the current link is not active, 
        // apply cancel to this interface
        if (!this.CurrentLink || (this.CurrentLink && !this.CurrentLink.Active))
          this.OnInterfaceCancel();

        // If there is a link active, redirect output
        if (RedirectInput<Link.CancelEvent>(e))
          return;

        //Trace.Script("Active = " + Active + ", CurrentLink = " + CurrentLink, this);
      }
      //-----------------------------------------------------------------/
      /// <summary>
      /// Invoked upon receiving a confirm event (from input)
      /// </summary>


      /// <summary>
      /// Invoked when a link has been selected.
      /// </summary>
      /// <param name="e"></param>
      void OnLinkSelectEvent(Link.SelectEvent e)
      {
        this.CurrentLink = e.Link;

        // If there's a layer associated with that link,
        // let's record that it is the current link in that layer
        if (CurrentLayer)
        {
          this.CurrentLayer.CurrentLink = this.CurrentLink;
        }

        this.OnLinkSelect();
        //Trace.Script("Current link now " + this.CurrentLink.name, this);
      }

      /// <summary>
      /// Invoked when a link has been opened (by this Interface)
      /// </summary>
      /// <param name="e"></param>
      void OnLinkOpenedEvent(Link.OpenedEvent e)
      {
        OnLinkOpened();
      }

      /// <summary>
      /// Invoked when a link has been closed.
      /// </summary>
      /// <param name="e"></param>
      void OnLinkClosedEvent(Link.ClosedEvent e)
      {
        OnLinkClosed();
      }

      /// <summary>
      /// Redirects input events to the window if there is one open.
      /// </summary>
      /// <typeparam name="U">The input event class</typeparam>
      /// <param name="inputEvent">The input event.</param>
      /// <returns>True if input was redirected, false otherwise. </returns>
      bool RedirectInput<U>(U inputEvent) where U : Stratus.Event
      {
        if (Active && CurrentLink)
        {
          //Trace.Script("Redirecting to '" + CurrentLink.name + "'", this);
          CurrentLink.gameObject.Dispatch<U>(inputEvent);
          return true;
        }

        //Trace.Script("Can't redirect!", this);
        return false;
      }

      /// <summary>
      /// Opens the link interface. This will call the subclass OnOpen as well.
      /// </summary>
      protected virtual void Open()
      {
        if (Tracing) Trace.Script("Opening interface!", this);
        Toggle(true);
        this.SelectFirstLink();
        this.OnInterfaceOpen();
        // How long to wait before accepting input
        var seq = Actions.Sequence(this);
        Actions.Property(seq, () => this.IsAcceptingInput, true, this.InputDelay, Ease.Linear);
      }

      /// <summary>
      /// Switches to a new link layer.
      /// </summary>
      /// <param name="layer">The layer which to switch to. </param>
      /// <param name="resetState">Whether to reset to the default state of this layer.</param>
      public void SwitchLayer(LinkLayer layer, bool resetState = true)
      {
        if (!Links.SwitchLayer(layer))
          return;

        // If resetting the state..
        ResetLayer(resetState);
      }

      /// <summary>
      /// Switches to a new link layer.
      /// </summary>
      /// <param name="layerName">The name layer which to switch to. </param>
      /// <param name="resetState">Whether to reset to the default state of this layer.</param>
      public void SwitchLayer(string layerName, bool resetState = true)
      {
        if (!Links.SwitchLayer(layerName))
          return;

        ResetLayer(resetState);        
      }

      /// <summary>
      /// Switches to the previous link layer.
      /// </summary>
      /// <param name="resetState"></param>
      public void SwitchLayerToPrevious(bool resetState = true)
      {
        if (!Links.SwitchLayerToPrevious())
          return;

        ResetLayer(resetState);
      }

      /// <summary>
      /// Resets the state of the current layer.
      /// </summary>
      /// <param name="resetState"></param>
      void ResetLayer(bool resetState)
      {
        // Select the first link
        if (resetState)
        {
          CurrentLayer.Reset();
          SelectFirstLink();
        }
        // Otherwise, go back to the previous state
        else
        {
          this.CurrentLink = CurrentLayer.Restore();
        }
      }

      /// <summary>
      /// Closes the link interface. This will call the subclass OnClose as well.
      /// </summary>
      protected virtual void Close()
      {
        if (Tracing) Trace.Script("Closing!", this);
        if (this.CurrentLink) this.CurrentLink.Deselect();
        this.OnInterfaceClose();
        Toggle(false);
        IsAcceptingInput = false;
      }

      /// <summary>
      /// Selects the first available link among all links.
      /// </summary>
      protected void SelectFirstLink()
      {
        if (Links)
          CurrentLink = Links.SelectFirstLink();

        // If there's a link available, select it
        if (CurrentLink)
        {
          CurrentLink.Select();
        }
        //Trace.Script("Selecting link!");
      }

      /// <summary>
      /// Selects the specified link.
      /// </summary>
      /// <param name="link"></param>
      protected void SelectLink(Link link)
      {
        this.CurrentLink.Deselect();
        this.CurrentLink = link;
        this.CurrentLink.Select();
      }

      /// <summary>
      /// Deselects the current link.
      /// </summary>
      protected void DeselectLink()
      {
        //Trace.Script("Deselecting link!");
        this.CurrentLink = null;
      }

      /// <summary>
      /// Selects, then opens the specified link.
      /// </summary>
      /// <param name="link"></param>
      protected void OpenLink(Link link)
      {
        //Trace.Script("Opening " + link.name, this);
        this.CurrentLink = link;
        this.CurrentLink.Select();
        CurrentLink.gameObject.Dispatch<Link.ConfirmEvent>(new Link.ConfirmEvent());
      }

      /// <summary>
      /// Selects, then opens the specified link.
      /// </summary>
      /// <param name="name">The name of the link.</param>
      protected void OpenLink(string name)
      {
        var link = Links.Find(name);

        

        SelectLink(link);
        link.Confirm();
      }

      /// <summary>
      /// Toggles the LinkInterface on and off.
      /// </summary>
      /// <param name="toggle"></param>
      public void Toggle(bool toggle)
      {
        Active = toggle;
        ToggleGraphics(toggle);
        ToggleInput(toggle);
      }

      /// <summary>
      /// Toggles graphics for the LinkInterface.
      /// </summary>
      /// <param name="showing"></param>
      public void ToggleGraphics(bool showing)
      {
        //if (showing)
        //  Graphical.Fade(this, 1.0f, 0.0f);
        //else
        //  Graphical.Fade(this, 0.0f, 0.0f);

        //Trace.Script("Graphics toggle = " + showing, this);
        foreach (var graphical in GetComponentsInChildren<Graphic>())
        {
          graphical.enabled = showing;
        }
      }

      /// <summary>
      /// Toggles input for the LinkInterface.
      /// </summary>
      /// <param name="enabled"></param>
      public void ToggleInput(bool enabled, bool nextFrame = false)
      {
        //Trace.Script("Toggle = " + enabled, this);
        this.gameObject.Dispatch<ToggleInputEvent>(new ToggleInputEvent(enabled), nextFrame);
      }

      /// <summary>
      /// Disables input for a specified duration.
      /// </summary>
      /// <param name="duration">How long should input be disabled for.</param>
      public void DisableInput(float duration)
      {
        //Trace.Script("Disabling input for " + duration + " seconds!", this);
        this.IsAcceptingInput = false;
        // How long to wait before accepting input
        var seq = Actions.Sequence(this);
        Actions.Property(seq, () => this.IsAcceptingInput, true, duration, Ease.Linear);
      }

      public void EnableLinks(bool enabled)
      {
        Links.Enable(enabled);
      }



    }
  }

}