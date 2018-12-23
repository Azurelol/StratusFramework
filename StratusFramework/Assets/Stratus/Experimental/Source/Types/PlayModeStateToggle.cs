using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
  [ExecuteInEditMode]
  public class PlayModeStateToggle : StratusBehaviour
  {
    public List<Behaviour> disabledInEditor = new List<Behaviour>();

    //private void Awake()
    //{
    //  if (!Application.isPlaying)
    //    return;
    //
    //  Toggle(true);
    //}

    private void OnEnable()
    {
      //if (Application.isPlaying)
      //  return;
      //Toggle(false);

      #if UNITY_EDITOR
      UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
      #endif

    }

    private void OnDisable()
    {
      //if (Application.isPlaying)
      //  return;
      //Toggle(true);

#if UNITY_EDITOR
      UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif

    }

    private void OnDestroy()
    {
      Toggle(true);
    }

    private void Toggle(bool toggle)
    {
      foreach (var behaviour in disabledInEditor)
      {
        behaviour.enabled = toggle;
      }
    }

    private void OnValidate()
    {
      Toggle(!Application.isPlaying);
    }

    #if UNITY_EDITOR
    protected virtual void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
    {
      switch (stateChange)
      {
        case UnityEditor.PlayModeStateChange.EnteredEditMode:
          Toggle(false);
          break;

        case UnityEditor.PlayModeStateChange.ExitingEditMode:
          Toggle(true);
          break;

        case UnityEditor.PlayModeStateChange.EnteredPlayMode:
          break;

        case UnityEditor.PlayModeStateChange.ExitingPlayMode:
          break;
      }

    }
    #endif

  }
}
