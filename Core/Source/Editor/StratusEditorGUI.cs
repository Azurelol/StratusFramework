using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	public static class StratusEditorGUI
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static float standardPadding { get; } = 8f;
		private static TextAlignment currentAlignGroup { get; set; }
		private static bool alignGroupActive { get; set; }

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static void GUILayoutPopup(string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
		{
			StratusSearchablePopup.Popup(label, selectedindex, displayedOptions, onSelected);
		}

		public static void GUILayoutPopup(string label, StratusDropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static void GUILayoutPopup(StratusDropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static void GUIPopup(Rect position, string label, StratusDropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(position, label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static void GUIPopup(Rect position, StratusDropdownList dropdownList)
		{
			StratusSearchablePopup.Popup(position, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
		}

		public static void GUIPopup(Rect position, string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
		{
			StratusSearchablePopup.Popup(position, label, selectedindex, displayedOptions, onSelected);
		}

		public static int GUILayoutPopup(string label, int selectedindex, string[] displayedOptions)
		{
			return EditorGUILayout.Popup(label, selectedindex, displayedOptions);
		}





		public static void EnumToolbar<T>(ref T enumValue)
		{
			string[] options = StratusSearchableEnum.GetEnumDisplayNames((Enum)(object)enumValue);
			enumValue = (T)(object)GUILayout.Toolbar(Convert.ToInt32(enumValue), options, GUILayout.ExpandWidth(false));
		}

		public static void BeginAligned(TextAlignment alignment)
		{
			currentAlignGroup = alignment;
			GUILayout.BeginHorizontal();
			switch (alignment)
			{
				case TextAlignment.Left:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Center:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Right:
					GUILayout.FlexibleSpace();
					break;
			}
			alignGroupActive = true;
		}

		public static void EndAligned()
		{
			if (!alignGroupActive)
			{
				throw new ArgumentException("Missing a matching BeginAligned call!");
			}

			switch (currentAlignGroup)
			{
				case TextAlignment.Left:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Center:
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Right:
					break;
			}
			GUILayout.EndHorizontal();
			alignGroupActive = false;
		}

		public static void Field(Rect position, FieldInfo field, object target)
		{
			StratusSerializedFieldType propertyType = SerializedFieldTypeExtensions.Deduce(field);
			string name = ObjectNames.NicifyVariableName(field.Name);
			object value = null;
			switch (propertyType)
			{
				case StratusSerializedFieldType.ObjectReference:
					value = EditorGUI.ObjectField(position, name, (UnityEngine.Object)field.GetValue(target), field.FieldType, true);
					break;
				case StratusSerializedFieldType.Integer:
					value = EditorGUI.IntField(position, field.GetValue<int>(target));
					break;
				case StratusSerializedFieldType.Boolean:
					value = EditorGUI.Toggle(position, name, field.GetValue<bool>(target));
					break;
				case StratusSerializedFieldType.Float:
					value = EditorGUI.FloatField(position, field.GetValue<float>(target));
					break;
				case StratusSerializedFieldType.String:
					value = EditorGUI.TextField(position, name, field.GetValue<string>(target));
					break;
				case StratusSerializedFieldType.Color:
					value = EditorGUI.ColorField(position, name, field.GetValue<Color>(target));
					break;
				case StratusSerializedFieldType.Enum:
					StratusSearchableEnum.EnumPopup(position, name, field.GetValue<Enum>(target), (Enum selected) => field.SetValue(target, selected));
					break;
				case StratusSerializedFieldType.Vector2:
					value = EditorGUI.Vector2Field(position, name, field.GetValue<Vector2>(target));
					break;
				case StratusSerializedFieldType.Vector3:
					value = EditorGUI.Vector3Field(position, name, field.GetValue<Vector3>(target));
					break;
				case StratusSerializedFieldType.Vector4:
					value = EditorGUI.Vector4Field(position, name, field.GetValue<Vector4>(target));
					break;
				case StratusSerializedFieldType.Rect:
					value = EditorGUI.RectField(position, name, field.GetValue<Rect>(target));
					break;
				default:
					EditorGUI.LabelField(position, $"No supported drawer for type {field.FieldType.Name}!");
					break;
			}
		}

		public static void DrawGUI(Rect position, string label, ref string value)
		{
			value = EditorGUI.TextField(position, label, value);
		}

		public static void DrawGUILayout(string label, ref string value)
		{
			value = EditorGUILayout.TextField(label, value);
		}

		public static bool ObjectFieldWithHeader<T>(Rect rect, ref T objectField, string label) where T : UnityEngine.Object
		{
			GUILayout.Label(label, StratusGUIStyles.header);
			EditorGUI.BeginChangeCheck();
			objectField = (T)EditorGUI.ObjectField(rect, objectField, typeof(T), true);
			return EditorGUI.EndChangeCheck();
		}

		public static bool ObjectField<T>(ref T objectField) where T : UnityEngine.Object
		{
			EditorGUI.BeginChangeCheck();
			objectField = (T)EditorGUILayout.ObjectField(objectField, typeof(T), true);
			return EditorGUI.EndChangeCheck();
		}
	}

}