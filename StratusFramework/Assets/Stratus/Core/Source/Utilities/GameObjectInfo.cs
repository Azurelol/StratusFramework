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
  public class ComponentInformation : ISerializationCallbackReceiver
  {
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
    public bool valid { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
      if (this.component == null)
      {
        this.valid = false;
        return;
      }

      this.Initialize();
      if (this.fieldCount != this.fields.Length || this.propertyCount != this.properties.Length)
        this.Save();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public ComponentInformation(Component component, bool alphabeticalSort = false)
    {
      if (component == null)
      {
        this.valid = false;
        return;
      }

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
        Array.Sort(this.fields, delegate (FieldInfo a, FieldInfo b) { return a.Name.CompareTo(b.Name); });

      this.fieldValues = new object[this.fields.Length];
      this.fieldValueStrings = new string[this.fields.Length];

      this.properties = this.type.GetProperties(bindingFlags);
      if (this.alphabeticalSorted)
        Array.Sort(this.properties, delegate (PropertyInfo a, PropertyInfo b) { return a.Name.CompareTo(b.Name); });

      this.propertyValues = new object[this.properties.Length];
      this.propertyValueStrings = new string[this.properties.Length];

      this.members = this.type.GetMembers(bindingFlags);
      this.membersByName = new Dictionary<string, MemberInfo>();

      this.valid = true;
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
      /// <summary>
      /// The name of this member
      /// </summary>
      public string name;
      /// <summary>
      /// The type for this member
      /// </summary>
      public MemberTypes type;
      /// <summary>
      /// The name of the component this member is part of
      /// </summary>
      public string componentName;
      /// <summary>
      /// The name of the GameObject for the component this member is a part of 
      /// </summary>
      public string gameObjectName;
      /// <summary>
      /// The index for the component that this member is part of in a GameObject
      /// </summary>
      public int componentIndex;
      /// <summary>
      /// THe index to this member for either the fields or properties of the component
      /// </summary>
      public int memberIndex;

      [NonSerialized]
      public GameObjectInformation gameObjectInfo;
      [NonSerialized]
      public ComponentInformation componentInfo;

      public string latestValueString { get; private set; }
      public object latestValue { get; private set; }
      public bool isFavorite
      {
        get
        {
          bool value = false;
          switch (this.type)
          {
            case MemberTypes.Field:
              value = componentInfo.favoriteFields[memberIndex];
              break;
            case MemberTypes.Property:
              value = componentInfo.favoriteProperties[memberIndex];
              break;
          }
          return value;
        }
      }


      public MemberReference(MemberInfo member, ComponentInformation componentInfo, GameObjectInformation gameObjectInformation, int index)
      {
        this.name = member.Name;
        this.componentName = componentInfo.name;
        this.gameObjectName = componentInfo.gameObjectName;
        this.type = member.MemberType;
        this.memberIndex = index;
        this.Set(componentInfo, gameObjectInformation);
      }

      public void Set(ComponentInformation componentInfo, GameObjectInformation gameObjectInformation)
      {
        this.componentInfo = componentInfo;
        this.gameObjectInfo = gameObjectInformation;
      }

      public void UpdateValue()
      {
        object value = null;
        try
        {
          switch (this.type)
          {
            case MemberTypes.Field:
              value = componentInfo.fields[memberIndex].GetValue(componentInfo.component);
              break;
            case MemberTypes.Property:
              value = componentInfo.properties[memberIndex].GetValue(componentInfo.component);
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
    public ComponentInformation[] components;
    public List<MemberReference> favorites = new List<MemberReference>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    public int numberofComponents { get; private set; }
    public int fieldCount { get; private set; }
    public int propertyCount { get; private set; }
    public int memberCount => fieldCount + propertyCount;
    public MemberReference[] memberReferences { get; private set; } = new MemberReference[0];
    public bool isValid => target != null && this.numberofComponents > 0;
    //public Dictionary<int, ComponentInformation> componentMap { get; private set; } = new Dictionary<int, ComponentInformation>();

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

      //this.componentMap = new Dictionary<int, ComponentInformation>();
      //for (int i = 0; i < this.components.Length; ++i)
      //{
      //  componentMap.Add(i, this.components[i]);
      //}

      // Update member references
      //List<MemberReference> validFavorites = new List<MemberReference>();      
      foreach (var member in this.favorites)
      {
        // If the component doesn't match...
        if (!this.AssertComponentIndex(member))
          continue;

        // Assert the member index
        if (!this.AssertMemberIndex(member))
          continue;
        
        // Member reference is still valid!
        ComponentInformation component = this.components[member.componentIndex];
        member.Set(component, this);


        //ComponentInformation component = this.components[member.componentIndex];
        //if (!(component.name == member.componentName) || !component.membersByName.ContainsKey(member.name))
        //  continue;
        //member.Set(component, this);

        /// if (!componentMap.ContainsKey(member.componentIndex))
        ///   continue;

        /// if (!componentMap.ContainsKey(member.componentIndex))
        ///   continue;

        // If the member doesn't match


        // Member is still valid
        //validFavorites.Add(member);

        //if (!(component.name == member.componentName) || !component.membersByName.ContainsKey(member.name))
        //  continue;



      }

      //this.favorites = validFavorites;

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
      List<ComponentInformation> components = new List<ComponentInformation>();
      for (int i = 0; i < targetComponents.Length; ++i)
      {
        Component component = targetComponents[i];
        if (component == null)
          return;

        ComponentInformation componentInfo = new ComponentInformation(component);
        this.fieldCount += componentInfo.fieldCount;
        this.propertyCount += componentInfo.propertyCount;
        components.Add(componentInfo);
        //this.componentMap.Add(i, componentInfo);
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
    // Methods: Watch
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Adds a member to the watch list
    /// </summary>
    /// <param name="member"></param>
    /// <param name="componentInfo"></param>
    /// <param name="memberIndex"></param>
    public void Watch(MemberInfo member, ComponentInformation componentInfo, int memberIndex)
    {
      MemberReference memberReference = new MemberReference(member, componentInfo, this, memberIndex);
      this.favorites.Add(memberReference);
      GameObjectBookmark.UpdateFavoriteMembers();
    }

    /// <summary>
    /// Removes a member from the watch list
    /// </summary>
    /// <param name="member"></param>
    /// <param name="componentInfo"></param>
    /// <param name="memberIndex"></param>
    public void RemoveWatch(MemberInfo member, ComponentInformation componentInfo, int memberIndex)
    {
      this.favorites.RemoveAll(x => x.name == member.Name && x.componentName == componentInfo.name && x.memberIndex == memberIndex);
      GameObjectBookmark.UpdateFavoriteMembers();
    }

    /// <summary>
    /// Adds a member to the watch list
    /// </summary>
    /// <param name="member"></param>
    /// <param name="componentInfo"></param>
    /// <param name="memberIndex"></param>
    public void Watch(MemberReference memberReference)
    {
      if (this.AssertReference(memberReference))
        this.favorites.Add(memberReference);
      GameObjectBookmark.UpdateFavoriteMembers();
    }

    /// <summary>
    /// Removes a member from the watch list
    /// </summary>
    /// <param name="memberReference"></param>
    public void RemoveWatch(MemberReference memberReference)
    {
       if (this.AssertReference(memberReference))
        this.favorites.RemoveAll(x => x.name == memberReference.name && x.memberIndex == memberReference.memberIndex);
      GameObjectBookmark.UpdateFavoriteMembers();
    }

    //------------------------------------------------------------------------/
    // Methods: Update
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Updates the values of all the favorite members for this GameObject
    /// </summary>
    public void UpdateFavoritesValues()
    {
      foreach (var member in this.favorites)
      {
        member.UpdateValue();
      }
    }

    //------------------------------------------------------------------------/
    // Methods: References
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Saves all member references for this GameObject
    /// </summary>
    /// <returns></returns>
    private MemberReference[] SetAllMemberReferences()
    {
      List<MemberReference> memberReferences = new List<MemberReference>();
      foreach (var component in this.components)
      {
        for (int f = 0; f < component.fieldCount; ++f)
        {
          MemberReference memberReference = new MemberReference(component.fields[f], component, this, f);
          memberReferences.Add(memberReference);
        }

        for (int p = 0; p < component.propertyCount; ++p)
        {
          MemberReference memberReference = new MemberReference(component.properties[p], component, this, p);
          memberReferences.Add(memberReference);
        }

      }
      return memberReferences.ToArray();
    }

    /// <summary>
    /// Verifies that this member reference is still valid
    /// </summary>
    /// <param name="memberReference"></param>
    /// <returns></returns>
    public bool AssertReference(MemberReference memberReference)
    {
      // If this is not the GameObject this member reference is for
      if (memberReference.gameObjectInfo != this)
      {
        //throw new ArgumentException($"The member {memberReference.name} is not a member among the components of the GameObject {this.target.name}");
        return false;
      }

      // Noww assert all the others!
      return AssertComponentIndex(memberReference) && AssertMemberIndex(memberReference);
    }

    /// <summary>
    /// If the component index doesn't match the current component index,
    /// this means the component could have been shuffled or removed
    /// </summary>
    /// <param name="memberReference"></param>
    /// <returns></returns>
    public bool AssertComponentIndex(MemberReference memberReference)
    {
      if ((memberReference.componentIndex > this.numberofComponents - 1) ||
          (memberReference.componentName != this.components[memberReference.componentIndex].name))
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// If the member at the index doesn't match the member reference index,
    /// this means the member could have been removed or rearranged
    /// </summary>
    /// <param name="memberReference"></param>
    /// <returns></returns>
    public bool AssertMemberIndex(MemberReference memberReference)
    {
      ComponentInformation componentInformation = this.components[memberReference.componentIndex];
      switch (memberReference.type)
      {
        case MemberTypes.Field:
          if (componentInformation.fields[memberReference.memberIndex].Name != memberReference.name)
            return false;
          break;
        case MemberTypes.Property:
          if (componentInformation.properties[memberReference.memberIndex].Name != memberReference.name)
            return false;
          break;
      }
      return true;
    }

  }

}