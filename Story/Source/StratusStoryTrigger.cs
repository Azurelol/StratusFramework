using UnityEngine;

namespace Stratus.Gameplay.Story
{
	/// <summary>
	/// A trigger which reacts to events in the Stratus Ink story reader
	/// </summary>
	public class StratusStoryTrigger : StratusTriggerBehaviour
	{
		[Tooltip("The scope of the event to listen to")]
		public StratusEvent.Scope eventScope = StratusEvent.Scope.GameObject;
		[Tooltip("The story this trigger is reacting to")]
		[DrawIf("eventScope", StratusEvent.Scope.GameObject, ComparisonType.Equals)]
		public StratusStoryReader reader;
		[Tooltip("The story this trigger is checking against. If none is specified, it will trigger on any.")]
		public TextAsset storyFile;
		[Tooltip("The knot of the selected story is checking against. If none is specified, it will trigger on any part of the story.")]
		public string knot;
		[Tooltip("What type of event this is being triggered by")]
		public StratusStory.ReaderEventType storyEvent;
		[Tooltip("The index of the choice selected")]
		[DrawIf("storyEvent", StratusStory.ReaderEventType.SelectChoice, ComparisonType.Equals)]
		public int choiceIndex = 0;

		protected override void OnAwake()
		{
			switch (storyEvent)
			{
				case StratusStory.ReaderEventType.Loaded:
					if (eventScope == StratusEvent.Scope.GameObject)
						reader.gameObject.Connect<StratusStory.LoadedEvent>(this.OnStoryLoadedEvent);
					else
						StratusScene.Connect<StratusStory.LoadedEvent>(this.OnStoryLoadedEvent);
					break;

				case StratusStory.ReaderEventType.Started:
					if (eventScope == StratusEvent.Scope.GameObject)
						reader.gameObject.Connect<StratusStory.StartedEvent>(this.OnStoryStartedEvent);
					else
						StratusScene.Connect<StratusStory.StartedEvent>(this.OnStoryStartedEvent);
					break;

				case StratusStory.ReaderEventType.Continue:
					if (eventScope == StratusEvent.Scope.GameObject)
						reader.gameObject.Connect<StratusStory.ContinueEvent>(this.OnStoryContinueEvent);
					else
						StratusScene.Connect<StratusStory.ContinueEvent>(this.OnStoryContinueEvent);
					break;

				case StratusStory.ReaderEventType.Ended:
					if (eventScope == StratusEvent.Scope.GameObject)
						reader.gameObject.Connect<StratusStory.EndedEvent>(this.OnStoryEndedEvent);
					else
						StratusScene.Connect<StratusStory.EndedEvent>(this.OnStoryEndedEvent);
					break;

				case StratusStory.ReaderEventType.SelectChoice:
					if (eventScope == StratusEvent.Scope.GameObject)
						reader.gameObject.Connect<StratusStory.SelectChoiceEvent>(this.OnSelectChoiceEvent);
					else
						StratusScene.Connect<StratusStory.SelectChoiceEvent>(this.OnSelectChoiceEvent);
					break;
			}
		}

		protected override void OnReset()
		{

		}

		void OnStoryLoadedEvent(StratusStory.LoadedEvent e)
		{
			if (ValidateStory(e))
				Activate();
		}

		void OnStoryStartedEvent(StratusStory.StartedEvent e)
		{
			if (ValidateStory(e))
				this.Activate();
		}

		void OnStoryContinueEvent(StratusStory.ContinueEvent e)
		{
			if (ValidateStory(e))
				this.Activate();
		}

		void OnStoryEndedEvent(StratusStory.EndedEvent e)
		{
			if (ValidateStory(e))
				this.Activate();
		}

		void OnSelectChoiceEvent(StratusStory.SelectChoiceEvent e)
		{
			if (ValidateStory(e) && e.choice.index == this.choiceIndex)
				this.Activate();
		}

		bool ValidateStory(StratusStory.ReaderEvent e)
		{
			if (this.storyFile == null)
				return true;

			bool matchingStory = e.story.name == this.storyFile.name;
			bool matchingKnot = !string.IsNullOrEmpty(knot) ? knot == e.story.latestKnot : true;

			return matchingStory && matchingKnot;
		}



	}
}