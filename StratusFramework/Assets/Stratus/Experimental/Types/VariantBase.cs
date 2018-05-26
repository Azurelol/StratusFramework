using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Types
{
  /// <summary>
  /// A Variant is a dynamic value type which represent a variety of types.
  /// It can be used in situtations where you need a common interface
  /// for your types to represent a variety of data.
  /// </summary>
  public abstract class VariantBase
  {
    protected static Type[] defaultTypes { get; } =
    {
      typeof(int),
      typeof(bool),
      typeof(float),
      typeof(string),
      typeof(Vector3)
    };

  }

}