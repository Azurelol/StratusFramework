using UnityEngine;
using Stratus.Utilities;
using UnityEditor;
using Stratus;

namespace Genitus
{
  [CustomEditor(typeof(Skill), true)]
  public class SkillEditor : ScriptableEditor<Skill>
  {
    private TypeSelector effectTypes;

    protected override void OnStratusEditorEnable()
    {
      effectTypes = new TypeSelector(typeof(EffectAttribute), true);
      //AddArea(this.ModifyEffects);
    }

    //void ModifyEffects(Rect rect)
    //{
    //  if (GUILayout.Button("Add Telegraph"))
    //  {
    //    this.target.components.Add(new SkillTelegraph());
    //    this.RecordModification();
    //  }

    //  if (GUILayout.Button("Add Timing"))
    //  {
    //    this.target.components.Add(new SkillTiming());
    //    this.RecordModification();
    //  }

    //  EditorGUILayout.BeginHorizontal();
    //  effectTypes.GUILayoutPopup();
    //  if (GUILayout.Button("Add", EditorStyles.miniButtonRight))
    //  {
    //    // A little tricky because we need to record it in the asset database as well        
    //    var newAttribute = CreateInstance(effectTypes.selectedClass) as EffectAttribute;
    //    newAttribute.hideFlags = HideFlags.HideInHierarchy;
    //    AssetDatabase.AddObjectToAsset(newAttribute, target);
    //    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));
    //    AssetDatabase.SaveAssets();
    //    // Oh jesus what was that
    //    target.effects.Add(newAttribute);
    //  }
    //  EditorGUILayout.EndHorizontal();

    //  if (StratusEditorUtility.DrawPolymorphicList(target.effects, "Effects"))
    //    this.Repaint();
    //}




  }
}