using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
	[CustomPropertyDrawer(typeof(StratusPositionField))]
	public class StratusPositionFieldDrawer : PropertyDrawer
	{
		float typeWidth { get; } = 0.3f;
		float inputValueWidth { get { return 1f - typeWidth; } }


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);
			Rect contentPosition = EditorGUI.PrefixLabel(position, label);

			SerializedProperty typeProp = property.FindPropertyRelative("type");
			StratusPositionField.Type type = (StratusPositionField.Type)typeProp.enumValueIndex;

			var width = contentPosition.width;

			EditorGUI.indentLevel = 0;
			{
				// 1. Modify the type
				contentPosition.width = width * typeWidth;
				EditorGUI.PropertyField(contentPosition, typeProp, GUIContent.none);
			}

			// 2. Modify the input depending on the type
			contentPosition.x += contentPosition.width + 4f;
			contentPosition.width = width * inputValueWidth;
			SerializedProperty inputProp = null;
			switch (type)
			{
				case StratusPositionField.Type.Transform:
					inputProp = property.FindPropertyRelative(StratusPositionField.transformFieldName);
					break;
				case StratusPositionField.Type.Vector:
					inputProp = property.FindPropertyRelative(StratusPositionField.pointFieldName);
					break;
			}

			EditorGUI.PropertyField(contentPosition, inputProp, GUIContent.none);
			EditorGUI.EndProperty();

			// 3. Save, if updated
			if (GUI.changed)
				property.serializedObject.ApplyModifiedProperties();
		}
	}

}