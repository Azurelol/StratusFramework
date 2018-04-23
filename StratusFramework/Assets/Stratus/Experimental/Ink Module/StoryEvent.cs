using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      public class StoryEvent : Triggerable
      {        
        public enum Scope
        {
          [Tooltip("The event will be sent to the specified target")]
          Target,
          [Tooltip("The event will be sent to the whole scene")]
          Scene
        }

        //------------------------------------------------------------------------------------------/
        // Public Fields
        //------------------------------------------------------------------------------------------/
        [Header("Story")]
        [Tooltip("The ink story file to play in .json format")]
        public TextAsset storyFile;
        [Tooltip("The starting knot")]
        public string knot;
        [Tooltip("Whether to restart the story if ended")]
        public bool restart = false;
        [Tooltip("Whether this story should be queued, if there's one currently running it will be read after")]
        public bool queue = false;
        [Tooltip("How long to wait to play the story, if queued")]
        [DrawIf(nameof(StoryEvent.queue), true, ComparisonType.Equals)]
        public float queueDelay = 0.0f;

        [Tooltip("The target for this event")]
        public Scope scope = Scope.Scene;

        [DrawIf("scope", Scope.Scene, ComparisonType.NotEqual, PropertyDrawingType.DontDraw)]
        [Tooltip("The reader to trigger")]
        public StoryReader reader;

        //------------------------------------------------------------------------------------------/
        // Properties
        //------------------------------------------------------------------------------------------/
        private Story.LoadEvent storyEvent => new Story.LoadEvent()
        {
          storyFile = this.storyFile,
          knot = this.knot,
          queue = this.queue,
          restart = this.restart,
          queueDelay = this.queueDelay
        };

        public override string automaticDescription
        {
          get
          {
            if (storyFile)
            {
              string value = $"Load {storyFile.name}";
              if (queue) value += " (Queued)";
              return value;
            }
            return string.Empty;
          }
        }

        //------------------------------------------------------------------------------------------/
        // Messages
        //------------------------------------------------------------------------------------------/
        protected override void OnAwake()
        {
        }

        protected override void OnReset()
        {

        }

        protected override void OnTrigger()
        {
          switch (scope)
          {
            case Scope.Target:
              reader.gameObject.Dispatch<Story.LoadEvent>(storyEvent);
              break;
            case Scope.Scene:
              Scene.Dispatch<Story.LoadEvent>(storyEvent);
              break;
            default:
              break;
          }

        }
      }
    } 
  }

}