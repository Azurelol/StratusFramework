using System.Collections.Generic;

namespace Stratus
{
	[StratusSingleton(instantiate = true)]
	public class StratusManagedBehaviourSystem : StratusSingletonBehaviour<StratusManagedBehaviourSystem>
	{
		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		private static List<StratusManagedBehaviour> behaviours = new List<StratusManagedBehaviour>();

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public bool update { get; set; } = true;
		public bool lateUpdate { get; set; } = true;
		public bool fixedUpdate { get; set; } = true;

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			//AddCurrentBehaviours();
			//foreach (var behaviour in behaviours)
			//  behaviour.OnBehaviourAwake();
		}

		private void Start()
		{
			foreach (StratusManagedBehaviour behaviour in behaviours)
			{
				behaviour.ManagedStart();
			}
		}

		private void Update()
		{
			if (!update)
				return;

			foreach (StratusManagedBehaviour behaviour in behaviours)
			{
				if (behaviour.enabled)
				{
					behaviour.ManagedUpdate();
				}
			}
		}

		private void FixedUpdate()
		{
			if (!fixedUpdate)
				return;

			foreach (StratusManagedBehaviour behaviour in behaviours)
			{
				if (behaviour.enabled)
				{
					behaviour.ManagedFixedUpdate();
				}
			}
		}

		private void LateUpdate()
		{
			if (!lateUpdate)
				return;

			foreach (StratusManagedBehaviour behaviour in behaviours)
			{
				if (behaviour.enabled)
				{
					behaviour.ManagedLateUpdate();
				}
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		public static void Add(StratusManagedBehaviour behaviour)
		{
			Instantiate();
			behaviours.Add(behaviour);
		}

		public static void Remove(StratusManagedBehaviour behaviour)
		{
			behaviours.Remove(behaviour);
		}

		private static void AddCurrentBehaviours()
		{
			StratusManagedBehaviour[] behaviours = StratusScene.GetComponentsInAllActiveScenes<StratusManagedBehaviour>();
			StratusDebug.Log($"Adding {behaviours.Length} behaviours");
			StratusManagedBehaviourSystem.behaviours.AddRange(behaviours);
		}




	}

}