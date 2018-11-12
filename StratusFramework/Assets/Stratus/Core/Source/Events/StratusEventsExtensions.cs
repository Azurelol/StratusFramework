using UnityEngine;
using System.Collections;
using System;

namespace Stratus
{
	public static class StratusEventsExtensions
	{
		/// <summary>
		/// Connects to the specified event on this given object.
		/// </summary>
		/// <typeparam name="T">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="func">The member function callback. </param>
		public static void Connect<T>(this GameObject gameObj, Action<T> func)
		{
			Stratus.StratusEvents.Connect(gameObj, func);
		}

		public static void Connect(this GameObject gameObj, Action<Stratus.StratusEvent> func, Type type)
		{
			Stratus.StratusEvents.Connect(gameObj, func, type);
		}

		/// <summary>
		/// Disconnects this component from all events.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj"></param>
		public static void Disconnect(this MonoBehaviour component)
		{
			//T type = default(T);
			//var eventName = type.ToString();
			Stratus.StratusEvents.Disconnect(component);
		}

		///// <summary>
		///// Disconnects this component from all events.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="gameObj"></param>
		//public static void Disconnect(this Stratus.UIElement component)
		//{
		//  //T type = default(T);
		//  //var eventName = type.ToString();
		//  Stratus.Events.Disconnect(component);
		//}

		/// <summary>
		/// Disconnects this component from events from the specified GameObject.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj"></param>
		public static void Disconnect(this MonoBehaviour component, GameObject gameObj)
		{
			//T type = default(T);
			//var eventName = type.ToString();
			Stratus.StratusEvents.Disconnect(component, gameObj);
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto this object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="eventObj">The event object. </param>
		/// <param name="nextFrame">Whether the event should be sent next frame.</param>
		public static void Dispatch<T>(this GameObject gameObj, T eventObj, bool nextFrame = false) where T : Stratus.StratusEvent
		{
			Stratus.StratusEvents.Dispatch<T>(gameObj, eventObj, nextFrame);
		}

		///// <summary>
		///// Dispatches the given event of the specified type onto this object.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="gameObj">The GameObject to which to connect to.</param>
		///// <param name="eventObj">The event object. </param>
		///// <param name="nextFrame">Whether the event should be sent next frame.</param>
		//public static void Dispatch<T>(this GameObject gameObj, params object[] eventParameters) where T : Stratus.Event, new()
		//{
		//  T eventObject = (T)Activator.CreateInstance(typeof(T), eventParameters);    
		//  Stratus.Events.Dispatch<T>(gameObj, eventObject, false);
		//}

		/// <summary>
		/// Dispatches the given event of the specified type onto this object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="eventObj">The event object. </param>
		/// <param name="nextFrame">Whether the event should be sent next frame.</param>
		public static void Dispatch(this GameObject gameObj, Stratus.StratusEvent eventObj, System.Type type, bool nextFrame = false)
		{
			Stratus.StratusEvents.Dispatch(gameObj, eventObj, type, nextFrame);
		}

		/// <summary>
		/// Dispatches the given event of the specified type to this object and all its children.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchDown<T>(this GameObject gameObj, T eventObj, bool nextFrame = false) where T : Stratus.StratusEvent
		{
			Stratus.StratusEvents.DispatchDown<T>(gameObj, eventObj, nextFrame);
		}

		/// <summary>
		/// Dispatches an event up the tree on each parent recursively.
		/// </summary>
		/// <typeparam name="T">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to dispatch to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchUp<T>(this GameObject gameObj, T eventObj, bool nextFrame = false) where T : Stratus.StratusEvent
		{
			Stratus.StratusEvents.DispatchUp<T>(gameObj, eventObj, nextFrame);
		}



	}
}