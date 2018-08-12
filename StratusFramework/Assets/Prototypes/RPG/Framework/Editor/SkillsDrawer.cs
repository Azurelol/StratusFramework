/******************************************************************************/
/*!
@file   SkillsDrawer.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using Stratus;
using UnityEditor;

namespace Prototype
{
  /// <summary>
  /// 
  /// </summary>
  /// 
  //[CustomPropertyDrawer(typeof(Skills))]
  //public class SkillsDrawer : PropertyDrawer
  //{
  //  //------------------------------------------------------------------------/
  //  // Properties
  //  //------------------------------------------------------------------------/

  //  //------------------------------------------------------------------------/
  //  // Methods
  //  //------------------------------------------------------------------------/
  //  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  //  {
  //    var target = property.serializedObject.targetObject;
  //    var skills = target as Skills;

  //    //// If inspecting a chara

  //    EditorGUI.BeginProperty(position, label, property);
  //    // Display available gambits
  //    DisplaySkills(tactics);
  //    // Add gambits
  //    AddGambit(tactics);
  //    EditorGUI.EndProperty();

  //  }

  //  void DisplaySkills(Skills skills)
  //  {
  //    foreach(var skill in skills)
  //    {

  //    }
  //  }

  //}

}
#endif