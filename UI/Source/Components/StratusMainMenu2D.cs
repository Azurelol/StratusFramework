using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Stratus.UI
{
	public abstract class StratusMainMenu2D<T> : StratusCanvasWindow<T>
		where T : StratusCanvasWindow<T>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private Image backgroundComponent;
		[SerializeField]
		private StratusLayoutTextElementGroupBehaviour optionsGroup;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The prompt emitted for quitting an application
		/// </summary>
		protected StratusDialogConfirmationRequest quitApplicationPrompt;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnMainMenuAwake();
		protected abstract Sprite GetBackground();
		protected abstract StratusLabeledAction[] GetOptions();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowAwake()
		{
			SetBackgroundImage();
			GenerateDefaultPrompts();
			GenerateMenu();
			OnMainMenuAwake();
		}

		protected override void OnWindowOpen()
		{
			Select();
		}

		protected override void OnWindowClose()
		{
		}

		private void OnValidate()
		{
			SetBackgroundImage();
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void GenerateMenu()
		{
			List<StratusLabeledAction> options = new List<StratusLabeledAction>();
			options.AddRange(this.GetOptions());
			options.Add(new StratusLabeledAction("Quit", PromptQuitApplication));

			List<StratusLayoutTextElementEntry> entries = new List<StratusLayoutTextElementEntry>();
			foreach(var option in options)
			{
				entries.Add(new StratusLayoutTextElementEntry(option.label, option.action));
			}
			optionsGroup.Set(entries);
		}

		private void GenerateDefaultPrompts()
		{
			quitApplicationPrompt = GenerateQuitPrompt();
			quitApplicationPrompt.onClose += Select;
		}

		protected virtual StratusDialogConfirmationRequest GenerateQuitPrompt()
		{
			return new StratusDialogConfirmationRequest("Quit",
				"Are you sure you want to quit?",
				(confirm) => 
				{ 
					if (confirm) QuitApplication(); }
				); ;
		}

		public override void Select()
		{
			optionsGroup.Select();
		}

		private void SetBackgroundImage()
		{
			if (backgroundComponent != null)
			{
				backgroundComponent.sprite = GetBackground();
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void PromptQuitApplication()
		{
			quitApplicationPrompt.Submit();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}


	}

}