using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Stratus.UI
{
    public class StratusScrollingText : StratusBehaviour
    {
        [SerializeField]
        private TextAsset _textAsset;
        [SerializeField]
        private TextMeshProUGUI textComponent;
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private float scrollDuration = 60f;
        [Range(0f, 1f)]
        [SerializeField]
        private float initialValue = 1f;
        [Range(0f, 1f)]
        [SerializeField]
        private float finalValue = 0f;

        private Coroutine scrollingRoutine;

        private void Awake()
        {
            if (_textAsset != null)
            {
                Set(_textAsset);
            }
        }

        [StratusInvokeMethod(isPlayMode = true)]
        public void StartScrolling()
        {
            scrollRect.verticalNormalizedPosition = initialValue;
            var routine = StratusRoutines.Interpolate(initialValue, finalValue, scrollDuration, 
                (value) =>
                {
                    scrollRect.verticalNormalizedPosition = value;
                });
            scrollingRoutine = StartCoroutine(routine);
            this.Log($"Now scrolling over {scrollDuration} seconds...");

        }

        [StratusInvokeMethod(isPlayMode = true)]
        public void EndScrolling()
        {
            StopCoroutine(scrollingRoutine);
            scrollingRoutine = null;
        }

        [StratusInvokeMethod(isPlayMode = true)]
        public void ResetScrolling()
        {
            if (scrollingRoutine != null)
            {
                EndScrolling();
            }
            scrollRect.verticalNormalizedPosition = 0;
        }

        public void Set(TextAsset textAsset)
        {
            textComponent.text = textAsset.text;
        }
    } 
}
