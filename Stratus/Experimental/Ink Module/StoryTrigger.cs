using UnityEngine;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// A trigger which reacts to changes in an ink story reader
      /// </summary>
      public class StoryTrigger : Trigger
      {


        [Tooltip("The story this trigger is reacting to")]
        public StoryReader reader;
        [Tooltip("What type of event this is being triggered by")]
        public Story.EventType storyEvent;
        [Tooltip("What variable we are ")]
        public Story.Variable variable;

        // Fields
        private TextAsset storyFile;

        protected override void OnAwake()
        {
          switch (storyEvent)
          {
            case Story.EventType.Loaded:
              reader.gameObject.Connect<Story.LoadedEvent>(this.OnStoryLoadedEvent);
              break;
            case Story.EventType.Started:
              reader.gameObject.Connect<Story.StartedEvent>(this.OnStoryStartedEvent);
              break;
            case Story.EventType.Continue:
              reader.gameObject.Connect<Story.ContinueEvent>(this.OnStoryContinueEvent);
              break;
            case Story.EventType.Ended:
              reader.gameObject.Connect<Story.EndedEvent>(this.OnStoryEndedEvent);
              break;
          }
        }

        void OnStoryLoadedEvent(Story.LoadedEvent e)
        {
          Activate();
        }

        void OnStoryStartedEvent(Story.StartedEvent e)
        {
          this.Activate();
        }

        void OnStoryContinueEvent(Story.ContinueEvent e)
        {
          this.Activate();
        }

        void OnStoryEndedEvent(Story.EndedEvent e)
        {
          this.Activate();
        }

      }

    } 
  }
}