using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus.Experimental
{
  public class SelectableHighlighter : StratusBehaviour
  {
    public Selectable selectable;
    public Gameplay.StratusImageEffectEvent imageEffect;
    public bool selectOnHighlight;

    private StratusSelectableProxy proxy;


    private void Awake()
    {
      proxy = StratusSelectableProxy.Construct(selectable);
      proxy.onSelect += OnSelected;
    }

    private void OnSelected(bool select)
    {
      Highlight(select);
    }

    private void OnHighlighted(bool highlight)
    {
      if (highlight && selectOnHighlight)
        selectable.Select();
    }

    public void Highlight(bool highlight)
    {
      imageEffect.alpha = highlight ? 1f : 0f;
      imageEffect.Activate();
    }


  }

}