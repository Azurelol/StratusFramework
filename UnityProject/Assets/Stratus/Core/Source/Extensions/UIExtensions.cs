using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// https://forum.unity.com/threads/change-the-value-of-a-toggle-without-triggering-onvaluechanged.275056/

namespace Stratus
{
  public static partial class Extensions
  {
    static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
    public static void SetValue(this Slider instance, float value)
    {
      var originalEvent = instance.onValueChanged;
      instance.onValueChanged = emptySliderEvent;
      instance.value = value;
      instance.onValueChanged = originalEvent;
    }

    static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();
    public static void SetValue(this Toggle instance, bool value)
    {
      var originalEvent = instance.onValueChanged;
      instance.onValueChanged = emptyToggleEvent;
      instance.isOn = value;
      instance.onValueChanged = originalEvent;
    }

    static UnityEngine.UI.InputField.OnChangeEvent emptyInputFieldEvent = new UnityEngine.UI.InputField.OnChangeEvent();
    public static void SetValue(this UnityEngine.UI.InputField instance, string value)
    {
      var originalEvent = instance.onValueChanged;
      instance.onValueChanged = emptyInputFieldEvent;
      instance.text = value;
      instance.onValueChanged = originalEvent;
    }

    static Dropdown.DropdownEvent emptyDropdownFieldEvent = new Dropdown.DropdownEvent();
    public static void SetValue(this Dropdown instance, int value)
    {
      var originalEvent = instance.onValueChanged;
      instance.onValueChanged = emptyDropdownFieldEvent;
      instance.value = value;
      instance.onValueChanged = originalEvent;
    }

  }
}