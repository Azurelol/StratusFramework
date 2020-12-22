using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay.Story
{
	/// <summary>
	/// Observers a variable insided an ink story
	/// </summary>
	public class StratusStoryObserver : StratusTriggerable
	{
		public enum Type { Observe, Retrieve };

		public Type type = Type.Observe;
		public StratusStoryReader reader;
		public StratusStory.Variable variable;

		// Whether the variable observer has been set up
		private bool setObserver = false;

		protected override void OnAwake()
		{
			reader.gameObject.Connect<StratusStory.EndedEvent>(this.OnStoryEndedEvent);
		}

		protected override void OnReset()
		{

		}

		protected override void OnTrigger()
		{
			switch (type)
			{
				case Type.Observe:
					ObserveValue();
					break;
				case Type.Retrieve:
					RetrieveValue();
					break;
				default:
					break;
			}
		}

		void OnStoryEndedEvent(StratusStory.EndedEvent e)
		{
			//var removeEvent = new Story.RemoveVariableObserverEvent();
			//removeEvent.variableObserver = OnValueChanged;
			//reader.gameObject.Dispatch<Story.RemoveVariableObserverEvent>(removeEvent);
		}

		public void OnValueChanged(string variableName, object value)
		{
			switch (variable.type)
			{
				case StratusStory.Types.Integer:
					variable.intValue = (int)value;
					break;
				case StratusStory.Types.Boolean:
					variable.boolValue = (bool)value;
					break;
				case StratusStory.Types.String:
					variable.stringValue = (string)value;
					break;
				case StratusStory.Types.Float:
					variable.floatValue = (float)value;
					break;
				default:
					break;
			}

			PrintValue();
		}

		void ObserveValue()
		{
			if (setObserver)
				return;

			var observeEvent = new StratusStory.ObserveVariableEvent();
			observeEvent.variableName = variable.name;
			observeEvent.variableObserver = OnValueChanged;
			reader.gameObject.Dispatch<StratusStory.ObserveVariableEvent>(observeEvent);
			setObserver = true;
		}

		void RetrieveValue()
		{
			var findValueEvent = new StratusStory.RetrieveVariableValueEvent();
			findValueEvent.variable = this.variable;
			this.gameObject.Dispatch<StratusStory.RetrieveVariableValueEvent>(findValueEvent);

			PrintValue();
		}



		void PrintValue()
		{
			if (debug)
				StratusDebug.Log($"The variable {variable.name} of type {variable.type} has a value of { variable.objectValue}", this);
		}

	}

}