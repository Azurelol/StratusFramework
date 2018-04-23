using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Stratus
{
  /// <summary>
  /// Base class for all windows
  /// </summary>
  public abstract class BaseWindow : StratusBehaviour, Interfaces.Debuggable
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// The current state of the window
    /// </summary>
    public enum State { Opened, Suspended, Closed }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    //[HideInInspector]

    [Header("Window")]
    [Tooltip("The main canvas group for this window")]
    public CanvasGroup canvas;
    [Tooltip("The first button to be selected")]
    public Selectable defaultSelected;
    [Tooltip("Whether this window will manage selectables within its hierarchy")]
    public bool controlSelectables = true;
    [Tooltip("Whether to print debug output")]
    public bool debug = false;

    [Header("Transition")]
    [Range(0f, 3f)]
    [Tooltip("The speed of transition for this window")]
    public float transitionSpeed = 1f;
    [Tooltip("Whether this window should be open on start")]
    public bool openOnStart = false;

    [Header("Input")]
    [Tooltip("Whether input is being polled for this window")]
    public bool pollInput = false;
    public InputAxisField cancel = new InputAxisField();

    protected ActionSet currentSeq;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// A stack of all open windows
    /// </summary>
    protected static Stack<BaseWindow> windows { get; set; } = new Stack<BaseWindow>();
    /// <summary>
    /// Returns the current, selected window
    /// </summary>
    public BaseWindow current => windows.Count > 0 ? windows.Peek() : null;
    /// <summary>
    /// Whether this is the currently active window
    /// </summary>
    public bool isCurrent => windows.Count > 0 ? windows.Peek() == this : false;
    /// <summary>
    /// The state of this window
    /// </summary>
    public State state { get; protected set; }
    /// <summary>
    /// A provided callback whenever the current window changes
    /// </summary>
    public System.Action<BaseWindow> onWindowChange { get; set; }
    /// <summary>
    /// Whether this window is currently polling input
    /// </summary>
    public bool pollingInput { get; private set; }
    /// <summary>
    /// Whether this window should open
    /// </summary>
    protected virtual bool shouldOpen { get { return true; } }
    /// <summary>
    /// The current child window
    /// </summary>
    protected BaseWindow child { get; private set; }
    /// <summary>
    /// Whether this window currently has a child window
    /// </summary>
    protected bool hasChild { get; private set; }
    /// <summary>
    /// The current object within this window being selected
    /// </summary>
    public Selectable selected { get; protected set; }
    /// <summary>
    /// Whether this window is currently visible
    /// </summary>
    public bool visible { get; protected set; }
    /// <summary>
    /// Whether this window can be interacted with
    /// </summary>
    public bool interactable
    {
      get { return canvas.interactable; }
      protected set { canvas.interactable = value; }
    }
    /// <summary>
    /// All the selectables this window controls
    /// </summary>
    public Selectable[] selectables { get; protected set; }

    /// <summary>
    /// The current event system
    /// </summary>
    public static UnityEngine.EventSystems.EventSystem eventSystem => UnityEngine.EventSystems.EventSystem.current;

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Shows/hides this window
    /// </summary>
    /// <param name="show"></param>
    /// <param name="onFinished"></param>
    public void Transition(bool show, System.Action onFinished = null)
    {
      if (debug)
        Trace.Script(show, this);

      if (!show)
        eventSystem.SetSelectedGameObject(null);

      canvas.blocksRaycasts = show;
      canvas.interactable = show;
      pollingInput = show;

      currentSeq?.Cancel();
      currentSeq = Actions.Sequence(this);



      // Fade the canvas
      Actions.Property(currentSeq, () => canvas.alpha, show ? 1f : 0f, transitionSpeed, Ease.Linear);
      Actions.Call(currentSeq, () => { visible = false; });

      // Optionally, select the default button
      if (defaultSelected)
      {
        //Trace.Script($"Selecting {defaultSelected.name}", this);
        if (show) 
          Actions.Call(currentSeq, () => eventSystem.SetSelectedGameObject(defaultSelected.gameObject));
      }

      // Optionally, reset the state of all other selectables
      if (controlSelectables)
      {
        foreach(var selectable in selectables)
        {
          if (selectable == defaultSelected)
            continue;

          selectable.OnDeselect(null);
        }
      }

      // Now invoke any callbacks
      if (onFinished != null)
        Actions.Call(currentSeq, () => { onFinished(); });
    }

    void Interfaces.Debuggable.Toggle(bool toggle)
    {
      debug = toggle;
    }
  }

  /// <summary>
  /// Base class for all windows
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class BaseWindow<T> : BaseWindow
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Signals that this windows should open
    /// </summary>
    public class OpenEvent : Stratus.Event
    {
      /// <summary>
      /// The parent window
      /// </summary>
      public BaseWindow parent;
      /// <summary>
      /// Generic data container
      /// </summary>
      public object[] data;
      /// <summary>
      /// A provided callback for when the window is closed
      /// </summary>
      public System.Action onOpened;
      /// <summary>
      /// A provided callback for when the window is closed
      /// </summary>
      public System.Action onClosed;

      public OpenEvent()
      {
      }

      public OpenEvent(object[] data, System.Action onOpened = null, System.Action onClosed = null, BaseWindow parent = null)
      {
        this.parent = parent;
        this.data = data;
        this.onOpened = onOpened;
        this.onClosed = onClosed;
      }
    }

    /// <summary>
    /// Signals that this windows should close
    /// </summary>
    public class CloseEvent : Stratus.Event
    {
    }

    //--------------------------------------------------------------------------------------------/
    // Virtual
    //--------------------------------------------------------------------------------------------/
    protected virtual void OnAwake() {}
    protected virtual void OnStart() {}
    protected abstract void OnOpen();
    protected abstract void OnClose();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Latest data received by this window
    /// </summary>
    protected OpenEvent latest { get; set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    void Awake()
    {
      selectables = GetComponentsInChildren<Selectable>();
      onWindowChange += OnWindowChange;
      Scene.Connect<OpenEvent>(this.OnOpenEvent);
      Scene.Connect<CloseEvent>(this.OnCloseEvent);

      if (openOnStart)
        Open();
      else
      {
        Show(false);
        state = State.Closed;
      }

      OnAwake();
    }

    private void Start()
    {
      OnStart();
    }

    private void Update()
    {
      if (pollInput && pollingInput && isCurrent && cancel.isDown)
      {
        if (debug)
          Trace.Script("Received input to close window");
        Close();
      }

    }

    //private void OnValidate()
    //{
    //  Validate();
    //}

    //public void Validate()
    //{
    //  if (canvas)
    //  {
    //    if (openOnStart)
    //      canvas.alpha = 1f;
    //    else
    //      canvas.alpha = 0f;
    //  }
    //}

    //--------------------------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------------------------/
    void OnOpenEvent(OpenEvent e)
    {
      if (state != State.Closed || !shouldOpen)
      {
        if (debug)
          Trace.Script($"Cannot open this window!, state = {state}");
        return;
      }

      latest = e;
      Open();      
    }

    void OnCloseEvent(CloseEvent e)
    {
      if (!isCurrent)
        return;
      
      Close();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Static
    //--------------------------------------------------------------------------------------------/  
    public static void Open(OpenEvent e, bool transition = false)
    {
      // If there is a transition requested, transition out the parent window before opening the child window
      if (transition && e.parent != null)
      {
        e.onClosed += () =>
        {
          e.parent.Transition(true);
        };
        e.parent.Transition(false);

        var seq = Actions.Sequence(e.parent);
        Actions.Delay(seq, e.parent.transitionSpeed);
        Actions.Call(seq, () => Scene.Dispatch<OpenEvent>(e));
      }
      // Otherwise the child window right away
      else
      {
        Scene.Dispatch<OpenEvent>(e);
      }
    }

    public static void OpenAsChildWindow(BaseWindow parent, bool transition = false) 
    {
      var e = new OpenEvent();
      e.parent = parent;

      // If there is a transition requested, transition out the parent window before opening the child window
      if (transition && e.parent != null)
      {
        e.onClosed += () =>
        {
          e.parent.Transition(true);
        };
        e.parent.Transition(false);

        var seq = Actions.Sequence(e.parent);
        Actions.Delay(seq, e.parent.transitionSpeed);
        Actions.Call(seq, () => Scene.Dispatch<OpenEvent>(e));
      }

      // Otherwise the child window right away
      else
      {
        Scene.Dispatch<OpenEvent>(e);
      }
    }

    public static void RequestOpen(object[] args = null, System.Action onClosed = null)
    {
      Scene.Dispatch<OpenEvent>(new OpenEvent() { data = args, onClosed = onClosed });
    }

    public static void RequestClose(object[] args = null)
    {
      Scene.Dispatch<CloseEvent>(new CloseEvent());
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Private
    //--------------------------------------------------------------------------------------------/  
    public void Open()
    {
      if (debug)
        Trace.Script("", this);
      canvas.gameObject.SetActive(true);
      Transition(true, () =>
      {
        //pollingInput = true;
        //if (defaultSelected)
        //{
        //  Trace.Script($"Selecting button", this);
        //  eventSystem.SetSelectedGameObject(defaultSelected.gameObject);
        //}
        //defaultSelected?.Select();
        OnOpen();
        windows.Push(this);
        state = State.Opened;
        latest?.onOpened?.Invoke();
      });
    }

    public void Close()
    {
      if (debug)
        Trace.Script("", this);

      System.Action close = ()=>{
        canvas.gameObject.SetActive(false);
        OnClose();
        if (isCurrent) windows.Pop();
        state = State.Closed;
        latest?.onClosed?.Invoke();
      };

      if (visible)
      {
        Transition(false, () => {
          close();
        });
      }
      else
      {
        close();
      }
      
    }

    /// <summary>
    /// Opens another window
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="onOpen"></param>
    /// <param name="onClosed"></param>
    public void Open<OpenEvent>(OpenEvent e, bool hide = false) where OpenEvent : BaseWindow<MonoBehaviour>.OpenEvent, new()
    {
      // Save the current button
      selected = UnityEngine.EventSystems.EventSystem.current.gameObject.GetComponent<Button>();

      // Open the new window
      //U e = new U();
      //e.data = args;
      //e.onOpened = onOpen;
      //e.onClosed = onClosed;
      // If hiding, hide this window until the other is closed
      if (hide)
      {
        Transition(false, null);
        e.onClosed += () => { Transition(true, null); };
      }
      e.parent = this;
      Scene.Dispatch<OpenEvent>(e);

      // Update the current child window
      //child = currn
    }

    private void Show(bool show)
    {
      canvas.alpha = show ? 1f : 0f;
      canvas.blocksRaycasts = false;
      canvas.interactable = false;
      visible = show;
    }
    

    private void OnWindowChange(BaseWindow window)
    {
      // If the window was changed, and it's now recently
      if (current == this)
        selected.Select();
    }

    


  }

}