using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CanEditMultipleObjects]
  [CustomEditor(typeof(StratusScriptable), true)]
  public class StratusScriptableEditor : ScriptableEditor<StratusScriptable>
  {
    protected override void OnStratusEditorEnable()
    {
    }
  }

}