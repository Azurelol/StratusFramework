using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

namespace Stratus.Gameplay
{
  [CustomPropertyDrawer(typeof(AnimatorParameterHook))]
  public class AnimatorParameterHookDrawer : StratusPropertyDrawer
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
      SerializedProperty memberProperty = property.FindPropertyRelative(nameof(AnimatorParameterHook.member));
      EditorGUI.PropertyField(position, memberProperty);
      position.y += lineHeight * 2f;

      // Parameter
      SerializedProperty parameterNameProperty = property.FindPropertyRelative(nameof(AnimatorParameterHook.parameterName));

      if (hasParameters)
      {
        SerializedProperty parameterTypeProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.parameterType));
        //AnimatorControllerParameterType parameterType = GetEnumValue<AnimatorControllerParameterType>(parameterTypeProperty);
        AnimatorControllerParameterType parameterType = (AnimatorControllerParameterType)parameterTypeProperty.intValue;
        string[] parameters = null;
        switch (parameterType)
        {
          case AnimatorControllerParameterType.Float:
            parameters = characterAnimator.floatParameters;
            break;

          case AnimatorControllerParameterType.Int:
            parameters = characterAnimator.intParameters;
            break;

          case AnimatorControllerParameterType.Bool:
            parameters = characterAnimator.boolParameters;
            break;

          case AnimatorControllerParameterType.Trigger:
            parameters = characterAnimator.triggerParameters;
            break;
        }
        DrawPopup(position, parameterNameProperty, parameters);
      }
      else
      {
        EditorGUI.PropertyField(position, parameterNameProperty);
      }
      
            
      //// Parameter 
      ////AnimatorControllerParameterType parameterType = (AnimatorControllerParameterType)parameterTypeProperty.intValue;
      //if (hasParameters)
      //{
      //  DrawPopup(position, parameterNameProperty, characterAnimator.animatorParameterNames);
      //  //int index = characterAnimator.animatorParameterNames.FindIndex(parameterNameProperty.stringValue);
      //  //index = EditorGUI.Popup(position, index, characterAnimator.animatorParameterNames);
      //  //parameterNameProperty.stringValue = characterAnimator.animatorParameterNames.AtIndexOrDefault(index, string.Empty);
      //}
      //else
      //{
      //  EditorGUI.PropertyField(position, parameterNameProperty);
      //}
    }

  }



}