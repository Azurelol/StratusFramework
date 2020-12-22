using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// #define STRATUS_EVENTS_DEBUG

namespace Stratus
{
	using DelegateMap = Dictionary<string, List<Delegate>>;
	using DelegateTypeList = List<Delegate>;
	using DispatchMap = Dictionary<GameObject, Dictionary<string, List<Delegate>>>;

	/// <summary>
	/// The class which manages the overlying Stratus event system.
	/// </summary>
	[StratusSingleton("Stratus Event System", true, true)]
	public class StratusEvents : StratusSingletonBehaviour<StratusEvents>
	{
		public class LoggingSetup
		{
			public bool construction = false;
			public bool register = false;
			public bool connect = true;
			public bool dispatch = false;
		}

		//------------------------------------------------------------------------/
		// Properties    
		//------------------------------------------------------------------------/    
		/// <summary>
		/// Whether we are doing tracing for debugging purposes.
		/// </summary>
		public static LoggingSetup logging { get; } = new LoggingSetup();
		/// <summary>
		/// A map of all GameObjects connected to the event system and a map of delegates that are connected to them.
		/// Whenever an event of a given type is sent to the GameObject, we invoke it on all delegates for a given type
		/// (essentially a list of delegates for each type)
		/// </summary>
		private DispatchMap dispatchMap { get; set; } = new DispatchMap();
		/// <summary>
		/// A map of all components that have connected to a GameObject for specific events
		/// </summary>
		private Dictionary<MonoBehaviour, List<GameObject>> connectMap { get; set; } = new Dictionary<MonoBehaviour, List<GameObject>>();
		/// <summary>
		/// A list of all event types that are being watched for at the moment.
		/// </summary>
		private List<string> eventWatchList { get; set; } = new List<string>();

		//------------------------------------------------------------------------/
		// Messages 
		//------------------------------------------------------------------------/
		/// <summary>
		/// Initializes the event system manager.
		/// </summary>
		protected override void OnAwake()
		{
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Connects to the event of a given object.
		/// </summary>
		/// <typeparam name="T">The event class. </typeparam>
		/// <param name="gameObject">The GameObject we are connecting to whose events we are connecting to. </param>
		/// <param name="memFunc">The member function to connect to. </param>
		public static void Connect<T>(GameObject gameObject, Action<T> memFunc)
		{
			Type type = typeof(T);
			string key = type.ToString();

			// If the GameObject hasn't been registered yet, add its key
			CheckRegistration(gameObject);

			// If the event has no delegates yet, add it
			if (!StratusEvents.instance.dispatchMap[gameObject].ContainsKey(key))
			{
				StratusEvents.instance.dispatchMap[gameObject].Add(key, new DelegateTypeList());
			}

			// If the delegate is already present, do not add it
			if (StratusEvents.instance.dispatchMap[gameObject][key].Contains(memFunc))
			{
				return;
			}

			// Add the component's delegate onto the gameobject
			AddDelegate(gameObject, key, memFunc);

			// Record that this component has connected to this GameObject
			Register((MonoBehaviour)memFunc.Target, gameObject);

#if STRATUS_EVENTS_DEBUG
      Trace.Script(memFunc.ToString() + " has connected to " + gameObject.name);
      //Trace.Script(obj.name + " now has '" + Events.Instance.DelegatesByType[obj].Count + "' delegates");
#endif
		}

		public static void Connect(GameObject gameObject, Action<Stratus.StratusEvent> memFunc, Type type)
		{
			string key = type.ToString();

			// If the GameObject hasn't been registered yet, add its key
			CheckRegistration(gameObject);

			// If this GameObject's dispatch map has no delegates for this event type yet, create it
			if (!StratusEvents.instance.dispatchMap[gameObject].ContainsKey(key))
			{
				StratusEvents.instance.dispatchMap[gameObject].Add(key, new DelegateTypeList());
			}

			// If the delegate is already present, do not add it
			if (StratusEvents.instance.dispatchMap[gameObject][key].Contains(memFunc))
			{
				return;
			}

			// Add the component's delegate onto the gameobject
			AddDelegate(gameObject, key, memFunc);
			// Record that this component has connected to this GameObject
			Register((MonoBehaviour)memFunc.Target, gameObject);

#if STRATUS_EVENTS_DEBUG
        Trace.Script(memFunc.ToString() + " has connected to " + gameObject.name);
#endif
		}

		/// <summary>
		/// Disconnects this component from all events it has subscribed to.
		/// </summary>
		/// <param name="component"></param>
		public static void Disconnect(MonoBehaviour component)
		{
			if (isQuitting)
			{
				return;
			}

			// If the component is already connected and present in the event system
			if (instance.connectMap.ContainsKey(component))
			{
				// For every gameobject that it has connected to
				foreach (GameObject gameobj in instance.connectMap[component])
				{
					// Disconnect its delegates from it
					DisconnectFromGameObject(component, gameobj);
				}
			}

			// Remove the component from the event system
			instance.connectMap.Remove(component);
		}


		/// <summary>
		/// Disconnects this component from all events it has subscribed on
		/// the given GameoObject.
		/// </summary>
		/// <param name="gameObj"></param>
		/// <param name="component"></param>
		public static void Disconnect(MonoBehaviour component, GameObject gameObj)
		{
			DisconnectFromGameObject(component, gameObj);
			// Remove the gameobject from the component's list in the system
			instance.connectMap[component].Remove(gameObj);
		}

		/// <summary>
		/// Disconnects this component from all events it has subscribed on
		/// the given GameoObject.
		/// </summary>
		/// <param name="gameObj"></param>
		/// <param name="component"></param>
		private static void DisconnectFromGameObject(Behaviour component, GameObject gameObj)
		{
			// If the GameObject has been removed previously...
			if (!StratusEvents.instance.dispatchMap.ContainsKey(gameObj))
			{
				return;
			}

			// For every delegate this GameObject has
			foreach (KeyValuePair<string, DelegateTypeList> pair in StratusEvents.instance.dispatchMap[gameObj])
			{
				// For every delegate in the list
				foreach (Delegate deleg in pair.Value)
				{
					if ((MonoBehaviour)deleg.Target == component)
					{
#if STRATUS_EVENTS_DEBUG
            Trace.Script("Disconnecting <i>" + deleg.Method.Name + "</i> from " + gameObj.name);
#endif
						// Remove this delegate
						pair.Value.Remove(deleg);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto the object.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="obj">The object to which to connect to.</param>
		/// <param name="e">The event to which to listen for.</param>
		/// <param name="nextFrame">Whether to send this event on the next frame.</param>
		public static void Dispatch<T>(GameObject obj, T e, bool nextFrame = false) where T : StratusEvent
		{
			string key = typeof(T).ToString();

#if STRATUS_EVENTS_DEBUG
      //  Trace.Script("'" + key + "' to '" + obj.name + "'");
#endif

			// If this a delayed dispatch...
			if (nextFrame)
			{
				instance.StartCoroutine(DispatchNextFrame<T>(obj, e));
			}

			// Check if the object has been registered onto the event system.
			// If not, it will be.
			CheckRegistration(obj);

			// If there is no delegate registered to this object, do nothing.
			if (!HasDelegate(obj, key))
			{
#if STRATUS_EVENTS_DEBUG
        //if (logging.Dispatch)
        //  Trace.Script("No delegate registered to " + obj.name + " for " + eventObj.ToString());
#endif
				return;
			}

			// If we are watching events of this type
			bool watching = false;
			if (instance.eventWatchList.Contains(key))
			{
				watching = true;
			}

			// Invoke the method for every delegate
			DelegateTypeList delegateMap = StratusEvents.instance.dispatchMap[obj][key];
			DelegateTypeList delegatesToRemove = null;
			foreach (Delegate deleg in delegateMap)
			{
				// If we are watching events of this type
				if (watching)
				{
					StratusDebug.Log("Invoking member function on " + deleg.Target.ToString());
				}

				// Do a lazy delete if it has been nulled out?
				if (IsNull(deleg.Method) || IsNull(deleg.Target))
				{
					if (delegatesToRemove == null)
					{
						delegatesToRemove = new DelegateTypeList();
					}

					delegatesToRemove.Add(deleg);
					continue;
				}

				deleg.DynamicInvoke(e);
				e.handled = true;
			}

			// If any delegates were found to be null, remove them (lazy delete)
			if (delegatesToRemove != null)
			{
				foreach (Delegate deleg in delegatesToRemove)
				{
					delegateMap.Remove(deleg);
				}
			}

		}

		/// <summary>
		/// Dispatches the given event of the specified type onto the object.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="obj">The object to which to connect to.</param>
		/// <param name="e">The name of the event to which to listen for.</param>
		/// <param name="nextFrame">Whether to send this event on the next frame.</param>
		public static void Dispatch(GameObject obj, StratusEvent e, System.Type type, bool nextFrame = false)
		{
			string key = type.ToString();

#if STRATUS_EVENTS_DEBUG
      if (logging.Connect)
        Trace.Script("'" + key + "' to '" + obj.name + "'");
#endif

			// If this a delayed dispatch...
			if (nextFrame)
			{
				instance.StartCoroutine(DispatchNextFrame(obj, e, type));
			}

			// Check if the object has been registered onto the event system.
			// If not, it will be.
			CheckRegistration(obj);

			// If there is no delegate registered to this object, do nothing.
			if (!HasDelegate(obj, key))
			{
#if STRATUS_EVENTS_DEBUG
        if (logging.Dispatch)
          Trace.Script("No delegate registered to " + obj.name + " for " + eventObj.ToString());
#endif
				return;
			}

			// If we are watching events of this type
			bool watching = false;
			if (instance.eventWatchList.Contains(key))
			{
				watching = true;
			}

			// Invoke the method for every delegate
			DelegateTypeList delegateMap = StratusEvents.instance.dispatchMap[obj][key];
			DelegateTypeList delegatesToRemove = null;
			foreach (Delegate deleg in delegateMap)
			{
				// If we are watching events of this type
				if (watching)
				{
					StratusDebug.Log("Invoking member function on " + deleg.Target.ToString());
				}

				// Do a lazy delete if it has been nulled out?
				if (IsNull(deleg.Method) || IsNull(deleg.Target))
				{
					if (delegatesToRemove == null)
					{
						delegatesToRemove = new DelegateTypeList();
					}

					delegatesToRemove.Add(deleg);
					continue;
				}

				deleg.DynamicInvoke(e);
				e.handled = true;
			}

			// If any delegates were found to be null, remove them (lazy delete)
			if (delegatesToRemove != null)
			{
				foreach (Delegate deleg in delegatesToRemove)
				{
					delegateMap.Remove(deleg);
				}
			}

		}

		public static bool IsNull(object obj)
		{
			if (obj == null || (obj is UnityEngine.Object & obj.Equals(null)))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Dispatches the event on the next frame.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="obj">The object to which to dispatch to.</param>
		/// <param name="eventObj">The event object we are sending.</param>
		/// <returns></returns>
		public static IEnumerator DispatchNextFrame<T>(GameObject obj, T eventObj) where T : StratusEvent
		{
			// Wait 1 frame
			yield return 0;
			// Dispatch the event
			Dispatch<T>(obj, eventObj);
		}

		/// <summary>
		/// Dispatches the event on the next frame.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="obj">The object to which to dispatch to.</param>
		/// <param name="eventObj">The event object we are sending.</param>
		/// <returns></returns>
		public static IEnumerator DispatchNextFrame(GameObject obj, StratusEvent eventObj, Type type)
		{
			// Wait 1 frame
			yield return 0;
			// Dispatch the event
			Dispatch(obj, eventObj, type);
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto the GameObject amd all its children.
		/// </summary>
		/// <typeparam name="T">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to dispatch to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchDown<T>(GameObject gameObj, T eventObj, bool nextFrame = false) where T : StratusEvent
		{
			foreach (GameObject child in gameObj.Children())
			{
				Dispatch<T>(child, eventObj, nextFrame);
			}

		}

		/// <summary>
		/// Dispatches an event up the tree on each parent recursively.
		/// </summary>
		/// <typeparam name="T">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to dispatch to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchUp<T>(GameObject gameObj, T eventObj, bool nextFrame = false) where T : StratusEvent
		{
			Transform[] parents = gameObj.transform.GetComponentsInParent<Transform>();
			foreach (Transform parent in parents)
			{
				Dispatch<T>(parent.gameObject, eventObj, nextFrame);
			}
		}

		/// <summary>
		/// Checks if the GameObject has been the given delegate.
		/// </summary>
		/// <param name="obj">A reference to the GameObject.</param>
		/// <param name="key">The key to the delegate list.</param>
		/// <returns>True if it has the delegate, false otherwise.</returns>
		private static bool HasDelegate(GameObject obj, string key)
		{
			if (StratusEvents.instance.dispatchMap[obj] != null
				&& StratusEvents.instance.dispatchMap[obj].ContainsKey(key))
			{
				return true;
			}

#if STRATUS_EVENTS_DEBUG
      if (logging.Dispatch)
      {
        Trace.Script("Events of type '" + key + "' for '" + obj.name + "' have no delegates yet!");

        //string keys = "";
        //foreach(var keyPresent in Events.Instance.DelegatesByType[obj])
        //{
        //  keys += keyPresent.Key + " ";
        //}
        //Trace.Script("Its current events are: " + keys);
      }
#endif

			return false;
		}

		/// <summary>
		/// Checks if the GameObject has been registered onto the event system.
		/// </summary>
		/// <param name="gameObj">A reference to the GameObject. </param>
		private static void CheckRegistration(GameObject gameObj)
		{
			// If the GameObject hasn't registered yet, add its key
			if (!StratusEvents.instance.dispatchMap.ContainsKey(gameObj))
			{
				StratusEvents.Connect(gameObj);
			}
		}


		/// <summary>
		/// Registers the MonoBehaviour to the event system.
		/// </summary>
		/// <param name="component"></param>
		private static void Register(MonoBehaviour component, GameObject gameObject)
		{
			// If its component hasn't registered yet...
			if (!instance.connectMap.ContainsKey(component))
			{
				// Record it
				instance.connectMap.Add(component, new List<GameObject>());
			}

			// If we haven't recorded that this component has connected to this GameObject yet
			if (!instance.connectMap[component].Contains(gameObject))
			{
#if STRATUS_EVENTS_DEBUG
        //Trace.Script(component.name + " has connected to " + gameObject.name);
#endif
				instance.connectMap[component].Add(gameObject);
				//component.gameObject.AddComponent<EventsRegistration>();
			}
		}

		/// <summary>
		/// Registers the GameObject to the event system.
		/// </summary>
		/// <param name="gameObject">The GameObject which is being registered. </param>
		public static void Connect(GameObject gameObject)
		{
#if STRATUS_EVENTS_DEBUGRATUS_EVENTS_DEBUG
      //if (logging.Register)
      //  Trace.Script(gameObject.name + " has been registered to the event system");
#endif

			StratusEvents.instance.dispatchMap.Add(gameObject, new DelegateMap());
			gameObject.AddComponent<StratusEventsRegistration>();
		}

		/// <summary>
		/// Returns true if this GameObjet has registered to the event system
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		public static bool IsConnected(GameObject gameObject)
		{
			return StratusEvents.instance.dispatchMap.ContainsKey(gameObject);
		}

		/// <summary>
		/// Unregisters the GameObject from the event system.
		/// </summary>
		/// <param name="obj"></param>
		public static void Disconnect(GameObject obj)
		{
			// Is this truly necessary, though?
			if (isQuitting || StratusEvents.instance.dispatchMap == null)
			{
				return;
			}

			// Remove this GameObject from the event dispatch map
			if (StratusEvents.instance.dispatchMap.ContainsKey(obj))
			{
				if (logging.register)
				{
					StratusDebug.Log(obj.name + " has been disconnected from the event system");
				}

				StratusEvents.instance.dispatchMap.Remove(obj);
			}
		}

		/// <summary>
		/// Adds the specified event to watch list, informing the user whenever
		/// the event is being dispatched.
		/// </summary>
		/// <typeparam name="T">The event type.</typeparam>
		public static void Watch<T>()
		{
			string type = typeof(T).ToString();

#if STRATUS_EVENTS_DEBUG
      //if (logging.Dispatch)
      //  Trace.Script("Now watching for events of type '" + type + "'");
#endif

			if (!instance.eventWatchList.Contains(type))
			{
				instance.eventWatchList.Add(type);
			}
		}

		/// <summary>
		/// Adds a member function delegate of a specific type onto the GameObject.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj"></param>
		/// <param name="key"></param>
		/// <param name="memFunc"></param>
		private static void AddDelegate<T>(GameObject gameObj, string key, Action<T> memFunc)
		{
#if STRATUS_EVENTS_DEBUG
      //if (logging.Connect)
      //  Trace.Script("Adding delegate for event: " + key);
#endif

			StratusEvents.instance.dispatchMap[gameObj][key].Add(memFunc);
		}

		public static IEnumerator WaitForFrames(int frameCount)
		{
			while (frameCount > 0)
			{
				frameCount--;
				yield return null;
			}
		}


		/// <summary>
		/// Handles cleanup operations for MonoBehaviours that are connecting to 
		/// events in the Stratus Event System.
		/// </summary>
		public class Setup
		{
			private MonoBehaviour Owner;

			public Setup(MonoBehaviour owner) { this.Owner = owner; }
			~Setup() { this.Owner.Disconnect(); }
		}

	}

}
