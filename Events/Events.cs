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
  // By string
  using DelegateList = List<EventCallback>;
  using SortedDelegateList = SortedList<string, List<EventCallback>>;
  using DelegateContainer = Dictionary<GameObject, SortedList<string, List<EventCallback>>>;
  // By type
  using DelegateTypeList = List<Delegate>;
  using SortedDelegateTypeList = SortedList<string, List<Delegate>>;
  using DelegateTypeContainer = Dictionary<GameObject, SortedList<string, List<Delegate>>>;

  /**************************************************************************/
  /*!
  @class Events The class which manages the overlying event system.
  */
  /**************************************************************************/
  public class Events : MonoBehaviour
  {
    // Whether debugging output is present
    public static bool Tracing = false;
    // A container of all the delegates for every GameObject
    private DelegateContainer Delegates;
    private DelegateTypeContainer DelegatesByType;
    // Singular instance of the Event system manager
    private static bool Active = false;

    //------------------------------------------------------------------------/
    // Instancing    
    //------------------------------------------------------------------------/
    private static Events EventManagerInst;
    public static Events Instance
    {
      get
      {
        if (!EventManagerInst)
        {
          // Look for the event instance in the scene
          EventManagerInst = FindObjectOfType(typeof(Events)) as Events;

          // If it can't find it, instantiate it
          if (!EventManagerInst)
          {
            Instantiate();
          }
          
        }
        return EventManagerInst;
      }
    }
    static void Instantiate()
    {
      //Instantiate(Resources.Load("StratusFramework"));
      var gameObj = new GameObject();
      gameObj.name = "Stratus Event System";
      EventManagerInst = gameObj.AddComponent<Events>();
      //gameObj.AddComponent<Space>();
    }

    void Awake()
    {
      Initialize();
    }

    ~Events()
    {
      Active = false;
    }
    //------------------------------------------------------------------------/
    /**************************************************************************/
    /*!
    @brief Initializes the Event system manager.
    */
    /**************************************************************************/
    void Initialize()
    {
      if (Active)
        return;

      DontDestroyOnLoad(this);
      Delegates = new DelegateContainer();
      DelegatesByType = new DelegateTypeContainer();
      EventManagerInst = this;
      Active = true;
      if (Tracing) Trace.Script("Initializing the event system manager");
    }

    /**************************************************************************/
    /*!
    @brief Connects to the event of a given object.
    @param obj The object to which to connect to.
    @param eventName The name of the event to which to listen for.
    @param memFunc The member function which to use as a callback for the event.
    */
    /**************************************************************************/
    public static void Connect<T>(GameObject obj, Action<T> memFunc)
    {
      if (Tracing)
        Trace.Script(obj.name);              

      var key = typeof(T).ToString();

      // If the GameObject hasn't registered yet, add its key
      if (!Events.Instance.DelegatesByType.ContainsKey(obj))
      {
        Subscribe(obj);
      }

      // If the event has no delegates yet, add it
      if (!Events.Instance.DelegatesByType[obj].ContainsKey(key))
      {
        Events.Instance.DelegatesByType[obj].Add(key, new DelegateTypeList());
      }

      // If the delegate is already present, do not add it
      if (Events.Instance.DelegatesByType[obj][key].Contains(memFunc))
        return;

      // Add it
      Events.Instance.DelegatesByType[obj][key].Add(memFunc);
      if (Tracing)
        Trace.Script(memFunc.ToString() + " has connected to " + obj.name);
      //Trace.Script(obj.name + " now has '" + Events.Instance.DelegatesByType[obj].Count + "' delegates");
    }

    public static void Disconnect(GameObject obj, string eventName)
    {
      // WRONG. This should be removing the one method, not all of them, man.
      Events.Instance.Delegates[obj].Remove(eventName);
    }
    
    /**************************************************************************/
    /*!
    @brief Dispatches the given event of the specified type onto the object.
    @param obj The object to which to connect to.
    @param eventName The name of the event to which to listen for.
    @param eventObj The event object.
    */
    /**************************************************************************/
    public static void Dispatch<T>(GameObject obj, T eventObj) where T : Event
    {
      var key = typeof(T).ToString();

      if (Tracing)
        Trace.Script("'" + key + "' to '" + obj.name + "'");

      // Check if the object has been registered onto the event system.
      // If not, it will be.
      CheckRegistration(obj);

      // If there is no delegate registered to this object, do nothing.
      if (!HasDelegate(obj, key))
      {
        if (Tracing)
          Trace.Script("No delegate registered to " + obj.name);
        return;
      }

      // Invoke the method for every delegate
      foreach (var deleg in Events.Instance.DelegatesByType[obj][key])
      {
        deleg.DynamicInvoke(eventObj);      
      }

    }

    /**************************************************************************/
    /*!
    @brief Dispatches the given event of the specified type onto the object.
    @param obj The object to which to connect to.
    @param eventName The name of the event to which to listen for.
    @param eventObj The event object.
    */
    /**************************************************************************/
    public static void DispatchDown<T>(GameObject obj, T eventObj) where T : Event
    {
      var key = typeof(T).ToString();

      if (Tracing)
        Trace.Script("'" + key + "' to '" + obj.name + "' and all children...");

      foreach(Transform child in obj.transform)
      {
        // Goddamn it, Unity
        if (child != obj.transform.GetChild(0))
        {
          DispatchDown(child.gameObject, eventObj);
        }
      }

      // Invoke the method for every delegate
      foreach (var deleg in Events.Instance.DelegatesByType[obj][key])
      {
        deleg.DynamicInvoke(eventObj);
      }

    }

    /**************************************************************************/
    /*!
    @brief Checks if the GameObject has been the given delegate.
    @param obj A reference to the object.        
    @param key The key to the delegate list.
    @return True if it has the delegate, false otherwise.
    */
    /**************************************************************************/
    static bool HasDelegate(GameObject obj, string key)
    {
      if (Events.Instance.DelegatesByType[obj] != null
          && Events.Instance.DelegatesByType[obj].ContainsKey(key))
      {
        return true;
      }

      if (Tracing)
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

    /**************************************************************************/
    /*!
    @brief Checks if the GameObject has been registered onto the event system.
    @param obj A reference to the object.        
    */
    /**************************************************************************/
    static void CheckRegistration(GameObject obj)
    {
      // If the GameObject hasn't registered yet, add its key
      if (!Events.Instance.DelegatesByType.ContainsKey(obj))
      {
        Events.Subscribe(obj);
      }
    }

    static void Subscribe(GameObject obj)
    {
      if (Tracing)
        Trace.Script(obj.name + " has been registered to the event system");
      Events.Instance.DelegatesByType.Add(obj, new SortedDelegateTypeList());
      obj.AddComponent<EventsRegistration>();
    }
    public static void Unsubscribe(GameObject obj)
    {
      // Do not instnatiate!
      if (!Events.EventManagerInst)      
        return;

      if (Events.Instance.DelegatesByType.ContainsKey(obj))
      {
        if (Tracing)
          Trace.Script(obj.name + " has been deregistered from the event system");
        Events.Instance.DelegatesByType.Remove(obj);
      }
    }

    //------------------------------------------------------------------------/  
    // Deprecated
    //------------------------------------------------------------------------/
    /**************************************************************************/
    /*!
    @brief Connects to the event of a given object.
    @param obj The object to which to connect to.
    @param eventName The name of the event to which to listen for.
    @param memFunc The member function which to use as a callback for the event.
    */
    /**************************************************************************/
    public static void Connect(GameObject obj, string eventName, EventCallback memFunc)
    {
      if (Tracing)
        Debug.Log("Connect: " + obj.name);

      // If the GameObject hasn't registered yet, add its key
      if (!Events.Instance.Delegates.ContainsKey(obj))
      {
        if (Tracing)
          Trace.Script(obj.name + " has been registered to the event system");
        Events.Instance.Delegates.Add(obj, new SortedDelegateList());
      }

      // If the event has no delegates yet, add it
      if (!Events.Instance.Delegates[obj].ContainsKey(eventName))
      {
        Events.Instance.Delegates[obj].Add(eventName, new DelegateList());
      }

      // Add it
      Events.Instance.Delegates[obj][eventName].Add(memFunc);

      if (Tracing)
        Trace.Script(obj.name + " now has '" + Events.Instance.Delegates[obj].Count + "' delegates");
    }


    /**************************************************************************/
    /*!
    @brief Dispatches the given event of the specified type onto the object.
    @param obj The object to which to connect to.
    @param eventName The name of the event to which to listen for.
    @param eventObj The event object.
    */
    /**************************************************************************/
    public static void Dispatch(GameObject obj, string eventName, Event eventObj)
    {
      if (Tracing)
        Trace.Script("'" + eventName + "' to '" + obj.name + "'");

      // Invoke the method for every delegate
      foreach (var deleg in Events.Instance.Delegates[obj][eventName])
      {
        deleg.Invoke(eventObj);
      }
    }


  }

}
