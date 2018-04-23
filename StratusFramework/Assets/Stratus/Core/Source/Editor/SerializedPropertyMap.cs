using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Rotorz.ReorderableList;

namespace Stratus
{
  /// <summary>
  /// Encapsulates all serialized properties of a given object, so they can be easily found
  /// </summary>
  public class SerializedPropertyMap
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
    public Tuple<Type, SerializedProperty[]> declaredProperties => propertyGroups.Last();
    /// <summary>
    /// A map of all property groups by the type
    /// </summary>
    public Dictionary<Type, SerializedProperty[]> propertiesByType { get; set; } = new Dictionary<Type, SerializedProperty[]>();
    /// <summary>
    /// A list of all different property groups, starting from the base class to the most-derived class
    /// </summary>
    public List<Tuple<Type, SerializedProperty[]>> propertyGroups { get; set; } = new List<Tuple<Type, SerializedProperty[]>>();
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
    public int propertyCount => propertyMap.Count;
    /// <summary>
    /// Whether the property map is currently valid
    /// </summary>
    public bool valid => serializedObject.targetObject != null;

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    public SerializedPropertyMap(UnityEngine.Object target, SerializedObject serializedObject)
    {
      this.target = target;
      this.serializedObject = serializedObject;
      AddProperties();
    }

    public SerializedPropertyMap(UnityEngine.Object target)
    {
      this.target = target;
      this.serializedObject = new SerializedObject(target);
      AddProperties();
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
        ReorderableListGUI.Title(property.displayName);
        ReorderableListGUI.ListField(property);
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
      return DrawSerializedProperty(property, serializedObject);
    }

    /// <summary>
    /// Draws a serialized property, saving any recorded changes
    /// </summary>
    /// <param name="prop"></param>
    /// <returns>True if the property changed, false if it was not drawn or found.</returns>
    public bool DrawProperty(string propertyName)
    {
      if (!propertyMap.ContainsKey(propertyName))
        return false;

      return DrawSerializedProperty(propertyMap[propertyName], serializedObject);
    }

    /// <summary>
    /// Returns the serialized property
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SerializedProperty GetProperty(string name)
    {
      //if (!valid)
      //  RecreateSerializedObject();

      return propertyMap[name];
    }

    /// <summary>
    /// Recreates the erialized object, if needed.
    /// When you start or stop playing, the editor is actually reloading the scene from a temporary save file.
    /// </summary>
    public void RecreateSerializedObject()
    {
      if (serializedObject.targetObject == null)
      {
        int id = serializedObject.targetObject.GetInstanceID();
        UnityEngine.Object targetObject = EditorUtility.InstanceIDToObject(id);
        if (targetObject != null)
        {
          serializedObject = new SerializedObject(targetObject);
        }
      }
    }
      

    /// <summary>
    /// Adds all the properties 
    /// </summary>
    private void AddProperties()
    {
      // For every type, starting from the most derived up to the base, get its serialized properties      
      Type declaredType = target.GetType();
      Type currentType = declaredType;
      while (currentType != typeof(MonoBehaviour))
      {
        // Add the properties onto the map
        var properties = GetSerializedProperties(serializedObject, currentType);
        foreach (var prop in properties)
        {
          // Check the attributes for this proeprty
          //prop.
          //Trace.Script($"- {prop.name}");

          propertyMap.Add(prop.name, prop);
          //if (prop.isArray && prop.propertyType != SerializedPropertyType.String)
          //{
          //  ReorderableList list = GetListWithFoldout(serializedObject, prop, true, true, true, true);
          //  reorderableLists.Add(prop, list);
          //}
        }

        // Add all the properties for this type into the property map by type        
        propertiesByType.Add(currentType, properties);
        propertyGroups.Add(new Tuple<Type, SerializedProperty[]>(currentType, properties));

        currentType = currentType.BaseType;
      }

      propertyGroups.Reverse();
    }





  }

}