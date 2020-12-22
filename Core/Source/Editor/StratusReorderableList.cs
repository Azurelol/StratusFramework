using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// An extended reorderable list, that draws System.Object types using custom object drawers
	/// </summary>
	public class StratusReorderableList : ReorderableList
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public StratusSerializedField odinSerializedProperty { get; private set; }
		public string title { get; private set; }

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

		public StratusReorderableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
			: base(elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton)
		{
		}

		public StratusReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
		{
		}

		public static StratusReorderableList PolymorphicList(StratusSerializedField serializedProperty)
		{
			if (!serializedProperty.isList)
			{
				throw new ArgumentException($"The field {serializedProperty.displayName} is not an array!");
			}

			IList list = serializedProperty.asList;
			Type baseElementType = Utilities.StratusReflection.GetIndexedType(list);
			StratusReorderableList reorderableList = new StratusReorderableList(list, baseElementType, true, true, true, true);
			reorderableList.SetPolymorphic(serializedProperty);
			return reorderableList;
		}

		public static StratusReorderableList List(FieldInfo field, object target)
		{
			StratusSerializedField odinSerializedProperty = new StratusSerializedField(field, target);
			return PolymorphicList(odinSerializedProperty);
		}

		public static StratusReorderableList List(SerializedProperty serializedProperty)
		{
			StratusReorderableList reorderableList = new StratusReorderableList(serializedProperty.serializedObject, serializedProperty, true, true, true, true);
			reorderableList.SetDefault(serializedProperty);
			return reorderableList;
		}

		public static StratusReorderableList List(SerializedProperty serializedProperty, string title)
		{
			StratusReorderableList reorderableList = List(serializedProperty);
			reorderableList.title = title;
			return reorderableList;
		}

		public static void DrawCachedPolymorphicList(FieldInfo field, object target)
		{
			IList list = field.GetValue(target) as IList;
			if (!cachedLists.ContainsKey(list))
			{
				StratusReorderableList reorderableList = List(field, target);
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

		public void SetPolymorphic(StratusSerializedField serializedField)
		{
			this.SetHeaderCallback(serializedField);
			this.SetPolymorphicElementDrawCallback(serializedField);
			this.SetPolymorphicElementHeightCallback(serializedField);
			this.SetElementAddCallback(serializedField);
		}

		//------------------------------------------------------------------------/
		// Callbacks
		//------------------------------------------------------------------------/
		public void SetHeaderCallback(SerializedProperty serializedProperty)
		{
			this.drawHeaderCallback = (Rect rect) =>
			{
				Rect newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
				serializedProperty.isExpanded = EditorGUI.Foldout(newRect, serializedProperty.isExpanded, serializedProperty.displayName);
			};
		}

		public void SetHeaderCallback(StratusSerializedField serializedProperty)
		{
			this.drawHeaderCallback = (Rect rect) =>
			{
				Rect newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
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
					 GUI.enabled = index == this.count;
					 return;
				 }

				 //EditorGUI.PropertyField(rect, serializedProperty);

				 SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
				 //rect.y += 2;
				 //rect.height = EditorGUIUtility.singleLineHeight;
				 Debug.Log($"Drawing {serializedProperty.name} index {index}");
				 EditorGUI.PropertyField(rect, element);
				 //EditorGUI.ObjectField(, element, GUIContent.none);
			 };
		}

		public void SetPolymorphicElementDrawCallback(StratusSerializedField serializedProperty)
		{
			this.drawElementCallback =
			 (Rect rect, int index, bool isActive, bool isFocused) =>
			 {
				 if (!serializedProperty.isExpanded)
				 {
					 return;
				 }

				 // Get the drawer for the element type
				 object element = serializedProperty.GetArrayElementAtIndex(index);
				 Type elementType = element.GetType();
				 StratusSerializedEditorObject.ObjectDrawer drawer = StratusSerializedEditorObject.GetObjectDrawer(elementType);

				 // Draw the element
				 Rect position = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
				 if (this.drawElementTypeLabel)
				 {
					 EditorGUI.LabelField(position, elementType.Name, elementLabelStyle);
					 position.y += StratusEditorUtility.lineHeight;
				 }
				 drawer.DrawEditorGUI(position, element);
			 };
		}

		public void SetElementHeightCallback(SerializedProperty serializedProperty)
		{
			this.elementHeightCallback = (int indexer) =>
			{
				if (!serializedProperty.isExpanded)
				{
					return 0;
				}
				else
				{
					return EditorGUI.GetPropertyHeight(serializedProperty.GetArrayElementAtIndex(indexer));
					//return this.elementHeight;
				}
			};
		}

		public void SetPolymorphicElementHeightCallback(StratusSerializedField serializedProperty)
		{
			this.elementHeightCallback = (int indexer) =>
			{
				if (!serializedProperty.isExpanded)
				{
					return 0;
				}
				else
				{
					StratusSerializedEditorObject.ObjectDrawer drawer = StratusSerializedEditorObject.GetObjectDrawer(serializedProperty.GetArrayElementAtIndex(indexer));
					float height = drawer.height;
					// We add an additional line of height since we are drawing a label for polymorphic list
					if (this.drawElementTypeLabel)
					{
						height += StratusSerializedEditorObject.DefaultObjectDrawer.lineHeight;
					}

					return height;
				}
			};
		}

		public void SetElementAddCallback(StratusSerializedField serializedProperty)
		{
			this.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
			{
				Type baseType = serializedProperty.listElementType;
				GenericMenu menu = new GenericMenu();
				string[] typeNames = Utilities.StratusReflection.GetSubclassNames(baseType);
				menu.AddItems(typeNames, (int index) =>
				{
					serializedProperty.asList.Add(Utilities.StratusReflection.Instantiate(Utilities.StratusReflection.GetSubclass(baseType)[index]));
				});
				menu.ShowAsContext();
			};

			this.displayAdd = true;
		}
	}

	public class DefaultReorderableListImplementation
	{
	}

}