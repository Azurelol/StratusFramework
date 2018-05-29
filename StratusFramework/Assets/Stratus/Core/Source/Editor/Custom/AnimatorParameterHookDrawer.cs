using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

namespace Stratus.Gameplay
{
  [CustomPropertyDrawer(typeof(CharacterAnimator.AnimatorParameterHook))]
  public class AnimatorParametertHookDrawer : StratusPropertyDrawer
  {
    private const float lines = 3f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return lineHeight * lines;
    }

    protected override void DrawProperty(Rect position, SerializedProperty property)
    {
      // Member
      //EditorGUI.BeginChangeCheck();
      //{
        SerializedProperty memberProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorParameterHook.member));
        EditorGUI.PropertyField(position, memberProperty);
        position.y += lineHeight * 2f;
      //}
      //if (EditorGUI.EndChangeCheck())
      //{
      //  SerializedProperty parameterTypeProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorParameterHook.parameterType));
      //  //if (member)
      //  parameterTypeProperty.intValue = 
      //}

      // Parameter 
      SerializedProperty parameterNameProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorParameterHook.parameterName));
      EditorGUI.PropertyField(position, parameterNameProperty);
    }

  }



}