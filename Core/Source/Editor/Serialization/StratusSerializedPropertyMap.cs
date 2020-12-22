using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Stratus
{
	/// <summary>
	/// Encapsulates all serialized properties of a given object, so they can be easily found
	/// </summary>
	public class StratusSerializedPropertyMap
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// A map of all available properties by name
		/// </summary>
		private Dictionary<string, SerializedProperty> propertyMap { get; set; } = new Dictionary<string, SerializedProperty>();
		/// <summary>
		/// The set of properties of the most-derived class
		/// </summary>
		public Tuple<Type, SerializedProperty[]> declaredProperties => this.propertyGroups.Last();
		/// <summary>
		/// A map of all property groups by the type
		/// </summary>
		public Dictionary<Type, SerializedProperty[]> propertiesByType { get; set; } = new Dictionary<Type, SerializedProperty[]>();
		/// <summary>
		/// A list of all different property groups, starting from the base class to the most-derived class
		/// </summary>
		public List<Tuple<Type, SerializedProperty[]>> propertyGroups { get; set; } = new List<Tuple<Type, SerializedProperty[]>>();
		/// <summary>
		/// All the serialized properties of the target object
		/// </summary>
		public SerializedProperty[] properties { get; private set; }
		/// <summary>
		/// The target being inspected
		/// </summary>
		public UnityEngine.Object target { get; set; }
		/// <summary>
		/// The target being inspected
		/// </summary>
		public SerializedObject serializedObject { get; set; }
		/// <summary>
		/// The total amount of properties
		/// </summary>
		public int propertyCount => this.propertyMap.Count;
		/// <summary>
		/// Whether the property map is currently valid
		/// </summary>
		public bool valid => this.serializedObject.targetObject != null;
		/// <summary>
		/// The base type of which this property map will stop at
		/// </summary>
		public Type baseType { get; private set; }

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusSerializedPropertyMap(UnityEngine.Object target, SerializedObject serializedObject)
		{
			this.target = target;
			this.serializedObject = serializedObject;
			this.AddProperties();
		}

		public StratusSerializedPropertyMap(SerializedObject serializedObject)
		{
			this.serializedObject = serializedObject;
			this.target = serializedObject.targetObject;
			this.AddProperties();
		}

		public StratusSerializedPropertyMap(UnityEngine.Object target, Type baseType)
		{
			this.target = target;
			this.serializedObject = new SerializedObject(target);
			this.baseType = baseType;
			this.AddProperties();
		}
		public StratusSerializedPropertyMap(UnityEngine.Object target)
		{
			this.target = target;
			this.serializedObject = new SerializedObject(target);
			this.AddProperties();
		}

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/
		/// <summary>
		/// Gets all the serialized property for the given Unity Object of a specified type
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static SerializedProperty[] GetSerializedProperties(SerializedObject serializedObject, Type type)
		{
			FieldInfo[] propInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			SerializedProperty[] properties = new SerializedProperty[propInfo.Length];

			for (int a = 0; a < properties.Length; a++)
			{
				properties[a] = serializedObject.FindProperty(propInfo[a].Name);

				if (properties[a] == null)
				{
					//Trace.Script("Could not find property: " + propInfo[a].Name);
				}
			}

			return properties;
		}

		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>

		public static bool DrawSerializedProperty(SerializedProperty property, SerializedObject serializedObject)
		{
			EditorGUI.BeginChangeCheck();

			// Arrays
			if (property.isArray && property.propertyType != SerializedPropertyType.String)
			{
				StratusReorderableList.List(property);
			}
			else
			{
				EditorGUILayout.PropertyField(property, true);
			}

			// If property was changed, save
			if (EditorGUI.EndChangeCheck())
			{
				// Record change
				Undo.RecordObject(property.objectReferenceValue, property.name);

				serializedObject.ApplyModifiedProperties();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public bool DrawProperty(SerializedProperty property)
		{
			return DrawSerializedProperty(property, this.serializedObject);
		}
        
        /// <summary>
        /// Draw all serialized properties
        /// </summary>
        public void DrawProperties()
        {
            foreach(var property in properties)
            {
                property.isExpanded = EditorGUILayout.PropertyField(property);
            }
        }

		/// <summary>
		/// Draws a serialized property, saving any recorded changes
		/// </summary>
		/// <param name="prop"></param>
		/// <returns>True if the property changed, false if it was not drawn or found.</returns>
		public bool DrawProperty(string propertyName)
		{
			if (!this.propertyMap.ContainsKey(propertyName))
			{
				return false;
			}

			return DrawSerializedProperty(this.propertyMap[propertyName], this.serializedObject);
		}

		/// <summary>
		/// Returns the serialized property
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SerializedProperty GetProperty(string name)
		{
			if (!this.propertyMap.ContainsKey(name))
			{
				throw new ArgumentException($"The key {name} is not present among serialized properties for this object");
			}

			//RecreateSerializedObject();

			return this.propertyMap[name];
		}

		/// <summary>
		/// Recreates the erialized object, if needed.
		/// When you start or stop playing, the editor is actually reloading the scene from a temporary save file.
		/// </summary>
		public void RecreateSerializedObject()
		{
			if (this.serializedObject.targetObject == null)
			{
				int id = this.serializedObject.targetObject.GetInstanceID();
				UnityEngine.Object targetObject = EditorUtility.InstanceIDToObject(id);
				if (targetObject != null)
				{
					this.serializedObject = new SerializedObject(targetObject);
				}
			}
		}


		/// <summary>
		/// Adds all the properties 
		/// </summary>
		private void AddProperties()
		{
			List<SerializedProperty> allProperties = new List<SerializedProperty>();

			// For every type, starting from the most derived up to the base, get its serialized properties      
			Type declaredType = this.target.GetType();
			Type currentType = declaredType;

			while (currentType != this.baseType)
			{
				// Add the properties onto the map
				SerializedProperty[] properties = GetSerializedProperties(this.serializedObject, currentType);
				foreach (SerializedProperty property in properties)
				{
					// Record this property
					if (property == null)
					{
						continue;
					}

					allProperties.Add(property);
					this.propertyMap.Add(property.name, property);
				}

				// Add all the properties for this type into the property map by type        
				this.propertiesByType.Add(currentType, properties);
				this.propertyGroups.Add(new Tuple<Type, SerializedProperty[]>(currentType, properties));

				currentType = currentType.BaseType;
			}

			this.propertyGroups.Reverse();
			this.properties = allProperties.ToArray();
		}





	}

}