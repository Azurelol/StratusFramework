using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Allows an easy interface for managing multiple instances of a single class,
	/// giving global access to them
	/// </summary>
	/// <typeparam name="T"></typeparam>
	//[ExecuteAlways]
	public abstract class StratusMultitonBehaviour<T> : StratusBehaviour 
		where T : MonoBehaviour
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// All currently active instances, indexed by their labels
		/// </summary>
		public static Dictionary<string, T> instancesByLabel { get; private set; } = new Dictionary<string, T>();
		/// <summary>
		/// Returns the first listed multiton
		/// </summary>
		public static T first => instances.FirstOrNull() as T;
		/// <summary>
		/// All currently active instances
		/// </summary>
		public static List<T> instances { get; private set; } = new List<T>();
		/// <summary>
		/// How many instances are active
		/// </summary>
		public static int instanceCount => instancesByLabel.Count;
		/// <summary>
		/// Whether there are available instances
		/// </summary>
		public static bool hasInstances => instances.Count > 0;
		/// <summary>
		/// Returns the underlying class for this multiton
		/// </summary>
		public T instance { get; private set; }
		/// <summary>
		/// The identifier for this instance within all multitons
		/// </summary>
		public virtual string label => _label != null ? _label : gameObject.name;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnMultitonStart();
		protected abstract void OnMultitonAwake();
		protected abstract void OnMultitonEnable();
		protected abstract void OnMultitonDisable();
		protected abstract void OnReset();

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Tooltip("The identifier for this instance")]
		[SerializeField]
		protected string _label;

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Invoked whenever a multiton instance has been enabled (MonoBehaviour Enable)
		/// </summary>
		public static event Action<T> onEnable;
		/// <summary>
		/// Invoked whenever a multiton instance has been disabled (MonoBehaviour Disable)
		/// </summary>
		public static event Action<T> onDisable;
		/// <summary>
		/// Invoked whenever a multiton instance has been updated 
		/// (This is invoked manually, left to the discretion of the subclass, through <see cref="NotifyUpdated"/>)
		/// </summary>
		public static event Action<T> onUpdate;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			instance = this as T;
			if (Application.isPlaying)
			{
				OnMultitonAwake(); 
			}
		}

		private void Start()
		{
			if (Application.isPlaying)
			{
				OnMultitonStart();				
			}
		}

		private void OnEnable()
		{
			if (label.IsValid())
			{
				instancesByLabel.Add(label, instance);
				instances.Add(instance);
				onEnable?.Invoke(instance);
				this.Log($"Added {label}");
			}
			else
			{
				this.LogError($"Failed to add multiton for GameObject {gameObject.name}");
			}

			if (!Application.isPlaying)
			{
				OnMultitonEnable();
			}

		}

		private void OnDisable()
		{
			if (label.IsValid())
			{
				this.Log($"Removed {label}");
				instancesByLabel.Remove(label);
				instances.Remove(instance);
				onDisable?.Invoke(instance);
			}

			if (Application.isPlaying)
			{
				OnMultitonDisable();
			}

		}

		private void Reset()
		{
			OnReset();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Notifies that this instance has been updated
		/// </summary>
		protected void NotifyUpdated()
		{
			onUpdate?.Invoke(instance);
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Returns a navigator, which allows easy navigation between instances of this class
		/// </summary>
		/// <returns></returns>
		public static StratusArrayNavigator<T> GetNavigator(bool loop = true)
		{
			var navigator = new StratusArrayNavigator<T>(instances, loop);
			return navigator;
		}

	}

}