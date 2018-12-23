using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Stratus.UI
{
  /// <summary>
  /// Helper methods for quickly setting up UI elements dynamically
  /// </summary>
  public static class Factory
  {
    /// <summary>
    /// Creates a choice button based on a prefab. It parents it to a layout group automatically as well.
    /// </summary>
    public static UnityEngine.UI.Button CreateChoice(UnityEngine.UI.Button buttonPrefab, LayoutGroup panel, string text, UnityAction onClick = null)
    {
      UnityEngine.UI.Button choice = Object.Instantiate(buttonPrefab) as UnityEngine.UI.Button;
      choice.transform.SetParent(panel.transform, false);
      if (onClick != null)
        choice.onClick.AddListener(onClick);

      TMPro.TextMeshProUGUI choiceText = choice.GetComponentInChildren<TMPro.TextMeshProUGUI>();
      choiceText.text = text;
      return choice;
    }
  }

}