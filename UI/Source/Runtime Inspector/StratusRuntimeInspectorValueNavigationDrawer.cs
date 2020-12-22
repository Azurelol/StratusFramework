using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Stratus.UI
{
	public class StratusRuntimeInspectorValueNavigationDrawer
		: StratusRuntimeInspectorDrawer
	{
		[SerializeField]
		private TextMeshProUGUI valueLabel;
		[SerializeField]
		private Button previousValueButton;
		[SerializeField]
		private Button nextValueButton;
		[SerializeField]
		private StratusOrientation inputOrientation = StratusOrientation.Horizontal;
		public override Selectable drawerSelectable { get; }

		private StratusArrayNavigator<string> values { get; set; }
		public override Color bodyColor
		{
			set
			{
				previousValueButton.targetGraphic.color = value;
				nextValueButton.targetGraphic.color = value;
				valueLabel.color = value;
			}
		}

		private static readonly bool reverseBooleanOrder = true;

		private static readonly string[] booleanValues = new string[]
		{
			"On",
			"Off",
		};
		private static readonly string[] booleanValuesReversed = new string[]
		{
			"Off",
			"On",
		};

		public override void Navigate(Vector2 dir)
		{
			switch (inputOrientation)
			{
				case StratusOrientation.Horizontal:
					if (dir.x > 0)
					{
						values.Next();
					}
					else if (dir.x < 0)
					{
						values.Previous();
					}
					break;
				case StratusOrientation.Vertical:
					if (dir.y > 0)
					{
						values.Previous();
					}
					else if (dir.y < 0)
					{
						values.Next();
					}
					break;
			}
		}

		protected override void OnSet(StratusRuntimeInspectorDrawerSettings settings)
		{
			(string[] values, int index) setup = GetValues(settings.field);
			values = new StratusArrayNavigator<string>(setup.values, setup.index, false);
			valueLabel.text = values.values[setup.index];
			values.onIndexChanged += this.OnValueChanged;

			nextValueButton.onClick.RemoveAllListeners();
			previousValueButton.onClick.RemoveAllListeners();
			nextValueButton.onClick.AddListener(values.NavigateToNext);
			previousValueButton.onClick.AddListener(values.NavigateToPrevious);
		}

		private void OnValueChanged(string value, int index)
		{
			valueLabel.text = value;
			switch (settings.field.fieldType)
			{
				case StratusSerializedFieldType.Boolean:
					int booleanMatch = reverseBooleanOrder ? 1 : 0;
					settings.field.value = (index == booleanMatch) ? true : false;
					break;
				case StratusSerializedFieldType.Integer:
					break;
				case StratusSerializedFieldType.Float:
					break;
				case StratusSerializedFieldType.String:
					break;
				case StratusSerializedFieldType.Enum:
					settings.field.value = Enum.Parse(settings.field.type, value);
					break;
			}
		}

		protected virtual (string[] values, int index) GetValues(StratusSerializedField field)
		{
			string[] values = null;
			int index = default;
			switch (field.fieldType)
			{
				case StratusSerializedFieldType.Boolean:
					bool boolean = (bool)field.value;
					if (reverseBooleanOrder)
					{
						boolean = !boolean;
					}
					index = boolean ? 0 : 1;
					values = reverseBooleanOrder ? booleanValuesReversed : booleanValues;
					break;
				case StratusSerializedFieldType.Integer:
					break;
				case StratusSerializedFieldType.Float:
					break;
				case StratusSerializedFieldType.String:
					break;
				case StratusSerializedFieldType.Enum:
					index = (int)field.value;
					values = field.enumValueNames;
					break;
			}
			return (values, index);
		}
	}

}