using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

namespace Stratus
{
  public class StratusReorderableList : ReorderableList
  {
    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public StratusReorderableList(IList elements, Type elementType) : base(elements, elementType)
    {
    }

    public StratusReorderableList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements)
    {
    }

    public StratusReorderableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton)
    {
    }

    public StratusReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
    {
    }

    public static StratusReorderableList PolymorphicList(OdinSerializedProperty serializedProperty)
    {
      if (!serializedProperty.isArray)
        throw new ArgumentException($"The field {serializedProperty.displayName} is not an array!");

      IList list = serializedProperty.list;
      Type baseElementType = Utilities.Reflection.GetIndexedType(list);
      var reorderableList = new StratusReorderableList(list, baseElementType, true, true, true, true);
      reorderableList.SetPolymorphic(serializedProperty);
      return reorderableList;
    }

    public static StratusReorderableList List(SerializedProperty serializedProperty, Type baseElementType)
    {
      var reorderableList = new StratusReorderableList(serializedProperty.serializedObject, serializedProperty, true, true, true, true);
      reorderableList.SetDefault(serializedProperty);
      return reorderableList;
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public void SetDefault(SerializedProperty serializedProperty)
    {
      this.SetHeaderCallback(serializedProperty);
      this.SetElementDrawCallback(serializedProperty);
      this.SetElementHeightCallback(serializedProperty);
    }

    public void SetPolymorphic(OdinSerializedProperty serializedProperty)
    {
      this.SetHeaderCallback(serializedProperty);
      this.SetElementDrawCallback(serializedProperty);
      this.SetElementHeightCallback(serializedProperty);
      this.SetElementAddCallback(serializedProperty);
    }

    public void SetHeaderCallback(SerializedProperty serializedProperty)
    {
      this.drawHeaderCallback = (Rect rect) =>
      {
        var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
        serializedProperty.isExpanded = EditorGUI.Foldout(newRect, serializedProperty.isExpanded, serializedProperty.displayName);
      };
    }

    public void SetHeaderCallback(OdinSerializedProperty serializedProperty)
    {
      this.drawHeaderCallback = (Rect rect) =>
      {
        var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
        serializedProperty.isExpanded = EditorGUI.Foldout(newRect, serializedProperty.isExpanded, $"{serializedProperty.displayName} ({serializedProperty.listElementType.Name}) ");
      };
    }

    public void SetElementDrawCallback(SerializedProperty serializedProperty)
    {
      this.drawElementCallback =
       (Rect rect, int index, bool isActive, bool isFocused) =>
       {
         if (!serializedProperty.isExpanded)
         {
           GUI.enabled = index == count;
           return;
         }

         var element = serializedProperty.GetArrayElementAtIndex(index);
         rect.y += 2;
         EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
       };
    }

    public void SetElementDrawCallback(OdinSerializedProperty serializedProperty)
    {
      this.drawElementCallback =
       (Rect rect, int index, bool isActive, bool isFocused) =>
       {
         if (!serializedProperty.isExpanded)
         {
           GUI.enabled = index == count;
           return;
         }
         
         // Get the drawer for the element type
         var element = serializedProperty.GetArrayElementAtIndex(index);
         Type elementType = element.GetType();
         SerializedSystemObject.SystemObjectDrawer drawer = StratusEditorUtility.GetDrawer(elementType);
         
         // Draw the element
         Rect position = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
         EditorGUI.LabelField(position, elementType.Name, EditorStyles.centeredGreyMiniLabel);
         position.y += StratusEditorUtility.lineHeight;
         drawer.DrawEditorGUI(position, element);
       };
    }

    public void SetElementHeightCallback(SerializedProperty serializedProperty)
    {
      elementHeightCallback = (int indexer) =>
      {
        if (!serializedProperty.isExpanded)
          return 0;
        else
          return this.elementHeight;
      };
    }

    public void SetElementHeightCallback(OdinSerializedProperty serializedProperty)
    {
      elementHeightCallback = (int indexer) =>
      {
        if (!serializedProperty.isExpanded)
          return 0;
        else
        {
          return 150;
        }
      };
    }

    public void SetElementAddCallback(OdinSerializedProperty serializedProperty)
    {
      onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
      {
        Type baseType = serializedProperty.listElementType;
        var menu = new GenericMenu();
        string[] typeNames = Utilities.Reflection.GetSubclassNames(baseType);
        menu.AddItems(typeNames, (int index) =>
        {
          serializedProperty.list.Add(Utilities.Reflection.Instantiate(Utilities.Reflection.GetSubclass(baseType)[index]));
        });
        menu.ShowAsContext();
      };

      this.displayAdd = true;
    }


    //public void Set(HeaderCallbackDelegate headerCallback, ElementHeightCallbackDelegate heightCallback)

    public static ReorderableList GetListWithFoldout(SerializedObject serializedObject, SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
    {
      var list = new ReorderableList(serializedObject, property, draggable, displayHeader, displayAddButton, displayRemoveButton);

      list.drawHeaderCallback = (Rect rect) =>
      {
        var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
        property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, property.displayName);
      };

      list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
          if (!property.isExpanded)
          {
            GUI.enabled = index == list.count;
            return;
          }

          var element = list.serializedProperty.GetArrayElementAtIndex(index);
          rect.y += 2;
          EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };

      list.elementHeightCallback = (int indexer) =>
      {
        if (!property.isExpanded)
          return 0;
        else
          return list.elementHeight;
      };

      return list;
    }
  }

}