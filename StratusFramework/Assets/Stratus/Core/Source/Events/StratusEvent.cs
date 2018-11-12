using System;
using System.Collections.Generic;
using Stratus.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
	/// <summary>
	/// Base event class for all events that use the Stratus Event System. 
	/// Derive from it in order to implement your own custom events.
	/// your own custom events.
	/// </summary>
	[Serializable]
	public class StratusEvent
	{
		//--------------------------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Whether the event is dispatched is being dispatched to a single gamobject or to the whole scene
		/// </summary>
		public enum Scope
		{
			GameObject,
			Scene
		}

		/// <summary>
		/// A callback consisting of the Stratus Event received
		/// </summary>
		[System.Serializable]
		public class EventCallback : UnityEvent<Stratus.StratusEvent> { }

		/// <summary>
		/// A delegate for a connect function
		/// </summary>
		/// <param name="connectFunc"></param>
		public delegate void ConnectFunction(System.Action<StratusEvent> connectFunc);
		public delegate void EventCallbackFunction(Stratus.StratusEvent eventObj);
		public delegate void GenericEventCallback<in T>(T eventObj);

		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		private static Dictionary<Type, StratusEvent> eventCache = new Dictionary<Type, StratusEvent>();

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Returns an instance of the given event object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reset">Whether the event should be reset to its default values (by constructing it anew)</param>
		/// <returns></returns>
		public static T Get<T>(bool reset = false) where T : StratusEvent, new()
		{
			Type type = typeof(T);

			if (!HasCached<T>(type))
			{
				Cache<T>(type);
			}
			else if (reset)
			{
				if (HasCached<T>(type))
				{
					ResetCache<T>(type); 
				}
				else
				{
					Cache<T>(type);
				}
			}

			T eventObject = (T)eventCache[type];			
			return eventObject;
		}

		public static StratusEvent Instantiate(Type type) => (Stratus.StratusEvent)Utilities.Reflection.Instantiate(type);

		public static StratusEvent Instantiate(Type type, string data)
		{
			StratusEvent instance = Instantiate(type);
			JsonUtility.FromJsonOverwrite(data, instance);
			return instance;
		}

		private static void Cache<T>(Type type) where T: StratusEvent, new() => eventCache.Add(type, (T)Reflection.Instantiate(type));
		private static bool HasCached<T>(Type type) where T : StratusEvent, new() => eventCache.ContainsKey(type);
		private static void ResetCache<T>(Type type) where T : StratusEvent, new() => eventCache[type] = (T)Reflection.Instantiate(type);
	}

	/// <summary>
	/// An interface for an event representing the state of an action
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ActionEvent<T>
	{
		/// <summary>
		/// An input event represents a request to perform a specific action
		/// </summary>
		public class InputEvent : Stratus.StratusEvent { public InputBinding.State state; }
		/// <summary>
		/// If an input has been accepted, and is legal, represents the beginning of an action
		/// </summary>
		public class StartedEvent : Stratus.StratusEvent { }
		/// <summary>
		/// If an input has been accepted, and is legal, represents the beginning of an action
		/// </summary>
		public class EndedEvent : Stratus.StratusEvent { }
	}


}
