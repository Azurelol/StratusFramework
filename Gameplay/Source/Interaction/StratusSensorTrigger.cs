using UnityEngine;
using System;
using Stratus.Gameplay;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Trigger invoked whenever a sensor picks interactions and detections
	/// </summary>
	public class StratusSensorTrigger : StratusTriggerBehaviour
	{
		public enum TriggerType 
		{ 
			Interaction, 
			Detection 
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Header("Interaction")]
		public TriggerType type = TriggerType.Interaction;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		/// <summary>
		/// Initializes the InteractionTrigger, subscribing to events with the player
		/// </summary>
		protected override void OnAwake()
		{
			switch (type)
			{
				case TriggerType.Interaction:
					this.gameObject.Connect<StratusSensor.InteractEvent>(this.OnInteractEvent);
					break;
				case TriggerType.Detection:
					this.gameObject.Connect<StratusSensor.InteractableDetectedEvent>(this.OnDetection);
					break;
			}
		}

		protected override void OnReset()
		{
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Received when this object is within vicinity of the agent.
		/// </summary>
		/// <param name="e"></param>
		private void OnDetection(StratusSensor.InteractableDetectedEvent e)
		{
			this.Activate();
		}

		/// <summary>
		/// Received when there's a request to interact with this object
		/// </summary>
		/// <param name="e"></param>
		private void OnInteractEvent(StratusSensor.InteractEvent e)
		{
			this.Activate();
		}


	}
}