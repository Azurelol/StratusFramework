using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Stratus
{
  public class SelectableProxy : Proxy, IEventSystemHandler, IPointerEnterHandler, 
    IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
  {
    public enum SelectionType { Selected, Highlighted, Pressed }
    public delegate void SelectionCallback(bool state);

    public SelectionCallback onSelect { get; set; }
    public SelectionCallback onHighlighted { get; set; }
    public SelectionCallback onPressed { get; set; }
    public bool interactable => selectable.IsInteractable();
    
    private Selectable selectable;
    
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    void Awake()
    {
      selectable = GetComponent<Selectable>();
    }

    public void OnSelect(BaseEventData data)
    {
      if (!interactable)
        return;
      onSelect?.Invoke(true);
    }

    public void OnDeselect(BaseEventData data)
    {
      if (!interactable)
        return;
      onSelect?.Invoke(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
      if (!interactable)
        return;
      onHighlighted?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (!interactable)
        return;
      onHighlighted?.Invoke(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (!interactable)
        return;
      onPressed?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (!interactable)
        return;
      onPressed?.Invoke(false);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Constructs a proxy in order to observe a given Selectable's selection messages
    /// </summary>
    /// <param name="target"></param>
    /// <param name="type"></param>
    /// <param name="onCollision"></param>
    /// <param name="persistent"></param>
    /// <returns></returns>
    public static SelectableProxy Construct(Selectable target, SelectionType type, SelectionCallback callback, bool persistent = true)
    {
      var proxy = target.gameObject.GetOrAddComponent<SelectableProxy>();
      proxy.persistent = persistent;

      if (type == SelectionType.Selected)
        proxy.onSelect += callback;
      else if (type == SelectionType.Highlighted)
        proxy.onHighlighted += callback;
      else if (type == SelectionType.Pressed)
        proxy.onPressed += callback;

      return proxy;
    }
    
  }

}