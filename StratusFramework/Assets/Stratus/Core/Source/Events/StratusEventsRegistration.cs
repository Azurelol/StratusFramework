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
  public class StratusEventsRegistration : MonoBehaviour
  {
    private void Awake()
    {
      this.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
      if (Application.isPlaying && !StratusEvents.IsConnected(this.gameObject))
      {
        StratusEvents.Connect(this.gameObject);
      }
    }

    void OnDestroy()
    {
      if (Application.isPlaying)
      {
        StratusEvents.Disconnect(this.gameObject);
      }
    }

  }
}