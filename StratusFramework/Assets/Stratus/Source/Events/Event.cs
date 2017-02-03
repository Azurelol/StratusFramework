/******************************************************************************/
/*!
@file   Event.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/

namespace Stratus {

  /**************************************************************************/
  /*!
  @class Event Base event class.
  */
  /**************************************************************************/
  public class Event {}

  // Declare the delegate
  public delegate void EventCallback(Event eventObj);
  public delegate void GenericEventCallback<in T>(T eventObj);

}
