using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// This field allows for manually tags layers in your inspector in a safe way, implicitly converting to the tag (string)
  /// </summary>
  [Serializable]
  public class TagField
  {
    [SerializeField]
    private string tag = "Untagged";
    public static implicit operator string(TagField tagField) { return tagField.tag; }

    /// <summary>
    /// The current value of this tag
    /// </summary>
    public string value { get { return tag; } set { tag = value; } }

    /// <summary>
    /// Checks whether the GameObject has this tag
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public bool Matches(GameObject go) => go.tag == this;
  }

}