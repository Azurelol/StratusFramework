using UnityEngine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// An object which can be interacted with
	/// </summary>
	public abstract class StratusInteractable : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether to show debug output for this object
		/// </summary>
		public bool log = false;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Relevant data about this object
		/// </summary>
		protected abstract object[] data { get; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnAwake();
		protected abstract void OnInteract(StratusSensor sensor);
		protected abstract void OnDetect(StratusSensor sensor);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			this.gameObject.Connect<StratusSensor.InteractEvent>(this.OnInteractEvent);
			this.gameObject.Connect<StratusSensor.InteractableDetectedEvent>(this.OnDetectEvent);
			OnAwake();
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Received when this object has been detected
		/// </summary>
		/// <param name="e"></param>

		void OnDetectEvent(StratusSensor.InteractableDetectedEvent e)
		{
			// Fill out information about this object
			e.scanData = this.data;
			// Inform this object that it has been scanned
			OnDetect(e.source);
		}

		/// <summary>
		/// Received when there's a request to interact with this object
		/// </summary>
		/// <param name="e"></param>
		void OnInteractEvent(StratusSensor.InteractEvent e)
		{
			// Signal this object that it's being interacted with
			this.OnInteract(e.source);
		}

	}
}