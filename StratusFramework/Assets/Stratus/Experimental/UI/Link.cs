/******************************************************************************/
/*!
@file   Link.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System;
using Stratus;

namespace Stratus
{
  namespace UI
  {
    /// <summary>
    /// The base class for all links in the framework.
    /// </summary>
    public abstract partial class Link : MonoBehaviour
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      [Serializable]
      public class LinkNavigation
      {
        public enum NavigationMode { Automatic, Manual }
        public NavigationMode Mode = NavigationMode.Automatic;
        public Link OnNavigateUp;
        public Link OnNavigateDown;
        public Link OnNavigateLeft;
        public Link OnNavigateRight;
      }

      [Serializable]
      public class LinkStyle
      {
        //public enum TransitionType { ColorTint, Animation }
        //public TransitionType Type = TransitionType.ColorTint;
        /// <summary>
        /// The animator controller for this link.
        /// </summary>
        public RuntimeAnimatorController Animator;
        //public bool Tint = true;
        public Font Font;
        public Color DefaultColor = Color.white;
        public Color SelectedColor = Color.red;
        public Color ActiveColor = Color.blue;
        public Color DisabledColor = Color.grey; 
        [HideInInspector] public Color Current;
        [Tooltip("How long the link takes to show/hide")]
        public float Duration = 0.5f;
      }      
            
      public enum EventType { Select, Deselect, Confirm, Cancel }
      public enum LinkState { Active, Default, Selected, Disabled, Hidden }

      [Serializable]
      public class LinkDescription
      {
        public enum DescriptionType { Title, Subtitle, Help }
        public DescriptionType Type;
        public string Message;
      }

      public enum NavigationMode { Automatic, Manual }

      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/
      public class DescriptionEvent : Stratus.Event
      {
        public LinkDescription Description;
        public DescriptionEvent(LinkDescription description) { Description = description; }
        public DescriptionEvent() { }
      }

      public class TransitionEvent : Stratus.Event
      {
        public LinkState State;
      }

      public abstract class LinkEvent : Stratus.Event { public Link Link; }

      /// <summary>
      /// Contains navigation data used for input in the system.
      /// </summary>
      public class NavigateEvent : LinkEvent
      {
        public Navigation Direction;
        public NavigateEvent(Navigation dir) { Direction = dir; }
      }

      /// <summary>
      /// Received by a link when it is selected by a link interface.
      /// </summary>
      public class SelectEvent : LinkEvent { }

      /// <summary>
      /// Received by a link when it is deselected by a link interface.
      /// </summary>
      public class DeselectEvent : LinkEvent { }

      /// <summary>
      /// Used for confirming the current link.
      /// </summary>
      public class ConfirmEvent : LinkEvent { }

      /// <summary>
      /// Used for cancelling the current link. 
      /// </summary>
      public class CancelEvent : Stratus.Event { }

      /// <summary>
      /// Used when a link has been opened.
      /// </summary>
      public class OpenedEvent : Stratus.Event { }

      /// <summary>
      /// Used when a link has been closed.
      /// </summary>
      public class ClosedEvent : Stratus.Event { }

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// Whether the link can be activated.
      /// </summary>
      [HideInInspector]
      public bool Enabled = true;      
      /// <summary>
      /// A hidden link cannot be navigated to by other links.
      /// </summary>
      public bool Hidden = false;
      /// <summary>
      ///The description of this link 
      /// </summary>
      public LinkDescription Description;
      /// <summary>
      /// The neighbouring links 
      /// </summary>
      public LinkNavigation Navigation = new LinkNavigation();
      // The default link style rules
      [HideInInspector]
      public LinkStyle Style;
      /// <summary>
      /// Whether this link has been activated 
      /// </summary>
      [ReadOnly] public bool Active;
      /// <summary>
      /// The interface this link has been registered to 
      /// </summary>
      [HideInInspector] public LinkInterface Interface;
      /// <summary>
      /// Whether this link is currently logging.
      /// </summary>
      [HideInInspector] public bool Tracing = false;
      /// <summary>
      /// The current state of this link
      /// </summary>
      LinkState State = LinkState.Default;
      //------------------------------------------------------------------------/    
      // Whether the link is currently being displayed
      bool IsVisible = true;
      //Vector2 DefaultScale;
      // The image used for this link
      Graphic[] Graphical { get { return GetComponentsInChildren<Graphic>(); } }
      public Image Button { get { return GetComponent<Image>(); } }
      public Text Text { get { return GetComponentInChildren<Text>(); } }

      //------------------------------------------------------------------------/
      // Virtual Methods
      //------------------------------------------------------------------------/
      protected abstract void OnSelect();
      protected abstract void OnDeselect();
      protected abstract void OnNavigate(Navigation dir);
      protected abstract void OnConfirm();
      protected abstract void OnCancel();
      protected abstract void OnActivate();
      protected virtual void OnStart() { }
      protected virtual void OnDescribe() { this.Interface.gameObject.Dispatch<DescriptionEvent>(new DescriptionEvent(this.Description)); }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/

      /// <summary>
      /// Initializes the Link.
      /// </summary>
      void Start()
      {
        // Subscribe to input events
        this.gameObject.Connect<NavigateEvent>(this.OnNavigateEvent);
        this.gameObject.Connect<ConfirmEvent>(this.OnConfirmEvent);
        this.gameObject.Connect<CancelEvent>(this.OnCancelEvent);
        this.gameObject.Connect<SelectEvent>(this.OnSelectEvent);
        this.gameObject.Connect<DeselectEvent>(this.OnDeselectEvent);
        //DefaultScale = this.transform.localScale;
        // If navigation is set to automatic, look for neighbors
        //if (Navigation.Mode == LinkNavigation.NavigationMode.Automatic)
        //  this.AssignNeighbors();

        this.OnStart();
      }

      void OnMouseDown()
      {
        Trace.Script("Ow!", this);
      }

      public void Initialize()
      {
        var seq = Actions.Sequence(this);
        Actions.Delay(seq, 0.1f);
        Actions.Call(seq, AssignNeighbors);
      }

      /// <summary>
      /// Looks for this link's neighbors amongst the parent's children.
      /// </summary>
      public void AssignNeighbors()
      {
        // If this is a hidden link, do not connect
        if (this.Hidden)
          return;
                
        //Trace.Script("Assigning!", this);

        // Reset the neighbors
        Navigation.OnNavigateUp = null; Navigation.OnNavigateDown = null;
        Navigation.OnNavigateLeft = null; Navigation.OnNavigateRight = null;

        // Look for neighboring links
        float closestRight = 0.0f, closestLeft = 0.0f, closestUp = 0.0f, closestDown = 0.0f;

        var links = transform.parent.GetComponentsInChildren<Link>();
        foreach (var link in links)
        {
          // If the link is hidden, do not connect
          if (link.Hidden)
            continue;

          var horizontalDistance = link.transform.position.x - transform.position.x;
          var verticalDistance = link.transform.position.y - transform.position.y;

          // Check for links stacked on each other!
          if (link != this && horizontalDistance == 0 && verticalDistance == 0)
          {
            var msg = new System.Text.StringBuilder();
            msg.AppendLine();        
            foreach (var neighbour in links) msg.AppendLine(neighbour.name + neighbour.transform.position);
            Trace.Error("Links placed at the same position! " + msg, this, true);
          }

          // Right 
          if (horizontalDistance > 0)
          {
            if (closestRight == 0.0f ||
                horizontalDistance < closestRight)
            {
              closestRight = horizontalDistance;
              Navigation.OnNavigateRight = link;
            }
          }
          // Left
          if (horizontalDistance < 0)
          {
            if (closestLeft == 0 ||
              horizontalDistance > closestLeft)
            {
              closestLeft = horizontalDistance;
              Navigation.OnNavigateLeft = link;
            }
          }
          // Up
          if (verticalDistance > 0)
          {
            if (closestUp == 0.0f ||
              verticalDistance < closestUp)
            {
              closestUp = verticalDistance;
              Navigation.OnNavigateUp = link;
            }
          }
          // Down
          if (verticalDistance < 0)
          {
            if (closestDown == 0.0f ||
              verticalDistance > closestDown)
            {
              closestDown = verticalDistance;
              Navigation.OnNavigateDown = link;
            }
          }
        }
      }      

      /// <summary>
      /// Navigates this link.
      /// </summary>
      /// <param name="dir">The direction of navigation.</param>
      /// <returns></returns>
      public Link Navigate(Navigation dir)
      {
        // If it's already active, pass navigation data to subclass
        //if (Active)
        //  this.OnNavigate

        // We will start by pointing at the current link
        Link link = this;

        // Deselect this link
        link.gameObject.Dispatch<DeselectEvent>(new DeselectEvent());
        //link.Deselect();

        switch (dir)
        {
          case UI.Navigation.Up:
            if (this.Navigation.OnNavigateUp)
              link = this.Navigation.OnNavigateUp;
            break;
          case UI.Navigation.Down:
            if (this.Navigation.OnNavigateDown)
              link = this.Navigation.OnNavigateDown;
            break;
          case UI.Navigation.Left:
            if (this.Navigation.OnNavigateLeft)
              link = this.Navigation.OnNavigateLeft;
            break;
          case UI.Navigation.Right:
            if (this.Navigation.OnNavigateRight)
              link = this.Navigation.OnNavigateRight;
            break;
        }

        //link.Select();
        //if (Navigation.OnNavigateRight) Trace.Script("nav right = " + Navigation.OnNavigateRight, this);
        //Trace.Script("Navigating to " + link.name + ", dir =  " + dir, this);

        var selectEvent = new SelectEvent();
        selectEvent.Link = link;
        // Select the new link
        link.gameObject.Dispatch<SelectEvent>(selectEvent);
        // Inform the interface this new link is being selected
        this.Interface.gameObject.Dispatch<SelectEvent>(selectEvent);

        return link;
      }

      //************
      // Events
      //------------
      void OnNavigateEvent(NavigateEvent e)
      {
        // If the link is not active, navigate to other links
        if (!Active)
        {
          Navigate(e.Direction);
        }
        // If it it's active, pass the navigation data to the subclass
        else
        {
          //Trace.Script("Active! Now redirecting to subclass... ", this);
          OnNavigate(e.Direction);
        }
      }

      void OnSelectEvent(SelectEvent e)
      {
        this.Select();
      }

      void OnDeselectEvent(DeselectEvent e)
      {
        this.Deselect();
      }

      void OnConfirmEvent(ConfirmEvent e)
      {
        if (!Enabled)
        {
          Trace.Script("Disabled!", this);
          return;
        }        

        this.Confirm();
      }

      void OnCancelEvent(CancelEvent e)
      {
        if (!Enabled)
        {
          Trace.Script("Disabled!", this);
          return;
        }
        this.Cancel();
      }

      //************
      // Methods
      //------------
      public void Select()
      {
        //Trace.Script("Selected!", this);
        TransitionTo(LinkState.Selected);
        this.OnDescribe();
        this.OnSelect();
      }
      public void Deselect()
      {
        //Trace.Script("Deselected!", this);
        TransitionTo(LinkState.Default);
        this.OnDeselect();
      }

      public void Confirm()
      {
        // If the link is not active, activate it
        if (!Active)
          this.Activate();
        // Otherwise, pass the confirm event      
        else
          this.OnConfirm();
      }

      public void Cancel()
      {
        // If the link is active, deactivate it
        if (Active)
          this.Deactivate();

        // Othewise, call its cancel method
        this.OnCancel();
      }

      /// <summary>
      /// Enables this link.
      /// </summary>
      /// <param name="enabled"></param>
      public void Enable(bool enabled)
      {
        Enabled = enabled;
        if (enabled)
          TransitionTo(LinkState.Default);
        else
          TransitionTo(LinkState.Disabled);
      }

      /// <summary>
      /// Activates this link.
      /// </summary>
      void Activate()
      {
        //Trace.Script("Opened!", this);
        //Trace.Script("Activated", this);
        Active = true;
        this.TransitionTo(LinkState.Active);        
        this.Interface.gameObject.Dispatch<OpenedEvent>(new OpenedEvent());
        this.OnActivate();
      }

      /// <summary>
      /// Deactivates this link.
      /// </summary>
      public void Deactivate()
      {
        //Trace.Script("Closed!", this);
        this.TransitionTo(LinkState.Selected);
        this.Interface.gameObject.Dispatch<ClosedEvent>(new ClosedEvent());
        Active = false;
      }



      //************
      // Subclass
      //------------


      //protected virtual void OnConfirm() {}
      //protected virtual void OnCancel() {}


    }

  } 
}