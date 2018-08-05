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
      // Special case if it's being used by the character animator
      CharacterAnimator characterAnimator = target as CharacterAnimator;
      bool hasParameters = characterAnimator != null && characterAnimator.animator != null && characterAnimator.hasParameters;

      // Member
      SerializedProperty memberProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorParameterHook.member));
      EditorGUI.PropertyField(position, memberProperty);
      position.y += lineHeight * 2f;
            
      // Parameter 
      //AnimatorControllerParameterType parameterType = (AnimatorControllerParameterType)parameterTypeProperty.intValue;
      SerializedProperty parameterNameProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorParameterHook.parameterName));
      if (hasParameters)
      {
        DrawPopup(position, parameterNameProperty, characterAnimator.animatorParameterNames);
        //int index = characterAnimator.animatorParameterNames.FindIndex(parameterNameProperty.stringValue);
        //index = EditorGUI.Popup(position, index, characterAnimator.animatorParameterNames);
        //parameterNameProperty.stringValue = characterAnimator.animatorParameterNames.AtIndexOrDefault(index, string.Empty);
      }
      else
      {
        EditorGUI.PropertyField(position, parameterNameProperty);
      }
    }

  }



}