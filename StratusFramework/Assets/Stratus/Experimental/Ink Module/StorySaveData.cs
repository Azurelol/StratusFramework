using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// Interface for easily saving the state of an ink runtime story
      /// </summary>
      [SaveData(extension = ".story")]
      public class StorySave : JsonSaveData<StorySave>
      {
        /// <summary>
        /// The saved states of all stories that were loaded by a reader
        /// </summary>
        public List<Story> stories = new List<Story>();

        /// <summary>
        /// The story currently being read
        /// </summary>
        public Story currentStory;


        protected override void OnSave()
        {
          foreach(var story in stories)
          {
            story.filePath = story.file.name;
          }
          currentStory.filePath = currentStory.file.name;
        }

        protected override bool OnLoad()
        {
          foreach(var story in stories)
          {
            story.file = Resources.Load(story.filePath) as TextAsset;
            if (story.file == null)
            {
              Trace.Error($"Failed to load {story.filePath}");
              return false;
            }
          }
          currentStory.file = Resources.Load(currentStory.filePath) as TextAsset;
          if (currentStory.file == null)
          {
            Trace.Error($"Failed to load {currentStory.filePath}");
            return false;
          }

          return true;
        }

      } 

    }
  }
}