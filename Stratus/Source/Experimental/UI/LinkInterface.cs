/******************************************************************************/
/*!
@file   LinkInterface.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;


namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// Base class for the LinkInterfaceSystem.
    /// </summary>
    public abstract partial class LinkInterface : MonoBehaviour
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      public class ToggleInputEvent : Stratus.Event
      {
        public bool Enabled;
        public ToggleInputEvent(bool enabled) { Enabled = enabled; }
      }

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/      
      /// <summary>
      /// Whether to print debug output
      /// </summary>
      [Tooltip("Whether to print debug output")]
      public bool Tracing = false;
      /// <summary>
      /// Whether this link interface is active
      /// </summary>
      [Tooltip("Whether this link interface is active")]
      public bool Active;
      /// <summary>
      /// Time before input is accepted after opening
      /// </summary>
      [Tooltip("Time before input is accepted after opening")]
      public float InputDelay = 0.25f;      
      /// <summary>
      /// The manager for all links
      /// </summary>
      [HideInInspector]
      public LinkController Controller;
      /// <summary>
      /// The currently selected link
      /// </summary>
      [Tooltip("The currently selected link")]
      [ReadOnly]
      public Link CurrentLink;
      /// <summary>
      ///Whether this link interface is accepting input
      /// </summary>
      public bool IsAcceptingInput { get { return _IsAcceptingInput; } }


      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// Whether this link interface is accepting input
      /// </summary>      
      protected bool _IsAcceptingInput = true;
      /// <summary>
      /// The current layer
      /// </summary>
      protected LinkLayer CurrentLayer { get { return Controller.CurrentLayer; } }

      //------------------------------------------------------------------------/
      // Interface
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
        Controller = GetComponentInChildren<LinkController>();
        if (Controller) Controller.Connect(this);

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
          ToggleControls(false, true);
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
        //Trace.Script("hey!");
        if (!_IsAcceptingInput)
        {
          if (Tracing) Trace.Script("Not accepting input!", this);
          return;
        }

        if (!CurrentLink)
        {
          if (Tracing) Trace.Script("No link available!", this);
          return;
        }
        
        // If there is a link active, redirect input
        RedirectInput<Link.NavigateEvent>(e);
      }

      /// <summary>
      /// Invoked upon receiving a 'Confirm' input
      /// </summary>
      /// <param name="e"></param>
      void OnConfirmEvent(Link.ConfirmEvent e)
      {
        if (!_IsAcceptingInput)
        {
          //Trace.Script("Not accepting input!", this);
          return;
        }

        //Trace.Script("Confirm", this);

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
        if (!_IsAcceptingInput)
          return;

        //Trace.Script("Cancel", this);

        // If there's no active link or the current link is not active, 
        // apply cancel to this interface
        if (!this.CurrentLink || (this.CurrentLink && !this.CurrentLink.Active))
        {
            this.OnInterfaceCancel();
        }
          

        // If there is a link active, redirect output
        if (RedirectInput<Link.CancelEvent>(e))
          return;

        //Trace.Script("Active = " + Active + ", CurrentLink = " + CurrentLink, this);
      }

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
          this.CurrentLayer.currentLink = this.CurrentLink;
        }

        this.OnLinkSelect();
        if (Tracing) Trace.Script("Current link now " + this.CurrentLink.name, this);
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
          if (Tracing) Trace.Script("Redirecting to '" + CurrentLink.name + "'", this);
          CurrentLink.gameObject.Dispatch<U>(inputEvent);
          return true;
        }

        if (Tracing) Trace.Script("Can't redirect!", this);
        return false;
      }

      /// <summary>
      /// Opens the link interface. This will call the subclass OnOpen as well.
      /// </summary>
      protected void Open()
      {
        // If the link interface is already open, do nothing
        if (Active) return;
        if (Tracing) Trace.Script("Opening interface!", this);

        Toggle(true);
        this.SelectFirstLink();
        this.OnInterfaceOpen();
        // How long to wait before accepting input
        //var seq = Actions.Sequence(this);
        //Actions.Property(seq, () => this.IsAcceptingInput, true, this.InputDelay, Ease.Linear);
      }

      /// <summary>
      /// Closes the link interface. This will call the subclass OnClose as well.
      /// </summary>
      protected void Close()
      {
        // If the link interface is already closed, do nothing
        if (!Active) return;
        if (Tracing) Trace.Script("Closing!", this);

        if (this.CurrentLink) this.CurrentLink.Deselect();
        this.OnInterfaceClose();
        Toggle(false);
        this.DisableInput();
      }





    }
  }

}