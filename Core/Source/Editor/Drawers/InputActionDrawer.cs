using UnityEditor;
using UnityEngine;

namespace Stratus
{
	[CustomPropertyDrawer(typeof(StratusInputEventField), true)]
	public class InputActionDrawer : StratusPropertyDrawer
	{
		protected override void OnDrawProperty(Rect position, SerializedProperty property)
		{
			SerializedProperty inputProperty = property.FindPropertyRelative(nameof(StratusInputEventField.input));
			SerializedProperty typeProp = inputProperty.FindPropertyRelative("_type");
			StratusInputBinding.Type type = (StratusInputBinding.Type)typeProp.enumValueIndex;

			SerializedProperty stateProperty = property.FindPropertyRelative(nameof(StratusInputEventField.state));

			SerializedProperty callbackProperty = null;
			switch (type)
			{
				case StratusInputBinding.Type.Key:
				case StratusInputBinding.Type.MouseButton:
					callbackProperty = property.FindPropertyRelative(nameof(StratusInputEventField.onInput));
					break;
				case StratusInputBinding.Type.Axis:
					callbackProperty = property.FindPropertyRelative(nameof(StratusInputEventField.onAxisInput));
					break;
			}

			this.DrawProperty(ref position, inputProperty);
			this.DrawProperty(ref position, stateProperty);
			this.DrawProperty(ref position, callbackProperty);
		}
	}


}