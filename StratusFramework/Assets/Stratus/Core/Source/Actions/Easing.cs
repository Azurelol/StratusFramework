using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Common interpolation algorithms
  /// </summary>
  public enum Ease
  {
    /// <summary>
    /// Linear interpolation
    /// </summary>
    Linear,
    QuadIn,
    QuadInOut,
    QuadOut,
    Smoothstep
  }

  /// <summary>
  /// Provides methods for common interpolation algorithms.
  /// </summary>
  public static class Easing
  {
    /// <summary>
    /// The easing function to be used
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate float Function(float t);

    /// <summary>
    /// An abstract interpolator for all supported easing types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Interpolator<T>
    {
      public System.Action<T> setter;
      public Function function;
      public T difference { get; private set; }
      public T initialValue;
      public T finalValue;
      
      public Interpolator(T initialValue, T finalValue, Function function)
      {
        this.initialValue = initialValue;
        this.finalValue = finalValue;
        this.difference = ComputeDifference();
      }

      public abstract T ComputeDifference();
      public abstract T ComputeCurrentValue(float t);
    } 

    public static float Linear(float t)
    {
      return t;
    }

    public static float QuadIn(float t)
    {
      return t * t;
    }

    public static float QuadOut(float t)
    {
      return t * (2 - t);
    }

    public static float QuadInOut(float k)
    {
      if ((k *= 2.0f) < 1.0f)
        return 0.5f * k * k;

      return -0.5f * ((k -= 1.0f) * (k - 2.0f) - 1.0f);
    }

    public static float Smoothstep(float t)
    {
      return t * t * (3 - 2 * t);
    }

    public static float Calculate(float t, Ease ease)
    {
      float easeVal = 0.0f;
      switch (ease)
      {
        case Ease.Linear:
          easeVal = Linear(t);
          break;
        case Ease.QuadIn:
          easeVal = QuadIn(t);
          break;
        case Ease.QuadOut:
          easeVal = QuadOut(t);
          break;
        case Ease.QuadInOut:
          easeVal = QuadInOut(t);
          break;
        case Ease.Smoothstep:
          easeVal = Smoothstep(t);
          break;
      }
      return easeVal;
    }

    public static Function GetFunction(this Ease ease)
    {
      Function func = null;
      switch (ease)
      {
        case Ease.Linear:
          func = Linear;
          break;
        case Ease.QuadIn:
          func = QuadIn;
          break;
        case Ease.QuadInOut:
          func = QuadInOut;
          break;
        case Ease.QuadOut:
          func = QuadOut;
          break;
        case Ease.Smoothstep:
          func = Smoothstep;
          break;
      }
      return func;
    }

  }

}