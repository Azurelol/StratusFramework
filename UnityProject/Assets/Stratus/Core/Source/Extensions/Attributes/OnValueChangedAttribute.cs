using System;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Invokes a provided method when the value is changed
  /// </summary>
  public class OnValueChangedAttribute : PropertyAttribute
  {
    public string method;

    public OnValueChangedAttribute(string method)
    {
      this.method = method;
    }
  }

}