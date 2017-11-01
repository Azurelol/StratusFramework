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
using System.Collections.ObjectModel;

namespace Stratus
{
  // By type
  //using DelegatePair = Tuple<bool, >
  using DelegateTypeList = List<Delegate>;
  using DelegateMap = Dictionary<string, List<Delegate>>;
  using DispatchMap = Dictionary<GameObject, Dictionary<string, List<Delegate>>>;
  //using DelegateConnectMap = Dictionary<>
  //using 

  /// <summary>
  /// The class which manages the overlying Stratus event system.
  /// </summary>
  [Singleton("Stratus Event System", true, true)]
  public class Events : Singleton<Events>
  {
    public class TracingSetup
    {
      public bool Construction = false;
      public bool Register = false;
      public bool Connect = true;
      public bool Dispatch = false;
    }
    
    //------------------------------------------------------------------------/
    // Properties    
    //------------------------------------------------------------------------/    
    /// <summary>
    /// Whether we are doing tracing for debugging purposes.
    /// </summary>
    public static TracingSetup logging { get; } = new TracingSetup();
    /// <summary>
    /// A map of all GameObjects connected to the event system and a map of delegates that are connected to them.
    /// Whenever an event of a given type is sent to the GameObject, we invoke it on all delegates for a given type
    /// (essentially a list of delegates for each type)
    /// </summary>
    private DispatchMap dispatchMap { get; set; } = new DispatchMap();
    /// <summary>
    /// A map of all components that have connected to a GameObject for specific events
    /// </summary>
    private Dictionary<MonoBehaviour, List<GameObject>> connectMap { get; set; } = new Dictionary<MonoBehaviour, List<GameObject>>();
    /// <summary>
    /// A list of all event types that are being watched for at the moment.
    /// </summary>
    private List<string> eventWatchList { get; set; } = new List<string>();        

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the event system manager.
    /// </summary>
    protected override void OnAwake()
    {
      //DontDestroyOnLoad(this);
      //gameObjectDelegateMap = new DelegateMap();
    }
    
    /// <summary>
    /// Connects to the event of a given object.
    /// </summary>
    /// <typeparam name="T">The event class. </typeparam>
    /// <param name="gameObj">The GameObject we are connecting to whose events we are connecting to. </param>
    /// <param name="memFunc">The member function to connect to. </param>
    public static void Connect<T>(GameObject gameObj, Action<T> memFunc)
    {
      var type = typeof(T);
      var key = type.ToString();

      // If the GameObject hasn't been registered yet, add its key
      if (!Events.get.dispatchMap.ContainsKey(gameObj))
      {
        Register(gameObj);
      }

      // If the event has no delegates yet, add it
      if (!Events.get.dispatchMap[gameObj].ContainsKey(key))
      {
        Events.get.dispatchMap[gameObj].Add(key, new DelegateTypeList());
      }

      // If the delegate is already present, do not add it
      if (Events.get.dispatchMap[gameObj][key].Contains(memFunc))
        return;

      // Add the component's delegate onto the gameobject
      AddDelegate(gameObj, key, memFunc);
      // Record that this component has connected to this GameObject
      Register((MonoBehaviour)memFunc.Target, gameObj);

      if (logging.Connect)
        Trace.Script(memFunc.ToString() + " has connected to " + gameObj.name);
      //Trace.Script(obj.name + " now has '" + Events.Instance.DelegatesByType[obj].Count + "' delegates");
    }

    public static void Connect(GameObject gameObj, Action<Stratus.Event> memFunc, Type type)
    {
      var key = type.ToString();

      // If the GameObject hasn't been added to the dispatch map, register it
      if (!Events.get.dispatchMap.ContainsKey(gameObj))
        Register(gameObj);      

      // If this GameObject's dispatch map has no delegates for this event type yet, create it
      if (!Events.get.dispatchMap[gameObj].ContainsKey(key))
        Events.get.dispatchMap[gameObj].Add(key, new DelegateTypeList());      

      // If the delegate is already present, do not add it
      if (Events.get.dispatchMap[gameObj][key].Contains(memFunc))
        return;

      // Add the component's delegate onto the gameobject
      AddDelegate(gameObj, key, memFunc);
      // Record that this component has connected to this GameObject
      Register((MonoBehaviour)memFunc.Target, gameObj);

      if (logging.Connect)
        Trace.Script(memFunc.ToString() + " has connected to " + gameObj.name);

    }

    /// <summary>
    /// Disconnects this component from all events it has subscribed to.
    /// </summary>
    /// <param name="component"></param>
    public static void Disconnect(MonoBehaviour component)
    {
      if (isQuitting)
        return;

      // If the component is already connected and present in the event system
      if (get.connectMap.ContainsKey(component))
      {
        // For every gameobject that it has connected to
        foreach (var gameobj in get.connectMap[component])
        {
          // Disconnect its delegates from it
          DisconnectFromGameObject(component, gameobj);
        }
      }

      // Remove the component from the event system
      get.connectMap.Remove(component);
    }


    /// <summary>
    /// Disconnects this component from all events it has subscribed on
    /// the given GameoObject.
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="component"></param>
    public static void Disconnect(MonoBehaviour component, GameObject gameObj)
    {
      DisconnectFromGameObject(component, gameObj);
      // Remove the gameobject from the component's list in the system
      get.connectMap[component].Remove(gameObj);
    }

    /// <summary>
    /// Disconnects this component from all events it has subscribed on
    /// the given GameoObject.
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="component"></param>
    static void DisconnectFromGameObject(Behaviour component, GameObject gameObj)
    {
      // If the GameObject has been removed previously...
      if (!Events.get.dispatchMap.ContainsKey(gameObj))
        return;

      // For every delegate this GameObject has
      foreach (var pair in Events.get.dispatchMap[gameObj])
      {
        // For every delegate in the list
        foreach (var deleg in pair.Value)
        {
          if ((MonoBehaviour)deleg.Target == component)
          {
            if (logging.Connect)
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

      if (logging.Connect)
        Trace.Script("'" + key + "' to '" + obj.name + "'");

      // If this a delayed dispatch...
      if (nextFrame)
        get.StartCoroutine(DispatchNextFrame<T>(obj, eventObj));      

      // Check if the object has been registered onto the event system.
      // If not, it will be.
      CheckRegistration(obj);

      // If there is no delegate registered to this object, do nothing.
      if (!HasDelegate(obj, key))
      {
        if (logging.Dispatch)
          Trace.Script("No delegate registered to " + obj.name + " for " + eventObj.ToString());
        return;
      }

      // If we are watching events of this type
      bool watching = false;
      if (get.eventWatchList.Contains(key))
        watching = true;


      // Invoke the method for every delegate
      var delegateMap = Events.get.dispatchMap[obj][key];
      DelegateTypeList delegatesToRemove = null;
      foreach (var deleg in delegateMap)
      {
        // If we are watching events of this type
        if (watching)
          Trace.Script("Invoking member function on " + deleg.Target.ToString());

        // Do a lazy delete if it has been nulled out?
        if (IsNull(deleg.Method) || IsNull(deleg.Target))
        {
          if (delegatesToRemove == null) delegatesToRemove = new DelegateTypeList();
          delegatesToRemove.Add(deleg);
          continue;
        }
        
        deleg.DynamicInvoke(eventObj);
      }

      // If any delegates were found to be null, remove them (lazy delete)
      if (delegatesToRemove != null)
      {
        foreach (var deleg in delegatesToRemove)
          delegateMap.Remove(deleg);
      }

    }

    public static bool IsNull(object obj)
    {
      if (obj == null || (obj is UnityEngine.Object & obj.Equals(null)))
        return true;
      return false;
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
      foreach (var child in gameObj.Children())
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
      foreach (var parent in parents)
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
      if (Events.get.dispatchMap[obj] != null
          && Events.get.dispatchMap[obj].ContainsKey(key))
      {
        return true;
      }

      if (logging.Dispatch)
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
      if (!Events.get.dispatchMap.ContainsKey(gameObj))
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
      if (logging.Register)
        Trace.Script(gameObj.name + " has been registered to the event system");
      Events.get.dispatchMap.Add(gameObj, new DelegateMap());
      gameObj.AddComponent<EventsRegistration>();
    }

    /// <summary>
    /// Registers the MonoBehaviour to the event system.
    /// </summary>
    /// <param name="component"></param>
    static void Register(MonoBehaviour component, GameObject gameObject)
    {
      // If its component hasn't registered yet...
      if (!get.connectMap.ContainsKey(component))
      {
        // Record it
        get.connectMap.Add(component, new List<GameObject>());
      }

      // If we haven't recorded that this component has connected to this GameObject yet
      if (!get.connectMap[component].Contains(gameObject))
      {
        //Trace.Script(component.name + " has connected to " + gameObject.name);
        get.connectMap[component].Add(gameObject);
        //component.gameObject.AddComponent<EventsRegistration>();
      }
    }

    /// <summary>
    /// Unregisters the GameObject from the event system.
    /// </summary>
    /// <param name="obj"></param>
    public static void Disconnect(GameObject obj)
    {
      // Is this truly necessary, though?
      if (isQuitting || Events.get.dispatchMap == null)
        return;

      // Remove this GameObject from the event dispatch map
      if (Events.get.dispatchMap.ContainsKey(obj))
      {
        if (logging.Register)
          Trace.Script(obj.name + " has been disconnected from the event system");
        Events.get.dispatchMap.Remove(obj);
      }

      // Remove all of the delegates its components created
      //foreach(var gameObject in Events.Instance.dispatchMap)
      //{
      //  if (gameObject.Value.)
      //}

    }

    /// <summary>
    /// Adds the specified event to watch list, informing the user whenever
    /// the event is being dispatched.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    public static void Watch<T>()
    {
      var type = typeof(T).ToString();
      if (logging.Dispatch)
        Trace.Script("Now watching for events of type '" + type + "'");
      if (!get.eventWatchList.Contains(type))
        get.eventWatchList.Add(type);
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
      if (logging.Connect)
        Trace.Script("Adding delegate for event: " + key);
      Events.get.dispatchMap[gameObj][key].Add(memFunc);
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
