using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus
{
    public static partial class Extensions
    {
        public static void SetOptions(this Dropdown dropdown, IEnumerable<string> values)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(values));
        }

        public static void ScrollToChildIfNotVisible(this ScrollRect scrollRect, RectTransform contentChild)
        {
            if (scrollRect.content != contentChild.parent)
            {
                StratusDebug.LogError($"Can only scroll to children of scrollRect's content {scrollRect.content}");
                return;
            }

            int childIndex = contentChild.transform.GetSiblingIndex();
            float targetValue = 1f - ((float)childIndex / (float)scrollRect.content.transform.childCount);
            float currentValue = scrollRect.verticalNormalizedPosition;
            StratusDebug.Log($"Scrolling to {targetValue}");
            scrollRect.verticalNormalizedPosition = targetValue;
        }
    }

}