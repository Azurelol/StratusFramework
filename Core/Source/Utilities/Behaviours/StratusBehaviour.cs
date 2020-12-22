using System.Collections;
using Stratus.OdinSerializer;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Stratus
{
	public interface IStratusBehaviour : IStratusLogger
	{
		/// <summary>
		/// Runs a coroutine on this behaviour
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns></returns>
		Coroutine Invoke(IEnumerator enumerator);

		/// <summary>
		/// Runs a coroutine on this behaviour that will invoke the given action after the set amount of time
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns></returns>
		Coroutine Invoke(System.Action action, float delay);

		/// <summary>
		/// A cached version of get component, building the cache as you invoke
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T GetComponentCached<T>() where T : Component;
	}

	/// <summary>
	/// Base class for MonoBehaviours that use Stratus's custom editors for components,
	/// and handles custom serialization (through Sirenix's Odin Serializer)
	/// </summary>
	public abstract class StratusBehaviour : SerializedMonoBehaviour, IStratusBehaviour
	{
		//--------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------/
		private Dictionary<Type, Component> componentCache;

		//--------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------/
		/// <summary>
		/// Prints the given message to the console
		/// </summary>
		/// <param name="value"></param>
		public void Log(object value) => StratusDebug.Log(value, this, 2);

		/// <summary>
		/// Prints the given warning message to the console
		/// </summary>
		/// <param name="value"></param>
		public void LogWarning(object value) => StratusDebug.LogWarning(value, this, 2);

		/// <summary>
		/// Prints the given error message to the console
		/// </summary>
		/// <param name="value"></param>
		public void LogError(object value) => StratusDebug.LogError(value, this, 2);

		/// <summary>
		/// Runs a coroutine on this behaviour
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns></returns>
		public Coroutine Invoke(IEnumerator enumerator)
		{
			return this.StartCoroutine(enumerator);
		}

		/// <summary>
		/// Invokes an action on the next frame
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public Coroutine InvokeNextFrame(Action action)
		{
			IEnumerator nextFrame()
			{
				yield return new WaitForEndOfFrame();
				action();
			}
			return Invoke(nextFrame());
		}

		/// <summary>
		/// Runs a coroutine on this behaviour that will invoke the given action after the set amount of time
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns></returns>
		public Coroutine Invoke(Action action, float delay)
		{
			return this.StartCoroutine(StratusRoutines.Call(action, delay));
		}

		/// <summary>
		/// Destroys the GameObject this component belongs to on the next frame
		/// </summary>
		public void DestroyGameObjectOnNextFrame()
		{
			IEnumerator routine()
			{
				yield return new WaitForEndOfFrame();
				Destroy(this.gameObject);
			}
			StratusCoroutineRunner.Run(routine());
		}

		/// <summary>
		/// Returns a cached version of the component
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetComponentCached<T>() where T : Component
		{
			if (componentCache == null)
			{
				componentCache = new Dictionary<Type, Component>();
			}

			Type type = typeof(T);
			if (!componentCache.ContainsKey(type))
			{
				componentCache.Add(type, GetComponent<T>());
			}
			return (T)componentCache[type];
		}

		/// <summary>
		/// Asserts that a component has been assigned
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="component"></param>
		protected void AssertNotNull<T>(T component, string label = null) where T : Component
		{
			if (component == null)
			{
				this.LogError($"Error: Component {(label != null ? label : " ")}of type {typeof(T)} on GameObject {gameObject.name } was not assigned");
			}
		}
	}

}