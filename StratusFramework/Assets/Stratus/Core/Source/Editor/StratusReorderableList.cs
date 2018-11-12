using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

namespace Stratus
{
  /// <summary>
  /// An extended reorderable list, that draws System.Object types using custom object drawers
  /// </summary>
  public class StratusReorderableList : ReorderableList
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public StratusOdinSerializedProperty odinSerializedProperty { get; private set; }
    //public string label { get; set; }
    //public bool isExpanded { get; set; }

    //------------------------------------------------------------------------/
    // Properties: Static
    //------------------------------------------------------------------------/
    public static GUIStyle elementLabelStyle => EditorStyles.boldLabel;
    public bool drawElementTypeLabel { get; set; } = true;
    private static Dictionary<IList, StratusReorderableList> cachedLists { get; set; } = new Dictionary<IList, StratusReorderableList>();

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

    public static StratusReorderableList PolymorphicList(StratusOdinSerializedProperty serializedProperty)
    {
      if (!serializedProperty.isArray)
        throw new ArgumentException($"The field {serializedProperty.displayName} is not an array!");

      IList list = serializedProperty.list;
      Type baseElementType = Utilities.Reflection.GetIndexedType(list);
      var reorderableList = new StratusReorderableList(list, baseElementType, true, true, true, true);
      reorderableList.SetPolymorphic(serializedProperty);
      return reorderableList;
    }


    public static StratusReorderableList PolymorphicList(FieldInfo field, object target)
    {
      StratusOdinSerializedProperty odinSerializedProperty = new StratusOdinSerializedProperty(field, target);
      return PolymorphicList(odinSerializedProperty);
    }

    public static StratusReorderableList List(SerializedProperty serializedProperty, Type baseElementType)
    {
      var reorderableList = new StratusReorderableList(serializedProperty.serializedObject, serializedProperty, true, true, true, true);
      reorderableList.SetDefault(serializedProperty);
      return reorderableList;
    }

    public static void DrawCachedPolymorphicList(FieldInfo field, object target)
    {
      IList list = field.GetValue(target) as IList;
      if (!cachedLists.ContainsKey(list))
      {
        StratusReorderableList reorderableList = PolymorphicList(field, target);
        cachedLists.Add(list, reorderableList);
      }
      cachedLists[list].DoLayoutList();
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

    public void SetPolymorphic(StratusOdinSerializedProperty serializedProperty)
    {
      this.SetHeaderCallback(serializedProperty);
      this.SetPolymorphicElementDrawCallback(serializedProperty);
      this.SetPolymorphicElementHeightCallback(serializedProperty);
      this.SetElementAddCallback(serializedProperty);
    }

    //public void SetPolymorphic(FieldInfo field, IList list)
    //{
    //  this.label = field.Name;
    //  this.SetHeaderCallback();
    //  this.SetPolymorphicElementDrawCallback(serializedProperty);
    //  this.SetPolymorphicElementHeightCallback(serializedProperty);
    //  this.SetElementAddCallback(serializedProperty);
    //}

    //------------------------------------------------------------------------/
    // Callbacks
    //------------------------------------------------------------------------/
    public void SetHeaderCallback(SerializedProperty serializedProperty)
    {
      this.drawHeaderCallback = (Rect rect) =>
      {
        var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
        serializedProperty.isExpanded = EditorGUI.Foldout(newRect, serializedProperty.isExpanded, serializedProperty.displayName);
      };
    }

    public void SetHeaderCallback(StratusOdinSerializedProperty serializedProperty)
    {
      this.drawHeaderCallback = (Rect rect) =>
      {
        var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
        serializedProperty.isExpanded = EditorGUI.Foldout(newRect, serializedProperty.isExpanded, $"{serializedProperty.displayName} ({serializedProperty.listElementType.Name}) ");
      };
    }

    //public void SetHeaderCallback()
    //{
    //  this.drawHeaderCallback = (Rect rect) =>
    //  {
    //    var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
    //    serializedProperty.isExpanded = EditorGUI.Foldout(newRect, this.isExpanded, $"{serializedProperty.displayName} ({serializedProperty.listElementType.Name}) ");
    //  };
    //}

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

    public void SetPolymorphicElementDrawCallback(StratusOdinSerializedProperty serializedProperty)
    {
      this.drawElementCallback =
       (Rect rect, int index, bool isActive, bool isFocused) =>
       {
         if (!serializedProperty.isExpanded)
         {
           //EditorGUI.LabelField(rect, elementType.Name, elementLabelStyle);
           //GUI.enabled = index == count;
           return;
         }
         
         // Get the drawer for the element type
         var element = serializedProperty.GetArrayElementAtIndex(index);
         Type elementType = element.GetType();
         SerializedSystemObject.ObjectDrawer drawer = SerializedSystemObject.GetObjectDrawer(elementType);
         
         // Draw the element
         Rect position = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
         if (drawElementTypeLabel)
         {
           EditorGUI.LabelField(position, elementType.Name, elementLabelStyle);
           position.y += StratusEditorUtility.lineHeight;
         }
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

    public void SetPolymorphicElementHeightCallback(StratusOdinSerializedProperty serializedProperty)
    {
      elementHeightCallback = (int indexer) =>
      {
        if (!serializedProperty.isExpanded)
        {
          return 0;
          //return SerializedSystemObject.ObjectDrawer.lineHeight;
        }
        else
        {
          SerializedSystemObject.ObjectDrawer drawer = SerializedSystemObject.GetObjectDrawer(serializedProperty.GetArrayElementAtIndex(indexer));
          float height = drawer.height;
          // We add an additional line of height since we are drawing a label for polymorphic list
          if (drawElementTypeLabel) height += SerializedSystemObject.DefaultObjectDrawer.lineHeight;
          return height;
        }
      };
    }

    public void SetElementAddCallback(StratusOdinSerializedProperty serializedProperty)
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
  }

}