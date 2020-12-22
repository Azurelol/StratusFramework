using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Stratus.UI
{
    public class StratusRuntimeInspectorNumericDrawer : StratusRuntimeInspectorDrawer
    {
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private float inputStepSize = 0.1f;
        [SerializeField]
        private StratusOrientation inputOrientation = StratusOrientation.Horizontal;

        private StratusNumericType numericType { get; set; }
        private bool isPercentage { get; set; }
        private bool updatingValue { get; set; }

        public override Selectable drawerSelectable => slider;

        public override Color bodyColor
        {
            set => inputField.textComponent.color = value;
        }

        public override void Navigate(Vector2 dir)
        {
            switch (inputOrientation)
            {
                case StratusOrientation.Horizontal:
                    if (dir.x > 0)
                    {
                        slider.value += inputStepSize;
                    }
                    else if (dir.x < 0)
                    {
                        slider.value -= inputStepSize;
                    }
                    break;
                case StratusOrientation.Vertical:
                    if (dir.y > 0)
                    {
                        slider.value += inputStepSize;
                    }
                    else if (dir.y < 0)
                    {
                        slider.value -= inputStepSize;
                    }
                    break;
            }
        }

        protected override void OnSet(StratusRuntimeInspectorDrawerSettings settings)
        {
            if (settings.field.fieldType == StratusSerializedFieldType.Float)
            {
                numericType = StratusNumericType.Float;
                slider.wholeNumbers = false;

            }
            else if (settings.field.fieldType == StratusSerializedFieldType.Integer)
            {
                numericType = StratusNumericType.Integer;
                slider.wholeNumbers = true;
            }
            else
            {
                throw new Exception("Unsupported field type for numeric drawer...");
            }

            RangeAttribute range = (RangeAttribute)settings.field.attributesByType.GetValueOrNull(typeof(RangeAttribute));
            if (range != null)
            {
                slider.minValue = range.min;
                slider.maxValue = range.max;
                if (numericType == StratusNumericType.Float)
                {
                    isPercentage = range.min == 0 && range.max == 1;
                }
            }
            else
            {
                slider.gameObject.SetActive(false);
            }

            switch (numericType)
            {
                case StratusNumericType.Integer:
                    inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    break;
                case StratusNumericType.Float:
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    break;
            }

            float currentValue = (float)settings.field.value;
            UpdateSliderValue(currentValue, false);
            UpdateTextValue(currentValue);

            inputField.onValueChanged.AddListener((value) =>
            {
                try
                {
                    switch (numericType)
                    {
                        case StratusNumericType.Integer:
                            {
                                int parsedValue = int.Parse(value);
                                settings.field.value = value;
                                UpdateSliderValue(parsedValue, true);
                            }
                            break;
                        case StratusNumericType.Float:
                            {
                                float parsedValue = float.Parse(value);
                                settings.field.value = value;
                                UpdateSliderValue(parsedValue, true);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    UpdateTextValue(slider.value);
                }
            });

            slider.onValueChanged.AddListener((value) =>
            {
                settings.field.value = value;
                UpdateTextValue(value);
            });
        }

        private void UpdateTextValue(float value)
        {
            if (updatingValue)
            {
                return;
            }
            updatingValue = true;
            inputField.text = isPercentage ? value.ToPercentageRoundedString() 
                : value.ToString();
            updatingValue = false;
        }

        private void UpdateSliderValue(float value, bool fromText)
        {
            if (updatingValue)
            {
                return;
            }
            updatingValue = true;
            if (fromText && isPercentage)
            {
                value = value.ToPercent();
            }
            slider.SetValue(value);
            updatingValue = false;
        }
    }

}