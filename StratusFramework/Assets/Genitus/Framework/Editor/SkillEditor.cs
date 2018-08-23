using UnityEngine;
using Stratus.Utilities;
using UnityEditor;
using Stratus;

namespace Genitus
{
  [CustomEditor(typeof(Skill), true)]
  public class SkillEditor : ScriptableEditor<Skill>
  {
    //private TypeSelector effectTypes;

    protected override void OnStratusEditorEnable()
    {
      //effectTypes = new TypeSelector(typeof(EffectAttribute), true);
      //AddArea(ModifyEffects);
    }

    //void ModifyEffects(Rect rect)
    //{
    //  SearchablePopup.Popup("Effect Types", 
    //    effectTypes.currentIndex, 
    //    effectTypes.displayedOptions, 
    //    (int i) => effectTypes.currentIndex = i);
    //}




  }
}