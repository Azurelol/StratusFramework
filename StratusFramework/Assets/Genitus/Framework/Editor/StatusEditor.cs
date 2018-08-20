using UnityEngine;
using Stratus.Utilities;
using UnityEditor;
using Stratus;
using Genitus.Effects;

namespace Genitus
{
  [CustomEditor(typeof(Status))]
  public class StatusEditor : ScriptableEditor<Status>
  {
    private TypeSelector effectTypes;

    protected override void OnStratusEditorEnable()
    {
      effectTypes = new TypeSelector(typeof(PersistentEffectAttribute), true);
      AddArea(this.ModifyEffects);
    }

    void ModifyEffects(Rect rect)
    {
      EditorGUILayout.BeginHorizontal();
      effectTypes.GUILayoutPopup();
      if (GUILayout.Button("Add", EditorStyles.miniButtonRight))
      {
        // A little tricky because we need to record it in the asset database as well        
        var newAttribute = CreateInstance(effectTypes.selectedClass) as PersistentEffectAttribute;
        newAttribute.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(newAttribute, target);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));
        AssetDatabase.SaveAssets();
        // Oh jesus what was that
        target.effects.Add(newAttribute);
      }
      EditorGUILayout.EndHorizontal();

      if (StratusEditorUtility.DrawPolymorphicList(target.effects, "Effects"))
        this.Repaint();
    }

  }
}