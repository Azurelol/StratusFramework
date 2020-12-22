using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
	/// <summary>
	/// A component that when triggered will perform a specific action.
	/// </summary>
	public abstract class StratusTriggerable : StratusTriggerBase
	{
		/// <summary>
		/// This event signals that the triggerable has finished
		/// </summary>
		public class EndedEvent : StratusEvent { }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this event dispatcher will respond to trigger events
		/// </summary>
		[Tooltip("How long after activation before the event is fired")]
		public float delay;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/  
		/// <summary>
		/// The latest received trigger event
		/// </summary>
		protected StratusTriggerBehaviour.TriggerEvent triggerEvent { get; private set; }

		/// <summary>
		/// The latest received instruction
		/// </summary>
		protected StratusTriggerBehaviour.Instruction instruction { get; private set; }

		/// <summary>
		/// Subscribe to be notified when this trigger has been activated
		/// </summary>
		public UnityAction<StratusTriggerable> onTriggered { get; set; }

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		abstract protected void OnAwake();
		abstract protected void OnTrigger();

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Called when the script instance is first being loaded.
		/// </summary>
		void Awake()
		{
			awoke = true;
			this.gameObject.Connect<StratusTriggerBehaviour.TriggerEvent>(this.OnTriggerEvent);
			this.OnAwake();
			onTriggered = (StratusTriggerable trigger) => { };
		}

		/// <summary>
		/// When the trigger event is received, runs the trigger sequence.
		/// </summary>
		/// <param name="e"></param>
		protected void OnTriggerEvent(StratusTriggerBehaviour.TriggerEvent e)
		{
			triggerEvent = e;
			instruction = triggerEvent.instruction;
			this.RunTriggerSequence();
		}

		/// <summary>
		/// Activates this triggerable
		/// </summary>
		public void Activate()
		{
			if (!enabled)
				return;

			if (debug)
				StratusDebug.Log($"<i>{description}</i> has been triggered!", this);
			this.RunTriggerSequence();
			activated = true;
		}

		/// <summary>
		/// Runs the trigger sequence. After a specified delay, it will invoke
		/// the abstract 'OnTrigger' method.
		/// </summary>
		protected void RunTriggerSequence()
		{
			var seq = StratusActions.Sequence(this.gameObject.Actions());
			StratusActions.Delay(seq, this.delay);
			StratusActions.Call(seq, this.OnTrigger);
			StratusActions.Call(seq, () => onTriggered(this));
		}

	}

}