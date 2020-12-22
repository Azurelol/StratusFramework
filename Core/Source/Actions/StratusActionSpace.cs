using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Handles the updating of all actions.
	/// </summary>
	[StratusSingleton("Stratus Action System", true, true)]
	public class StratusActionSpace : StratusSingletonBehaviour<StratusActionSpace>
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public class ActionsContainer
		{
			public GameObject gameObject;
			public StratusActionDriver owner;

			public ActionsContainer(GameObject gameObject, StratusActionDriver owner)
			{
				this.gameObject = gameObject;
				this.owner = owner;
			}
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		private List<ActionsContainer> activeActions { get; set; }
		//private ActionsOwnerContainer recentlyAddedActions { get; set; }
		private Dictionary<GameObject, ActionsContainer> actionsOwnerMap;

		//------------------------------------------------------------------------/    
		// Messages
		//------------------------------------------------------------------------/    
		/// <summary>
		/// Initializes the ActionSpace
		/// </summary>
		protected override void OnAwake()
		{
			this.actionsOwnerMap = new Dictionary<GameObject, ActionsContainer>();
			this.activeActions = new List<ActionsContainer>();
			DontDestroyOnLoad(this.gameObject);
		}

		/// <summary>
		/// Updates all the Actions in the ActionSpace, through the ActionOwners
		/// for every GameObject.
		/// </summary>
		private void FixedUpdate()
		{
			this.Propagate();
		}

		//------------------------------------------------------------------------/    
		// Methods: Static
		//------------------------------------------------------------------------/  
		/// <summary>
		/// Subscribe the specified GameObject to the ActionSpace.
		/// </summary>
		/// <param name="gameObj">reference to the gameobject.</param>
		/// <returns></returns>
		public static StratusActionDriver Subscribe(GameObject gameObj)
		{
			return instance.SubscribeToActions(gameObj);
		}

		/// <summary>
		/// Unsubscribes the specified GameObject from the ActionSpace.
		/// </summary>
		/// <param name="gameObj">A reference to the gameobject.</param>
		public static void Unsubscribe(GameObject gameObj)
		{
			if (isQuitting)
			{
				return;
			}

			instance.UnsubscribeFromActions(gameObj);
		}

		public static void Clear(MonoBehaviour component)
		{
			if (isQuitting)
			{
				return;
			}

			component.gameObject.Actions().Clear();
		}

		//------------------------------------------------------------------------/    
		// Methods: Private
		//------------------------------------------------------------------------/  
		/// <summary>
		/// Propagates an update to all active actions through ActionOwners.
		/// </summary>
		private void Propagate()
		{
			// Update all actions
			ActionsContainer[] currentActions = this.activeActions.ToArray();
			for (int i = 0; i < currentActions.Length; ++i)
			{
				currentActions[i].owner.Update(Time.deltaTime);
			}
		}

		/// <summary>
		/// Subscribes this gameobject to the action space
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		private StratusActionDriver SubscribeToActions(GameObject gameObject)
		{
			if (this.actionsOwnerMap.ContainsKey(gameObject))
			{
				return this.actionsOwnerMap[gameObject].owner;
			}

			if (StratusActions.debug)
			{
				StratusDebug.Log("Adding the GameObject to the ActionSpace");
			}

			StratusActionDriver owner = new StratusActionDriver(gameObject);
			ActionsContainer container = new ActionsContainer(gameObject, owner);

			this.activeActions.Add(container);
			this.actionsOwnerMap.Add(gameObject, container);
			gameObject.GetOrAddComponent<StratusActionsRegistration>();
			return owner;
		}

		/// <summary>
		/// Unsubscribe this gameobject from the action system
		/// </summary>
		/// <param name="gameObject"></param>
		private void UnsubscribeFromActions(GameObject gameObject)
		{
			// @TODO: Why is this an issue?
			if (gameObject == null)
			{
				return;
			}

			if (StratusActions.debug)
			{
				StratusDebug.Log("'" + gameObject.name + "'");
			}

			ActionsContainer container = this.actionsOwnerMap[gameObject];
			this.activeActions.Remove(container);
			this.actionsOwnerMap.Remove(gameObject);
		}

		/// <summary>
		/// Prints all active actions
		/// </summary>

		public static void PrintActiveActions()
		{
			string actionsLeft = "Active Actions: ";
			foreach (ActionsContainer action in StratusActionSpace.instance.activeActions)
			{
				actionsLeft += action.gameObject.name + ", ";
			}
			StratusDebug.Log(actionsLeft);
		}
	}
}