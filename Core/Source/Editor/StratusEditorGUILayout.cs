using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	public static class StratusEditorGUILayout
	{
		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Draws a field using EditorGUILayout based on its members,
		/// (without using SerializedProperty)
		/// <returns>True if the field was changed</returns>
		public static bool Field(FieldInfo field, object target)
		{
			return StratusSerializedEditorObject.GetFieldDrawer(field).DrawEditorGUILayout(target);
		}

		public static bool ObjectFieldWithHeader<T>(ref T objectField, string label) where T : UnityEngine.Object
		{
			EditorGUILayout.LabelField(label, StratusGUIStyles.header);
			EditorGUI.BeginChangeCheck();
			objectField = (T)EditorGUILayout.ObjectField(objectField, typeof(T), true);
			return EditorGUI.EndChangeCheck();
		}

		public static bool Button(string label, System.Action onClick)
		{
			if (GUILayout.Button(label, StratusGUIStyles.button))
			{
				onClick();
				return true;
			}
			return false;
		}

		public static bool Property(SerializedProperty serializedProperty, string label)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(serializedProperty.serializedObject.targetObject, serializedProperty.displayName);
				serializedProperty.serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		public static bool Property(SerializedProperty serializedProperty)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedProperty);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(serializedProperty.serializedObject.targetObject, serializedProperty.displayName);
				serializedProperty.serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		public static string FolderPath(string title)
		{
			return EditorUtility.SaveFolderPanel(title, string.Empty, string.Empty);
		}
	}

}