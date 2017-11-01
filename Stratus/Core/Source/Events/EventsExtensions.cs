/******************************************************************************/
/*!
@file   EventsExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@brief  Extends base Unity classes to provide functionality.
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;

public static class EventsExtensions
{
  /// <summary>
  /// Connects to the specified event on this given object.
  /// </summary>
  /// <typeparam name="T">The event class. </typeparam>
  /// <param name="gameObj">The GameObject to which to connect to.</param>
  /// <param name="func">The member function callback. </param>
  public static void Connect<T>(this GameObject gameObj, Action<T> func)
  {
    Stratus.Events.Connect(gameObj, func);
  }

  public static void Connect(this GameObject gameObj, Action<Stratus.Event> func, Type type)
  {
    Stratus.Events.Connect(gameObj, func, type);    
  }

  /// <summary>
  /// Disconnects this component from all events.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="gameObj"></param>
  public static void Disconnect(this MonoBehaviour component)
  {
    //T type = default(T);
    //var eventName = type.ToString();
    Stratus.Events.Disconnect(component);
  }

  ///// <summary>
  ///// Disconnects this component from all events.
  ///// </summary>
  ///// <typeparam name="T"></typeparam>
  ///// <param name="gameObj"></param>
  //public static void Disconnect(this Stratus.UIElement component)
  //{
  //  //T type = default(T);
  //  //var eventName = type.ToString();
  //  Stratus.Events.Disconnect(component);
  //}

  /// <summary>
  /// Disconnects this component from events from the specified GameObject.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="gameObj"></param>
  public static void Disconnect(this MonoBehaviour component, GameObject gameObj)
  {
    //T type = default(T);
    //var eventName = type.ToString();
    Stratus.Events.Disconnect(component, gameObj);
  }

  /// <summary>
  /// Dispatches the given event of the specified type onto this object.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="gameObj">The GameObject to which to connect to.</param>
  /// <param name="eventObj">The event object. </param>
  /// <param name="nextFrame">Whether the event should be sent next frame.</param>
  public static void Dispatch<T>(this GameObject gameObj, T eventObj, bool nextFrame = false) where T : Stratus.Event
  {
    //if (delayed)
    //  gameObj.transform.St
    Stratus.Events.Dispatch<T>(gameObj, eventObj, nextFrame);
  }
  // ---- // 

  /// <summary>
  /// Dispatches the given event of the specified type to this object and all its children.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="gameObj">The GameObject to which to connect to.</param>
  /// <param name="eventObj">The event object. </param>
  public static void DispatchDown<T>(this GameObject gameObj, T eventObj, bool nextFrame = false) where T : Stratus.Event
  {
    Stratus.Events.DispatchDown<T>(gameObj, eventObj, nextFrame);
  }

  /// <summary>
  /// Dispatches an event up the tree on each parent recursively.
  /// </summary>
  /// <typeparam name="T">The event class. </typeparam>
  /// <param name="gameObj">The GameObject to which to dispatch to.</param>
  /// <param name="eventObj">The event object. </param>
  public static void DispatchUp<T>(this GameObject gameObj, T eventObj, bool nextFrame = false) where T : Stratus.Event
  {
    Stratus.Events.DispatchUp<T>(gameObj, eventObj, nextFrame);
  }


}