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
    /**************************************************************************/
    /*!
    @class Link 
    */
    /**************************************************************************/
    public abstract class Link : StratusBehaviour
    {
      //------------------------------------------------------------------------/
      // Nested classes
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
        public Color SelectedColor = Color.red;
        public Color ActiveColor = Color.blue;
        [Tooltip("How long the link takes to show/hide")]
        public float Transition = 0.5f;
      }

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
      /// A hidden link cannot be navigated to by other links.
      /// </summary>
      public bool Hidden = false;
      // The description of this link
      public LinkDescription Description;
      // The neighbouring links
      public LinkNavigation Navigation = new LinkNavigation();
      // The default link style rules
      [HideInInspector]
      public LinkStyle Style;
      // Whether this link has been activated
      [ReadOnly]
      public bool Active;
      // The interface this link has been registered to
      [HideInInspector]
      public LinkInterface Interface;
      [HideInInspector]
      public bool Tracing = false;
      //------------------------------------------------------------------------/    
      // Defaults
      Color DefaultColor;
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
        // Save the default settings
        if (Button) DefaultColor = this.Button.color;
        //DefaultScale = this.transform.localScale;
        // If navigation is set to automatic, look for neighbors
        //if (Navigation.Mode == LinkNavigation.NavigationMode.Automatic)
        //  this.AssignNeighbors();

        this.OnStart();
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
        this.Confirm();
      }

      void OnCancelEvent(CancelEvent e)
      {
        this.Cancel();
      }

      //************
      // Methods
      //------------
      public void Select()
      {
        //Trace.Script("Selected!", this);
        Highlight(this.Style.SelectedColor);
        this.OnDescribe();
        this.OnSelect();
      }
      public void Deselect()
      {
        //Trace.Script("Deselected!", this);
        Highlight(DefaultColor);
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
      /// Activates this link.
      /// </summary>
      void Activate()
      {
        //Trace.Script("Opened!", this);
        //Trace.Script("Activated", this);
        Active = true;
        Highlight(this.Style.ActiveColor);
        this.Interface.gameObject.Dispatch<OpenedEvent>(new OpenedEvent());
        this.OnActivate();
      }

      /// <summary>
      /// Deactivates this link.
      /// </summary>
      public void Deactivate()
      {
        //Trace.Script("Closed!", this);
        Highlight(this.Style.SelectedColor);
        this.Interface.gameObject.Dispatch<ClosedEvent>(new ClosedEvent());
        Active = false;
      }

      /// <summary>
      /// Shows or hides this link.
      /// </summary>
      /// <param name="show"></param>
      /// <param name="immediate">Whether this should be done immediately. </param>
      public void Show(bool show, bool immediate = false)
      {
        // Show
        if (show)
        {
          this.IsVisible = true;
          if (Button)
          {
            if (immediate)
              Button.CrossFadeAlpha(1.0f, 0.0f, true);
            else
              Button.CrossFadeAlpha(1.0f, this.Style.Transition, true);
          }
          if (Text)
          {
            if (immediate)
              Text.CrossFadeAlpha(1.0f, 0.0f, true);
            else
              Text.CrossFadeAlpha(1.0f, this.Style.Transition, true);
          }

          //Trace.Script("Displaying", this);
        }
        // Hide
        else
        {
          this.IsVisible = false;
          if (Button)
          {
            if (immediate)
              Button.CrossFadeAlpha(0.0f, 0.0f, true);
            else
              Button.CrossFadeAlpha(0.0f, this.Style.Transition, true);
          }
          if (Text)
          {
            if (immediate)
              Text.CrossFadeAlpha(0.0f, 0.0f, true);
            else
              Text.CrossFadeAlpha(0.0f, this.Style.Transition, true);
          }
        }
      }

      /// <summary>
      /// Highlights this link, changing its color
      /// </summary>
      /// <param name="enabled"></param>
      void Highlight(Color color)
      {
        // If the link is not supposed to be visible, do nothing
        // This will prevent clashes such as this overriding a crossfadealpha
        if (!IsVisible)
          return;

        //Trace.Script("Changing to " + color, this);
        if (Button) Button.CrossFadeColor(color, this.Style.Transition, true, true);
      }

      //************
      // Subclass
      //------------


      //protected virtual void OnConfirm() {}
      //protected virtual void OnCancel() {}


    }

  } 
}