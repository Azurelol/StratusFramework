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
    public string gameObjectName;
    public Type type;
    public FieldInfo[] fields;
    public PropertyInfo[] properties;
    public MemberInfo[] members;
    public bool[] favoriteFields, favoriteProperties;
    public int fieldCount;
    public int propertyCount;
    public bool alphabeticalSorted;

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
    public ComponentInfo(Component component, bool alphabeticalSort = false)
    {
      this.gameObjectName = component.gameObject.name;
      this.component = component;
      this.Initialize();
      this.Save();
    }

    private void Initialize()
    {
      this.type = this.component.GetType();

      this.fields = this.type.GetFields(bindingFlags);
      if (this.alphabeticalSorted)
        Array.Sort(this.fields, delegate(FieldInfo a, FieldInfo b) { return a.Name.CompareTo(b.Name); });

      this.fieldValues = new object[this.fields.Length];
      this.fieldValueStrings = new string[this.fields.Length];

      this.properties = this.type.GetProperties(bindingFlags);
      if (this.alphabeticalSorted)
        Array.Sort(this.properties, delegate (PropertyInfo a, PropertyInfo b) { return a.Name.CompareTo(b.Name); });

      this.propertyValues = new object[this.properties.Length];
      this.propertyValueStrings = new string[this.properties.Length];

      this.members = this.type.GetMembers(bindingFlags);
      this.membersByName = new Dictionary<string, MemberInfo>();
      //this.gameObjectName = this.component.name;
    }

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
    }

    public object GetValue(FieldInfo field) => field.GetValue(component);
    public object GetValue(PropertyInfo property) => property.GetValue(component);
  }

  /// <summary>
  /// Information about a gameobject and all its components
  /// </summary>
  [Serializable]
  public class GameObjectInformation : ISerializationCallbackReceiver
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
      public string gameObjectName; 
      public int componentIndex;
      public int memberIndex;
      public ComponentInfo component;

      public string latestValueString { get; private set; }
      public object latestValue { get; private set; }         

      public MemberReference(MemberInfo member, ComponentInfo componentInfo, int index)
      {
        this.name = member.Name;
        this.componentName = componentInfo.name;
        this.gameObjectName = componentInfo.gameObjectName;
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
    public int fieldCount { get; private set; }
    public int propertyCount { get; private set; }
    public int memberCount => fieldCount + propertyCount;
    public MemberReference[] memberReferences { get; private set; }
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

      // Update member references
      foreach (var member in this.favorites)
      {
        if (!componentMap.ContainsKey(member.componentIndex))        
          continue;
        
        ComponentInfo component = this.componentMap[member.componentIndex];
        if (!(component.name == member.componentName) || !component.membersByName.ContainsKey(member.name))
          continue;
        
        member.Set(component);
      }

      this.Initialize();
    }

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/  
    public GameObjectInformation(GameObject target)
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

      this.Initialize();
    }

    private void Initialize()
    {
      this.memberReferences = this.SetAllMemberReferences();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  
    public void Watch(MemberInfo member, ComponentInfo componentInfo, int memberIndex)
    {
      MemberReference memberReference = new MemberReference(member, componentInfo, memberIndex);
      this.favorites.Add(memberReference);      
    }

    public void RemoveWatch(MemberInfo member, ComponentInfo componentInfo, int memberIndex)
    {
      this.favorites.RemoveAll(x => x.name == member.Name && x.memberIndex == memberIndex);      
    }

    public void UpdateFavorites()
    {
      foreach(var member in this.favorites)
      {
        member.UpdateValue();
      }
    }

    private MemberReference[] SetAllMemberReferences()
    {
      List<MemberReference> memberReferences = new List<MemberReference>();
      foreach (var component in this.components)
      {
        for(int f = 0; f < component.fieldCount; ++f)
        {
          MemberReference memberReference = new MemberReference(component.fields[f], component, f);
          memberReferences.Add(memberReference);
        }

        for (int p = 0; p < component.propertyCount; ++p)
        {
          MemberReference memberReference = new MemberReference(component.properties[p], component, p);
          memberReferences.Add(memberReference);
        }

      }
      return memberReferences.ToArray();
    }

  }

}