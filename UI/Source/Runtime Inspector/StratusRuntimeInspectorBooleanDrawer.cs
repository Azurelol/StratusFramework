using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus.UI
{
    public class StratusRuntimeInspectorBooleanDrawer : StratusRuntimeInspectorDrawer<bool>
    {
        [SerializeField]
        private Toggle toggleComponent;

        public override Selectable drawerSelectable => toggleComponent;

        public override Color bodyColor 
        {
            set => toggleComponent.image.color = value;
        }

        protected override void OnSet(StratusRuntimeInspectorDrawerSettings settings)
        {
            toggleComponent.SetValue((bool)settings.field.value);
            toggleComponent.onValueChanged.AddListener((value) =>
            {
                settings.field.value = value;
            });
        }

        public override void Navigate(Vector2 dir)
        {
            if (dir.x < 0)
            {
                toggleComponent.isOn = false;
            }
            else if(dir.x > 0)
            {
                toggleComponent.isOn = true;
            }
        }
    }

}