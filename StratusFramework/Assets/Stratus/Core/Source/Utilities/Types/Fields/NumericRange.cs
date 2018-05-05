using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Base class for numeric range fields
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class NumericRange<T> where T : struct
  {
    public T minimum, maximum;
    public abstract T randomInRange { get; }
  }

  /// <summary>
  /// Represents a range consisting of two floating point values
  /// </summary>
  [Serializable]
  public class FloatRange : NumericRange<float>
  {
    public override float randomInRange => UnityEngine.Random.Range(minimum, maximum);
  }

  /// <summary>
  /// Represents a range consisting of two integer values
  /// </summary>
  [Serializable]
  public class IntegerRange : NumericRange<int>
  {
    public override int randomInRange => UnityEngine.Random.Range(minimum, maximum);
  }

}