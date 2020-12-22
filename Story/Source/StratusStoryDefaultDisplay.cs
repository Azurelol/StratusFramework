using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Stratus.Gameplay.Story
{
	public abstract class StratusStoryDefaultDisplay : StratusStoryDisplay
	{
		//------------------------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------------------------/
		[Header("Dialog")]
		[SerializeField]
		protected Image background;
		[SerializeField]
		protected HorizontalLayoutGroup characterPortraitLayout;
		[SerializeField]
		protected TextMeshProUGUI speakerText;
		[SerializeField]
		protected TextMeshProUGUI messageText;

		[Header("Choices")]
		[SerializeField]
		protected Button choicePrefab;
		[SerializeField]
		protected Canvas dialogPanel;
		[SerializeField]
		protected VerticalLayoutGroup choicesPanel;

		//------------------------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------------------------/
		public bool displayChoices
		{
			set
			{
				this.choicesPanel.gameObject.SetActive(value);
				this.dialogPanel.gameObject.SetActive(!value);
			}
		}

		public bool display
		{
			set
			{
				this.dialogPanel.gameObject.SetActive(value);
				this.choicesPanel.gameObject.SetActive(value);
			}
		}

		protected Dictionary<string, Sprite> instantiatedCharacterPortraits { get; set; }

		//------------------------------------------------------------------------------------------/
		// Abstract
		//------------------------------------------------------------------------------------------/
		protected abstract string GetSpeaker(ParsedLine line);
		protected abstract string GetMessage(ParsedLine line);
		protected abstract Sprite GetCharacterPortrait(string character, string tag = null);

		//------------------------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------------------------/
		protected override void OnStoryDisplayStart()
		{
			instantiatedCharacterPortraits = new Dictionary<string, Sprite>();
			this.display = false;
		}

		protected override void OnStoryStarted()
		{
			this.display = true;
			this.displayChoices = false;
		}

		protected override void OnStoryEnded()
		{
			this.OnChoiceSelected();
			this.display = false;
		}

		protected override void OnStoryUpdate(ParsedLine line, bool visited)
		{
			if (!line.isParsed)
			{
				this.speakerText.text = "";
				this.messageText.text = line.line;
			}
			else
			{
				string speaker = GetSpeaker(line);
				if (speaker != null)
				{
					this.speakerText.text = speaker;
					Sprite portrait = GetCharacterPortrait(speaker);
					if (portrait != null)
					{

					}
				}

				string message = GetMessage(line);
				if (message != null)
				{
					this.messageText.text = message;
				}
			}
		}

		protected override void OnPresentChoices(StratusStoryChoice[] choices)
		{
			this.displayChoices = true;
			this.AddChoices(choices, this.choicePrefab, this.choicesPanel);
		}

		protected override void OnChoiceSelected()
		{
			this.displayChoices = false;
			this.RemoveChoices(this.choicesPanel);
		}

		//------------------------------------------------------------------------------------------/
		// Procesures
		//------------------------------------------------------------------------------------------/
		protected void ClearDialog()
		{
			speakerText.text = messageText.text = string.Empty;
		}

		protected override void SetChoiceViewText(Button choice, string text)
		{
			TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
			choiceText.text = text;
		}

		protected void SetBackground(Sprite sprite)
		{
			background.sprite = sprite;
		}
	}

}