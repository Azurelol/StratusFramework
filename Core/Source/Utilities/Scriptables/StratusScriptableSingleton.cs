using System;
using System.Collections.Generic;
using Stratus.Utilities;
using UnityEngine;
using Stratus.OdinSerializer;

namespace Stratus
{
	/// <summary>
	/// A required attribute that specifies the wanted folder path and name for a singleton asset
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class StratusScriptableSingletonAttribute : Attribute
	{
		public string path { get; set; }
		public string name { get; set; }
		public bool hidden { get; set; }

		/// <param name="relativePath">The relative path to the folder where you want the asset to be stored</param>
		/// <param name="name">The name of the asset</param>
		public StratusScriptableSingletonAttribute(string relativePath, string name)
		{
			this.path = relativePath;
			this.name = name;
		}

		public class MissingException : Exception
		{
			public MissingException(string className) : base("The class declaration for " + className + " is missing the [SingletonAsset] attribute, which provides the path information needed in order to construct the asset.")
			{
				// Fill later?
				this.HelpLink = "http://msdn.microsoft.com";
				this.Source = "Exception_Class_Samples";
			}
		}
	}

	/// <summary>
	/// An asset of which there is only a single instance of in the project. Mainly used
	/// for global configuration data.
	/// </summary>
	public abstract class StratusScriptableSingleton : StratusScriptable
	{
		/// <summary>
		/// A cached dictionary of all present/loaded singletons!
		/// </summary>
		public static Dictionary<Type, ScriptableObject> singletons { get; private set; } = new Dictionary<Type, ScriptableObject>();
		/// <summary>
		/// Whether this scriptable has been initialized
		/// </summary>
		public bool initialized { get; private set; } = false;

		internal void Initialize(bool force = false)
		{
			if (initialized && !force)
			{
				this.LogWarning("Already initialized");
				return;
			}
			OnInitialize();
			initialized = true;
			this.Log("Initialized");
		}

		protected abstract void OnInitialize();
	}

	/// <summary>
	/// An asset of which there is only a single instance of in the project. Mainly used
	/// for global configuration data.
	/// </summary>
	public abstract class StratusScriptableSingleton<T> : 
		StratusScriptableSingleton 
		where T : StratusScriptableSingleton
	{
		/// <summary>
		/// The singular instance to the asset, after it has been loaded from memory
		/// </summary>
		private static T _instance { get; set; }

		/// <summary>
		/// Whether this singleton has been instantiated
		/// </summary>
		public static bool instantiated => _instance != null;

		/// <summary>
		/// Access the data of this object.
		/// </summary>
		public static T instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = LoadOrCreate();
				}

				return _instance;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Used for editing the properties of the asset in a generic way
		/// </summary>
		public static UnityEditor.SerializedObject serializedObject
		{
			get
			{
				if (_serializedObject == null)
				{
					LoadOrCreate();
				}

				return _serializedObject;
			}
		}

		// Fields
		private static UnityEditor.SerializedObject _serializedObject;
#endif

		/// <summary>
		/// Creates an instance of the asset
		/// </summary>
		/// <returns></returns>
		protected static T LoadOrCreate()
		{
			Type type = typeof(T);
			StratusScriptableSingletonAttribute settings = type.GetAttribute<StratusScriptableSingletonAttribute>();
			if (settings == null)
			{
				throw new StratusScriptableSingletonAttribute.MissingException(type.Name);
			}

			string path = settings.GetProperty<string>(nameof(StratusScriptableSingletonAttribute.path));
			string name = settings.GetProperty<string>(nameof(StratusScriptableSingletonAttribute.name));
			bool hidden = settings.GetProperty<bool>(nameof(StratusScriptableSingletonAttribute.hidden));

			string folderPath = StratusIO.GetFolderPath(path);
			if (folderPath == null)
			{
				throw new NullReferenceException("The given folder path '" + path + "' to be used for the asset '" + name + "' could not be found!");
			}

			string fullPath = folderPath + "/" + name + ".asset";

			// Now create the proper instance
			_instance = StratusAssets.LoadOrCreateScriptableObject<T>(fullPath);
			_instance.Initialize(true);

#if UNITY_EDITOR
			// Also create the serialized object      
			_serializedObject = new UnityEditor.SerializedObject(_instance);
#endif

			if (hidden)
			{
				_instance.hideFlags = HideFlags.HideInHierarchy;
			}
			
			return _instance;
		}

		/// <summary>
		/// Saves this asset
		/// </summary>
		public static void Save()
		{
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(_instance);
			UnityEditor.AssetDatabase.SaveAssets();
#endif
		}

		public static void Inspect()
		{
#if UNITY_EDITOR
			UnityEditor.Selection.activeObject = instance;
#endif
		}

	}
}
