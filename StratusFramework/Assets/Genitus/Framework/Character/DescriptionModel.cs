using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Genitus
{
  /// <summary>
  /// Defines how a character is described
  /// </summary>
  public abstract class DescriptionModel
  {
    [TextArea]
    public string description;
  }

  namespace Models
  {
    [Serializable]
    public class StandardDescription : DescriptionModel
    {
      /// <summary>
      /// A portrait for this character
      /// </summary>
      public Sprite icon;
    }
  }
}
