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
  /**************************************************************************/
  /*!
  @brief Connects to the specified event on this given object.
  @param gameObj The object to which to connect to.
  @param eventName The name of the event to which to listen for.
  @param memFunc The member function which to use as a callback for the event.
  */
  /**************************************************************************/
  public static void Connect<T>(this GameObject gameObj, Action<T> func)
  {
    Stratus.Events.Connect(gameObj, func);
  }

  /**************************************************************************/
  /*!
    @brief Dispatches the given event of the specified type onto this object.
    @param obj The object to which to connect to.
    @param eventName The name of the event to which to listen for.
    @param eventObj The event object.
  */
  /**************************************************************************/
  public static void Dispatch(this GameObject gameObj, string eventName, Stratus.Event eventObj)
  {
    Stratus.Events.Dispatch(gameObj, eventName, eventObj);
  }

  public static void Dispatch<T>(this GameObject gameObj, T eventObj) where T : Stratus.Event
  {
    Stratus.Events.Dispatch<T>(gameObj, eventObj);
  }

  public static void DispatchDown<T>(this GameObject gameObj, T eventObj) where T : Stratus.Event
  {
    Stratus.Events.DispatchDown<T>(gameObj, eventObj);
  }
}