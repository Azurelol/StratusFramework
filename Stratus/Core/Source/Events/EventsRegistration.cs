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
using System;

namespace Stratus
{
  /// <summary>
  /// Whenever an Monobehaviour on a GameObject connects to the event system,
  /// this component is attached to it at runtime in order to handle book-keeping.
  /// It is only destroyed at the moment the GameObject is being destroyed.
  /// </summary>
  [DisallowMultipleComponent]
  public class EventsRegistration : MonoBehaviour
  {    
    void Start()
    {      
      // This component is only used for runtime book-keeping
      this.hideFlags = HideFlags.HideAndDontSave;
    }
    
   void OnDestroy()
   {
      // Unsubscribe this GameObject (removing all delegates attached to its components)
      Events.Disconnect(this.gameObject);
   }    

  }
}