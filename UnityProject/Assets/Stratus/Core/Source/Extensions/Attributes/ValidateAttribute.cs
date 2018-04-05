using System;
using UnityEngine;

namespace Stratus
{
  public enum ValidateLevel { Warning, Error }

  /// <summary>
  /// Validates a property whenever it's changed
  /// </summary>
  public class ValidateAttribute : PropertyAttribute
  {
    public string message;
    public Func<string> messageFunction;
    public ValidateLevel level;
    public bool valid;
    public float height;
    public string method;

    public ValidateAttribute(string method, ValidateLevel level)
    {
      this.method = method;
      this.level = level;
    }
  }

}