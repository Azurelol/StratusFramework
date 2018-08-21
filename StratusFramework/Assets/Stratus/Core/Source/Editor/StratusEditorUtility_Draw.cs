using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rotorz.ReorderableList;
using System;
using UnityEditor;
using System.Reflection;
using OdinSerializer;

namespace Stratus
{
  public static partial class StratusEditorUtility
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    private static GUILayoutOption[] polymorphicListLayoutOptions { get; } = new GUILayoutOption[]
    {
      GUILayout.ExpandWidth(false)
    };

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public static SerializedSystemObject.SystemObjectDrawer GetDrawer(object element)
    {
      Type elementType = element.GetType();
      if (!objectDrawers.ContainsKey(elementType))
        objectDrawers.Add(elementType, new SerializedSystemObject.SystemObjectDrawer(elementType));
      SerializedSystemObject.SystemObjectDrawer drawer = objectDrawers[elementType];
      return drawer;
    }

    /// <summary>
    /// Draws a list of elements deriving from a base class
    /// </summary>
    /// <returns>True if the height of the list changed, which signals a repaint event</returns>
    public static bool DrawPolymorphicList<T>(List<T> list, string title, bool useTypeLabel = true)
    {
      // We need to remember this list since the height is variable depending on the
      // amount of fields being drawn      
      int hashCode = list.GetHashCode();
      if (!abstractListHeights.ContainsKey(hashCode))
        abstractListHeights.Add(hashCode, 0);

      IntegerReference maxCount = 0;
      ReorderableListGUI.Title(title);
      ReorderableListGUI.ListField(list, (Rect position, T value) =>
      {
        // Get the drawer
        Type type = value.GetType();
        if (!objectDrawers.ContainsKey(type))
          objectDrawers.Add(type, new SerializedSystemObject.SystemObjectDrawer(type));
        SerializedSystemObject.SystemObjectDrawer drawer = objectDrawers[type];

        // We draw one line at a time
        position.height = lineHeight;

        // Calculate height for this type
        int count = drawer.fieldCount;
        if (useTypeLabel)
        {
          EditorGUI.LabelField(position, type.Name, EditorStyles.centeredGreyMiniLabel);
          position.y += lineHeight;
          count++;
        }
        if (count > maxCount)
          maxCount = count;

        // Draw
        drawer.DrawEditorGUI(position, value);
        return value;
      }, abstractListHeights[hashCode], ReorderableListFlags.HideAddButton);

      float currentHeight = maxCount * lineHeight;
      if (abstractListHeights[hashCode] != currentHeight)
      {
        abstractListHeights[hashCode] = currentHeight;
        return true;
      }
      return false;
    }



    /// <summary>
    /// Draws a list of elements deriving from a base class
    /// </summary>
    /// <returns>True if the height of the list changed, which signals a repaint event</returns>
    public static bool DrawPolymorphicList(FieldInfo field, object target, string title, bool useTypeLabel = true)
    {
      // We need to remember this list since the height is variable depending on the
      // amount of fields being drawn      
      var fieldValue = field.GetValue(target);
      IList list = fieldValue as IList;


      //EditorGUILayout.BeginVertical(ReorderableListStyles.Container);

      // Draw the header
      EditorGUILayout.BeginHorizontal(ReorderableListStyles.Title);
      EditorGUILayout.PrefixLabel(title);
      if (GUILayout.Button("Add", polymorphicListLayoutOptions))
      {
        Type baseType = Utilities.Reflection.GetIndexedType(list); // list.GetType().GetElementType();
        var menu = new GenericMenu();
        string[] typeNames = Utilities.Reflection.GetSubclassNames(baseType);
        menu.AddItems(typeNames, (int index) =>
        {
          list.Add(Utilities.Reflection.Instantiate(Utilities.Reflection.GetSubclass(baseType)[index]));
        });
        menu.ShowAsContext();
      }

      // Clear
      if (GUILayout.Button("Clear", polymorphicListLayoutOptions))
      {
        list.Clear();
      }

      EditorGUILayout.EndHorizontal();

      if (list.Count == 0)
        return false;

      // Draw the elements
      bool changed = false;
      EditorGUILayout.BeginVertical(ReorderableListStyles.Container2);
      foreach (var element in list)
      {
        EditorGUILayout.Space();

        // Get the drawer for the type
        Type elementType = element.GetType();
        if (!objectDrawers.ContainsKey(elementType))
          objectDrawers.Add(elementType, new SerializedSystemObject.SystemObjectDrawer(elementType));
        SerializedSystemObject.SystemObjectDrawer drawer = objectDrawers[elementType];


        EditorGUILayout.BeginVertical(ReorderableListStyles.Container);
        {
          EditorGUILayout.LabelField(elementType.Name, EditorStyles.centeredGreyMiniLabel);
          if (!drawer.isDrawable)
            EditorGUILayout.LabelField($"There are no serialized fields for {elementType.Name}");
          else
            changed |= drawer.DrawEditorGUILayout(element);
        }
        EditorGUILayout.EndVertical();

      }
      EditorGUILayout.Space();
      EditorGUILayout.EndVertical();
      //EditorGUILayout.EndVertical();
      return changed;
    }

    /// <summary>
    /// Draws a list of elements deriving from a base class
    /// </summary>
    /// <returns>True if the height of the list changed, which signals a repaint event</returns>
    public static bool DrawPolymorphicList2(FieldInfo field, object target, string title, bool useTypeLabel = true)
    {
      // We need to remember this list since the height is variable depending on the
      // amount of fields being drawn      
      var fieldValue = field.GetValue(target);
      IList list = fieldValue as IList;

      if (list.Count == 0)
        return false;

      
      bool changed = false;

      return changed;
    }

      /// <summary>
      /// Draws a field using EditorGUILayout based on its members,
      /// (without using SerializedProperty)
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="field"></param>
      /// <returns>True if the field was changed</returns>
      public static bool DrawField<T>(T field)
    {
      Type type = field.GetType();
      if (!objectDrawers.ContainsKey(type))
        objectDrawers.Add(type, new SerializedSystemObject.SystemObjectDrawer(type));
      return objectDrawers[type].DrawEditorGUILayout(field);
    }

    /// <summary>
    /// Draws a field using EditorGUILayout based on its members,
    /// (without using SerializedProperty)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field"></param>
    /// <returns>True if the field was changed</returns>
    public static bool DrawField(FieldInfo field, object target)
    {
      if (!fieldDrawers.ContainsKey(field))
        fieldDrawers.Add(field, new SerializedSystemObject.FieldDrawer(field));
      return fieldDrawers[field].DrawEditorGUILayout(target);
    }
  }
}