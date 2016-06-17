using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Stratus {

  /**************************************************************************/
  /*!
  @class Event Base event class.
  */
  /**************************************************************************/
  public class Event
  {
  }

  // Declare the delegate
  public delegate void EventCallback(Event eventObj);
  public delegate void GenericEventCallback<in T>(T eventObj);

}
