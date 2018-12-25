using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;

namespace Stratus
{
  namespace AI
  {
    [CustomEditor(typeof(Planner))]
    public class PlannerEditor : UnityEditor.Editor
    {
      public override void OnInspectorGUI()
      {
        base.OnInspectorGUI();
      }
    } 

  }
}