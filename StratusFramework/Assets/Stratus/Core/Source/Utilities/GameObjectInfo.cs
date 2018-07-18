using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Serialization;
using System.Linq.Expressions;

namespace Stratus
{
  /// <summary>
  /// Information about a component
  /// </summary>
  [Serializable]
  public class ComponentInfo : ISerializationCallbackReceiver
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/


    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public Component component;
    public Type type;
    public FieldInfo[] fields;
    public PropertyInfo[] properties;
    public MemberInfo[] members;
    public bool[] favoriteFields, favoriteProperties;
    public int fieldCount;
    public int propertyCount;

    [NonSerialized]
    public object[] fieldValues, propertyValues, favoriteValues;
    [NonSerialized]
    public string[] fieldValueStrings, propertyValueStrings, favoriteValueStrings;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public string name => type.Name;
    private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    public int memmberCount => members.Length;
    public bool hasFields => fieldCount > 0;
    public bool hasProperties => propertyCount > 0;
    public Dictionary<string, MemberInfo> membersByName { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
      this.Initialize();
      if (this.fieldCount != this.fields.Length || this.propertyCount != this.properties.Length)
        this.Save();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public ComponentInfo(Component component)
    {
      this.component = component;
      this.Initialize();
      this.Save();
    }

    private void Initialize()
    {
      this.type = this.component.GetType();

      this.fields = this.type.GetFields(bindingFlags);
      this.fieldValues = new object[this.fields.Length];
      this.fieldValueStrings = new string[this.fields.Length];

      this.properties = this.type.GetProperties(bindingFlags);
      this.propertyValues = new object[this.properties.Length];
      this.propertyValueStrings = new string[this.properties.Length];

      this.members = this.type.GetMembers(bindingFlags);
      this.membersByName = new Dictionary<string, MemberInfo>();

      //for (int i = 0; i < this.favoriteFields.Length; ++i)
      //{
      //  if (this.favoriteFields[i] == true)
      //    this.Watch(this.fields[i]);
      //}
      //
      //for (int i = 0; i < this.favoriteProperties.Length; ++i)
      //{
      //  if (this.favoriteProperties[i] == true)
      //    this.Watch(this.properties[i]);
      //}
    }

    //public void Watch(MemberInfo member)
    //{
    //  favorites.Add(member);
    //}
    //
    //public void StopWatch(MemberInfo member)
    //{
    //  favorites.Remove(member);
    //}

    private void Save()
    {
      this.fieldCount = this.fields.Length;
      this.propertyCount = this.properties.Length;
      this.favoriteFields = new bool[this.fieldCount];
      this.favoriteProperties = new bool[this.propertyCount];
    }

    public void UpdateValues()
    {
      // Some properties may fail in editor or in play mode
      for (int f = 0; f < fields.Length; ++f)
      {
        try
        {
          object value = this.GetValue(this.fields[f]);
          this.fieldValues[f] = this.GetValue(this.fields[f]);
          this.fieldValueStrings[f] = value.ToString();
        }
        catch (Exception e)
        {
        }
      }

      for (int p = 0; p < properties.Length; ++p)
      {
        try
        {
          object value = this.GetValue(this.properties[p]);
          this.propertyValues[p] = value;
          this.propertyValueStrings[p] = value.ToString();
        }
        catch (Exception e)
        {
        }
      }

      //for (int p = 0; p < favorites.Count; ++p)
      //{
      //  try
      //  {
      //    object value = this.GetValue(this.favorites[p]);
      //    this.propertyValues[p] = value;
      //    this.propertyValueStrings[p] = value.ToString();
      //  }
      //  catch (Exception e)
      //  {
      //  }
      //}
    }

    public object GetValue(FieldInfo field) => field.GetValue(component);
    public object GetValue(PropertyInfo property) => property.GetValue(component);


  }

  /// <summary>
  /// Information about a gameobject and all its components
  /// </summary>
  [Serializable]
  public class GameObjectInfo : ISerializationCallbackReceiver
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/  
    [Serializable]
    public class MemberReference
    {
      public MemberTypes type;
      public string name;
      public string componentName;
      public int componentIndex;
      public int memberIndex;
      public ComponentInfo component;

      public string latestValueString { get; private set; }
      public object latestValue { get; private set; }         

      public MemberReference(MemberInfo member, ComponentInfo componentInfo, int index)
      {
        this.name = member.Name;
        this.componentName = componentInfo.name;
        this.type = member.MemberType;
        this.memberIndex = index;

        this.Set(componentInfo);
      }

      public void Set(ComponentInfo componentInfo)
      {
        this.component = componentInfo;
      }

      public void UpdateValue()
      {
        object value = null;
        try
        {
          switch (this.type)
          {
            case MemberTypes.Field:
              value = component.fields[memberIndex].GetValue(component.component);
              break;
            case MemberTypes.Property:
              value = component.properties[memberIndex].GetValue(component.component);
              break;
          }
        }
        catch
        {
        }

        this.latestValue = value;
        this.latestValueString = value != null ? value.ToString() : string.Empty;
      }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    public GameObject target;
    public ComponentInfo[] components;
    public List<MemberReference> favorites = new List<MemberReference>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    public int numberofComponents { get; private set; }
    public string[] displayContent { get; private set; }
    public int fieldCount { get; private set; }
    public int propertyCount { get; private set; }
    public int memberCount => fieldCount + propertyCount;
    public bool isValid => target != null && this.numberofComponents > 0;
    public Dictionary<int, ComponentInfo> componentMap { get; private set; } = new Dictionary<int, ComponentInfo>();


    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/  
    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
      if (this.components == null)
        return;

      this.componentMap = new Dictionary<int, ComponentInfo>();
      for (int i = 0; i < this.components.Length; ++i)
      {
        componentMap.Add(i, this.components[i]);
      }

      foreach (var member in this.favorites)
      {
        if (!componentMap.ContainsKey(member.componentIndex))
        {
          continue;
        }

        ComponentInfo component = this.componentMap[member.componentIndex];
        if (!(component.name == member.componentName) || !component.membersByName.ContainsKey(member.name))
        {
          continue;
        }

        member.Set(component);
      }
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  
    public GameObjectInfo(GameObject target)
    {
      // We will be counting the total number of members
      this.fieldCount = 0;
      this.propertyCount = 0;

      // Set target
      this.target = target;

      // Set 
      Component[] targetComponents = target.GetComponents<Component>();
      List<ComponentInfo> components = new List<ComponentInfo>();
      for (int i = 0; i < targetComponents.Length; ++i)
      {
        Component component = targetComponents[i];
        if (component == null)
          return;

        ComponentInfo componentInfo = new ComponentInfo(component);
        this.fieldCount += componentInfo.fieldCount;
        this.propertyCount += componentInfo.propertyCount;
        components.Add(componentInfo);
        this.componentMap.Add(i, componentInfo);
      }

      this.components = components.ToArray();
      this.numberofComponents = this.components.Length;
      this.SetDisplayContent();
    }

    public void Watch(MemberInfo member, ComponentInfo componentInfo, int memberIndex)
    {
      MemberReference memberReference = new MemberReference(member, componentInfo, memberIndex);
      this.favorites.Add(memberReference);
    }
    public void RemoveWatch(MemberInfo member, ComponentInfo componentInfo)
    {
      this.favorites.RemoveAll(x => x.name == member.Name && x.component == componentInfo);
    }

    public void UpdateFavorites()
    {
      foreach(var member in this.favorites)
      {
        member.UpdateValue();
      }
    }

    private void SetDisplayContent()
    {
      // Now set the display content list            
      this.displayContent = new string[4 * this.memberCount];

      int i = 0;
      foreach (var component in this.components)
      {

        foreach (var field in component.fields)
        {
          this.displayContent[i++] = component.type.Name;
          this.displayContent[i++] = "Field";
          this.displayContent[i++] = field.Name;
          this.displayContent[i++] = string.Empty;
        }

        foreach (var property in component.properties)
        {
          this.displayContent[i++] = component.type.Name;
          this.displayContent[i++] = "Property";
          this.displayContent[i++] = property.Name;
          this.displayContent[i++] = string.Empty;
        }

      }
    }

    public void UpdateDisplayContent()
    {
      int i = 0;

      foreach (var component in this.components)
      {
        for (int k = 0; k < component.fieldCount; ++k)
        {
          i += 3;
          this.displayContent[i] = component.fieldValueStrings[k];
        }

        for (int k = 0; k < component.propertyCount; ++k)
        {
          i += 4;
          this.displayContent[i] = component.propertyValueStrings[k];
        }
      }
    }


  }

}