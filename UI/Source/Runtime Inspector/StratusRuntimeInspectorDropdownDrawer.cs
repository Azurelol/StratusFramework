using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace Stratus.UI
{
    public class StratusRuntimeInspectorDropdownDrawer : StratusRuntimeInspectorDrawer
    {
        [SerializeField]
        private TMP_Dropdown dropdown;
        [SerializeField]
        private StratusOrientation inputOrientation = StratusOrientation.Horizontal;
        public override Selectable drawerSelectable => dropdown;
        private StratusArrayNavigator<string> values { get; set; }
        public override Color bodyColor 
        {
            set => dropdown.itemText.color = value;
        }

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
            switch (settings.field.fieldType)
            {
                case StratusSerializedFieldType.Boolean:
                    break;
                case StratusSerializedFieldType.Integer:
                    break;
                case StratusSerializedFieldType.Float:
                    break;
                case StratusSerializedFieldType.Enum:
                    values = new StratusArrayNavigator<string>(settings.field.enumValueNames);
                    break;
            }

            values.onIndexChanged += this.ValuesOnIndexChanged;

            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(values.values));
            SetValue((int)settings.field.value);

            dropdown.onValueChanged.AddListener((value) =>
            {
                settings.field.value = value;
            });
        }

        private void ValuesOnIndexChanged(string value, int index)
        {
            SetValue(index);
        }

        private void SetValue(int enumIndex, bool notify = true)
        {
            if (notify)
            {
                dropdown.value = enumIndex;
            }
            else
            {
                dropdown.SetValueWithoutNotify(enumIndex);
            }
        }
    }

}