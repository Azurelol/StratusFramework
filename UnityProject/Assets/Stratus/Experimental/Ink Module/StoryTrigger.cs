using UnityEngine;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// A trigger which reacts to events in the Stratus Ink story reader
      /// </summary>
      public class StoryTrigger : Trigger
      {
        [Tooltip("The scope of the event to listen to")]
        public Event.Scope eventScope = Event.Scope.GameObject;
        [Tooltip("The story this trigger is reacting to")]
        [DrawIf("eventScope", Event.Scope.GameObject, ComparisonType.Equals)]
        public StoryReader reader;
        [Tooltip("The story this trigger is checking against. If none is specified, it will trigger on any.")]
        public TextAsset storyFile;
        [Tooltip("The knot of the selected story is checking against. If none is specified, it will trigger on any part of the story.")]
        public string knot;
        [Tooltip("What type of event this is being triggered by")]
        public Story.ReaderEventType storyEvent;
        [Tooltip("The index of the choice selected")]
        [DrawIf("storyEvent", Story.ReaderEventType.SelectChoice, ComparisonType.Equals)]
        public int choiceIndex = 0;

        protected override void OnAwake()
        {
          switch (storyEvent)
          {
            case Story.ReaderEventType.Loaded:
              if (eventScope == Event.Scope.GameObject)
                reader.gameObject.Connect<Story.LoadedEvent>(this.OnStoryLoadedEvent);
              else 
                Scene.Connect<Story.LoadedEvent>(this.OnStoryLoadedEvent);
              break;

            case Story.ReaderEventType.Started:
              if (eventScope == Event.Scope.GameObject)
                reader.gameObject.Connect<Story.StartedEvent>(this.OnStoryStartedEvent);
              else
                Scene.Connect<Story.StartedEvent>(this.OnStoryStartedEvent);
              break;

            case Story.ReaderEventType.Continue:
              if (eventScope == Event.Scope.GameObject)
                reader.gameObject.Connect<Story.ContinueEvent>(this.OnStoryContinueEvent);
              else
                Scene.Connect<Story.ContinueEvent>(this.OnStoryContinueEvent);
              break;

            case Story.ReaderEventType.Ended:
              if (eventScope == Event.Scope.GameObject)
                reader.gameObject.Connect<Story.EndedEvent>(this.OnStoryEndedEvent);
              else
                Scene.Connect<Story.EndedEvent>(this.OnStoryEndedEvent);
              break;

            case Story.ReaderEventType.SelectChoice:
              if (eventScope == Event.Scope.GameObject)
                reader.gameObject.Connect<Story.SelectChoiceEvent>(this.OnSelectChoiceEvent);
              else
                Scene.Connect<Story.SelectChoiceEvent>(this.OnSelectChoiceEvent);
              break;
          }
        }

        protected override void OnReset()
        {

        }

        void OnStoryLoadedEvent(Story.LoadedEvent e)
        {
          if (ValidateStory(e))
            Activate();
        }

        void OnStoryStartedEvent(Story.StartedEvent e)
        {
          if (ValidateStory(e))
            this.Activate();
        }

        void OnStoryContinueEvent(Story.ContinueEvent e)
        {
          if (ValidateStory(e))
            this.Activate();
        }

        void OnStoryEndedEvent(Story.EndedEvent e)
        {
          if (ValidateStory(e))
            this.Activate();
        }

        void OnSelectChoiceEvent(Story.SelectChoiceEvent e)
        {
          if (ValidateStory(e) && e.choice.index == this.choiceIndex)
            this.Activate();
        }

        bool ValidateStory(Story.ReaderEvent e)
        {
          if (this.storyFile == null)
            return true;

          bool matchingStory = e.story.name == this.storyFile.name;
          bool matchingKnot = !string.IsNullOrEmpty(knot) ? knot == e.story.latestKnot : true;          

          return matchingStory && matchingKnot;
        }



      }

    } 
  }
}