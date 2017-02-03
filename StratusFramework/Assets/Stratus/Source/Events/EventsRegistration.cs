/******************************************************************************/
/*!
@file   EventsRegistration.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class EventsRegistration 
  */
  /**************************************************************************/
  public class EventsRegistration : MonoBehaviour
  {
    public MonoBehaviour[] SubscribedComponents;
    bool Quitting = false;
    
    void Start()
    {
      SubscribedComponents = gameObject.GetComponents<MonoBehaviour>();
    }

    /// <summary>
    /// Unsubscribes this GameObject and all its components from the Stratus
    /// event system.
    /// </summary>
   void OnDestroy()
   {
      if (Quitting)
        return;

      //Trace.Script("Unsubscribed!", this);
      
      // Unsubscribe this GameObject (removing all delegates attached to it)
      if (this.enabled)
        Events.Unsubscribe(this.gameObject);
   }    


   void OnApplicationQuit()
    {
      Quitting = true;
    }


  }

}