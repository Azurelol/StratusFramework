using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	public abstract class StratusSingleton<T> : IStratusLogger
		where T : class
	{
		/// <summary>
		/// Returns a reference to the singular instance of this class. If not available currently, 
		/// it will instantiate it when accessed.
		/// </summary>
		public static T instance
		{
			get
			{
				// If not found, instantiate
				if (_instance == null)
				{
					if (shouldInstantiate == false)
					{						
						StratusDebug.LogWarning($"Will not automatically instantiate singleton of type {typeof(T).Name}...");
						return null;
					}

					if (isPlayerOnly && StratusEditorBridge.isEditMode)
					{
						StratusDebug.LogWarning($"Will not instantiate singleton of type {typeof(T).Name} outside of playmode");
						return null;
					}

					_instance = (T)Activator.CreateInstance(typeof(T)); //  new T();

				}

				return _instance;
			}
		}
		/// <summary>
		/// Whether this singleton has been instantiated
		/// </summary>
		public static bool instantiated => instance != null;

		/// <summary>
		/// Whether the class should be instantiated. By default, true.
		/// </summary>
		private static bool shouldInstantiate => attribute?.GetProperty<bool>(nameof(StratusSingletonAttribute.instantiate)) ?? true;
		/// <summary>
		/// Whether the class should be instantiated while in editor mode
		/// </summary>
		private static bool isPlayerOnly => attribute?.GetProperty<bool>(nameof(StratusSingletonAttribute.isPlayerOnly)) ?? true;

		/// <summary>
		/// Returns the current specific attributes for the derived singleton class, if any are present
		/// </summary>
		private static StratusSingletonAttribute attribute => typeof(T).GetAttribute<StratusSingletonAttribute>();

		/// <summary>
		/// Whether this singleton has been initialized
		/// </summary>
		public bool initialized { get; private set; }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The singular instance of the class
		/// </summary>
		protected static T _instance;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		protected StratusSingleton()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (initialized)
			{
				return;
			}
			OnInitialize();
			initialized = true;
		}

		protected abstract void OnInitialize();
	}

}