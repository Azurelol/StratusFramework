using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;

namespace Stratus
{
  /// <summary>
  /// An asset designed for quick documentation within the editor
  /// </summary>
  [CreateAssetMenu(fileName = "DocumentationAsset", menuName = "Stratus/Documentation Asset")]
  public class DocumentationAsset : StratusScriptable
  {
    public enum ElementType
    {
      Text,
      Image,
      Video
    }

    [Serializable]
    public class Element
    {
      public ElementType type = ElementType.Text;
      [TextArea] public string text;
      public Texture2D image;
      public VideoClip video;
    }

    public List<Element> elements = new List<Element>();
    public Element element;

  }

}