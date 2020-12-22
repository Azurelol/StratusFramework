using UnityEngine;


namespace Stratus.Examples
{
	// Whenever using the Stratus Event System, please use the provided MonoBehaviour
	public class EventsSample : StratusBehaviour
	{
		//--------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------/
		/// <summary>
		/// Custom events must derive from our custom event class 
		/// </summary>
		public class SampleEvent : Stratus.StratusEvent
		{
			public int number;
			public Vector2 vector2;
			public bool boolean;
		}

		//--------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------/
		public int sampleEventsReceived { get; private set; }
		public SampleEvent latestEvent { get; private set; }

		//--------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------/
		private void Start()
		{
			Subscribe();
		}

		//--------------------------------------------------------------------------/
		// Procedures
		//--------------------------------------------------------------------------/		
		private void Subscribe()
		{
			// Connect a member function to the given event for this GameObject or the Scene
			this.gameObject.Connect<SampleEvent>(this.OnSampleEvent);
			StratusScene.Connect<SampleEvent>(this.OnSampleEvent);
		}

		private void SampleEventToGameObject()
		{
			// Construct the event object
			SampleEvent eventObj = new SampleEvent
			{
				number = 5
			};

			// Dispatch the event
			this.gameObject.Dispatch<SampleEvent>(eventObj);
		}

		private void SampleEventToScene()
		{
			// Construct the event object
			SampleEvent eventObj = new SampleEvent
			{
				number = 15
			};

			// Dispatch the event
			StratusScene.Dispatch<SampleEvent>(eventObj);
		}

		/// <summary>
		/// The callback function called when an event has been dispatched.
		/// </summary>
		/// <param name="e">The event object, a custom class which may contain member variables.</param>
		public void OnSampleEvent(SampleEvent e)
		{
			latestEvent = e;
			sampleEventsReceived++;
		}



	}

}