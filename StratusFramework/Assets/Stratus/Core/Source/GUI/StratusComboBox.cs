using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Stratus
{
	public class StratusComboBox : StratusBehaviour
	{
		//--------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------/
		public Dropdown dropdown;
		public StratusInputField inputField;
		private FilteredStringList searchFilter;
		private bool updateDropdown;

		//--------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------/
		/// <summary>
		/// Provided function that will name the entries from the given list
		/// </summary>
		public Func<string, string> entryFunction { get; set; }
		public string text => this.dropdown.captionText.text;
		public bool hasDisplayOptions => this.dropdown.options.Count > 0;


		//--------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------/
		private void Awake()
		{
			this.SetFilter();

			// Set the initial value oft the input field
			this.OnDropdownChanged(this.dropdown.value);

			// Set callbacks
			this.inputField.onEndEdit.AddListener(this.OnInputFieldEndEdit);
			this.inputField.onValueChanged.AddListener(this.OnInputFieldChanged);
			this.dropdown.onValueChanged.AddListener(this.OnDropdownChanged);
		}

		//--------------------------------------------------------------------------/
		// Procedures
		//--------------------------------------------------------------------------/
		private void OnDropdownChanged(int index)
		{
			this.Log($"Selected = " + this.text);
			this.updateDropdown = false;
			{
				this.Filter(string.Empty);
				this.inputField.text = this.text;
			}
			this.updateDropdown = true;
		}

		private void OnInputFieldChanged(string value)
		{
			if (!this.updateDropdown)
			{
				return;
			}

			this.Filter(value);
			UpdateDropdownDisplay();
		}

		private void OnInputFieldEndEdit(string value)
		{
			this.Log("Ended at " + value);
		}

		public void OnDropdownClicked()
		{
			this.Log("Dropdown clicked!");
		}

		private void ResetInputValue()
		{
		}

		private void SetFilter()
		{
			// If there's no function provided to name the entries, use the default
			if (this.entryFunction == null)
			{
				this.entryFunction = (x => x);
			}

			string[] entries = null;
			if (this.hasDisplayOptions)
			{
				entries = this.dropdown.GetDisplayOptions();
			}

			this.searchFilter = new FilteredStringList(entries);
		}

		private void Filter(string value)
		{

			bool changed = this.searchFilter.UpdateFilter(value);
			if (!changed)
			{
				return;
			}

			this.Log("Filtering " + value);

			// Reset
			this.dropdown.ClearOptions();
			List<string> options = new List<string>(this.searchFilter.displayOptions);
			this.dropdown.AddOptions(options);

			this.dropdown.Hide();
			this.dropdown.RefreshShownValue();
			this.UpdateDropdownDisplay();
		}

		private void UpdateDropdownDisplay()
		{
			this.dropdown.Show();
			this.inputField.ActivateInputField();
		}



	}

}