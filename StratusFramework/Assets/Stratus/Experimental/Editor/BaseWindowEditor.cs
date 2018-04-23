using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(BaseWindow), true)]
  public class BaseWindowEditor : BehaviourEditor<BaseWindow>
  {
    protected override void OnStratusEditorEnable()
    {
      AddPropertyChangeCallback(nameof(BaseWindow.openOnStart), ShowCanvas);
    }

    private void ShowCanvas()
    {
      if (target.canvas)
      {
        if (target.openOnStart)
          target.canvas.alpha = 1f;
        else
          target.canvas.alpha = 0f;
      }
    }

  }

}