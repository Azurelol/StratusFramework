using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace AI
  {
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : ScriptableEditor<BehaviorTree>
    {
      protected override void OnStratusEditorEnable()
      {
        AddArea(Options);
      }

      private void Options(Rect position)
      {
        if (GUILayout.Button("Open"))
          BehaviorTreeEditorWindow.Open(target);
      }
    }
  }
}