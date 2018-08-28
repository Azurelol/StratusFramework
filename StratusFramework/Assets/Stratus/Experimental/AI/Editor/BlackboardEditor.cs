using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace AI
  {
    [CustomEditor(typeof(Blackboard))]
    public class BlackboardEditor : ScriptableEditor<Blackboard>
    {
      private static string[] blackboardOptions { get; } = SearchableEnum.GetEnumDisplayNames(typeof(Blackboard.Scope));


      protected override void OnStratusEditorEnable()
      {
      }
    }


  }
}