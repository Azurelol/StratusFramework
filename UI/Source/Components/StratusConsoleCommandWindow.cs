using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Stratus.UI
{
	public class StratusConsoleCommandWindow : StratusCanvasWindow<StratusConsoleCommandWindow>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private StratusInputBinding toggle = new StratusInputBinding(KeyCode.BackQuote);
		[SerializeField]
		private StratusInputBinding submit = new StratusInputBinding(KeyCode.Return);
		[SerializeField]
		private TMP_InputField inputField = null;
		[SerializeField]
		private TMP_Text historyText = null;
		[SerializeField]
		private StratusLayoutTextElementGroupBehaviour matchesLayout = null;

		[SerializeField]
		private Color submitColor = StratusGUIStyles.Colors.azure;
		[SerializeField]
		private Color resultColor = Color.magenta;
		[SerializeField]
		private Color warningColor = Color.yellow;
		[SerializeField]
		private Color errorColor = Color.red;		

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public string input
		{
			get => this.inputField.text;
			set => this.inputField.text = value;
		}

		public string history
		{
			get => this.historyText.text;
			set => this.historyText.text = value;
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowAwake()
		{
			StratusConsoleCommand.onEntry += this.OnConsoleCommandEntry;
			this.history = string.Empty;
			PopulateMatches();
		}

		protected override void OnWindowClose()
		{
		}

		protected override void OnWindowOpen()
		{
			ResetInputField();
		}

		private void Update()
		{
			if (this.toggle.isDown)
			{
				this.Toggle();
				if (this.open)
				{
					ResetInputField();
				}
			}

			if (!this.open)
			{
				return;
			}

			if (this.submit.isDown && this.input.IsValid())
			{
				this.Submit();
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Submits the given input
		/// </summary>
		/// <param name="command"></param>
		public void Submit(string command)
		{
			this.input = command;
			this.Submit();
		}

		/// <summary>
		/// Submits the current input
		/// </summary>
		public void Submit()
		{
			StratusConsoleCommand.Submit(this.input);
			this.ResetInputField();
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void ResetInputField()
		{
			this.inputField.ActivateInputField();
			this.inputField.text = string.Empty;
		}

		private void SetInputField(string text)
		{
			this.inputField.SetTextWithoutNotify(text);
		}

		private void OnConsoleCommandEntry(StratusConsoleCommand.History.Entry e)
		{
			switch (e.type)
			{
				case StratusConsoleCommand.History.EntryType.Result:
				case StratusConsoleCommand.History.EntryType.Warning:
				case StratusConsoleCommand.History.EntryType.Error:
					this.history = this.history.AppendLine($"[{e.timestamp.ToRichText(Color.green)}] {e.text.ToRichText(GetEntryColor(e.type))}");
					break;
			}
		}

		private void PopulateMatches()
		{
			if (matchesLayout != null)
			{
				List<StratusLayoutTextElementEntry> elements = new List<StratusLayoutTextElementEntry>();
				foreach(var command in StratusConsoleCommand.commands)
				{
					elements.Add(new StratusLayoutTextElementEntry(command.name, () => SetInputField(command.name)));
				}
				matchesLayout.Set(elements);
			}
		}

		private Color GetEntryColor(StratusConsoleCommand.History.EntryType entryType)
		{
			switch (entryType)
			{
				case StratusConsoleCommand.History.EntryType.Submit:
					return this.submitColor;
				case StratusConsoleCommand.History.EntryType.Result:
					return this.resultColor;
				case StratusConsoleCommand.History.EntryType.Warning:
					return this.warningColor;
				case StratusConsoleCommand.History.EntryType.Error:
					return this.errorColor;
				default:
					return Color.white;
			}
			throw new NotImplementedException($"Unsupported type {entryType}");
		}

		private void UpdateMatches()
		{

		}

	}
}