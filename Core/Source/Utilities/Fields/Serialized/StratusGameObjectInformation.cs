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
	public class StratusGameObjectInformation : ISerializationCallbackReceiver
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
		public StratusComponentInformation[] components;
		public int fieldCount;
		public int propertyCount;
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/  
		public bool initialized { get; private set; }
		public int numberofComponents => components.Length;
		public StratusComponentInformation.MemberReference[] members { get; private set; }
		public StratusComponentInformation.MemberReference[] watchList { get; private set; }
		public int memberCount => fieldCount + propertyCount;
		public bool isValid => target != null && this.numberofComponents > 0;
		public static UnityAction<StratusGameObjectInformation, Change> onChanged { get; set; } = new UnityAction<StratusGameObjectInformation, Change>((StratusGameObjectInformation information, Change change) => { });

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
		public StratusGameObjectInformation(GameObject target)
		{
			// Set target
			this.target = target;

			// Set components
			this.fieldCount = 0;
			this.propertyCount = 0;
			Component[] targetComponents = target.GetComponents<Component>();
			List<StratusComponentInformation> components = new List<StratusComponentInformation>();
			for (int i = 0; i < targetComponents.Length; ++i)
			{
				Component component = targetComponents[i];
				if (component == null)
				{
					throw new Exception($"The component at index {i} is null!");
				}

				StratusComponentInformation componentInfo = new StratusComponentInformation(component);
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

			StratusGameObjectBookmark.UpdateWatchList();
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
			List<StratusComponentInformation.MemberReference> memberReferences = new List<StratusComponentInformation.MemberReference>();
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
			List<StratusComponentInformation.MemberReference> watchList = new List<StratusComponentInformation.MemberReference>();
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
				else
				{
					if (component.valid)
						changed |= component.Refresh();
				}
			}

			// Check for other component changes
			Component[] targetComponents = target.GetComponents<Component>();
			changed |= this.numberofComponents != targetComponents.Length;

			// If there's noticeable changes, let's add any components that were not there before
			if (changed)
			{
				List<StratusComponentInformation> currentComponents = new List<StratusComponentInformation>();
				currentComponents.AddRangeWhere((StratusComponentInformation component) => { return component.component != null; }, this.components);

				// If there's no information for this component, let's add it
				foreach (var component in targetComponents)
				{
					StratusComponentInformation ci = currentComponents.Find(x => x.component == component);

					if (ci == null)
					{
						ci = new StratusComponentInformation(component);
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