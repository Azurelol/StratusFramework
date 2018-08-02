using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Serialization;
using System.Linq.Expressions;
using System;

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
      /// THe index to this member for either the fields or properties of the component
      /// </summary>
      public int memberIndex;
      /// <summary>
      /// Whether this memebr reference is favorited
      /// </summary>
      public bool isWatched = false;
      /// <summary>
      /// Information regarding the component this member belongs to
      /// </summary>
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
        this.gameObjectName = componentInfo.gameObject.name;
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
    /// <summary>
    /// The component this information is about
    /// </summary>
    public Component component;
    /// <summary>
    /// Whether the members of this component shoudl be sorted alphabetically
    /// </summary>
    public bool alphabeticalSorted;
    /// <summary>
    /// A list of all members fields and properties of this component
    /// </summary>
    public List<MemberReference> memberReferences;

    [NonSerialized]
    public Type type;
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
    public FieldInfo[] fields { get; private set; }
    public Dictionary<string, FieldInfo> fieldsByName { get; private set; }
    public int fieldCount => fields.Length;
    public PropertyInfo[] properties { get; private set; }
    public Dictionary<string, PropertyInfo> propertiesByName { get; private set; }
    public int propertyCount => properties.Length;
    public bool hasFields => fieldCount > 0;
    public bool hasProperties => propertyCount > 0;    
    public Dictionary<string, MemberReference> membersByName { get; private set; }
    public List<MemberReference> watchList { get; private set; } = new List<MemberReference>();
    public bool hasWatchList => watchList.NotEmpty();
    public bool valid { get; private set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
      // If there's no component, stuff is gone!
      if (this.component == null)
      {
        this.valid = false;
        return;
      }

      // Initialize step
      this.InitializeComponentInformation();
      this.InitializeMemberReferences();

      // This information is now valid
      this.valid = true;
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
      this.InitializeComponentInformation();
      this.memberReferences = this.CreateAllMemberReferences();
      this.OnMemberReferencesSet();

      // This information is now valid
      this.valid = true;
    }

    /// <summary>
    /// Runtime: Record all type information about the members of the component
    /// </summary>
    private void InitializeComponentInformation()
    {
      // Type
      this.type = this.component.GetType();
      
      // Fields
      this.fields = this.type.GetFields(bindingFlags);
      if (this.alphabeticalSorted)
        Array.Sort(this.fields, delegate (FieldInfo a, FieldInfo b) { return a.Name.CompareTo(b.Name); });
      //this.fieldsByName = new Dictionary<string, FieldInfo>();
      //this.fieldsByName.AddRange(this.fields, (FieldInfo fi) => fi.Name);
      this.fieldValues = new object[this.fields.Length];
      this.fieldValueStrings = new string[this.fields.Length];
      
      // Properties
      this.properties = this.type.GetProperties(bindingFlags);
      if (this.alphabeticalSorted)
        Array.Sort(this.properties, delegate (PropertyInfo a, PropertyInfo b) { return a.Name.CompareTo(b.Name); });
      //this.propertiesByName = new Dictionary<string, PropertyInfo>();
      //this.propertiesByName.AddRange(this.properties, (PropertyInfo pi) => pi.Name);
      this.propertyValues = new object[this.properties.Length];
      this.propertyValueStrings = new string[this.properties.Length];

      // Member Reference Dictionary

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
      {
        this.watchList.Add(memberReference);
        GameObjectBookmark.UpdateWatchList(true);
      }
    }

    /// <summary>
    /// Removes a member from the watch list
    /// </summary>
    /// <param name="memberReference"></param>
    public void RemoveWatch(ComponentInformation.MemberReference memberReference)
    {
      memberReference.isWatched = false;
      if (this.AssertMemberIndex(memberReference))
      {
        this.watchList.RemoveAll(x => x.name == memberReference.name && x.memberIndex == memberReference.memberIndex);
        GameObjectBookmark.UpdateWatchList(true);
      }
    }

    /// <summary>
    /// Saves all member references for this GameObject
    /// </summary>
    /// <returns></returns>
    private List<MemberReference> CreateAllMemberReferences()
    {
      // Make a reference for all members
      List<MemberReference> memberReferences = new List<MemberReference>();
      for (int f = 0; f < this.fields.Length; ++f)
      {
        MemberReference memberReference = new MemberReference(this.fields[f], this, f);
        memberReferences.Add(memberReference);
      }

      for (int p = 0; p < this.properties.Length; ++p)
      {
        MemberReference memberReference = new MemberReference(this.properties[p], this, p);
        memberReferences.Add(memberReference);
      }
      this.watchList = new List<MemberReference>();      
      return memberReferences;
    }

    /// <summary>
    /// Initializes all member references (done after deserialization)
    /// </summary>
    private void InitializeMemberReferences()
    {
      // Set all member references, also record initial watchlist
      this.watchList = new List<MemberReference>();

      // Validate
      Func<MemberReference, bool> validate = (MemberReference member) =>
      {
        return AssertMemberIndex(member);
      };

      // Iteration
      System.Action<MemberReference> iterate = (MemberReference member) => 
      {
        member.Initialize(this);
        if (member.isWatched)
          this.watchList.Add(member);
      };

      this.memberReferences.IterateAndRemoveInvalid(iterate, validate);
      this.OnMemberReferencesSet();
    }

    private void OnMemberReferencesSet()
    {
      this.membersByName = new Dictionary<string, MemberReference>();
      this.membersByName.AddRangeUnique(this.memberReferences, (MemberReference member) => member.name);
    }

    /// <summary>
    /// Checks for any new members added onto the component. Returns true if any members
    /// were added
    /// </summary>
    public bool Refresh()
    {
      bool changed = false;
      for (int i = 0; i < this.fields.Length; ++i)
      {
        FieldInfo field = this.fields[i];
        if (field == null)
          return true;

        if (!this.membersByName.ContainsKey(field.Name))
        {
          changed |= true;
          Debug.Log($"New field detected! {field.Name}");
          MemberReference memberReference = new MemberReference(field, this, i);
          this.memberReferences.Add(memberReference);
          this.membersByName.Add(field.Name, memberReference);
        }
      }

      for (int i = 0; i < this.properties.Length; ++i)
      {
        PropertyInfo property = this.properties[i];
        if (property == null)
          return true;

        if (!this.membersByName.ContainsKey(property.Name))
        {
          changed |= true;
          Debug.Log($"New property detected! {property.Name}");
          MemberReference memberReference = new MemberReference(property, this, i);
          this.memberReferences.Add(memberReference);
          this.membersByName.Add(property.Name, memberReference);
        }
      }
      return changed;
    }

    /// <summary>
    /// If the member at the index doesn't match the member reference index,
    /// this means the member could have been removed or rearranged
    /// </summary>
    /// <param name="memberReference"></param>
    /// <returns></returns>
    private bool AssertMemberIndex(MemberReference memberReference)
    {
      int index = memberReference.memberIndex;
      switch (memberReference.type)
      {
        case MemberTypes.Field:
          if (!this.fields.HasIndex(index))
            return false;
          if (this.fields[index].Name != memberReference.name)
            return UpdateMemberIndex(memberReference);          
          break;

        case MemberTypes.Property:
          if (!this.properties.HasIndex(index))
            return false;
          if (this.properties[index].Name != memberReference.name)
            return UpdateMemberIndex(memberReference);
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

    /// <summary>
    /// Attempts to update the member index for this member, if possible
    /// </summary>
    /// <param name="memberReference"></param>
    /// <returns></returns>
    private bool UpdateMemberIndex(MemberReference memberReference)
    {
      if (memberReference.type == MemberTypes.Field)
      {
        for(int i = 0; i < this.fields.Length; ++i)
        {
          // Found a field with the same name
          if (this.fields[i].Name == memberReference.name)
          {
            memberReference.memberIndex = i;
            return true;
          }
        }
      }
      else if (memberReference.type == MemberTypes.Property)
      {
        for (int i = 0; i < this.properties.Length; ++i)
        {
          // Found a property with the same name
          if (this.properties[i].Name == memberReference.name)
          {
            memberReference.memberIndex = i;
            return true;
          }
        }
      }

      // Couldn't update this member reference
      return false;
    }

    /// <summary>
    /// Retrieves the value of the selected field
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private object GetValue(FieldInfo field) => field.GetValue(component);

    /// <summary>
    /// Retrieves the value of the selected property
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private object GetValue(PropertyInfo property) => property.GetValue(component);
  }


}