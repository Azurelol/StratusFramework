using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

namespace Stratus.Gameplay
{
  [CustomPropertyDrawer(typeof(AnimatorEventHook))]
  public class AnimatorEventHookDrawer : StratusPropertyDrawer
  {
    private const float lines = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return lineHeight * lines;
    }

    protected override void DrawProperty(Rect position, SerializedProperty property)
    {
      // Special case if it's being used by the character animator
      CharacterAnimator characterAnimator = target as CharacterAnimator;
      bool hasParameters = characterAnimator != null && characterAnimator.animator != null && characterAnimator.hasParameters;

      // Event
      SerializedProperty onEventProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.onEvent));
      EditorGUI.PropertyField(position, onEventProperty);
      position.y += lineHeight;

      // Parameter: Type
      SerializedProperty parameterTypeProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.parameterType));
      
      // Paramater: Name, Value
      SerializedProperty parameterNameProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.parameterName));
      SerializedProperty parameterValueProperty = null;
      AnimatorControllerParameterType parameterType = (AnimatorControllerParameterType)parameterTypeProperty.intValue;

      if (hasParameters)
      {
        string[] parameters = null;
        switch (parameterType)
        {
          case AnimatorControllerParameterType.Float:
            parameterValueProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.floatValue));
            parameters = characterAnimator.floatParameters;
            break;

          case AnimatorControllerParameterType.Int:
            parameterValueProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.intValue));
            parameters = characterAnimator.intParameters;
            break;

          case AnimatorControllerParameterType.Bool:
            parameterValueProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.boolValue));
            parameters = characterAnimator.boolParameters;
            break;

          case AnimatorControllerParameterType.Trigger:
            parameters = characterAnimator.triggerParameters;
            break;
        }

        // Draw parameters in a single line
        DrawCommand drawType = new DrawCommand(parameterTypeProperty);
        DrawCommand drawName = new DrawPopUp(parameterNameProperty, parameters);
        switch (parameterType)
        {
          case AnimatorControllerParameterType.Float:
          case AnimatorControllerParameterType.Int:
          case AnimatorControllerParameterType.Bool:
            DrawPropertiesInSingleLine(position, new DrawCommand[] { drawType, drawName, new DrawCommand(parameterValueProperty) });
            break;
          case AnimatorControllerParameterType.Trigger:
            DrawPropertiesInSingleLine(position, new DrawCommand[] { drawType, drawName });
            break;
        }
      }
      else
      {
        switch (parameterType)
        {
          case AnimatorControllerParameterType.Float:
            parameterValueProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.floatValue));            
            break;

          case AnimatorControllerParameterType.Int:
            parameterValueProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.intValue));
            break;

          case AnimatorControllerParameterType.Bool:
            parameterValueProperty = property.FindPropertyRelative(nameof(AnimatorEventHook.boolValue));
            break;
        }

        switch (parameterType)
        {
          case AnimatorControllerParameterType.Float:
          case AnimatorControllerParameterType.Int:
          case AnimatorControllerParameterType.Bool:
            DrawPropertiesInSingleLine(position, parameterTypeProperty, parameterNameProperty, parameterValueProperty);
            break;
          case AnimatorControllerParameterType.Trigger:
            DrawPropertiesInSingleLine(position, parameterTypeProperty, parameterNameProperty);
            break;
        }

      }
    }

  }



}