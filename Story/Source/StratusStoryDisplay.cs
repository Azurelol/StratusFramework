using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

namespace Stratus.Gameplay.Story
{
	/// <summary>
	/// An abstract interface for displaying an ink story in an event-driven way
	/// </summary>
	public abstract class StratusStoryDisplay : StratusBehaviour
	{
		//------------------------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------------------------/
		public StratusStory story { get; private set; }
		public StratusStoryReader reader { get; private set; }

		//------------------------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------------------------/
		public bool logging = false;

		//------------------------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------------------------/
		protected abstract void OnStoryDisplayStart();
		protected abstract void OnStoryStarted();
		protected abstract void OnStoryEnded();
		protected abstract void OnStoryUpdate(ParsedLine parse, bool visited);
		protected abstract void OnPresentChoices(StratusStoryChoice[] choices);
		protected abstract void OnChoiceSelected();
		protected abstract void SetChoiceViewText(Button choice, string text);

		//------------------------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// Initializes the script
		/// </summary>
		private void Start()
		{
			StratusScene.Connect<StratusStory.StartedEvent>(this.OnStoryStartedEvent);
			StratusScene.Connect<StratusStory.EndedEvent>(this.OnStoryEndedEvent);
			StratusScene.Connect<StratusStory.UpdateLineEvent>(this.OnStoryUpdateEvent);
			StratusScene.Connect<StratusStory.PresentChoicesEvent>(this.OnStoryPresentChoicesEvent);
			OnStoryDisplayStart();
		}

		//------------------------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// Received when a story has started
		/// </summary>
		/// <param name="e"></param>
		void OnStoryStartedEvent(StratusStory.StartedEvent e)
		{
			reader = e.reader;
			story = e.story;
			OnStoryStarted();
		}

		/// <summary>
		/// Received when a story has ended
		/// </summary>
		/// <param name="e"></param>
		void OnStoryEndedEvent(StratusStory.EndedEvent e)
		{
			OnStoryEnded();
		}

		/// <summary>
		/// Called upon when a new line is read
		/// </summary>
		/// <param name="e"></param>
		void OnStoryUpdateEvent(StratusStory.UpdateLineEvent e)
		{
			OnStoryUpdate(e.parse, e.visited);
		}

		/// <summary>
		/// Called upon when the current conversation presents choices to the player.
		/// </summary>
		/// <param name="e"></param>
		void OnStoryPresentChoicesEvent(StratusStory.PresentChoicesEvent e)
		{
			OnPresentChoices(e.choices);
		}

		//------------------------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------------------------/
		/// <summary>
		/// Creates a choice button based on a prefab. It parents it to a layout group automatically as well.
		/// </summary>
		/// <param name="choicePrefab"></param>
		/// <param name="choicesPanel"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		protected Button CreateChoiceView(Button choicePrefab, LayoutGroup choicesPanel, string text)
		{
			Button choice = Instantiate(choicePrefab) as Button;
			choice.transform.SetParent(choicesPanel.transform, false);

			
			SetChoiceViewText(choice, text);
			return choice;
		}

		/// <summary>
		/// Presents choices based on a specified prefab containing a button and a horizontal/vertical layout group
		/// </summary>
		/// <param name="choices"></param>
		/// <param name="buttonPrefab"></param>
		/// <param name="panel"></param>
		protected Button[] AddChoices(StratusStoryChoice[] choices, Button choicePrefab, LayoutGroup choicesPanel)
		{
			Button[] buttons = new Button[choices.Length];

			// For each given choice,
			for (int i = 0; i < choices.Length; ++i)
			{
				StratusStoryChoice choice = choices[i];
				Button button = CreateChoiceView(choicePrefab, choicesPanel, choices[i].text.Trim());
				button.onClick.AddListener(delegate
				{
					SelectChoice(choice);
				});
				buttons[i] = button;
			}
			return buttons;
		}

		/// <summary>
		/// Removes choices from a given choices panel
		/// </summary>
		protected void RemoveChoices(LayoutGroup choicesPanel)
		{
			var choiceButtons = choicesPanel.GetComponentsInChildren<Button>();
			foreach (var choiceButton in choiceButtons)
			{
				Destroy(choiceButton.gameObject);
			}
		}

		/// <summary>
		/// Called upon when a particular choice has been selected
		/// </summary>
		/// <param name="choice"></param>
		public void SelectChoice(StratusStoryChoice choice)
		{
			if (logging)
				StratusDebug.Log(choice.text + " was selected", this);

			// Inform the current conversation of the choice
			var choiceEvent = new StratusStory.SelectChoiceEvent() { story = this.story, reader = this.reader };
			choiceEvent.choice = choice;
			reader.gameObject.Dispatch<StratusStory.SelectChoiceEvent>(choiceEvent);
			StratusScene.Dispatch<StratusStory.SelectChoiceEvent>(choiceEvent);

			// Now do any extra stuff
			OnChoiceSelected();
		}

		/// <summary>
		/// Called upon to continue the story
		/// </summary>
		public void ContinueStory()
		{
			var continueEvent = new StratusStory.ContinueEvent() { reader = this.reader, story = this.story };
			reader.gameObject.Dispatch<StratusStory.ContinueEvent>(continueEvent);
			StratusScene.Dispatch<StratusStory.ContinueEvent>(continueEvent);
		}
	}
}