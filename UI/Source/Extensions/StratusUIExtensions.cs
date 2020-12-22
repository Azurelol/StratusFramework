using System.Linq;
using UnityEngine.UI;

// https://forum.unity.com/threads/change-the-value-of-a-toggle-without-triggering-onvaluechanged.275056/

namespace Stratus
{
	public static partial class Extensions
	{
		private static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
		public static void SetValue(this Slider instance, float value)
		{
			Slider.SliderEvent originalEvent = instance.onValueChanged;
			instance.onValueChanged = emptySliderEvent;
			instance.value = value;
			instance.onValueChanged = originalEvent;
		}

		private static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();
		public static void SetValue(this Toggle instance, bool value)
		{
			Toggle.ToggleEvent originalEvent = instance.onValueChanged;
			instance.onValueChanged = emptyToggleEvent;
			instance.isOn = value;
			instance.onValueChanged = originalEvent;
		}

		private static UnityEngine.UI.InputField.OnChangeEvent emptyInputFieldEvent = new UnityEngine.UI.InputField.OnChangeEvent();
		public static void SetValue(this UnityEngine.UI.InputField instance, string value)
		{
			InputField.OnChangeEvent originalEvent = instance.onValueChanged;
			instance.onValueChanged = emptyInputFieldEvent;
			instance.text = value;
			instance.onValueChanged = originalEvent;
		}

		private static Dropdown.DropdownEvent emptyDropdownFieldEvent = new Dropdown.DropdownEvent();
		public static void SetValue(this Dropdown instance, int value)
		{
			Dropdown.DropdownEvent originalEvent = instance.onValueChanged;
			instance.onValueChanged = emptyDropdownFieldEvent;
			instance.value = value;
			instance.onValueChanged = originalEvent;
		}

		public static string[] GetDisplayOptions(this Dropdown dropdown)
		{
			return dropdown.options.Select(x => x.text).ToArray();
		}

		public static string[] GetDisplayOptions(this TMPro.TMP_Dropdown dropdown)
		{
			return dropdown.options.Select(x => x.text).ToArray();
		}

	}
}