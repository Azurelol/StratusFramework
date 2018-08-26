using System;
using UnityEngine.Events;
using System.Collections.Generic;
using Stratus.Utilities;
using UnityEngine;

namespace Stratus
{
  
  /// <summary>
  /// Base event class for all Stratus events. Derive from it in order to implement
  /// your own custom events.
  /// </summary>
  [Serializable]
  public class Event //  : UnityEngine.Object
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Whether the event is dispatched is being dispatched to a single gamobject or to the whole scene
    /// </summary>
    public enum Scope
    {      
      GameObject,
      Scene
    }

    /// <summary>
    /// A callback consisting of the Stratus Event received
    /// </summary>
    [System.Serializable]
    public class EventCallback : UnityEvent<Stratus.Event> { }

    /// <summary>
    /// A delegate for a connect function
    /// </summary>
    /// <param name="connectFunc"></param>
    public delegate void ConnectFunction(System.Action<Event> connectFunc);
    public delegate void EventCallbackFunction(Stratus.Event eventObj);
    public delegate void GenericEventCallback<in T>(T eventObj);

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    private static Dictionary<Type, Event> eventCache = new Dictionary<Type, Event>();

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public static T Cache<T>() where T : Event, new()
    {
      Type type = typeof(T);
      if (!eventCache.ContainsKey(type))
        eventCache.Add(type, (T)Reflection.Instantiate(type));
      return (T)eventCache[type];
    }

    public static Event Instantiate(Type type) => (Stratus.Event)Utilities.Reflection.Instantiate(type);
    public static Event Instantiate(Type type, string data)
    {
      Event instance = Instantiate(type);
      JsonUtility.FromJsonOverwrite(data, instance);
      return instance;
    }


  }

  /// <summary>
  /// An interface for an event representing the state of an action
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ActionEvent<T>
  {
    /// <summary>
    /// An input event represents a request to perform a specific action
    /// </summary>
    public class InputEvent : Stratus.Event { public InputField.State state; }
    /// <summary>
    /// If an input has been accepted, and is legal, represents the beginning of an action
    /// </summary>
    public class StartedEvent : Stratus.Event { }
    /// <summary>
    /// If an input has been accepted, and is legal, represents the beginning of an action
    /// </summary>
    public class EndedEvent : Stratus.Event { }
  }


}
