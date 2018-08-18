using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Dependencies.Ludiq.Reflection.Editor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(MemberField))]
  public class MemberFieldDrawer : SinglePropertyDrawer
  {
    protected override string childPropertyName => nameof(MemberField.member);

    //protected override void DrawProperty(Rect position, SerializedProperty property)
    //{
    //  SerializedProperty memberProperty = property.FindPropertyRelative(nameof(MemberField.member));
    //  EditorGUI.PropertyField(position, memberProperty);
    //}

  }

}