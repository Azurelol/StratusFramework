/******************************************************************************/
/*!
@file   Event.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using System;
using UnityEngine.Events;

namespace Stratus
{
  
  /// <summary>
  /// Base event class for all Stratus events. Derive from it in order to implement
  /// your own custom events.
  /// </summary>
  [Serializable]
  public class Event //  : UnityEngine.Object
  {
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
  }
  

}
