using UnityEditor;
using UnityEngine;

namespace Stratus.Gameplay
{
	[CustomPropertyDrawer(typeof(StratusAnimatorParameterHook))]
	public class AnimatorParameterHookDrawer : StratusPropertyDrawer
	{
		protected override void OnDrawProperty(Rect position, SerializedProperty property)
		{
			// Special case if it's being used by the character animator
			StratusCharacterAnimator characterAnimator = this.target as StratusCharacterAnimator;
			bool hasParameters = characterAnimator != null && characterAnimator.animator != null && characterAnimator.hasParameters;

			// Member
			SerializedProperty memberProperty = property.FindPropertyRelative(nameof(StratusAnimatorParameterHook.member));
			DrawProperty(ref position, memberProperty);

			// Parameter
			SerializedProperty parameterNameProperty = property.FindPropertyRelative(nameof(StratusAnimatorParameterHook.parameterName));

			if (hasParameters)
			{
				SerializedProperty parameterTypeProperty = property.FindPropertyRelative(nameof(StratusAnimatorEventHook.parameterType));
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
				DrawPopup(ref position, parameterNameProperty, parameters);
			}
			else
			{
				DrawProperty(ref position, parameterNameProperty);
			}
		}

	}



}