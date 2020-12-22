using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public partial class StratusGUI : StratusSingletonBehaviour<StratusGUI>
  {
    public enum RichTextOption
    {
      Bold,
      Italic
    }

    public static GUIContent Content(string text, int size)
    {
      return new GUIContent($"<size={size}>{text}</size>");
    }

    public static GUIContent Content(string text, int size, Color color)
    {
      return new GUIContent($"<color={color.ToHex()}><size={size}>{text}</size></color");
    }

    public static GUIContent Content(string text, Color color)
    {
      return new GUIContent($"<color={color.ToHex()}>{text}</color");
    }


  }
}