using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stratus.Experimental
{
  public class SelectableHighlighter : StratusBehaviour
  {
    public Selectable selectable;
    public ImageEffectEvent imageEffect;
    public bool selectOnHighlight;

    private SelectableProxy proxy;


    private void Awake()
    {
      proxy = SelectableProxy.Construct(selectable);
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
      imageEffect.Trigger();
    }


  }

}