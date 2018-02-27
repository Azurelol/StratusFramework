using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

namespace Stratus
{
  namespace Modules
  {
    namespace InkModule
    {
      /// <summary>
      /// An abstract interface for displaying an ink story in an event-driven way
      /// </summary>
      public abstract class StoryDisplay : StratusBehaviour
      {
        //------------------------------------------------------------------------------------------/
        // Properties
        //------------------------------------------------------------------------------------------/
        public Story story { get; private set; }
        public StoryReader reader { get; private set; }

        //------------------------------------------------------------------------------------------/
        // Fields
        //------------------------------------------------------------------------------------------/
        public bool logging = false;

        //------------------------------------------------------------------------------------------/
        // Events
        //------------------------------------------------------------------------------------------/
        protected abstract void OnStart();
        protected abstract void OnStoryStarted();
        protected abstract void OnStoryEnded();
        protected abstract void OnStoryUpdate(ParsedLine parse, bool visited);
        protected abstract void OnPresentChoices(List<Choice> choices);
        protected abstract void OnChoiceSelected();

        /// <summary>
        /// Initializes the script
        /// </summary>
        void Start()
        {
          Scene.Connect<Story.StartedEvent>(this.OnStoryStartedEvent);
          Scene.Connect<Story.EndedEvent>(this.OnStoryEndedEvent);
          Scene.Connect<Story.UpdateLineEvent>(this.OnStoryUpdateEvent);
          Scene.Connect<Story.PresentChoicesEvent>(this.OnStoryPresentChoicesEvent);
          OnStart();
        }

        //------------------------------------------------------------------------------------------/
        // Events
        //------------------------------------------------------------------------------------------/
        /// <summary>
        /// Received when a story has started
        /// </summary>
        /// <param name="e"></param>
        void OnStoryStartedEvent(Story.StartedEvent e)
        {
          reader = e.reader;
          story = e.story;
          OnStoryStarted();
        }

        /// <summary>
        /// Received when a story has ended
        /// </summary>
        /// <param name="e"></param>
        void OnStoryEndedEvent(Story.EndedEvent e)
        {
          OnStoryEnded();
        }

        /// <summary>
        /// Called upon when a new line is read
        /// </summary>
        /// <param name="e"></param>
        void OnStoryUpdateEvent(Story.UpdateLineEvent e)
        {
          OnStoryUpdate(e.parse, e.visited);
        }

        /// <summary>
        /// Called upon when the current conversation presents choices to the player.
        /// </summary>
        /// <param name="e"></param>
        void OnStoryPresentChoicesEvent(Story.PresentChoicesEvent e)
        {
          OnPresentChoices(e.Choices);
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

          TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
          choiceText.text = text;

          return choice;
        }

        /// <summary>
        /// Presents choices based on a specified prefab containing a button and a horizontal/vertical layout group
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="buttonPrefab"></param>
        /// <param name="panel"></param>
        protected Button[] AddChoices(List<Choice> choices, Button choicePrefab, LayoutGroup choicesPanel)
        {
          Button[] buttons = new Button[choices.Count];

          // For each given choice,
          for (int i = 0; i < choices.Count; ++i)
          {
            Choice choice = choices[i];
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
        public void SelectChoice(Choice choice)
        {
          if (logging)
            Trace.Script(choice.text + " was selected", this);

          // Inform the current conversation of the choice
          var choiceEvent = new Story.SelectChoiceEvent() { story = this.story, reader = this.reader };
          choiceEvent.choice = choice;
          reader.gameObject.Dispatch<Story.SelectChoiceEvent>(choiceEvent);
          Scene.Dispatch<Story.SelectChoiceEvent>(choiceEvent);

          // Now do any extra stuff
          OnChoiceSelected();
        }

        /// <summary>
        /// Called upon to continue the story
        /// </summary>
        public void ContinueStory()
        {
          var continueEvent = new Story.ContinueEvent() { reader = this.reader, story = this.story };
          reader.gameObject.Dispatch<Story.ContinueEvent>(continueEvent);
          Scene.Dispatch<Story.ContinueEvent>(continueEvent);
        }
      }
    } 
  }

}