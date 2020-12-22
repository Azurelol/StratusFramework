using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// A generic behaviour that has extensions that can be added or removed to it
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusExtensibleBehaviour : StratusBehaviour, IStratusExtensible
	{
		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		[HideInInspector]
		public List<MonoBehaviour> extensionBehaviours = new List<MonoBehaviour>();
		[SerializeField, HideInInspector]
		public int selectedExtensionTypeIndex;

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public IStratusExtensionBehaviour[] extensions => GetExtensionBehaviours(this.extensionBehaviours);
		public bool hasExtensions => this.extensionBehaviours.Count > 0;
		private static Dictionary<IStratusExtensionBehaviour, StratusExtensibleBehaviour> extensionOwnershipMap { get; set; } = new Dictionary<IStratusExtensionBehaviour, StratusExtensibleBehaviour>();
		public static HideFlags extensionFlags { get; } = HideFlags.HideInInspector;

		//--------------------------------------------------------------------------------------------/
		// Virtual
		//--------------------------------------------------------------------------------------------/
		protected abstract void OnAwake();
		protected abstract void OnStart();

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		private void Awake()
		{
			this.OnAwake();

			foreach (IStratusExtensionBehaviour extension in this.extensions)
			{
				extension.OnExtensibleAwake(this);
			}
		}

		private void Start()
		{
			this.OnStart();

			foreach (IStratusExtensionBehaviour extension in this.extensions)
			{
				extension.OnExtensibleStart();
			}
		}

		private void OnValidate()
		{
			foreach(var extension in extensions)
			{
				HideExtensions();
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Adds the extension to this behaviour
		/// </summary>
		/// <param name="extension"></param>
		public void Add(IStratusExtensionBehaviour extension)
		{
			MonoBehaviour behaviour = (MonoBehaviour)extension;
			behaviour.hideFlags = StratusExtensibleBehaviour.extensionFlags;
			this.extensionBehaviours.Add(behaviour);
		}

		/// <summary>
		/// Removes the extension from this behaviour
		/// </summary>
		/// <param name="extension"></param>
		public void Remove(int index)
		{
			MonoBehaviour extension = this.extensionBehaviours[index];
			this.extensionBehaviours.RemoveAt(index);
		}

		/// <summary>
		/// Retrieves the extension of the given type, if its present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetExtension<T>() where T : MonoBehaviour
		{
			Type type = typeof(T);
			foreach (MonoBehaviour extension in this.extensionBehaviours)
			{
				if (extension.GetType() == type)
				{
					return (T)extension;
				}
			}
			return default(T);
		}

		/// <summary>
		/// Retrieves the extension of the given type, if its present
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool HasExtension(IStratusExtensionBehaviour behaviour)
		{
			return this.extensionBehaviours.Contains((MonoBehaviour)behaviour);
		}

		/// Retrieves the extensible that the extension is for
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static T GetExtensible<T>(IStratusExtensionBehaviour extension) where T : StratusExtensibleBehaviour
		{
			return extensionOwnershipMap[extension] as T;
		}

		public static IStratusExtensionBehaviour[] GetExtensionBehaviours(MonoBehaviour[] monoBehaviours)
		{
			return (from MonoBehaviour e in monoBehaviours select (e as IStratusExtensionBehaviour)).ToArray();
		}

		public static IStratusExtensionBehaviour[] GetExtensionBehaviours(List<MonoBehaviour> monoBehaviours)
		{
			return (from MonoBehaviour e in monoBehaviours select (e as IStratusExtensionBehaviour)).ToArray();
		}

		[ContextMenu("Show Extensions")]
		private void ShowExtensions()
		{
			foreach (MonoBehaviour extension in this.extensionBehaviours)
			{
				extension.hideFlags = HideFlags.None;
			}
		}

		[ContextMenu("Hide Extensions")]
		private void HideExtensions()
		{
			foreach (MonoBehaviour extension in this.extensionBehaviours)
			{
				extension.hideFlags = HideFlags.HideInInspector;
			}
		}


		/// <summary>


	}

	/// <summary>
	/// Tells the Editor which extensible class this extension is for
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class StratusCustomExtensionAttribute : Attribute
	{
		public StratusCustomExtensionAttribute(params Type[] supportedExtensibles)
		{
			this.supportedExtensibles = supportedExtensibles;
		}

		public Type[] supportedExtensibles { get; set; }

	}

	/// <summary>
	/// Allows additional configuration of an extensible behaviour
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class StratusExtensibleBehaviourAttribute : Attribute
	{
		public StratusExtensibleBehaviourAttribute(params Type[] extensionTypes)
		{
			this.extensionTypes = extensionTypes;
		}

		public Type[] extensionTypes { get; set; }
	}


	/// <summary>
	/// Interface type used to validate all extensible behaviours
	/// </summary>
	public interface IStratusExtensible { }

	/// <summary>
	/// Interface type a behaviours that is an extension of another
	/// </summary>
	public interface IStratusExtensionBehaviour
	{
		void OnExtensibleAwake(StratusExtensibleBehaviour extensible);
		void OnExtensibleStart();
	}

	/// <summary>
	/// Interface type used to validate all extensible behaviours
	/// </summary>
	public interface IStratusExtensionBehaviour<T> : IStratusExtensionBehaviour where T : StratusExtensibleBehaviour
	{
		/// <summary>
		/// The extensible component this extension is for
		/// </summary>
		T extensible { set; get; }
	}

}