using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	public abstract partial class StratusEditor
	{
		//------------------------------------------------------------------------/
		// Properties: Static
		//------------------------------------------------------------------------/
		private static Type hideInInspectorType { get; } = typeof(HideInInspector);
		private static Dictionary<Type, System.Func<Attribute, SerializedProperty, bool>> attributeFunctions { get; } = new Dictionary<Type, Func<Attribute, SerializedProperty, bool>>()
		{
		  { typeof(RangeAttribute), OnRangeAttribute },
		};

		private StratusLabeledContextAction<StratusInvokeMethodAttribute>[] buttons { get; set; }

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		/// <summary>
		/// Handles Unity's default attributes. Returns true if the property's default
		/// drawing behaviour was overridden by a custom atribute.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected virtual bool DrawDefaultPropertyAttributes(SerializedProperty property)
		{
			bool overridden = false;

			// Perhaps handle reordering ???
			foreach (Attribute attribute in this.propertyAttributes[property])
			{
				Type attributeType = attribute.GetType();
				overridden |= attributeFunctions.ContainsKey(attributeType) && attributeFunctions[attributeType](attribute, property);
			}

			return overridden;
		}

		/// <summary>
		/// Handles Unity's default attributes that can affect draw ordering and such
		/// </summary>
		/// <param name="property"></param>
		protected virtual void OnPropertyAttributesAdded(SerializedProperty property)
		{
			bool hideInInspector = this.GetAttribute<HideInInspector>(property, hideInInspectorType) != null;
			if (hideInInspector)
			{
				this.AddConstraint(this.False, property);
			}
			else
			{
				this.drawnProperties++;
			}
		}

		/// <summary>
		/// Returns the specified attribute from the property if present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="property"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		protected T GetAttribute<T>(SerializedProperty property, Type type) where T : Attribute
		{
			return (T)(this.propertyAttributesMap[property].ContainsKey(type) ? this.propertyAttributesMap[property][type] : null);
		}

		/// <summary>
		/// Scans all the methods of the inspected object to check for special attributes
		/// </summary>
		private void ScanMethods()
		{
			BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			MethodInfo[] methods = this.targetType.GetMethods(flags);

			List<StratusLabeledContextAction<StratusInvokeMethodAttribute>> buttons = new List<StratusLabeledContextAction<StratusInvokeMethodAttribute>>();
			foreach (MethodInfo method in methods)
			{
				IEnumerable<StratusInspectorAttribute> attributes = method.GetCustomAttributes<StratusInspectorAttribute>();
				attributes.ForEach((attribute) =>
				{
					if (attribute is StratusInvokeMethodAttribute)
					{
						StratusInvokeMethodAttribute buttonAttribute = attribute as StratusInvokeMethodAttribute;
						buttons.Add(new StratusLabeledContextAction<StratusInvokeMethodAttribute>(buttonAttribute.hasLabel ? buttonAttribute.label : method.Name,
							() => method.Invoke(method.IsStatic ? null : this.target, null), buttonAttribute));
					}
				});
			}

			this.buttons = buttons.ToArray();
		}

		//------------------------------------------------------------------------/
		// Methods: Unity Attributes
		//------------------------------------------------------------------------/
		public static bool OnRangeAttribute(Attribute attribute, SerializedProperty property)
		{
			RangeAttribute range = attribute as RangeAttribute;
			if (property.propertyType == SerializedPropertyType.Integer)
			{
				EditorGUILayout.IntSlider(property, (int)range.min, (int)range.max);
			}
			else if (property.propertyType == SerializedPropertyType.Float)
			{
				EditorGUILayout.Slider(property, range.min, range.max);
			}

			return true;
		}

		//private static bool OnHideInInspector(Attribute attribute, SerializedProperty property) => true;

	}
}