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
  [ExecuteInEditMode]
  public class EventsRegistration : MonoBehaviour
  {
    private void Awake()
    {
      this.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
      if (Application.isPlaying && !Events.IsConnected(this.gameObject))
      {
        Events.Connect(this.gameObject);
      }
    }

    void OnDestroy()
    {
      if (Application.isPlaying)
      {
        Events.Disconnect(this.gameObject);
      }
    }

  }
}