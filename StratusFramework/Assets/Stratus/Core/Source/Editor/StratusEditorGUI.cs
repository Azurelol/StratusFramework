using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

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
      SearchablePopup.Popup(label, selectedindex, displayedOptions, onSelected);
    }

    public static void GUILayoutPopup(string label, DropdownList dropdownList)
    {
      SearchablePopup.Popup(label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUILayoutPopup(DropdownList dropdownList)
    {
      SearchablePopup.Popup(dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, string label, DropdownList dropdownList)
    {
      SearchablePopup.Popup(position, label, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, DropdownList dropdownList)
    {
      SearchablePopup.Popup(position, dropdownList.selectedIndex, dropdownList.displayedOptions, (int index) => dropdownList.selectedIndex = index);
    }

    public static void GUIPopup(Rect position, string label, int selectedindex, string[] displayedOptions, System.Action<int> onSelected)
    {
      SearchablePopup.Popup(position, label, selectedindex, displayedOptions, onSelected);
    }

    public static int GUILayoutPopup(string label, int selectedindex, string[] displayedOptions)
    {
      return EditorGUILayout.Popup(label, selectedindex, displayedOptions);
    }

    public static bool EditorGUILayoutProperty(SerializedProperty serializedProperty, string label)
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

    public static void EnumToolbar<T>(ref T enumValue)
    {
      string[] options = SearchableEnum.GetEnumDisplayNames((Enum)(object)enumValue);
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
        throw new ArgumentException("Missing a matching BeginAligned call!");

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

    public static void DrawGUI(Rect position, FieldInfo field, object target)
    {
      SerializedPropertyType propertyType = SerializedSystemObject.DeducePropertyType(field);
      string name = ObjectNames.NicifyVariableName(field.Name);
      object value = null;
      switch (propertyType)
      {
        case SerializedPropertyType.ObjectReference:
          value = EditorGUI.ObjectField(position, name, (UnityEngine.Object)field.GetValue(target), field.FieldType, true);
          break;
        case SerializedPropertyType.Integer:
          value = EditorGUI.IntField(position, field.GetValue<int>(target));
          break;
        case SerializedPropertyType.Boolean:
          value = EditorGUI.Toggle(position, name, field.GetValue<bool>(target));
          break;
        case SerializedPropertyType.Float:
          value = EditorGUI.FloatField(position, field.GetValue<float>(target));
          break;
        case SerializedPropertyType.String:
          value = EditorGUI.TextField(position, name, field.GetValue<string>(target));
          break;
        case SerializedPropertyType.Color:
          value = EditorGUI.ColorField(position, name, field.GetValue<Color>(target));
          break;
        case SerializedPropertyType.LayerMask:
          value = EditorGUI.LayerField(position, name, field.GetValue<LayerMask>(target));
          break;
        case SerializedPropertyType.Enum:
          SearchableEnum.EnumPopup(position, name, field.GetValue<Enum>(target), (Enum selected) => field.SetValue(target, selected));
          break;
        case SerializedPropertyType.Vector2:
          value = EditorGUI.Vector2Field(position, name, field.GetValue<Vector2>(target));
          break;
        case SerializedPropertyType.Vector3:
          value = EditorGUI.Vector3Field(position, name, field.GetValue<Vector3>(target));
          break;
        case SerializedPropertyType.Vector4:
          value = EditorGUI.Vector4Field(position, name, field.GetValue<Vector4>(target));
          break;
        case SerializedPropertyType.Rect:
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

    public static bool ObjectFieldWithHeader<T>(ref T objectField, string label) where T : UnityEngine.Object
    {
      EditorGUILayout.LabelField(label, StratusGUIStyles.header);
      EditorGUI.BeginChangeCheck();
      objectField = (T)EditorGUILayout.ObjectField(objectField, typeof(T), true);
      Rect rect = GUILayoutUtility.GetLastRect();
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