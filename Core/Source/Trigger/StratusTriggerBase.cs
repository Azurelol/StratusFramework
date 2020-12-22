using Stratus.Interfaces;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Base class for all trigger-related components in the Stratus Trigger framework
	/// </summary>
	public abstract class StratusTriggerBase : StratusBehaviour, IStratusDebuggable, IStratusValidator
	{
		/// <summary>
		/// Whether this component has specific restart behaviour
		/// </summary>
		public interface Restartable
		{
			void OnRestart();
		}

		public enum DescriptionMode
		{
			Automatic,
			Manual
		}

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Signals the GameObject it's on that this basetrigger has been active
		/// </summary>
		public class ActivityEvent : Stratus.StratusEvent
		{
			public StratusTriggerBase triggerBase;
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// How descriptions are set
		/// </summary>
		[Tooltip("How descriptions are set")]
		public DescriptionMode descriptionMode = DescriptionMode.Automatic;
		/// <summary>
		/// A short description of what this is for
		/// </summary>
		[Tooltip("A short description on the purpose of this trigger. " +
		  "If not filled, it will attempt to generate an automatic one.")]
		public string description;
		/// <summary>
		/// Whether we are printing debug output
		/// </summary>
		[Tooltip("Whether we are printing debug output")]
		public bool debug = false;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this component has triggered
		/// </summary>
		public bool activated { get; protected set; }
		/// <summary>
		/// Whether this component has had Awake/Start called
		/// </summary>
		public bool awoke { get; protected set; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnReset();
		/// <summary>
		/// An automatically generated description of what the trigger does
		/// </summary>
		public virtual string automaticDescription => string.Empty;

		public virtual StratusObjectValidation Validate() => null;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Reset()
		{
			// If a trigger system is present, hide this component and set its default
			CheckForTriggerSystem();
			// Call subclass reset
			OnReset();
		}

		void IStratusDebuggable.Toggle(bool toggle)
		{
			debug = toggle;
		}

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/
		/// <summary>
		/// Restarts this trigger to its initial state
		/// </summary>
		public void Restart(bool enable = true)
		{
			activated = false;
			enabled = enable;
			var restartable = this as Restartable;
			restartable?.OnRestart();
			//OnRestart();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		private void CheckForTriggerSystem()
		{
			var triggerSystem = gameObject.GetComponent<StratusTriggerSystem>();
			if (triggerSystem)
			{
				this.hideFlags = HideFlags.HideInInspector;
				triggerSystem.Add(this);
			}
			else
			{
				this.hideFlags = HideFlags.None;
			}
		}

		protected void Error(string msg, Behaviour trigger)
		{
			StratusDebug.LogError(ComposeLog(msg), trigger);
		}

		protected void Log(string msg, Behaviour trigger)
		{
			StratusDebug.Log(ComposeLog(msg), trigger);
		}

		protected string ComposeLog(string msg)
		{
			return $"<i>{description}</i> : {msg}";
		}


	}

}