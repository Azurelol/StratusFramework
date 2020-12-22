using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Stratus.UI
{
    public class StratusProgressBar : StratusBehaviour
    {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private TextMeshProUGUI text;

        /// <summary>
        /// The current progress, a percentage value between 0 and 1
        /// </summary>
        public float progress => slider.value;

        private Coroutine progressRoutine { get; set; }

        public void Toggle(bool toggle)
        {
            gameObject.SetActive(toggle);
        }

        public void ResetProgress()
        {
            SetProgress(0f);
        }

        public void UpdateProgress(float value, float duration, Action onFinished)
        {
            IEnumerator routine = StratusRoutines.Lerp(progress, value, duration, SetProgress,StratusRoutines.Lerp, onFinished);
            progressRoutine = StartCoroutine(routine);
        }

        public  void SetProgress(float value)
        {
            slider.value = value;
            text.text = value.ToPercentageString();
        }

    }

}