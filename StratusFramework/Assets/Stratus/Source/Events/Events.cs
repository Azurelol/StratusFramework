/******************************************************************************/
/*!
@file   Events.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@brief  The main event system class that acts as a proxy/operator for all events.
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Stratus
{
  // By type
  using DelegateTypeList = List<Delegate>;
  using SortedDelegateTypeList = Dictionary<string, List<Delegate>>;
  using DelegateTypeContainer = Dictionary<GameObject, Dictionary<string, List<Delegate>>>;
  
  /// <summary>
  /// The class which manages the overlying event system.
  /// </summary>
  public class Events : Singleton<Events>
  {
    public class TracingSetup
    {
      public bool Construction = false;
      public bool Register = false;
      public bool Connect = false;
      public bool Dispatch = false;
    }

    //------------------------------------------------------------------------/
    // Properties    
    //------------------------------------------------------------------------/
    // Whether debugging output is present
    /// <summary>
    /// Whether we are doing tracing for debugging purposes.
    /// 
    /// </summary>
    public static new TracingSetup Tracing = new TracingSetup();    
    /// <summary>
    /// A container of all the delegates for every GameObject
    /// </summary>
    DelegateTypeContainer Delegates;        
    /// <summary>
    /// A dictionary of all components that have connected to events, and a list of their delegates.
    /// </summary>
    Dictionary<Behaviour, List<GameObject>> ConnectedComponents = new Dictionary<Behaviour, List<GameObject>>();
    /// <summary>
    /// A list of all event types that are being watched for at the moment.
    /// </summary>
    List<string> WatchList = new List<string>();

    //------------------------------------------------------------------------/
    // Instancing    
    //------------------------------------------------------------------------/
    protected override string Name
    {
      get
      {
        return "Event System";
      }
    }
    
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the event system manager.
    /// </summary>
    protected override void OnAwake()
    {
      DontDestroyOnLoad(this);
      Delegates = new DelegateTypeContainer();
    }    

    /// <summary>
    /// Connects to the event of a given object.
    /// </summary>
    /// <typeparam name="T">The event class. </typeparam>
    /// <param name="gameObj">The GameObject we are connecting to whose events we are connecting to. </param>
    /// <param name="memFunc">The member function to connect to. </param>
    public static void Connect<T>(GameObject gameObj, Action<T> memFunc)
    {
      var key = typeof(T).ToString();

      // If the GameObject hasn't registered yet, add its key
      if (!Events.Instance.Delegates.ContainsKey(gameObj))
      {
        Register(gameObj);
      }

      // If the event has no delegates yet, add it
      if (!Events.Instance.Delegates[gameObj].ContainsKey(key))
      {
        Events.Instance.Delegates[gameObj].Add(key, new DelegateTypeList());
      }

      // If the delegate is already present, do not add it
      if (Events.Instance.Delegates[gameObj][key].Contains(memFunc))
        return;

      // Add the component's delegate onto the gameobject
      AddDelegate(gameObj, key, memFunc);
      //Events.Instance.Delegates[gameObj][key].Add(memFunc);
      // Record that this component has connected to this GameObject
      Register((MonoBehaviour)memFunc.Target, gameObj);

      if (Tracing.Connect)
        Trace.Script(memFunc.ToString() + " has connected to " + gameObj.name);
      //Trace.Script(obj.name + " now has '" + Events.Instance.DelegatesByType[obj].Count + "' delegates");
    }

    /// <summary>
    /// Disconnects this component from all events it has subscribed to.
    /// </summary>
    /// <param name="component"></param>
    public static void Disconnect(Behaviour component)
    {
      if (Quitting)
        return;

      // If the component is already connected and present in the event system
      if (Instance.ConnectedComponents.ContainsKey(component))
      {
        // For every gameobject that it has connected to
        foreach(var gameobj in Instance.ConnectedComponents[component])
        {
          // Disconnect its delegates from it
          DisconnectProcedure(component, gameobj);
        }
      }

      // Remove the component from the event system
      Instance.ConnectedComponents.Remove(component);
    }    
        

    /// <summary>
    /// Disconnects this component from all events it has subscribed on
    /// the given GameoObject.
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="component"></param>
    public static void Disconnect(Behaviour component, GameObject gameObj)
    {
      DisconnectProcedure(component, gameObj);
      // Remove the gameobject from the component's list in the system
      Instance.ConnectedComponents[component].Remove(gameObj);
    }

    /// <summary>
    /// Disconnects this component from all events it has subscribed on
    /// the given GameoObject.
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="component"></param>
    static void DisconnectProcedure(Behaviour component, GameObject gameObj)
    {
      // If the GameObject has been removed previously...
      if (!Events.Instance.Delegates.ContainsKey(gameObj))
        return;

      // For every delegate this GameObject has
      foreach (var pair in Events.Instance.Delegates[gameObj])
      {        
        // For every delegate in the list
        foreach (var deleg in pair.Value)
        {          
          if ((MonoBehaviour)deleg.Target == component)
          {
            if (Tracing.Connect)
              Trace.Script("Disconnecting <i>" + deleg.Method.Name + "</i> from " + gameObj.name);
            // Remove this delegate
            pair.Value.Remove(deleg);
            break;
          }
        }
      }
    }
    
    /// <summary>
    /// Dispatches the given event of the specified type onto the object.
    /// </summary>
    /// <typeparam name="T">The event class.</typeparam>
    /// <param name="obj">The object to which to connect to.</param>
    /// <param name="eventObj">The name of the event to which to listen for.</param>
    /// <param name="nextFrame">Whether to send this event on the next frame.</param>
    public static void Dispatch<T>(GameObject obj, T eventObj, bool nextFrame = false) where T : Event
    {
      var key = typeof(T).ToString();

      if (Tracing.Connect)
        Trace.Script("'" + key + "' to '" + obj.name + "'");
      
      // If this a delayed dispatch...
      if (nextFrame)
      {
        //Trace.Script("Delayed dispatch!");
        Instance.StartCoroutine(DispatchNextFrame<T>(obj, eventObj));
      }

      // Check if the object has been registered onto the event system.
      // If not, it will be.
      CheckRegistration(obj);

      // If there is no delegate registered to this object, do nothing.
      if (!HasDelegate(obj, key))
      {
        if (Tracing.Dispatch)
          Trace.Script("No delegate registered to " + obj.name + " for " + eventObj.ToString());
        return;
      }

      // If we are watching events of this type
      bool watching = false;
      if (Instance.WatchList.Contains(key))
        watching = true;


      // Invoke the method for every delegate
      foreach (var deleg in Events.Instance.Delegates[obj][key])
      {
        // If we are watching events of this type
        if (watching)
          Trace.Script("Invoking member function on " + deleg.Target.ToString());        

        deleg.DynamicInvoke(eventObj);      
      }

    }

    /// <summary>
    /// Dispatches the event on the next frame.
    /// </summary>
    /// <typeparam name="T">The event class.</typeparam>
    /// <param name="obj">The object to which to dispatch to.</param>
    /// <param name="eventObj">The event object we are sending.</param>
    /// <returns></returns>
    public static IEnumerator DispatchNextFrame<T>(GameObject obj, T eventObj) where T : Event
    {
      // Wait 1 frame
      yield return 0;
      // Dispatch the event
      //Trace.Script("Now dispatching!");
      Dispatch<T>(obj, eventObj);
    }

    /// <summary>
    /// Dispatches the given event of the specified type onto the GameObject amd all its children.
    /// </summary>
    /// <typeparam name="T">The event class. </typeparam>
    /// <param name="gameObj">The GameObject to which to dispatch to.</param>
    /// <param name="eventObj">The event object. </param>
    public static void DispatchDown<T>(GameObject gameObj, T eventObj, bool nextFrame = false) where T : Event
    {
      foreach(var child in gameObj.Children())
      {
        Dispatch<T>(child, eventObj, nextFrame);
      }

    }

    /// <summary>
    /// Dispatches an event up the tree on each parent recursively.
    /// </summary>
    /// <typeparam name="T">The event class. </typeparam>
    /// <param name="gameObj">The GameObject to which to dispatch to.</param>
    /// <param name="eventObj">The event object. </param>
    public static void DispatchUp<T>(GameObject gameObj, T eventObj, bool nextFrame = false) where T : Event
    {
      var parents = gameObj.transform.GetComponentsInParent<Transform>();
      foreach(var parent in parents)
      {
        Dispatch<T>(parent.gameObject, eventObj, nextFrame);
      }
    }

      /// <summary>
      /// Checks if the GameObject has been the given delegate.
      /// </summary>
      /// <param name="obj">A reference to the GameObject.</param>
      /// <param name="key">The key to the delegate list.</param>
      /// <returns>True if it has the delegate, false otherwise.</returns>
      static bool HasDelegate(GameObject obj, string key)
    {
      if (Events.Instance.Delegates[obj] != null
          && Events.Instance.Delegates[obj].ContainsKey(key))
      {
        return true;
      }

      if (Tracing.Dispatch)
      //if (true)
      {
        Trace.Script("Events of type '" + key + "' for '" + obj.name + "' have no delegates yet!");

        //string keys = "";
        //foreach(var keyPresent in Events.Instance.DelegatesByType[obj])
        //{
        //  keys += keyPresent.Key + " ";
        //}
        //Trace.Script("Its current events are: " + keys);
      }
      return false;
    }

    /// <summary>
    /// Checks if the GameObject has been registered onto the event system.
    /// </summary>
    /// <param name="gameObj">A reference to the GameObject. </param>
    static void CheckRegistration(GameObject gameObj)
    {
      // If the GameObject hasn't registered yet, add its key
      if (!Events.Instance.Delegates.ContainsKey(gameObj))
      {
        Events.Register(gameObj);
      }
    }

    /// <summary>
    /// Registers the GameObject to the event system.
    /// </summary>
    /// <param name="gameObj">The GameObject which is being registered. </param>
    static void Register(GameObject gameObj)
    {
      if (Tracing.Register)
        Trace.Script(gameObj.name + " has been registered to the event system");
      Events.Instance.Delegates.Add(gameObj, new SortedDelegateTypeList());
      gameObj.AddComponent<EventsRegistration>();
    }

    /// <summary>
    /// Registers the MonoBehaviour to the event system.
    /// </summary>
    /// <param name="component"></param>
    static void Register(MonoBehaviour component, GameObject gameObject)
    {
      // If its component hasn't registered yet...
      if (!Instance.ConnectedComponents.ContainsKey(component))
      {
        // Record it
        Instance.ConnectedComponents.Add(component, new List<GameObject>());
      }

      // If we haven't recorded that this component has connected to this GameObject yet
      if (!Instance.ConnectedComponents[component].Contains(gameObject))
      {
        //Trace.Script(component.name + " has connected to " + gameObject.name);
        Instance.ConnectedComponents[component].Add(gameObject);
      }
    }

    /// <summary>
    /// Unregisters the GameObject from the event system.
    /// </summary>
    /// <param name="obj"></param>
    public static void Unsubscribe(GameObject obj)
    {
      // Do not instnatiate!
      //if (!Events.EventManagerInst)      
      //  return;
      if (Quitting)
        return;

      if (Events.Instance.Delegates == null)
        return;

      // Remove all delegates connected to it
      if (Events.Instance.Delegates.ContainsKey(obj))
      {
        if (Tracing.Register)
          Trace.Script(obj.name + " has been deregistered from the event system");
        Events.Instance.Delegates.Remove(obj);
      }

      // Remove all of the delegates it created?
    }

    /// <summary>
    /// Adds the specified event to watch list, informing the user whenever
    /// the event is being dispatched.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    public static void Watch<T>()
    {
      var type = typeof(T).ToString();
      if (Tracing.Dispatch)
        Trace.Script("Now watching for events of type '" + type + "'");
      if (!Instance.WatchList.Contains(type))
        Instance.WatchList.Add(type);
    }

    /// <summary>
    /// Adds a member function delegate of a specific type onto the GameObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObj"></param>
    /// <param name="key"></param>
    /// <param name="memFunc"></param>
    static void AddDelegate<T>(GameObject gameObj, string key, Action<T> memFunc)
    {
      if (Tracing.Connect)
        Trace.Script("Adding delegate for event: " + key);
      Events.Instance.Delegates[gameObj][key].Add(memFunc);
    }

    //void RemoveDelegate()

    public static IEnumerator WaitForFrames(int frameCount)
    {
      while (frameCount > 0)
      {
        frameCount--;
        yield return null;
      }
    }


    /// <summary>
    /// Handles cleanup operations for MonoBehaviours that are connecting to 
    /// events in the Stratus Event System.
    /// </summary>
    public class Setup
    {
      MonoBehaviour Owner;

      public Setup(MonoBehaviour owner) { Owner = owner; }
      ~Setup() { Owner.Disconnect(); }
    }

  }

}
