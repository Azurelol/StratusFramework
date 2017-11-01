using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Text.RegularExpressions;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// An abstract interface for reading an ink story file in an event-driven way
      /// </summary>
      public abstract class StoryReader : MonoBehaviour
      {
        //------------------------------------------------------------------------------------------/
        // Public Fields
        //------------------------------------------------------------------------------------------/
        [Tooltip("Whether to log to the console")]
        public bool logging = false;

        [Header("Options")]
        [Tooltip("Whether this reader will react to scene-wide story events")]
        public bool listeningToScene = false;
        [Tooltip("Allow the story to be restarted if it has ended already when triggered")]
        public bool allowRestart = false;

        [Header("States")]
        [Tooltip("Whether the state of stories should automatically be saved by default")]
        public bool saveStates = true;
        [Tooltip("Whether to save story data when exiting playmode")]
        public bool saveOnExit = false;

        //------------------------------------------------------------------------------------------/
        // Private Fields
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// The current data for this reader
        /// </summary>
        private StorySave storySave = new StorySave();

        //------------------------------------------------------------------------------------------/
        // Properties
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// The current story being managed by this reader. It encapsulates Ink's runtime story
        /// data structure
        /// </summary>
        protected Story story { get; private set; }
        /// <summary>
        /// All persistent stories are tracked here. When a story that was loaded is marked as persistent,
        /// once its ended, we will save its state so next time its asked to be loaded, we will
        /// load from a previous state.
        /// </summary>
        private Dictionary<string, Story> stories { get; set; } = new Dictionary<string, Story>();
        /// <summary>
        /// /The current knot (sub-section) of the story we are on
        /// </summary>
        private string stitch { get; set; }
        /// <summary>
        /// The default name for the story data being serialized
        /// </summary>
        protected virtual string saveFileName => "StoryReaderData";
        /// <summary>
        /// The name of the folder to store the data of this reader at
        /// </summary>
        protected virtual string saveFolder => "Stratus Example";

        //------------------------------------------------------------------------------------------/
        // Virtual Functions
        //------------------------------------------------------------------------------------------/
        protected abstract void OnAwake();
        protected virtual void OnStoryLoaded(Story story) {}
        protected abstract void OnBindExternalFunctions(Story story);
        
        //------------------------------------------------------------------------------------------/
        // Messages
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// Subscribe to common events
        /// </summary>
        private void Awake()
        {
          Subscribe();
          OnLoad(stories);
          OnAwake();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
          if (saveOnExit)
            Save();
        }

        //------------------------------------------------------------------------------------------/
        // Initialization
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// Once a story has been set, loads it. If its's set to save,
        /// it will first look for the story among the specified save data
        /// folder
        /// </summary>
        void InitializeStory()
        {
          // Announce it
          this.gameObject.Dispatch<Story.LoadedEvent>(new Story.LoadedEvent() { story = this.story });
          OnStoryLoaded(story);

          // If the story can't be continued, check if it should be restarted
          if (!story.runtime.canContinue)
            TryRestart();

          // Now start the story
          this.StartStory();
        }

        /// <summary>
        /// Loads a story from file
        /// </summary>
        /// <param name="storyFile"></param>
        public void LoadStory(TextAsset storyFile)
        {
          Story newStory = null;

          // If this story has already been loaded,
          // use the previous state
          if (stories.ContainsKey(storyFile.name))
          {
            if (logging)
              Trace.Script($"{storyFile.name} has already been loaded! Using it!");
            newStory = stories[storyFile.name];
            newStory.LoadState();
          }
          // If the story hasn't been loaded yet
          else
          {
            if (logging)
              Trace.Script($"{storyFile.name} has not been loaded yet. Constructing a new story!");
            newStory = ConstructStory(storyFile);
          }

          // Assign the story
          story = newStory;

          // Start it
          InitializeStory();
        }
        

        /// <summary>
        /// Constructs the ink story runtime object from a given text asset file
        /// </summary>
        Story ConstructStory(TextAsset storyFile)
        {
          Story newStory = new Story();
          newStory.file = storyFile;
          newStory.runtime = new Ink.Runtime.Story(storyFile.text);
          if (!newStory.runtime)
            Trace.Error("Failed to load the story", this, true);

          // Bind external functions to it
          OnBindExternalFunctions(newStory);

          return newStory;
        }

        /// <summary>
        /// Attempt to restart the stry
        /// </summary>
        void TryRestart()
        {
          if (allowRestart)
          {
           if (logging)
              Trace.Script("Restarting the story '" + story.name + "'!", this);

            story.runtime.state.GoToStart();
          }
          else
          {
            if (logging)
              Trace.Script("The story '" + story.name + "' is over!", this);
            return;
          }
        }

        //------------------------------------------------------------------------------------------/
        // Events
        //------------------------------------------------------------------------------------------/ 
        /// <summary>
        /// Connect to common events
        /// </summary>
        void Subscribe()
        {
          this.gameObject.Connect<Story.LoadEvent>(this.OnLoadEvent);
          if (listeningToScene)
            Scene.Connect<Story.LoadEvent>(this.OnLoadEvent);

          this.gameObject.Connect<Story.ContinueEvent>(this.OnContinueEvent);
          this.gameObject.Connect<Story.SelectChoiceEvent>(this.OnSelectChoiceEvent);
          this.gameObject.Connect<Story.RetrieveVariableValueEvent>(this.OnRetrieveVariableValueEvent);
          this.gameObject.Connect<Story.SetVariableValueEvent>(this.OnSetVariableValueEvent);
          this.gameObject.Connect<Story.ObserveVariableEvent>(this.OnObserveVariableEvent);
          this.gameObject.Connect<Story.ObserveVariablesEvent>(this.OnObserveVariablesEvent);
          this.gameObject.Connect<Story.RemoveVariableObserverEvent>(this.OnRemoveVariableObserverEvent);
        }

        void OnLoadEvent(Story.LoadEvent e)
        {
          this.LoadStory(e.storyFile);
        }

        void OnContinueEvent(Story.ContinueEvent e)
        {
          this.ContinueStory();
        }

        void OnSelectChoiceEvent(Story.SelectChoiceEvent e)
        {
          this.SelectChoice(e.choice);
          this.ContinueStory();
        }

        void OnRetrieveVariableValueEvent(Story.RetrieveVariableValueEvent e)
        {
          switch (e.variable.type)
          {
            case Story.ValueType.Integer:
              e.variable.intValue = Story.GetVariableValue<int>(story.runtime, e.variable.name);
              break;
            case Story.ValueType.Boolean:
              e.variable.boolValue = Story.GetVariableValue<bool>(story.runtime, e.variable.name);
              break;
            case Story.ValueType.Float:
              e.variable.floatValue = Story.GetVariableValue<float>(story.runtime, e.variable.name);
              break;
            case Story.ValueType.String:
              e.variable.stringValue = Story.GetVariableValue<string>(story.runtime, e.variable.name);
              break;
          }
        }

        void OnSetVariableValueEvent(Story.SetVariableValueEvent e)
        {
          switch (e.variable.type)
          {
            case Story.ValueType.Integer:
              Story.SetVariableValue<int>(story.runtime, e.variable.name, e.variable.intValue);
              break;
            case Story.ValueType.Boolean:
              Story.SetVariableValue<bool>(story.runtime, e.variable.name, e.variable.boolValue);
              break;
            case Story.ValueType.String:
              Story.SetVariableValue<string>(story.runtime, e.variable.name, e.variable.stringValue);
              break;
            case Story.ValueType.Float:
              Story.SetVariableValue<float>(story.runtime, e.variable.name, e.variable.floatValue);
              break;
          }
        }

        void OnObserveVariableEvent(Story.ObserveVariableEvent e)
        {
          if (logging)
            Trace.Script("Observing " + e.variableName);          
          story.runtime.ObserveVariable(e.variableName, e.variableObserver);
        }

        void OnObserveVariablesEvent(Story.ObserveVariablesEvent e)
        {
          story.runtime.ObserveVariables(e.variableNames, e.variableObserver);
        }

        void OnRemoveVariableObserverEvent(Story.RemoveVariableObserverEvent e)
        {
          story.runtime.RemoveVariableObserver(e.variableObserver);
        }

        //------------------------------------------------------------------------------------------/
        // Methods: Public
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// Sets the value of a variable from the current story
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetVariableValue(string name, object value)
        {
          story.runtime.variablesState[name] = value;
        }

        /// <summary>
        /// Returns the value of a variable from the current story
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetVariableValue(string name)
        {
          return story.runtime.variablesState[name];
        }

        /// <summary>
        /// Requests the reader to save the state of all the stories it's storing so far
        /// </summary>
        public void Save()
        {
          if (logging)
            Trace.Script("Saving...", this);

          // Save the current story
          if (saveStates)
            SaveState(story);

          storySave.currentStory = story;
          OnSave(stories);
        }

        /// <summary>
        /// Request the reader to clear out all its story state data
        /// </summary>
        public void Clear()
        {
          if (logging)
            Trace.Script("Cleared!", this);
          stories.Clear();
          storySave = new StorySave();
          OnClear();
        }

        /// <summary>
        /// Stops the current story
        /// </summary>
        public void Pause()
        {
          EndStory();
        }

        /// <summary>
        /// Resumes the last story played
        /// </summary>
        public void Resume()
        {
          if (storySave.currentStory == null)
          {
            if (logging)
              Trace.Script("No story to resume from!", this);
            return;
          }

          story = storySave.currentStory;
          story.LoadState();
          StartStory(true);
          Trace.Script($"Resuming {story.name}", this);
        }

        //------------------------------------------------------------------------------------------/
        // Methods: Serialization
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// Saves the state of the specified story
        /// </summary>
        /// <param name="story"></param>
        private void SaveState(Story story)
        {
          if (!stories.ContainsKey(story.name))
            stories.Add(story.name, story);

          story.timesRead++;
          stories[story.name].savedState = story.runtime.state.ToJson();          
          //Trace.Script($"Saving {story.name}");
        }

        protected virtual void OnSave(Dictionary<string, Story> stories)
        {
          // From dictionary to list
          List<Story> storyList = new List<Story>();
          foreach (var story in stories)
            storyList.Add(story.Value);
          storySave.stories = storyList;

          // Now save it
          StorySave.Save(storySave, saveFileName, saveFolder);

          if (logging)
            Trace.Script("Saved!");
        }

        protected virtual void OnLoad(Dictionary<string, Story> stories)
        {
          if (StorySave.Exists(saveFileName, saveFolder))
          {
            storySave = StorySave.Load(saveFileName, saveFolder);

            // From list to dictionary!
            foreach (var story in storySave.stories)
            {
              if (logging)
                Trace.Script($"Loaded {story.name}");
              stories.Add(story.name, story);
            }
            if (logging)
              Trace.Script("Loaded!");
          }
        }

        protected virtual void OnClear()
        {
          StorySave.Delete(saveFileName, saveFolder);
        }


        //------------------------------------------------------------------------------------------/
        // Methods: Parsing
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// Starts the current dialog.
        /// </summary>
        void StartStory(bool resume = false)
        {
          if (logging)
            Trace.Script($"The story {story.name} has started!");

          // If a knot has been selected...
          if (story.startingKnot.Length > 0)
          {
            this.JumpToKnot(story.startingKnot);
          }

          // Inform the space that dialog has started
          var startedEvent = new Story.StartedEvent();
          startedEvent.readerObject = this.gameObject;
          this.gameObject.Dispatch<Story.StartedEvent>(startedEvent);
          Scene.Dispatch<Story.StartedEvent>(startedEvent);

          // Update the first line of dialog
          this.ContinueStory(!resume);
        }

        /// <summary>
        /// Updates the current dialog. This will check if the conversation can
        /// be continued.If it can't ,it will then check if there are any choices
        /// to be made.If there aren't, it will end the dialog.
        /// </summary>
        void ContinueStory(bool forward = true)
        {
          // If there is more dialog
          if (story.runtime.canContinue)
          {
            if (forward)
              story.runtime.Continue();
            UpdateCurrentLine();
          }
          // If we are given a choice
          else if (story.runtime.currentChoices.Count > 0)
          {
            PresentChoices();
          }
          // If we are done with the conversation, end story
          else
          {
            this.EndStory();
          }
        }

        /// <summary>
        /// Ends the dialog.
        /// </summary>
        void EndStory()
        {
          if (logging)
            Trace.Script("Ending story!");

          // Dispatch the ended event
          var storyEnded = new Story.EndedEvent();
          this.gameObject.Dispatch<Story.EndedEvent>(storyEnded);
          Scene.Dispatch<Story.EndedEvent>(storyEnded);

          // Save the story
          Save();          
        }

        //------------------------------------------------------------------------------------------/
        // Methods: Story
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// Updates the current line of the story
        /// </summary>
        protected abstract void UpdateCurrentLine();

        /// <summary>
        /// Presents choices at the current story node
        /// </summary>
        void PresentChoices()
        {
          if (logging)
            Trace.Script("Presenting dialog choices!");

          var choicesEvent = new Story.PresentChoicesEvent();
          choicesEvent.Choices = story.runtime.currentChoices;
          Scene.Dispatch<Story.PresentChoicesEvent>(choicesEvent);
        }

        /// <summary>
        /// Selects a choice for the current conversation.
        /// </summary>
        /// <param name="choice">A 0-indexed choice.</param>
        void SelectChoice(int choice)
        {
          this.story.runtime.ChooseChoiceIndex(choice);
        }


        /// <summary>
        /// Selects a choice for the current conversation.
        /// </summary>
        /// <param name="choice">A 0-indexed choice.</param>
        void SelectChoice(Choice choice)
        {
          this.story.runtime.ChooseChoiceIndex(choice.index);
        }

        /// <summary>
        /// Jumps to the specified knot in the story.
        /// </summary>
        /// <param name="knotName">The name of the knot.</param>
        void JumpToKnot(string knotName)
        {
          //Trace.Script("Jumping to the knot '" + knotName + "'", this);
          this.story.runtime.ChoosePathString(knotName + this.stitch);
        }
        /// <summary>
        /// Updates the current stitch
        /// </summary>
        /// <param name="stitchName"></param>
        void UpdateStitch(string stitchName)
        {
          if (stitchName.Length == 0)
            return;

          this.stitch = "." + stitchName;
          if (logging)
            Trace.Script("Updating stitch to '" + stitch + "'", this);
        }       

      }

      /// <summary>
      /// An abstract interface for reading an Ink story file in an event-driven way
      /// <typeparamref name="ParserType"> The parser class to use </typeparamref>/>
      /// </summary>
      public abstract class StoryReader<ParserType> : StoryReader where ParserType : Parser, new()
      {
        /// <summary>
        /// The list of all selected parses for this reader
        /// </summary>
        protected ParserType parser { get; private set; }

        /// <summary>
        /// Invoked in order to configure the parser
        /// </summary>
        /// <param name="parser"></param>
        protected abstract void OnConfigureParser(ParserType parser);

        protected override void OnAwake()
        {
          parser = new ParserType();
          OnConfigureParser(parser);
        }

        public string latestKnot { get; private set; }
        public string latestStitch { get; private set; }

        protected override void UpdateCurrentLine()
        {          
          var line = story.runtime.currentText;
          var tags = story.runtime.currentTags;

          // Check whether this line has been visited before
          bool visited = CheckIfRead();
          if (visited)
            Trace.Script("This line has been visited previously!");

          if (logging)
            Trace.Script($"\"{line}\" ");

          var updateEvent = new Story.UpdateLineEvent(parser.Parse(line, tags), visited);
          Scene.Dispatch<Story.UpdateLineEvent>(updateEvent);
        }

        // @TODO: Not working properly for the very first knot?
        bool CheckIfRead()
        {
          var keys = new List<string>(story.runtime.state.visitCounts.Keys);
          string latest = keys[keys.Count - 1];
          int count = story.runtime.state.visitCounts[latest];
          //Trace.Script($"Latest stitch = {latest},  times read = {count}");
          

          if (count > 1)
            return true;
          return false;
        }



      }

    }
  }
}