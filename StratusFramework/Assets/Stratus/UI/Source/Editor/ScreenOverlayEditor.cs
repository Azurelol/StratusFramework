using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.UI
{
  [CustomEditor(typeof(ScreenOverlay), true)]
  public class ScreenOverlayEditor : StratusBehaviourEditor<ScreenOverlay>
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

  }

}