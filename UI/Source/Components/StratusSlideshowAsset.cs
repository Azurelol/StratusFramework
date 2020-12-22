using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace Stratus.UI
{
    [CreateAssetMenu(menuName = "Stratus/UI/Slideshow Asset")]
    public class StratusSlideshowAsset : StratusScriptable
    {
        [Serializable]
        public class Slide
        {
            [TextArea]
            public string text;
            public Sprite sprite;
            public float transition = 0f;
        }

        [SerializeField]
        private List<Slide> _slides;
        public Slide[] slides => _slides.ToArray();
    }

}