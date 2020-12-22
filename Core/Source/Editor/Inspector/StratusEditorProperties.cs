using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace Stratus
{
	public abstract partial class StratusEditor : UnityEditor.Editor
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Specific constraints that dictate whether a specified property should be displayed
		/// </summary>
		public PropertyConstraintMap propertyConstraints { get; set; } = new PropertyConstraintMap();
		/// <summary>
		/// Provided callbacks for when specific properties are changed
		/// </summary>
		public Dictionary<SerializedProperty, System.Action> propertyChangeCallbacks { get; set; } = new Dictionary<SerializedProperty, System.Action>();
		/// <summary>
		/// Overrides for drawing specific properties from specific types
		/// </summary>
		public Dictionary<SerializedProperty, System.Action<SerializedProperty>> propertyDrawOverrides { get; set; } = new Dictionary<SerializedProperty, Action<SerializedProperty>>();
		/// <summary>
		/// A map of all available properties by name
		/// </summary>
		public Dictionary<string, SerializedProperty> propertyMap { get; set; } = new Dictionary<string, SerializedProperty>();
		/// <summary>
		/// The custom attributes for this property
		/// </summary>
		public Dictionary<SerializedProperty, Attribute[]> propertyAttributes { get; private set; } = new Dictionary<SerializedProperty, Attribute[]>();
		/// <summary>
		/// The custom attributes for this property
		/// </summary>
		public Dictionary<SerializedProperty, Dictionary<Type, Attribute>> propertyAttributesMap { get; private set; } = new Dictionary<SerializedProperty, Dictionary<Type, Attribute>>();
		/// <summary>
		/// The set of properties of the most-derived class
		/// </summary>
		public Tuple<Type, StratusSerializedPropertyModel[]> declaredProperties => this.propertyGroups.Last();
		/// <summary>
		/// A map of all property groups by the type
		/// </summary>
		public Dictionary<Type, StratusSerializedPropertyModel[]> propertiesByType { get; set; } = new Dictionary<Type, StratusSerializedPropertyModel[]>();
		/// <summary>
		/// A map of all property groups by the type
		/// </summary>
		private Dictionary<Type, SerializedProperty[]> unityPropertiesByType { get; set; } = new Dictionary<Type, SerializedProperty[]>();
		/// <summary>
		/// A list of all different property groups, starting from the base class to the most-derived class
		/// </summary>
		public List<Tuple<Type, StratusSerializedPropertyModel[]>> propertyGroups { get; set; } = new List<Tuple<Type, StratusSerializedPropertyModel[]>>();
		/// <summary>
		/// A list of properties serialized by Odin, starting from the base class to the most derived class
		/// </summary>
		public List<Tuple<Type, StratusSerializedField[]>> odinSerializedProperties { get; set; } = new List<Tuple<Type, StratusSerializedField[]>>();
		/// <summary>
		/// The number of properties drawn by the base editor GUI
		/// </summary>
		protected int drawnProperties { get; set; }

		//------------------------------------------------------------------------/
		// Draw
		//------------------------------------------------------------------------/
		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>
		/// <returns>True if the property changed, false otherwise.</returns>

		public bool DrawSerializedProperty(SerializedProperty property, SerializedObject serializedObject)
		{
			EditorGUI.BeginChangeCheck();

			// If this property has been requested to be drawn in a custom manner
			if (this.propertyDrawOverrides.ContainsKey(property))
			{
				this.propertyDrawOverrides[property].Invoke(property);
			}
			else
			{
				// Arrays
				if (property.isArray && property.propertyType != SerializedPropertyType.String && this.drawReorderableLists)
				{
					this.reorderableLists[this.unitySerializedPropertyModels[property]].DoLayoutList();
					//this.DrawReorderableList(property, property.displayName);
				}
				else
				{
					// Enums
					if (property.propertyType == SerializedPropertyType.Enum && drawEnumPopup)
					{
						StratusSearchableEnum.EnumPopup(property);
					}
					else
					{
						EditorGUILayout.PropertyField(property, true);
					}
				}
			}

			// If property was changed, save
			if (EditorGUI.EndChangeCheck())
			{
				//StratusDebug.Log($"Detected change on {property.displayName}");

				// Record change
				Undo.RecordObject(this.target, property.name);

				// Apply the modified property
				serializedObject.ApplyModifiedProperties();

				// Inform that this property has been changed
				if (this.propertyChangeCallbacks.ContainsKey(property))
				{
					this.propertyChangeCallbacks[property].Invoke();
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Draws a property (really, a field) serialized by Odin Serializer
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public bool DrawSerializedProperty(StratusSerializedField property)
		{
			bool changed = false;
			EditorGUI.BeginChangeCheck();
			{
				if (property.isList)
				{
					//Debug.Log($"Now drawing the list { property.displayName}");
					this.reorderableLists[this.customSerializedPropertyModels[property]].DoLayoutList();
				}
				else
				{
					changed |= StratusEditorGUILayout.Field(property.field, this.target);
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				//StratusDebug.Log($"Detected change on {property.displayName}");
				EditorUtility.SetDirty(this.target);
				// Record change
				Undo.RecordObject(this.target, property.field.Name);
				// Apply the modified property
				this.serializedObject.ApplyModifiedProperties();
				changed = true;
			}
			return changed;
		}

		/// <summary>
		/// Draws the given set of properties according to any present constraints in the editor
		/// </summary>
		/// <param name="properties"></param>
		private bool DrawSerializedProperties(StratusSerializedPropertyModel[] properties)
		{
			bool propertiesChanged = false;
			for (int i = 0; i < properties.Length; i++)
			{
				StratusSerializedPropertyModel property = properties[i];
				//Debug.Log($"Drawing property {property.displayName}");
				switch (property.type)
				{
					case StratusSerializedPropertyModel.SerializationType.Unity:
						bool hasConstraint = this.propertyConstraints.ContainsKey(property.unitySerialized);
						if (hasConstraint)
						{
							bool canBeDrawn = this.propertyConstraints[property.unitySerialized].Invoke();
							if (!canBeDrawn)
							{
								continue;
							}
						}
						propertiesChanged |= this.DrawSerializedProperty(property.unitySerialized, this.serializedObject);
						break;

					case StratusSerializedPropertyModel.SerializationType.Custom:
						propertiesChanged |= this.DrawSerializedProperty(property.customSerialized);
						break;
				}


			}
			return propertiesChanged;
		}

		/// <summary>
		/// Adds all SerializedProperties to be inspected
		/// </summary>
		internal void ScanProperties()
		{
			// Reset the number of drawn properties
			this.drawnProperties = 0;

			// For every type, starting from the most derived up to the base, get its serialized properties      
			Type currentType = this.target.GetType();
			Type previousType = null;

			while (currentType != this.baseType && currentType != null)
			{
				// Serialized Properties
				StratusSerializedPropertyModel.Query query = new StratusSerializedPropertyModel.Query(serializedObject, currentType);

				foreach (StratusSerializedPropertyModel propertyModel in query.models)
				{
					if (propertyModel == null)
					{
						Debug.LogError($"A property was found to not be serialized properly while inspecting {this.target.name}. Did you forget a [Serializable] attribute on a class definition?");
						continue;
					}

					//Debug.Log($"Drawer for serialized property: {propertyModel.displayName} with type {propertyModel.type}");

					switch (propertyModel.type)
					{
						case StratusSerializedPropertyModel.SerializationType.Unity:
							{
								SerializedProperty serializedProperty = propertyModel.unitySerialized;
								this.propertyMap.Add(serializedProperty.name, serializedProperty);
								//Debug.Log($"Added property {serializedProperty.name}");
								this.unitySerializedPropertyModels.Add(serializedProperty, propertyModel);

								// Record the attributes for this property
								Attribute[] attributes = propertyModel.attributes;

								this.propertyAttributes.Add(serializedProperty, attributes);
								this.propertyAttributesMap.AddUnique(serializedProperty, new Dictionary<Type, Attribute>());
								foreach (Attribute attr in attributes)
								{
									this.propertyAttributesMap[serializedProperty].AddUnique(attr.GetType(), attr);
								}
								this.OnPropertyAttributesAdded(serializedProperty);

								// Check whether this property is an array
								if (serializedProperty.isArray && serializedProperty.propertyType != SerializedPropertyType.String)
								{
									this.reorderableLists.Add(propertyModel, StratusReorderableList.List(serializedProperty));
									//this.AddReorderableList(serializedProperty);
								}
							}
							break;

						case StratusSerializedPropertyModel.SerializationType.Custom:
							{
								this.customSerializedPropertyModels.Add(propertyModel.customSerialized, propertyModel);
								if (propertyModel.customSerialized.isList)
								{
									this.reorderableLists.Add(propertyModel, StratusReorderableList.PolymorphicList(propertyModel.customSerialized));
								}
							}
							break;
					}
				}


				this.propertiesByType.Add(currentType, query.models);
				this.unityPropertiesByType.Add(currentType, query.unitySerialized);
				this.propertyGroups.Add(new Tuple<Type, StratusSerializedPropertyModel[]>(currentType, query.models));

				//// Add all the properties for this type into the property map by type  
				//if (!currentType.IsGenericType)
				//{
				//	this.propertiesByType.Add(currentType, query.models);
				//	this.unityPropertiesByType.Add(currentType, query.unitySerialized);
				//	this.propertyGroups.Add(new Tuple<Type, StratusSerializedPropertyModel[]>(currentType, query.models));
				//}
				//else
				//{
				//	if (!previousType.IsGenericType)
				//	{
				//		SerializedProperty[] joinedUnityProperties = this.unityPropertiesByType[previousType].Concat(query.unitySerialized);
				//		this.unityPropertiesByType[previousType] = joinedUnityProperties;
				//		// Combined
				//		StratusSerializedPropertyModel[] joinedProperties = this.propertiesByType[previousType].Concat(query.models);
				//		this.propertiesByType[previousType] = joinedProperties;
				//		// Concat property groups
				//		this.propertyGroups.RemoveLast();
				//		this.propertyGroups.Add(new Tuple<Type, StratusSerializedPropertyModel[]>(previousType, joinedProperties));
				//	}
				//}

				// Move on to the parent type (if any)
				previousType = currentType;
				currentType = currentType.BaseType;
			}

			this.propertyGroups.Reverse();
		}

		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected bool DrawSerializedProperty(SerializedProperty property)
		{
			return this.DrawSerializedProperty(property, this.serializedObject);
		}


		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="prop"></param>
		/// <returns>True if the property changed, false if it was not drawn or found.</returns>
		public bool DrawSerializedProperty(string propertyName)
		{
			if (!this.propertyMap.ContainsKey(propertyName))
			{
				Debug.Log($"Property map does not contain {propertyName}");
				return false;
			}

			return this.DrawSerializedProperty(this.propertyMap[propertyName], this.serializedObject);
		}

		/// <summary>
		/// Returns all the serialized properties belonging to the specified inherited class
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public SerializedProperty[] GetSerializedPropertiesOfType(Type type)
		{
			return this.unityPropertiesByType[type];
		}

		/// <summary>
		/// Returns the serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public SerializedProperty GetSerializedProperty(string propertyName)
		{
			return this.propertyMap[propertyName];
		}

		/// <summary>
		/// Checks whether all the properties are under constraints. Returns false if none
		/// of the properties can be drawn.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		private bool ValidateConstraints(StratusSerializedPropertyModel[] properties)
		{
			foreach (StratusSerializedPropertyModel property in properties)
			{
				if (property.type != StratusSerializedPropertyModel.SerializationType.Unity)
				{
					continue;
				}

				bool foundConstraint = this.propertyConstraints.ContainsKey(property.unitySerialized);

				// If no constraint was found for this property, it means 
				// that at least one property can be drawn
				if (!foundConstraint)
				{
					return true;
				}
				// If the property was found and validated, it means we can draw it
				else
				{
					bool validated = this.propertyConstraints[property.unitySerialized]();
					if (validated)
					{
						return true;
					}
				}
			}

			// No constraints were validated
			return false;
		}

		//------------------------------------------------------------------------/
		// Utility
		//------------------------------------------------------------------------/
		/// <summary>
		/// Gets all the serialized property for the given Unity Object of a specified type
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Tuple<SerializedProperty[], StratusSerializedField[]> GetSerializedProperties(SerializedObject serializedObject, Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			//FieldInfo[] fields = type.GetSerializedFields(true);

			List<SerializedProperty> serializedProperties = new List<SerializedProperty>();
			List<StratusSerializedField> odinSerializedProperties = new List<StratusSerializedField>();

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo field = fields[i];
				if (field != null && (field.Attributes != FieldAttributes.NotSerialized))
				{
					bool serializedbyOdin = OdinSerializer.UnitySerializationUtility.OdinWillSerialize(field, true);
					bool serializedByUnity = OdinSerializer.UnitySerializationUtility.GuessIfUnityWillSerialize(field);

					// Odin
					if (serializedbyOdin && !serializedByUnity)
					{
						StratusSerializedField property = new StratusSerializedField(field, serializedObject.targetObject);
						odinSerializedProperties.Add(property);
					}
					// Unity
					else
					{
						SerializedProperty property = serializedObject.FindProperty(field.Name);
						if (property != null)
						{
							serializedProperties.Add(property);
						}
					}
				}
			}

			return new Tuple<SerializedProperty[], StratusSerializedField[]>(serializedProperties.ToArray(), odinSerializedProperties.ToArray());
		}
	}
}