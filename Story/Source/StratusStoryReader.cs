using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Text.RegularExpressions;

namespace Stratus.Gameplay.Story
{
	/// <summary>
	/// An abstract interface for reading an ink story file in an event-driven way
	/// </summary>
	public abstract class StratusStoryReader : StratusBehaviour
	{
		//------------------------------------------------------------------------------------------/
		// Public Fields
		//------------------------------------------------------------------------------------------/
		[Header("Options")]
		/// <summary>
		/// Whether this reader will react to scene-wide story events
		/// </summary>
		[Tooltip("Whether this reader will react to scene-wide story events")]
		public bool listeningToScene = false;
		/// <summary>
		/// Whether to automatically restart an ended story on load
		/// </summary>
		[Tooltip("Whether to automatically restart an ended story on load")]
		public bool automaticRestart = false;
		/// <summary>
		/// Whether story events should be queued, for one to play after the other
		/// </summary>
		[Tooltip("Whether story events should be queued, for one to play after the other")]
		public bool queueStories = true;

		[Header("Debug")]
		/// <summary>
		/// Whether to log to the console
		/// </summary>
		[Tooltip("Whether to log to the console")]
		public bool debug = false;
		/// <summary>
		/// The color used for debug output
		/// </summary>    
		[DrawIf(nameof(StratusStoryReader.debug), true, ComparisonType.Equals)]
		public Color debugTextColor = Color.white;
		/// <summary>
		/// What area of the screen to use to draw the debug content
		/// </summary>
		[DrawIf(nameof(StratusStoryReader.debug), true, ComparisonType.Equals)]
		public StratusGUI.Anchor debugArea = StratusGUI.Anchor.TopRight;
		/// <summary>
		/// Whether the state of stories should automatically be saved by default
		/// </summary>
		[Header("States")]
		[Tooltip("Whether the state of stories should automatically be saved by default")]
		public bool saveStates = true;
		/// <summary>
		/// Whether to save the story when its being ended
		/// </summary>
		[Tooltip("Whether to save story data when exiting playmode")]
		public bool saveOnEnd = true;
		/// <summary>
		/// Whether to save story data when exiting playmode
		/// </summary>
		[Tooltip("Whether to save story data when exiting playmode")]
		public bool saveOnExit = false;
		/// <summary>
		/// Whether state information should be cleared on a restart. If a story has been ended 
		/// and you try to load it again, you likely want this checked.
		/// </summary>
		[Tooltip("Whether state information should be cleared on a restart. If a story has been ended " +
				"and you try to load it again, you likely want this checked.")]
		public bool clearStateOnRestart = true;

		//------------------------------------------------------------------------------------------/
		// Private Fields
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// The current data for this reader
		/// </summary>
		private StratusStorySave storySave = new StratusStorySave();
		/// <summary>
		/// The queue of stories to be played
		/// </summary>
		private Queue<StratusStory.LoadEvent> storyQueue = new Queue<StratusStory.LoadEvent>();


		//------------------------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// The current story being managed by this reader. It encapsulates Ink's runtime story
		/// data structure
		/// </summary>
		protected StratusStory story { get; private set; }
		/// <summary>
		/// All persistent stories are tracked here. When a story that was loaded is marked as persistent,
		/// once its ended, we will save its state so next time its asked to be loaded, we will
		/// load from a previous state.
		/// </summary>
		private Dictionary<string, StratusStory> stories { get; set; } = new Dictionary<string, StratusStory>();
		/// <summary>
		/// /The current knot (sub-section) of the story we are on
		/// </summary>
		private string stitch { get; set; }
		/// <summary>
		/// The default name for the story data being serialized
		/// </summary>
		protected virtual string saveFileName => $"{GetType().Name}";
		/// <summary>
		/// The name of the folder to store the data of this reader at
		/// </summary>
		protected virtual string saveFolder => "Stratus";
		/// <summary>
		/// Whether this story reader is currently reading a story
		/// </summary>
		public bool currentlyReading { get; private set; }
		/// <summary>
		/// The name of the latest knot
		/// </summary>
		public string latestKnot => story.latestKnot;
		/// <summary>
		/// The name of the latest stitch
		/// </summary>
		public string latestStitch { get; private set; }
		/// <summary>
		/// String representation of the location where teh story currently is
		/// </summary>
		private string currentPath => story.runtime.state.currentPathString;

		//------------------------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------------------------/
		public event Action<StratusStory> onStoryStarted;
		public event Action<StratusStory> onStoryEnded;

		//------------------------------------------------------------------------------------------/
		// Virtual Functions
		//------------------------------------------------------------------------------------------/
		protected abstract void OnAwake();
		protected virtual void OnStoryLoaded(StratusStory story) { }
		protected abstract void OnBindExternalFunctions(StratusStory story);

		//------------------------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------------------------/
		private void Awake()
		{
			Subscribe();
			OnAwake();
			Load(stories);
		}
		private void OnDestroy()
		{
			if (saveOnExit)
			{
				Save();
			}
		}

		private void OnGUI()
		{
			if (debug)
			{
				DrawDebug();
			}
		}

		//------------------------------------------------------------------------------------------/
		// Initialization
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// Loads a story from a story file, restarting depending on the default automatic setting
		/// </summary>
		/// <param name="storyFile"></param>
		/// <param name="restart"></param>
		public void LoadStory(TextAsset storyFile)
		{
			//bool restart = !story.runtime.canContinue && automaticRestart;
			LoadStory(storyFile, automaticRestart, null);
		}

		/// <summary>
		/// Loads a story from a story file, optionally restarting it from the beginning
		/// </summary>
		/// <param name="storyFile"></param>
		/// <param name="restart"></param>
		public void LoadStory(TextAsset storyFile, bool restart = false)
		{
			LoadStory(storyFile, restart, null);
		}

		/// <summary>
		/// Loads a story from file, jumping to the specified knot
		/// </summary>
		/// <param name="storyFile"></param>
		/// <param name="knot"></param>
		public void LoadStory(TextAsset storyFile, string knot)
		{
			LoadStory(storyFile, false, knot);
		}

		/// <summary>
		/// Loads a story from file
		/// </summary>
		/// <param name="storyFile"></param>
		private void LoadStory(TextAsset storyFile, bool restart = false, string knot = null)
		{
			StratusStory newStory = null;

			// If this story has already been loaded, use the previous state
			bool previouslyLoaded = stories.ContainsKey(storyFile.name);

			if (previouslyLoaded)
			{
				if (debug)
					StratusDebug.Log($"{storyFile.name} has already been loaded! Using the previous state.");
				newStory = stories[storyFile.name];
				LoadState(newStory);
			}
			// If the story hasn't been loaded yet
			else
			{
				if (debug)
					StratusDebug.Log($"{storyFile.name} has not been loaded yet. Constructing a new state.");
				newStory = ConstructStory(storyFile);
			}

			// Assign the story
			story = newStory;

			// If a knot was provided
			if (knot != null && knot.Length > 0)
			{
				if (!story.runtime.canContinue)
				{
					if (automaticRestart)
						Restart(clearStateOnRestart);
					else
						StratusDebug.LogError($"The story {story.name} has already been ended, thus we can't jump to the knot!", this);
				}
				JumpToKnot(knot);
			}
			else if (restart || automaticRestart)
			{
				Restart(clearStateOnRestart);
			}


			// Announce that we are loding the story
			var loadedEvent = new StratusStory.LoadedEvent() { reader = this, story = this.story };
			this.gameObject.Dispatch<StratusStory.LoadedEvent>(loadedEvent);
			StratusScene.Dispatch<StratusStory.LoadedEvent>(loadedEvent);

			// Invoke any subclass callbacks
			OnStoryLoaded(story);

			// Now start the story
			// If the story was previously loaded, we need not start from a new line
			this.StartStory(previouslyLoaded && story.started);
		}

		/// <summary>
		/// Constructs the ink story runtime object from a given text asset file
		/// </summary>
		StratusStory ConstructStory(TextAsset storyFile)
		{
			StratusStory newStory = new StratusStory();
			newStory.file = storyFile;
			newStory.runtime = new Ink.Runtime.Story(storyFile.text);
			if (!newStory.runtime)
			{
				this.LogError("Failed to load the story");
			}

			// Bind external functions to it
			OnBindExternalFunctions(newStory);

			return newStory;
		}

		/// <summary>
		/// Starts the story
		/// </summary>
		void GoToStart()
		{
			if (debug)
			{
				this.Log($"Navigating to the start of the story {story.name}");
			}
			story.runtime.ResetCallstack();
		}

		/// <summary>
		/// Attempt to restart the story back to its initial state
		/// </summary>
		void Restart(bool clearState)
		{
			if (debug)
			{
				this.Log("Restarting the state for the story '" + story.name + "'");
			}

			if (clearState)
			{
				story.runtime.ResetState();
			}
			else
			{
				story.runtime.GoToStart();
			}

			story.started = false;
		}

		//------------------------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------------------------/ 
		/// <summary>
		/// Connect to common events
		/// </summary>
		void Subscribe()
		{
			this.gameObject.Connect<StratusStory.LoadEvent>(this.OnLoadEvent);

			if (listeningToScene)
			{
				StratusScene.Connect<StratusStory.LoadEvent>(this.OnLoadEvent);
			}

			this.gameObject.Connect<StratusStory.ContinueEvent>(this.OnContinueEvent);
			this.gameObject.Connect<StratusStory.SelectChoiceEvent>(this.OnSelectChoiceEvent);
			this.gameObject.Connect<StratusStory.RetrieveVariableValueEvent>(this.OnRetrieveVariableValueEvent);
			this.gameObject.Connect<StratusStory.SetVariableValueEvent>(this.OnSetVariableValueEvent);
			this.gameObject.Connect<StratusStory.ObserveVariableEvent>(this.OnObserveVariableEvent);
			this.gameObject.Connect<StratusStory.ObserveVariablesEvent>(this.OnObserveVariablesEvent);
			this.gameObject.Connect<StratusStory.RemoveVariableObserverEvent>(this.OnRemoveVariableObserverEvent);
		}

		void OnLoadEvent(StratusStory.LoadEvent e)
		{
			// If we are currently in the middle of a story, have allowed stories to be queued, and this story
			// requests to be queued...
			if (currentlyReading && queueStories && e.queue)
			{
				if (debug)
				{ 
					this.Log($"Queued up <i>{e.storyFile.name}</i>");
				}
				storyQueue.Enqueue(e);
			}
			// Otherwise take over the current story
			else
				this.LoadStory(e.storyFile, e.restart, e.knot);
		}

		void OnContinueEvent(StratusStory.ContinueEvent e)
		{
			this.ContinueStory();
		}

		void OnSelectChoiceEvent(StratusStory.SelectChoiceEvent e)
		{
			this.SelectChoice(e.choice.index);
			this.ContinueStory();
		}

		void OnRetrieveVariableValueEvent(StratusStory.RetrieveVariableValueEvent e)
		{
			switch (e.variable.type)
			{
				case StratusStory.Types.Integer:
					e.variable.intValue = StratusStory.GetVariableValue<int>(story.runtime, e.variable.name);
					break;
				case StratusStory.Types.Boolean:
					e.variable.boolValue = StratusStory.GetVariableValue<bool>(story.runtime, e.variable.name);
					break;
				case StratusStory.Types.Float:
					e.variable.floatValue = StratusStory.GetVariableValue<float>(story.runtime, e.variable.name);
					break;
				case StratusStory.Types.String:
					e.variable.stringValue = StratusStory.GetVariableValue<string>(story.runtime, e.variable.name);
					break;
			}
		}

		void OnSetVariableValueEvent(StratusStory.SetVariableValueEvent e)
		{
			switch (e.variable.type)
			{
				case StratusStory.Types.Integer:
					StratusStory.SetVariableValue<int>(story.runtime, e.variable.name, e.variable.intValue);
					break;
				case StratusStory.Types.Boolean:
					StratusStory.SetVariableValue<bool>(story.runtime, e.variable.name, e.variable.boolValue);
					break;
				case StratusStory.Types.String:
					StratusStory.SetVariableValue<string>(story.runtime, e.variable.name, e.variable.stringValue);
					//StratusDebug.Log($"Setting variable {e.variable.name} to {e.variable.stringValue}");
					break;
				case StratusStory.Types.Float:
					StratusStory.SetVariableValue<float>(story.runtime, e.variable.name, e.variable.floatValue);
					break;
			}
		}

		void OnObserveVariableEvent(StratusStory.ObserveVariableEvent e)
		{
			if (debug)
			{
				this.Log("Observing " + e.variableName);
			}
			story.runtime.ObserveVariable(e.variableName, e.variableObserver);
		}

		void OnObserveVariablesEvent(StratusStory.ObserveVariablesEvent e)
		{
			story.runtime.ObserveVariables(e.variableNames, e.variableObserver);
		}

		void OnRemoveVariableObserverEvent(StratusStory.RemoveVariableObserverEvent e)
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
			if (debug)
			{
				this.Log($"Saving. story {story}");
			}

			// Save the current story
			if (saveStates)
			{
				this.Log($"Saving current state of the story {story}...");
				SaveState(story);
			}

			storySave.currentStory = story;
			Save(stories);
		}

		/// <summary>
		/// Request the reader to clear out all its story state data
		/// </summary>
		public void Clear()
		{
			if (debug)
			{
				this.Log("Cleared!");
			}

			stories.Clear();
			storySave = new StratusStorySave();
			OnClear();
		}

		/// <summary>
		/// Stops the current story
		/// </summary>
		public void Stop()
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
				if (debug)
				{
					this.LogWarning("No story to resume from!");
				}
				return;
			}

			story = storySave.currentStory;
			LoadState(story);
			StartStory(true);

			if (debug)
			{
				this.Log($"Resuming {story.name}");
			}
		}

		private void LoadState(StratusStory story)
		{
			// If we are constructing the runtime
			if (!story.runtime)
			{
				story.runtime = new Ink.Runtime.Story(story.file.text);
				OnBindExternalFunctions(story);
			}

			story.runtime.state.LoadJson(story.savedState);
		}

		//------------------------------------------------------------------------------------------/
		// Methods: Serialization
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// Saves the state of the specified story
		/// </summary>
		/// <param name="story"></param>
		private void SaveState(StratusStory story)
		{
			if (!stories.ContainsKey(story.name))
				stories.Add(story.name, story);

			story.timesRead++;
			stories[story.name].savedState = story.runtime.state.ToJson();
			//Trace.Script($"Saving {story.name}");
		}

		protected virtual void Save(Dictionary<string, StratusStory> stories)
		{
			// From dictionary to list
			List<StratusStory> storyList = new List<StratusStory>();
			foreach (var story in stories)
				storyList.Add(story.Value);
			storySave.stories = storyList;

			// Now save it
			StratusStorySaveSystem.instance.Save(storySave);

			if (debug)
			{
				StratusDebug.Log($"Saved {StratusStorySaveSystem.ComposePath(saveFileName, saveFolder)}");

			}
		}

		protected virtual void Load(Dictionary<string, StratusStory> stories)
		{
			if (StratusStorySaveSystem.Exists(saveFileName, saveFolder))
			{
				storySave = StratusStorySaveSystem.instance.Load(saveFileName, saveFolder);

				// From list to dictionary!
				foreach (var story in storySave.stories)
				{
					if (debug)
						StratusDebug.Log($"Loaded {story.name}");
					stories.Add(story.name, story);
				}
				//if (debug)
				//  Trace.Script("Loaded!");
			}
		}

		protected virtual void OnClear()
		{
			if (!StratusStorySaveSystem.instance.Delete(saveFileName, saveFolder))
			{
				StratusDebug.LogError("Failed to delete save file!", this);
			}
		}

		private void GetLatestKnot()
		{
			string latestKnot = story.runtime.state.currentPathString != null ? story.runtime.state.currentPathString : string.Empty;
			if (latestKnot != string.Empty)
				story.latestKnot = latestKnot;

			//if (logging)
			//  Trace.Script($"Latest knot = {latestKnot}");
		}

		//------------------------------------------------------------------------------------------/
		// Methods: Parsing
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// Starts the current dialog.
		/// </summary>
		void StartStory(bool resume = false)
		{
			// If a knot has been selected...
			if (story.startingKnot.Length > 0)
			{
				this.JumpToKnot(story.startingKnot);
			}

			// Inform the space that dialog has started
			var startedEvent = new StratusStory.StartedEvent() { reader = this, story = story };

			// Dispatch to this gameobject and the scene
			this.gameObject.Dispatch<StratusStory.StartedEvent>(startedEvent);
			StratusScene.Dispatch<StratusStory.StartedEvent>(startedEvent);

			onStoryStarted?.Invoke(story);

			currentlyReading = true;

			// Update the first line of dialog
			this.ContinueStory(!resume);

			story.started = true;

			if (debug)
			{
				StratusDebug.Log($"The story {story.name} has started at the knot '{latestKnot}'");
			}
		}

		/// <summary>
		/// Updates the current dialog. This will check if the conversation can
		/// be continued. If it can't ,it will then check if there are any choices
		/// to be made.If there aren't, it will end the dialog.
		/// </summary>
		private void ContinueStory(bool forward = true)
		{
			// If there is more dialog
			if (story.runtime.canContinue)
			{
				if (forward)
					story.runtime.Continue();

				// Retrieves the latest knot in the story
				GetLatestKnot();

				// Get the next line
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
		private void EndStory()
		{
			if (debug)
			{
				this.Log($"The story {story.name} has ended at the knot '{story.latestKnot}'");
			}

			// Dispatch the ended event
			var storyEnded = new StratusStory.EndedEvent() { reader = this, story = this.story };
			this.gameObject.Dispatch<StratusStory.EndedEvent>(storyEnded);
			StratusScene.Dispatch<StratusStory.EndedEvent>(storyEnded);
			onStoryEnded?.Invoke(story);

			// Save the story
			if (saveOnEnd)
			{
				Save();
			}

			// We are no longer reading a story
			currentlyReading = false;

			// If we are queuing stories and there's one queueud up, let's start it
			if (queueStories && storyQueue.Count > 0)
			{
				QueueNextStory();
			}
		}

		private void QueueNextStory()
		{
			var e = storyQueue.Dequeue();

			if (debug)
			{
				this.Log($"<i>{e.storyFile.name}</i> to be played in {e.queueDelay} seconds");
			}

			var seq = StratusActions.Sequence(this);
			StratusActions.Delay(seq, e.queueDelay);
			StratusActions.Call(seq, () => LoadStory(e.storyFile, e.restart, e.knot));
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
			if (debug)
			{
				this.Log("Presenting dialog choices!");
			}

			var choicesEvent = new StratusStory.PresentChoicesEvent();
			choicesEvent.choices = story.runtime.currentChoices.ToArray(x => new StratusStoryChoice(x));
			StratusScene.Dispatch<StratusStory.PresentChoicesEvent>(choicesEvent);
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
		/// Jumps to the specified knot in the story.
		/// </summary>
		/// <param name="knotName">The name of the knot.</param>
		void JumpToKnot(string knotName)
		{
			story.latestKnot = knotName;
			if (debug)
			{
				this.Log("Jumping to the knot '" + knotName + "'");
			}
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
			if (debug)
			{
				this.Log("Updating stitch to '" + stitch + "'");
			}
		}

		/// <summary>
		/// Checks whether the latest knot has been visited before
		/// </summary>
		/// <returns></returns>
		protected bool CheckIfKnotVisited()
		{
			return story.runtime.HasCurrentKnotBeenVisited();
		}

		/// <summary>
		/// Saves the variables of the current story
		/// </summary>
		public void SaveVariables()
		{
			this.Log("Variables:");
			foreach (var variable in story.runtime.variablesState)
			{
				this.Log($"Variable {variable}");
			}
		}

		private void DrawDebug()
		{
			if (currentlyReading)
			{
				string content = $"Story: {story.name}";
				content += $"\nCurrent Text: {story.runtime.currentText}";
				if (currentPath != null)
				{
					content += $"Current Path: {currentPath}";
				}

				GUIContent msg = StratusGUI.Content(content, 9, debugTextColor);
				StratusGUI.GUILayoutArea(debugArea, StratusGUI.quarterScreen, (Rect rect) =>
				{
					GUILayout.Label(msg, StratusGUIStyles.skin.label);
				});
			}
		}

	}


	/// <summary>
	/// An abstract interface for reading an Ink story file in an event-driven way
	/// <typeparamref name="ParserType"> The parser class to use </typeparamref>/>
	/// </summary>
	public abstract class StratusStoryReader<ParserType> : StratusStoryReader where ParserType : Parser, new()
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

		protected override void UpdateCurrentLine()
		{
			var line = story.runtime.currentText;
			var tags = story.runtime.currentTags;

			// Check whether this line has been visited before
			bool visited = CheckIfKnotVisited();

			if (visited && debug)
				StratusDebug.Log("This knot has been visited previously!");
			if (debug)
				StratusDebug.Log($"\"{line}\" ");

			var updateEvent = new StratusStory.UpdateLineEvent(parser.Parse(line, tags), visited);
			StratusScene.Dispatch<StratusStory.UpdateLineEvent>(updateEvent);
		}

		// @TODO: Not working properly for the very first knot?




	}

}