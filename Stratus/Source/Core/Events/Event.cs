/******************************************************************************/
/*!
@file   Event.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using System;

namespace Stratus
{
  
  /// <summary>
  /// Base event class for all Stratus events. Derive from it in order to implement
  /// your own custom events.
  /// </summary>
  [Serializable]
  public class Event
  {
    /// <summary>
    /// Whether the event is dispatched is being dispatched to a single gamobject or to the whole scene
    /// </summary>
    public enum Scope
    {      
      GameObject,
      Scene
    }
  }
  
  public delegate void EventCallback(Event eventObj);
  public delegate void GenericEventCallback<in T>(T eventObj);

}
