using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.UI
{
    [Serializable]
    public abstract class StratusCanvasButtonGroup : StratusCanvasGroup
    {
        public StratusLayoutTextElementGroupBehaviour buttons;

        protected override void OnInitialize()
        {
            if (buttons != null)
            {
                buttons.Set(GetButtons());
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
            buttons.Select();
        }

        protected abstract StratusLayoutTextElementEntry[] GetButtons();
    }

}