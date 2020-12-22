using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;

namespace Stratus.UI
{
    public abstract class StratusLogWindow<T> : StratusCanvasWindow<T>
        where T : StratusLogWindow<T>
    {
        public class AddLineEvent : StratusEvent
        {
            public string line { get; set; }
        }

        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private TextMeshProUGUI text;

        private static AddLineEvent lineEvent = new AddLineEvent();
        private const int lineCapacity = 500;

        private StringBuilder stringBuilder;
        private StratusCircularBuffer<string> lines;

        protected abstract void OnLogWindowAwake();

        protected override void OnWindowAwake()
        {
            lines = new StratusCircularBuffer<string>(lineCapacity);
            stringBuilder = new StringBuilder();
            StratusScene.Connect<AddLineEvent>(OnAddLineEvent);
            OnLogWindowAwake();
        }

        protected override void OnWindowClose()
        {
        }

        protected override void OnWindowOpen()
        {
        }

        public static void SubmitLine(string line)
        {
            lineEvent.line = line;
            StratusScene.Dispatch<AddLineEvent>(lineEvent);
        }

        private void OnAddLineEvent(AddLineEvent e)
        {
            AddLine(e.line);
        }

        public void AddLine(string line)
        {
            stringBuilder.AppendLine(line);
            this.Log(line);
            UpdateText();
        }

        private void UpdateText()
        {
            text.text = stringBuilder.ToString();
            StartCoroutine(ApplyScrollPosition(scrollRect, 0));
        }

        public void Clear()
        {
            stringBuilder.Clear();
            UpdateText();
        }

        IEnumerator ApplyScrollPosition(ScrollRect sr, float verticalPos)
        {
            yield return new WaitForEndOfFrame();
            sr.verticalNormalizedPosition = verticalPos;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)sr.transform);
        }
    }
}
