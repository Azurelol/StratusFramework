using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(ScreenOverlay), true)]
  public class ScreenOverlayEditor : BehaviourEditor<ScreenOverlay>
  {
    protected override void OnStratusEditorEnable()
    {
      AddPropertyChangeCallback(nameof(ScreenOverlay.hideInEditor), ShowOverlay);
      AddPropertyChangeCallback(nameof(ScreenOverlay.screen), ShowOverlay);
    }

    private void ShowOverlay()
    {
      if (target.screen)
      {
        if (target.hideInEditor)
          target.screen.enabled = false;
        else
          target.screen.enabled = true;
      }
    }

    //protected override void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    //{
    //  if (!target.hideInEditor || !target.screen)
    //    return;
    //
    //  switch (stateChange)
    //  {
    //    case PlayModeStateChange.EnteredEditMode:
    //      target.screen.enabled = false;
    //      break;
    //
    //    case PlayModeStateChange.ExitingEditMode:
    //      target.screen.enabled = true;
    //      break;
    //
    //    case PlayModeStateChange.EnteredPlayMode:
    //      break;
    //
    //    case PlayModeStateChange.ExitingPlayMode:
    //      break;          
    //  }
    //}

  }

}