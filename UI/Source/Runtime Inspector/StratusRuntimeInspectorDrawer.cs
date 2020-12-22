using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus;
using UnityEngine.UI;

namespace Stratus.UI
{
    public class StratusRuntimeInspectorDrawerSettings
    {
        public Transform parent;
        public StratusSerializedField field;
        public Action onDeselect;
        public StratusInputLayer inputLayer;
        public float width;
        public StratusLayoutElementStyle style;

        public StratusRuntimeInspectorDrawerSettings(StratusSerializedField field, Action onDeselect)
        {
            this.field = field;
            this.onDeselect = onDeselect;
        }
    }

    public abstract class StratusRuntimeInspectorDrawer : StratusBehaviour, IStratusInputUIActionHandler
    {
        public RectTransform rectTransform => GetComponentCached<RectTransform>();
        public StratusRuntimeInspectorDrawerSettings settings { get; private set; }
        public abstract Selectable drawerSelectable { get; }
        protected abstract void OnSet(StratusRuntimeInspectorDrawerSettings settings);
        public abstract Color bodyColor { set; }

        public float width
        {
            set
            {
                rectTransform.SetWidth(value);
            }
        }

        public void Set(StratusRuntimeInspectorDrawerSettings settings)
        {
            this.transform.SetParent(settings.parent, false);
            this.transform.localScale = Vector3.one;
            this.transform.SetAsLastSibling();
            rectTransform.SetWidth(settings.width);
            this.settings = settings;
            if (settings.style != null)
            {
                bodyColor = settings.style.defaultStyle.bodyColor;
            }
            OnSet(settings);
        }

        public void Select()
        {
            if (settings.inputLayer != null)
            {
                drawerSelectable.Select();
                settings.inputLayer.PushByEvent();
            }
        }

        public void Deselect()
        {
            if (settings.inputLayer != null && settings.inputLayer.pushed)
            {
                settings.inputLayer.PopByEvent();
            }
        }

        public abstract void Navigate(Vector2 dir);
    }

    public abstract class StratusRuntimeInspectorDrawer<T> : StratusRuntimeInspectorDrawer
    {
    }

}