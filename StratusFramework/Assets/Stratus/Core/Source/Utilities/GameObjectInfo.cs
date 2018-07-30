﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Serialization;
using System.Linq.Expressions;
using UnityEngine.Events;

namespace Stratus
{

  /// <summary>
  /// Information about a gameobject and all its components
  /// </summary>
  [Serializable]
  public class GameObjectInformation : ISerializationCallbackReceiver
  {
    //------------------------------------------------------------------------/
    // Declaration
    //------------------------------------------------------------------------/  
    public enum Change
    {
      Components,
      WatchList,
      ComponentsAndWatchList,
      None
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    public GameObject target;
    public ComponentInformation[] components;
    public int fieldCount;
    public int propertyCount;
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    public bool initialized { get; private set; }
    public int numberofComponents => components.Length;
    public ComponentInformation.MemberReference[] members { get; private set; }
    public ComponentInformation.MemberReference[] watchList { get; private set; }
    public int memberCount => fieldCount + propertyCount;
    public bool isValid => target != null && this.numberofComponents > 0;
    public static UnityAction<GameObjectInformation, Change> onChanged { get; set; } = new UnityAction<GameObjectInformation, Change>( (GameObjectInformation information, Change change) => { });

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

      // Verify that components are still valid
      //this.ValidateComponents();

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
      List<ComponentInformation> components = new List<ComponentInformation>();
      for (int i = 0; i < targetComponents.Length; ++i)
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
    /// <summary>
    /// Clears the watchlist for every component
    /// </summary>
    public void ClearWatchList()
    {
      foreach (var component in this.components)
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
      foreach (var component in this.components)
      {
        component.UpdateWatchValues();
      }
    }

    /// <summary>
    /// Updates the values of all the members for this GameObject
    /// </summary>
    public void UpdateValues()
    {
      foreach (var component in this.components)
      {
        component.UpdateValues();
      }
    }

    /// <summary>
    /// Caches all member references from among their components
    /// </summary>
    public void CacheReferences()
    {
      // Now cache!
      List<ComponentInformation.MemberReference> memberReferences = new List<ComponentInformation.MemberReference>();
      foreach (var component in this.components)
      {
        memberReferences.AddRange(component.memberReferences);
      }
      this.members = memberReferences.ToArray();

      this.CacheWatchList();
      this.initialized = true;
    }

    /// <summary>
    /// Caches all member references under a watchlist for each component
    /// </summary>
    public void CacheWatchList()
    {
      List<ComponentInformation.MemberReference> watchList = new List<ComponentInformation.MemberReference>();
      foreach (var component in this.components)
      {
        if (component.valid)
          watchList.AddRange(component.watchList);
      }
      this.watchList = watchList.ToArray();
    }

    /// <summary>
    /// Refreshes the information for the target GameObject. If any components wwere added or removed,
    /// it will update the cache
    /// </summary>
    public void Refresh()
    {
      Change change = ValidateComponents();
      switch (change)
      {
        case Change.Components:          
          this.CacheReferences();
          onChanged(this, change);
          break;
        case Change.ComponentsAndWatchList:          
          this.CacheReferences();
          onChanged(this, change);
          break;
        case Change.None:
          break;
      }
    }

    /// <summary>
    /// Verifies that the component references for this GameObject are still valid,
    /// returning false if any components were removed
    /// </summary>
    private Change ValidateComponents()
    {
      bool watchlistChanged = false;
      bool changed = false;

      // Check if any components are null
      foreach (var component in this.components)
      {
        if (component.component == null)
        {
          changed = true;
          if (component.hasWatchList)
          {
            watchlistChanged = true;
          }
        }
      }
      
      // Check for other component changes
      Component[] targetComponents = target.GetComponents<Component>();
      changed |= this.numberofComponents != targetComponents.Length;

      // If there's noticeable changes, let's add any components that were not there before
      if (changed)
      {
        List<ComponentInformation> currentComponents = new List<ComponentInformation>();
        Func<ComponentInformation, bool> validate = (ComponentInformation component) => { return component.component != null; };
        currentComponents.AddRangeFiltered(this.components, validate);
          
        // If there's no information for this component, let's add it
        foreach (var component in targetComponents)
        {
          ComponentInformation ci = currentComponents.Find(x => x.component == component);

          if (ci == null)
          {
            ci = new ComponentInformation(component);
            currentComponents.Add(ci);
          }
        }

        // Now update the list of components
        this.components = currentComponents.ToArray();
      }      

      if (changed)
      {
        if (watchlistChanged)
        {
          return Change.ComponentsAndWatchList;
        }

        return Change.Components;
      }

      // If any components were removed, or added, note the change
      return Change.None;
    }


  }

}