using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Stratus
{
	public class StratusComboBox : StratusBehaviour
	{
		//--------------------------------------------------------------------------/
		// Declaration
		//--------------------------------------------------------------------------/
		public class SelectionEvent : UnityEvent<string>
		{
		}

		//--------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------/
		public StratusDropdown dropdown;
		public StratusInputField inputField;
		private FilteredStringList searchFilter;
		private bool lockInputField, lockDropdownSelectionUpdate;


		private bool settingIndex;
		private Dictionary<string, int> entryIndex;

		//--------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------/
		public string currentValue { get; private set; }
		public Func<string, string> entryFunction { get; set; }
		public string selectedText => this.dropdown.displayedValue;
		public bool hasDisplayOptions => this.dropdown.options.Count > 0;
		public string displayText
		{
			get { return inputField.text; }
			set { inputField.text = value; }
		}
		public bool hasFilter { get { return searchFilter.hasFilter; } }
		public SelectionEvent onValueChanged { get; private set; }

		//--------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------/
		private void Awake()
		{
			// Set the filter
			this.SetFilter();

			// Set callbacks
			this.onValueChanged = new SelectionEvent();
			this.dropdown.onValueChanged.AddListener(this.OnDropdownChanged);
			this.inputField.onValueChanged.AddListener(this.OnInputFieldChanged);
		}

		//--------------------------------------------------------------------------/
		// Events
		//--------------------------------------------------------------------------/
		private void OnDropdownChanged(int index)
		{
			if (settingIndex)
				return;

			UpdateInputField(this.selectedText);
		}

		private void OnInputFieldChanged(string value)
		{
			if (this.lockInputField)
				return;

			this.Filter(value);
		}

		private void UpdateInputField(string text)
		{
			this.lockInputField = true;
			{
				this.currentValue = text;
				this.displayText = this.currentValue;
				this.Filter(string.Empty);
			}
			this.lockInputField = false;
		}

		//--------------------------------------------------------------------------/
		// Procedures
		//--------------------------------------------------------------------------/
		private void SetFilter()
		{
			// If there's no function provided to name the entries, use the default
			if (this.entryFunction == null)
			{
				this.entryFunction = (x => x);
			}

			// Use dropdown values
			if (this.hasDisplayOptions)
			{
				SetFilter(this.dropdown.GetDisplayOptions());
			}


			// Set the initial value oft the input field			
			this.OnDropdownChanged(this.dropdown.value);
		}

		private void SetFilter(string[] entries)
		{
			this.entryIndex = new Dictionary<string, int>();
			for(int i = 0; i < entries.Length; ++i)
			{
				this.entryIndex.Add(entries[i], i);
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


			// Reset
			this.dropdown.ClearOptions();			
			List<string> options = new List<string>();

			if (hasFilter)
			{
				options.Add("<color=blue>" + currentValue + "</color>");
				foreach(var entry in this.searchFilter.displayOptions)
				{
					if (entry.Equals(currentValue))
						continue;
					options.Add(entry);
				}
			}
			else
			{
				options.AddRange(this.searchFilter.displayOptions);

			}

			

			this.dropdown.AddOptions(options);			
			this.Log("Filter: " + value + " (" + searchFilter.currentEntryCount + ")");
			StartCoroutine(UpdateDisplay());
		}

		private IEnumerator UpdateDisplay()
		{
			this.dropdown.Hide();
			//yield return new WaitForEndOfFrame();
			this.dropdown.RefreshShownValue();
			if (hasFilter)
				this.SetDropdownIndex(0);
			else
				SetDropdownIndex(entryIndex[this.currentValue]);
			yield return new WaitForEndOfFrame();
			this.SelectInput();
			
		}


		public void SelectInput()
		{
			this.inputField.ActivateInputField();
			//Log("Activating input..");
		}


		private void SetDropdownIndex(int index)
		{
			settingIndex = true;
			this.dropdown.value = index;
			settingIndex = false;
		}



	}

}