using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Stratus.Utilities
{
  /// <summary>
  /// Helper methods for quickly setting up UI elements dynamically
  /// </summary>
  public static class UI
  {
    /// <summary>
    /// Creates a choice button based on a prefab. It parents it to a layout group automatically as well.
    /// </summary>
    public static Button CreateChoice(Button buttonPrefab, LayoutGroup panel, string text, UnityAction onClick = null)
    {
      Button choice = Object.Instantiate(buttonPrefab) as Button;
      choice.transform.SetParent(panel.transform, false);
      if (onClick != null)
        choice.onClick.AddListener(onClick);

      TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
      choiceText.text = text;
      return choice;
    }
  }

}