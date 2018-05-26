using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(CharacterAnimator.AnimatorEventHook))]
  public class AnimatorEventHookDrawer : StratusPropertyDrawer
  {
    private const float lines = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return lineHeight * lines;
    }

    protected override void DrawProperty(Rect position, SerializedProperty property)
    {
      //CharacterAnimator.AnimationEventHook aeh = property.GetValue<CharacterAnimator.AnimationEventHook>();

      // Event
      SerializedProperty onEventProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorEventHook.onEvent));
      EditorGUI.PropertyField(position, onEventProperty);
      position.y += lineHeight;

      // Parameter 
      SerializedProperty parameterTypeProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorEventHook.parameterType));
      SerializedProperty parameterNameProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorEventHook.parameterName));

      // Paramater - Value
      SerializedProperty parameterValueProperty = null;
      AnimatorControllerParameterType parameterType = (AnimatorControllerParameterType)parameterTypeProperty.intValue;
      switch (parameterType)
      {
        case AnimatorControllerParameterType.Float:          
          parameterValueProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorEventHook.floatValue));          
          break;

        case AnimatorControllerParameterType.Int:
          parameterValueProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorEventHook.intValue));          
          break;

        case AnimatorControllerParameterType.Bool:
          parameterValueProperty = property.FindPropertyRelative(nameof(CharacterAnimator.AnimatorEventHook.boolValue));          
          break;
      }

      // Draw parameters in a single line
      SerializedProperty[] parameterProperties = null;
      switch (parameterType)
      {
        case AnimatorControllerParameterType.Float:
        case AnimatorControllerParameterType.Int:
        case AnimatorControllerParameterType.Bool:
          parameterProperties = new SerializedProperty[] { parameterTypeProperty, parameterNameProperty, parameterValueProperty };
          break;
        case AnimatorControllerParameterType.Trigger:
          parameterProperties = new SerializedProperty[] { parameterTypeProperty, parameterNameProperty };
          break;
      }
      DrawPropertiesInSingleLine(position, parameterProperties); 
    }

  }



}