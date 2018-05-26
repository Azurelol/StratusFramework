using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Dependencies.Ludiq.Reflection.Editor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(MemberField))]
  public class MemberFieldDrawer : PropertyDrawer
  {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return base.GetPropertyHeight(property.FindPropertyRelative("member"), label);
    }
  
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      base.OnGUI(position, property.FindPropertyRelative("member"), label);
    }
  
  }

}