using UnityEditor;
using UnityEngine;

namespace Stratus.Gameplay
{
	[CustomPropertyDrawer(typeof(StratusAnimatorEventHook))]
	public class AnimatorEventHookDrawer : StratusPropertyDrawer
	{

		protected override void OnDrawProperty(Rect position, SerializedProperty property)
		{
			// Special case if it's being used by the character animator
			StratusCharacterAnimator characterAnimator = this.target as StratusCharacterAnimator;
			bool hasParameters = characterAnimator != null && characterAnimator.animator != null && characterAnimator.hasParameters;

			// Event
			SerializedProperty onEventProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.onEvent));
			DrawProperty(ref position, onEventProperty);

			// Parameter: Type
			SerializedProperty parameterTypeProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.parameterType));

			// Paramater: Name, Value
			SerializedProperty parameterNameProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.parameterName));
			SerializedProperty parameterValueProperty = null;
			AnimatorControllerParameterType parameterType = (AnimatorControllerParameterType)parameterTypeProperty.intValue;

			if (hasParameters)
			{
				string[] parameters = null;
				switch (parameterType)
				{
					case AnimatorControllerParameterType.Float:
						parameterValueProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.floatValue));
						parameters = characterAnimator.floatParameters;
						break;

					case AnimatorControllerParameterType.Int:
						parameterValueProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.intValue));
						parameters = characterAnimator.intParameters;
						break;

					case AnimatorControllerParameterType.Bool:
						parameterValueProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.boolValue));
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
						DrawPropertiesHorizontal(ref position, new DrawCommand[] { drawType, drawName, new DrawCommand(parameterValueProperty) });
						break;
					case AnimatorControllerParameterType.Trigger:
						DrawPropertiesHorizontal(ref position, new DrawCommand[] { drawType, drawName });
						break;
				}
			}
			else
			{
				switch (parameterType)
				{
					case AnimatorControllerParameterType.Float:
						parameterValueProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.floatValue));
						break;

					case AnimatorControllerParameterType.Int:
						parameterValueProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.intValue));
						break;

					case AnimatorControllerParameterType.Bool:
						parameterValueProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.boolValue));
						break;
				}

				switch (parameterType)
				{
					case AnimatorControllerParameterType.Float:
					case AnimatorControllerParameterType.Int:
					case AnimatorControllerParameterType.Bool:
						DrawPropertiesHorizontal(ref position, parameterTypeProperty, parameterNameProperty, parameterValueProperty);
						break;
					case AnimatorControllerParameterType.Trigger:
						DrawPropertiesHorizontal(ref position, parameterTypeProperty, parameterNameProperty);
						break;
				}

			}
		}

	}



}