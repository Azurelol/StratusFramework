using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public struct BooleanOptionField
  {
    public enum Value
    {
      Default,
      True,
      False
    }

    public Value value;

  }

  public abstract class OverrideField<T> where T : struct
  {
    /// <summary>
    /// Whether to use this override
    /// </summary>
    [Tooltip("Whether to use this override")]
    public bool enabled;

    /// <summary>
    /// The value to use as an override
    /// </summary>
    [Tooltip("The value to use as an override")]
    public T value;

    /// <summary>
    /// Based on whether this field is enabled, retrieves its value
    /// (or uses the default provided)
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public T Get(ref T defaultValue) => enabled ? value : defaultValue;
  }
  
  public class FloatOverride : OverrideField<float> {}
  public class IntOverride : OverrideField<float> { }
  public class Vector3Override : OverrideField<float> { }


}