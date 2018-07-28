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
    // Declarations
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Serialized reference to the member of a component
    /// </summary>
    [Serializable]
    public class MemberReference
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
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
      /// <summary>
      /// Whether this memebr reference is favorited
      /// </summary>
      public bool isWatched = false;

      [NonSerialized]
      public ComponentInformation componentInfo;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public string latestValueString { get; private set; }
      public object latestValue { get; private set; }
      public bool initialized { get; private set; } = false;

      //------------------------------------------------------------------------/
      // CTOR
      //------------------------------------------------------------------------/
      public MemberReference(MemberInfo member, ComponentInformation componentInfo, int index)
      {
        this.name = member.Name;
        this.componentName = componentInfo.name;
        this.gameObjectName = componentInfo.gameObjectName;
        this.type = member.MemberType;
        this.memberIndex = index;

        this.Initialize(componentInfo);
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Initializes this member reference, linking it to the component it's part of.
      /// This needs to be done before attempting to retrieve the value
      /// </summary>
      /// <param name="componentInfo"></param>
      public void Initialize(ComponentInformation componentInfo)
      {
        this.componentInfo = componentInfo;
        this.initialized = true;
      }

      /// <summary>
      /// Retrieves the latest value for this member
      /// </summary>
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
    public Component component;
    public string gameObjectName;
    public Type type;
    public int fieldCount;
    public int propertyCount;
    public bool alphabeticalSorted;
    public MemberReference[] memberReferences;

    [NonSerialized]
    public object[] fieldValues, propertyValues, favoriteValues;
    [NonSerialized]
    public string[] fieldValueStrings, propertyValueStrings, favoriteValueStrings;
    

    private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public string name => type.Name;
    public GameObject gameObject => component.gameObject;
    public MemberInfo[] members { get; private set; }
    public FieldInfo[] fields { get; private set; }
    public PropertyInfo[] properties { get; private set; }
    public int memmberCount => members.Length;
    public bool hasFields => fieldCount > 0;
    public bool hasProperties => propertyCount > 0;
    public Dictionary<string, MemberInfo> membersByName { get; private set; }
    public List<MemberReference> watchList { get; private set; }
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

      this.component = component;
      this.memberReferences = this.CreateAllMemberReferences();
      this.gameObjectName = component.gameObject.name;
      this.Initialize();
      this.Save();
    }

    /// <summary>
    /// Runtime: Record all type information about the members of the component
    /// </summary>
    private void Initialize()
    {
      // Type
      this.type = this.component.GetType();
      // Fields
      this.fields = this.type.GetFields(bindingFlags);
      if (this.alphabeticalSorted)
        Array.Sort(this.fields, delegate (FieldInfo a, FieldInfo b) { return a.Name.CompareTo(b.Name); });
      this.fieldValues = new object[this.fields.Length];
      this.fieldValueStrings = new string[this.fields.Length];
      // Properties
      this.properties = this.type.GetProperties(bindingFlags);
      if (this.alphabeticalSorted)
        Array.Sort(this.properties, delegate (PropertyInfo a, PropertyInfo b) { return a.Name.CompareTo(b.Name); });
      this.propertyValues = new object[this.properties.Length];
      this.propertyValueStrings = new string[this.properties.Length];
      // Members
      this.members = this.type.GetMembers(bindingFlags);
      this.membersByName = new Dictionary<string, MemberInfo>();
      // Set all member references
      foreach (var member in this.memberReferences)
      {
        member.Initialize(this);
      }

      // This information is now valid
      this.valid = true;
    }

    private void Save()
    {
      this.fieldCount = this.fields.Length;
      this.propertyCount = this.properties.Length;
    }

    /// <summary>
    /// Updates the values of all fields and properties for this component
    /// </summary>
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

    /// <summary>
    /// Updates the values of all the watched variables for this component
    /// </summary>
    public void UpdateWatchValues()
    {
      foreach (var member in this.watchList)
      {
        member.UpdateValue();
      }
    }

    /// <summary>
    /// Adds a member to the watch list
    /// </summary>
    /// <param name="member"></param>
    /// <param name="componentInfo"></param>
    /// <param name="memberIndex"></param>
    public void Watch(ComponentInformation.MemberReference memberReference)
    {
      memberReference.isWatched = true;
      if (this.AssertMemberIndex(memberReference))
        this.watchList.Add(memberReference);
      GameObjectBookmark.UpdateWatchList();
    }

    /// <summary>
    /// Removes a member from the watch list
    /// </summary>
    /// <param name="memberReference"></param>
    public void RemoveWatch(ComponentInformation.MemberReference memberReference)
    {
      memberReference.isWatched = false;
      if (this.AssertMemberIndex(memberReference))
        this.watchList.RemoveAll(x => x.name == memberReference.name && x.memberIndex == memberReference.memberIndex);
      GameObjectBookmark.UpdateWatchList();
    }

    /// <summary>
    /// Saves all member references for this GameObject
    /// </summary>
    /// <returns></returns>
    private MemberReference[] CreateAllMemberReferences()
    {
      // Make a reference for all members
      List<MemberReference> memberReferences = new List<MemberReference>();
      for (int f = 0; f < this.fieldCount; ++f)
      {
        MemberReference memberReference = new MemberReference(this.fields[f], this, f);
        memberReferences.Add(memberReference);
      }

      for (int p = 0; p < this.propertyCount; ++p)
      {
        MemberReference memberReference = new MemberReference(this.properties[p], this, p);
        memberReferences.Add(memberReference);
      }


      // Also rebuild favorites!
      this.watchList = new List<MemberReference>();

      return memberReferences.ToArray();
    }

    /// <summary>
    /// If the member at the index doesn't match the member reference index,
    /// this means the member could have been removed or rearranged
    /// </summary>
    /// <param name="memberReference"></param>
    /// <returns></returns>
    public bool AssertMemberIndex(ComponentInformation.MemberReference memberReference)
    {
      switch (memberReference.type)
      {
        case MemberTypes.Field:
          if (this.fields[memberReference.memberIndex].Name != memberReference.name)
            return false;
          break;
        case MemberTypes.Property:
          if (this.properties[memberReference.memberIndex].Name != memberReference.name)
            return false;
          break;
      }
      return true;
    }


    /// <summary>
    /// Clears the watchlist
    /// </summary>
    public void ClearWatchList(bool updateBookmark = true)
    {
      // Clear the watch list
      foreach (var member in this.watchList)
      {
        member.isWatched = false;
      }
      this.watchList.Clear();

      // Optionally, let the bookmarks know
      if (updateBookmark)
        GameObjectBookmark.UpdateWatchList();
    }


    ///// <summary>
    ///// Verifies that this member reference is still valid
    ///// </summary>
    ///// <param name="memberReference"></param>
    ///// <returns></returns>
    //public bool AssertReference(ComponentInformation.MemberReference memberReference)
    //{
    //  // If this is not the GameObject this member reference is for
    //  //if (memberReference.gameObjectInfo != this)
    //  //{
    //  //  //throw new ArgumentException($"The member {memberReference.name} is not a member among the components of the GameObject {this.target.name}");
    //  //  return false;
    //  //}
    //
    //  // Noww assert all the others
    //  bool valid = AssertComponentIndex(memberReference) && AssertMemberIndex(memberReference);
    //  return valid;
    //}
    //
    ///// <summary>
    ///// If the component index doesn't match the current component index,
    ///// this means the component could have been shuffled or removed
    ///// </summary>
    ///// <param name="memberReference"></param>
    ///// <returns></returns>
    //public bool AssertComponentIndex(ComponentInformation.MemberReference memberReference)
    //{
    //  if ((memberReference.componentIndex > this.numberofComponents - 1) ||
    //      (memberReference.componentName != this.components[memberReference.componentIndex].name))
    //  {
    //    return false;
    //  }
    //  return true;
    //}

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
    // Fields
    //------------------------------------------------------------------------/  
    public GameObject target;
    public ComponentInformation[] components;
    public int fieldCount;
    public int propertyCount;
    public int numberofComponents;
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    public bool initialized { get; private set; }
    public ComponentInformation.MemberReference[] members { get; private set; }
    public ComponentInformation.MemberReference[] watchList { get; private set; }
    public int memberCount => fieldCount + propertyCount;
    public bool isValid => target != null && this.numberofComponents > 0;

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

      //// Count the number of fields and properties
      ////int fieldCount = 0;
      ////int propertyCount = 0;
      //for (int i = 0; i < components.Length; ++i)
      //{
      //  ComponentInformation component = this.components[i];
      //  this.fieldCount += component.fieldCount;
      //  this.propertyCount += component.propertyCount;
      //}
      //if (fieldCount != this.fieldCount)

      //this.UpdateReferences();

      // Cache current member references
      this.CacheReferences();
    }

    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/  
    public GameObjectInformation(GameObject target)
    {
      // Set target
      this.target = target;

      // Set components
      this.fieldCount = 0;
      this.propertyCount = 0;
      Component[] targetComponents = target.GetComponents<Component>();
      this.numberofComponents = targetComponents.Length;
      List<ComponentInformation> components = new List<ComponentInformation>();
      for (int i = 0; i < this.numberofComponents; ++i)
      {
        Component component = targetComponents[i];
        if (component == null)
        {
          throw new Exception($"The component at index {i} is null!");
        }

        ComponentInformation componentInfo = new ComponentInformation(component);
        this.fieldCount += componentInfo.fieldCount;
        this.propertyCount += componentInfo.propertyCount;
        components.Add(componentInfo);
      }
      this.components = components.ToArray();

      // Now cache member references
      this.CacheReferences();
    }

    //------------------------------------------------------------------------/
    // Methods: Watch
    //------------------------------------------------------------------------/  
    ///// <summary>
    ///// Adds a member to the watch list
    ///// </summary>
    ///// <param name="member"></param>
    ///// <param name="componentInfo"></param>
    ///// <param name="memberIndex"></param>
    //public void Watch(MemberInfo member, ComponentInformation componentInfo, int memberIndex)
    //{
    //  MemberReference memberReference = new MemberReference(member, componentInfo, this, memberIndex);
    //  this.favorites.Add(memberReference);
    //  GameObjectBookmark.UpdateFavoriteMembers();
    //}
    //
    ///// <summary>
    ///// Removes a member from the watch list
    ///// </summary>
    ///// <param name="member"></param>
    ///// <param name="componentInfo"></param>
    ///// <param name="memberIndex"></param>
    //public void RemoveWatch(MemberInfo member, ComponentInformation componentInfo, int memberIndex)
    //{
    //  this.favorites.RemoveAll(x => x.name == member.Name && x.componentName == componentInfo.name && x.memberIndex == memberIndex);
    //  GameObjectBookmark.UpdateFavoriteMembers();
    //}

    /// <summary>
    ///// Adds a member to the watch list
    ///// </summary>
    ///// <param name="member"></param>
    ///// <param name="componentInfo"></param>
    ///// <param name="memberIndex"></param>
    //public void Watch(ComponentInformation.MemberReference memberReference)
    //{
    //  memberReference.isFavorite = true;
    //  if (this.AssertReference(memberReference))
    //    this.favorites.Add(memberReference);
    //  GameObjectBookmark.UpdateFavoriteMembers();
    //}
    //
    ///// <summary>
    ///// Removes a member from the watch list
    ///// </summary>
    ///// <param name="memberReference"></param>
    //public void RemoveWatch(ComponentInformation.MemberReference memberReference)
    //{
    //  memberReference.isFavorite = false;
    //  if (this.AssertReference(memberReference))
    //    this.favorites.RemoveAll(x => x.name == memberReference.name && x.memberIndex == memberReference.memberIndex);
    //  GameObjectBookmark.UpdateFavoriteMembers();
    //}

    /// <summary>
    /// Clears the watchlist for every component
    /// </summary>
    public void ClearWatchList()
    {
      foreach(var component in this.components)
      {
        component.ClearWatchList(false);
      }

      GameObjectBookmark.UpdateWatchList();
    }

    //------------------------------------------------------------------------/
    // Methods: Update
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Updates the values of all the favorite members for this GameObject
    /// </summary>
    public void UpdateWatchValues()
    {
      foreach(var component in this.components)
      {
        component.UpdateWatchValues();
      }
    }

    /// <summary>
    /// Caches all member references from among their components
    /// </summary>
    public void CacheReferences()
    {
      List<ComponentInformation.MemberReference> memberReferences = new List<ComponentInformation.MemberReference>();
      List<ComponentInformation.MemberReference> watchList = new List<ComponentInformation.MemberReference>();
      foreach (var component in this.components)
      {
        memberReferences.AddRange(component.memberReferences);
        watchList.AddRange(component.watchList);
      }
      this.members = memberReferences.ToArray();
      this.watchList = watchList.ToArray();

      this.initialized = true;
    }

    ///// <summary>
    ///// Initializes the current member references in the object, also setting current favorites
    ///// </summary>
    //public void UpdateReferences()
    //{
    //  this.favorites = new List<ComponentInformation.MemberReference>();
    //  foreach (var member in this.members)
    //  {
    //    // If the component doesn't match...
    //    //if (!this.AssertComponentIndex(member))
    //    //{
    //    //  Trace.Script($"Component index doesn't match!");
    //    //  continue;
    //    //}
    //    //
    //    //// Assert the member index
    //    //if (!this.AssertMemberIndex(member))
    //    //{
    //    //  Trace.Script($"Member index doesn't match!");
    //    //  continue;
    //    //}
    //
    //    // Set the component
    //    //ComponentInformation component = this.components[member.componentIndex];
    //    //member.Set(component, this);
    //    //
    //    //// If the member is a favorite, add it to the favorite list
    //    //if (member.isFavorite)
    //    //  this.favorites.Add(member);
    //  }
    //
    //  this.initialized = true;
    //}

    //------------------------------------------------------------------------/
    // Methods: References
    //------------------------------------------------------------------------/  
    ///// <summary>
    ///// Saves all member references for this GameObject
    ///// </summary>
    ///// <returns></returns>
    //private ComponentInformation.MemberReference[] CreateAllMemberReferences()
    //{
    //  // Make a reference for all members
    //  List<ComponentInformation.MemberReference> memberReferences = new List<ComponentInformation.MemberReference>();
    //  foreach (var component in this.components)
    //  {
    //    for (int f = 0; f < component.fieldCount; ++f)
    //    {
    //      ComponentInformation.MemberReference memberReference = new ComponentInformation.MemberReference(component.fields[f], component, this, f);
    //      memberReferences.Add(memberReference);
    //    }
    //
    //    for (int p = 0; p < component.propertyCount; ++p)
    //    {
    //      ComponentInformation.MemberReference memberReference = new ComponentInformation.MemberReference(component.properties[p], component, this, p);
    //      memberReferences.Add(memberReference);
    //    }
    //
    //  }
    //
    //  // Also rebuild favorites!
    //  this.favorites = new List<ComponentInformation.MemberReference>();
    //
    //  return memberReferences.ToArray();
    //}

    ///// <summary>
    ///// Verifies that this member reference is still valid
    ///// </summary>
    ///// <param name="memberReference"></param>
    ///// <returns></returns>
    //public bool AssertReference(ComponentInformation.MemberReference memberReference)
    //{
    //  // If this is not the GameObject this member reference is for
    //  if (memberReference.gameObjectInfo != this)
    //  {
    //    //throw new ArgumentException($"The member {memberReference.name} is not a member among the components of the GameObject {this.target.name}");
    //    return false;
    //  }
    //
    //  // Noww assert all the others
    //  bool valid = AssertComponentIndex(memberReference) && AssertMemberIndex(memberReference);
    //  return valid;
    //}
    //
    ///// <summary>
    ///// If the component index doesn't match the current component index,
    ///// this means the component could have been shuffled or removed
    ///// </summary>
    ///// <param name="memberReference"></param>
    ///// <returns></returns>
    //public bool AssertComponentIndex(ComponentInformation.MemberReference memberReference)
    //{
    //  if ((memberReference.componentIndex > this.numberofComponents - 1) ||
    //      (memberReference.componentName != this.components[memberReference.componentIndex].name))
    //  {
    //    return false;
    //  }
    //  return true;
    //}

    ///// <summary>
    ///// If the member at the index doesn't match the member reference index,
    ///// this means the member could have been removed or rearranged
    ///// </summary>
    ///// <param name="memberReference"></param>
    ///// <returns></returns>
    //public bool AssertMemberIndex(ComponentInformation.MemberReference memberReference)
    //{
    //  ComponentInformation componentInformation = this.components[memberReference.componentIndex];
    //  switch (memberReference.type)
    //  {
    //    case MemberTypes.Field:
    //      if (componentInformation.fields[memberReference.memberIndex].Name != memberReference.name)
    //        return false;
    //      break;
    //    case MemberTypes.Property:
    //      if (componentInformation.properties[memberReference.memberIndex].Name != memberReference.name)
    //        return false;
    //      break;
    //  }
    //  return true;
    //}


  }

}