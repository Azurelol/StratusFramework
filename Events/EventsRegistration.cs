/******************************************************************************/
/*!
@file   EventsRegistration.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class EventsRegistration 
  */
  /**************************************************************************/
  public class EventsRegistration : MonoBehaviour
  {
    /**************************************************************************/
    /*!
    @class When this GameObject dies, it gets deregistered from the Stratus
           Events system.
    */
    /**************************************************************************/
    void OnDestroy()
   {
      //Trace.Script("Unsubscribed!", this);
      if (this.enabled)
        Events.Unsubscribe(this.gameObject);
   }    

  }

}