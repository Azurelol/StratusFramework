using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Stratus.UI
{
    public struct StratusRuntimeInspectorFieldSettings
    {
        public StratusSerializedField field;
        public StratusRuntimeInspectorDrawer drawer;
        public StratusRuntimeInspectorFieldBehaviour parent;
        public float drawerWidth;
        public StratusLayoutElementStyle style;
    }

    public class StratusRuntimeInspectorFieldBehaviour : StratusSelectable
    {
        [SerializeField]
        private TextMeshProUGUI labelComponent;
        [SerializeField]
        private Button buttonComponent;
        [SerializeField]
        private RectTransform drawerAnchor;

        public override Selectable selectable => buttonComponent;
        public StratusSerializedField serializedField { get; set; }
        public int depth { get; set; }
        public StratusRuntimeInspectorDrawer drawer { get; private set; }
        public Color bodyColor
        {
            set => labelComponent.color = value;
        }

        private List<StratusRuntimeInspectorFieldBehaviour> children = new List<StratusRuntimeInspectorFieldBehaviour>();

        protected override void OnSelectableAwake()
        {
            
        }

        public void Set(StratusRuntimeInspectorFieldSettings fieldSettings)
        {
            serializedField = fieldSettings.field;
            labelComponent.text = fieldSettings.field.displayName;
            gameObject.name = $"Serialized Field : {fieldSettings.field.displayName}";
            buttonComponent.colors = fieldSettings.style.buttonStyle.colorBlock;
            this.drawer = fieldSettings.drawer;

            if (drawer != null)
            {
                StratusRuntimeInspectorDrawerSettings drawerSettings = new StratusRuntimeInspectorDrawerSettings(fieldSettings.field, Select);
                drawerSettings.width = fieldSettings.drawerWidth;
                drawerSettings.parent = this.drawerAnchor;
                drawerSettings.style = fieldSettings.style;
                drawer.Set(drawerSettings);
                buttonComponent.onClick.AddListener(SelectDrawer);
            }

            if (fieldSettings.parent != null)
            {
                fieldSettings.parent.children.Add(this);
            }
        }

        private void SelectDrawer()
        {
            drawer.Select();
        }

        public void Toggle(bool toggle)
        {
            this.gameObject.SetActive(toggle);
            foreach(var child in children)
            {
                child.Toggle(toggle);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            ((ISelectHandler)this.buttonComponent).OnSelect(eventData);
        }

        public void Expand(bool expand)
        {
            children.ForEach(x => x.Toggle(expand));
        }

        public override void Navigate(Vector2 dir)
        {
            if (children.NotEmpty())
            {
                if (dir.x > 0)
                {
                    Expand(true);
                }
                else if (dir.x < 0)
                {
                    Expand(false);
                }
            }
            if (drawer != null)
            {
                drawer.Navigate(dir);
            }
        }
    }

}