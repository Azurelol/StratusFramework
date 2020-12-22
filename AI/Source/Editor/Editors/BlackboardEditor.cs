using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace AI
  {
    [CustomEditor(typeof(StratusBlackboard))]
    public class BlackboardEditor : StratusScriptableEditor<StratusBlackboard>
    {
      private static string[] blackboardOptions { get; } = StratusSearchableEnum.GetEnumDisplayNames(typeof(StratusBlackboard.Scope));


      protected override void OnStratusEditorEnable()
      {
      }
    }


  }
}