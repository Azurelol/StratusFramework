using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Generic Property Drawer with a added utility functions
	/// </summary>
	public abstract class StratusPropertyDrawer : PropertyDrawer
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public class DrawCommand
		{
			public SerializedProperty property;
			public bool drawContent;

			public DrawCommand(SerializedProperty property)
			{
				this.property = property;
			}

			public virtual void Draw(Rect position)
			{
				position.height = EditorGUI.GetPropertyHeight(this.property);

				if (this.drawContent)
				{
					EditorGUI.PropertyField(position, this.property);
				}
				else
				{
					EditorGUI.PropertyField(position, this.property, GUIContent.none);
				}
			}
		}

		public class DrawPopUp : DrawCommand
		{
			public string[] values;

			public DrawPopUp(SerializedProperty property, string[] values) : base(property)
			{
				this.values = values;
			}

			public override void Draw(Rect position)
			{
				DrawPopup(position, this.property, this.values);
			}
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The number of fields in this property
		/// </summary>
		public float fieldCount { get; private set; }
		/// <summary>
		/// An array of the types of fields in this property
		/// </summary>
		public FieldInfo[] fields { get; private set; }
		/// <summary>
		/// The line height to be used
		/// </summary>
		public static float singleLineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		/// <summary>
		/// Standard vertical spacing betwween controls
		/// </summary>
		public static float verticalSpacing => EditorGUIUtility.standardVerticalSpacing;
		/// <summary>
		/// Whether this property has multiple values
		/// </summary>
		public bool hasMultipleFields { get; private set; }
		/// <summary>
		/// The current height for this property
		/// </summary>
		public float propertyHeight { get; protected set; }
		/// <summary>
		/// The parent property in an array
		/// </summary>
		protected SerializedProperty parent { get; private set; }
		/// <summary>
		/// The inspected object that this property belongs to
		/// </summary>
		protected Object target { get; private set; }
		/// <summary>
		/// Whether this property can fold
		/// </summary>
		public virtual bool fold { get; protected set; }

		private Dictionary<SerializedProperty, float> heights = new Dictionary<SerializedProperty, float>();
		public const float foldIndent = 8f;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			bool isArrayElement = this.IsArray(property);
			//this.isArray = property.isArray;

			float height = 0f;
			

			//StratusDebug.Log($"Height of {property.displayName}, array ? {property.isArray}");

			//if (property.isArray)
			//{
			//	foreach (SerializedProperty child in GetChildren(property))
			//	{
			//		float childHeight = heights[child];
			//		height += childHeight;
			//		//StratusDebug.Log($"height ({child.displayName}) += {childHeight}");
			//		//height += EditorGUI.GetPropertyHeight(child, true); // this.GetPropertyHeight(child);
			//		//height += GetPropertyHeight(child);
			//	}
			//}
			//else
			//{
			//	height = EditorGUI.GetPropertyHeight(property, true);
			//	//height = heights.GetValueOrDefault(property, propertyHeight);
			//	//StratusDebug.Log($"= {height}");
			//	//height = this.propertyHeight;
			//	//height = this.GetPropertyHeight(property);
			//}

			if (isArrayElement)
			{
				//if (property.isExpanded)
				//	height = this.GetPropertyHeight(property);
				//else
				height = EditorGUI.GetPropertyHeight(property, true);
			}
			else
			{
				height = this.GetPropertyHeight(property);
			}

			//StratusDebug.Log($"Height of {property.name} : {property.displayName}, array ? {isArrayElement} = {height}");

			//height += verticalSpacing;
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool isArrayElement = this.IsArray(property);
			// Reset the property height
			this.propertyHeight = 0f;
			this.fold = property.hasChildren;
			this.target = property.serializedObject.targetObject;

			//StratusDebug.Log($"Drawing {property.displayName}, position: {position}");

			label = EditorGUI.BeginProperty(position, label, property);
			{
				//if (isArray)
				//{
				//	//EditorGUI.indentLevel++;
				//	this.OnDrawArray(position, property);
				//	//EditorGUI.indentLevel--;
				//}
				//else
				//{
				// Fold behavior
				if (this.fold)
				{
					if (isArrayElement)
					{
						position.x += foldIndent;
						position.width -= foldIndent;
						label.text = property.displayName;
					}

					position.height = singleLineHeight;
					property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

					this.AddLine(ref position);
					if (property.isExpanded)
					{
						position = EditorGUI.IndentedRect(position);
						this.OnDrawProperty(position, property);
					}
					else
					{
						this.propertyHeight = singleLineHeight;
					}
				}
				else
				{
					this.DrawLabel(ref position, property.displayName);
					//EditorGUI.indentLevel++;
					this.OnDrawProperty(position, property);
					//EditorGUI.indentLevel--;
				}
			}
			//}

			EditorGUI.EndProperty();
			this.heights.AddOrUpdate(property, this.propertyHeight);

			//StratusDebug.Log($"Height of {property.name} : {property.displayName}, array element ? {isArrayElement} = {this.propertyHeight}");
		}

		protected abstract void OnDrawProperty(Rect position, SerializedProperty property);
		protected virtual float GetPropertyHeight(SerializedProperty property)
		{
			//return this.propertyHeight;
			return this.heights.GetValueOrDefault(property, this.propertyHeight);
			//return this.propertyHeight;
			//float value = 0;
			//this.fields = this.fieldInfo.FieldType.GetFields();
			//this.fieldCount = this.fields.Length;
			//property = property.serializedObject.FindProperty(this.fieldInfo.Name);
			//this.hasMultipleFields = this.fieldCount > 1;
			//value = property.isExpanded ? this.fieldCount + 1 : 1;
			//value *= lineHeight;
			//return value;
		}

		public bool IsArray(SerializedProperty serializedProperty)
		{
			return serializedProperty.name != this.fieldInfo.Name;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		private void OnDrawArray(Rect position, SerializedProperty property)
		{
			this.parent = property;

			for (int e = 0; e < property.arraySize; ++e)
			{
				SerializedProperty arrayElement = property.GetArrayElementAtIndex(e);
				this.OnDrawProperty(position, arrayElement);
			}
			//SerializedProperty[] children = property.arr
			//EditorGUI.PropertyField(position, property, true);
		}

		protected void DrawLabel(ref Rect position, string text)
		{
			EditorGUI.LabelField(position, text);
			position.y += singleLineHeight;
			this.propertyHeight += singleLineHeight;
		}

		protected void DrawProperty(ref Rect position, SerializedProperty property)
		{
			this.LayoutPropertyField(ref position, property);
		}

		private void LayoutPropertyField(ref Rect position, SerializedProperty property)
		{
			float height = EditorGUI.GetPropertyHeight(property);
			position.height = height;
			EditorGUI.PropertyField(position, property);

			// We need to increment by at least the default single line height
			height = Mathf.Max(height, singleLineHeight);
			position.y += height;
			position.height = singleLineHeight;
			this.propertyHeight += height;

			//StratusDebug.Log($"{property.displayName} : height now {propertyHeight}");
		}


		protected void DrawMultipleFields(ref Rect position, SerializedProperty property, GUIContent label)
		{
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

			if (property.isExpanded)
			{
				position.y += singleLineHeight;
				this.DrawFields(ref position, property);
			}
		}

		private void DrawFields(ref Rect position, SerializedProperty property)
		{
			foreach (FieldInfo field in this.fields)
			{
				SerializedProperty childProperty = property.FindPropertyRelative(field.Name);
				//EditorGUI.PropertyField(position, childProperty);
				this.LayoutPropertyField(ref position, childProperty);
			}
		}

		protected void DrawPropertiesHorizontal(ref Rect position, params SerializedProperty[] children)
		{
			int n = children.Length;
			position.width /= n;
			for (int p = 0; p < n; ++p)
			{
				SerializedProperty property = children[p];
				EditorGUI.PropertyField(position, property, GUIContent.none);
				position.x += position.width;
			}
			this.AddLine(ref position);
		}

		protected void DrawPropertiesHorizontal(ref Rect position, params DrawCommand[] drawCommands)
		{
			int n = drawCommands.Length;
			position.width /= n;
			for (int p = 0; p < n; ++p)
			{
				drawCommands[p].Draw(position);
				position.x += position.width;
			}
			this.AddLine(ref position);
		}

		protected void DrawPropertiesVertical(ref Rect position, params SerializedProperty[] children)
		{
			int n = children.Length;
			for (int p = 0; p < n; ++p)
			{
				SerializedProperty property = children[p];
				//EditorGUI.PropertyField(position, property);
				this.LayoutPropertyField(ref position, property);
			}
		}

		protected void DrawPropertiesInSingleLineLabeled(ref Rect position, params SerializedProperty[] children)
		{
			int n = children.Length;
			position.width /= n;
			for (int p = 0; p < n; ++p)
			{
				SerializedProperty property = children[p];
				EditorGUI.PropertyField(position, property);
				position.x += position.width;
			}
			this.AddLine(ref position);
		}

		protected void AddLine(ref Rect position)
		{
			position.y += singleLineHeight;
			this.propertyHeight += singleLineHeight;
		}

		//------------------------------------------------------------------------/
		// Utility
		//------------------------------------------------------------------------/
		/// <summary>
		/// Retrieves the enum value of the given property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="property"></param>
		/// <param name="enumPropertyName"></param>
		/// <returns></returns>
		public T GetEnumValue<T>(SerializedProperty property, string enumPropertyName)
		{
			SerializedProperty enumProperty = property.FindPropertyRelative(enumPropertyName);
			T value = (T)(object)enumProperty.enumValueIndex;
			return value;
		}

		/// <summary>
		///  Doesn't work for flags apparently
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumProperty"></param>
		/// <returns></returns>
		public static T GetEnumValue<T>(SerializedProperty enumProperty)
		{
			bool isFlag = typeof(T).IsDefined(typeof(System.FlagsAttribute), inherit: false);
			if (isFlag)
			{
				return (T)(object)enumProperty.intValue;
			}

			return (T)(object)enumProperty.enumValueIndex;
		}

		public void DrawPopup(ref Rect position, SerializedProperty stringProperty, string[] values)
		{
			DrawPopup(position, stringProperty, values);
			this.AddLine(ref position);
		}

		public static void DrawPopup(Rect position, SerializedProperty stringProperty, string[] values)
		{
			int index = values.FindIndex(stringProperty.stringValue);
			index = EditorGUI.Popup(position, index, values);
			stringProperty.stringValue = values.AtIndexOrDefault(index, string.Empty);

		}

		public static IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
		{
			property = property.Copy();
			SerializedProperty nextElement = property.Copy();
			bool hasNextElement = nextElement.NextVisible(false);
			if (!hasNextElement)
			{
				nextElement = null;
			}

			property.NextVisible(true);
			while (true)
			{
				if ((SerializedProperty.EqualContents(property, nextElement)))
				{
					yield break;
				}

				yield return property;

				bool hasNext = property.NextVisible(false);
				if (!hasNext)
				{
					break;
				}
			}
		}


	}

}