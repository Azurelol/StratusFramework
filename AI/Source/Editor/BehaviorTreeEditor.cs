using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace AI
  {
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : StratusScriptableEditor<BehaviorTree>
    {
      protected override void OnStratusEditorEnable()
      {
        AddArea(Options);
      }

      private void Options(Rect position)
      {
        if (GUILayout.Button("Open"))
          StratusBehaviorTreeEditorWindow.Open(target);
      }
    }
  }
}