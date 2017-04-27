/******************************************************************************/
/*!
@file   Coroutines.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;

namespace Stratus
{
  public static partial class Routines
  {
    public delegate void LerpFunction(float t);

    /// <summary>
    /// A routine for waiting for a specific amount of time, in real time.
    /// This is mostly used when the time scale has been set to 0.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static IEnumerator WaitForRealSeconds(float time)
    {
      float start = Time.realtimeSinceStartup;
      while (Time.realtimeSinceStartup < start + time)
      {
        yield return null;
      }
    }

    /// <summary>
    /// A routine for linearly interpolating between two values 
    /// a and b by the interpolant t. This parameter is clamped to the range [0,1]
    /// </summary>
    /// <param name="onUpdate">The function to call each update with the t value passed to it.</param>
    /// <param name="duration">The duration of this interpolation.</param>
    /// <returns></returns>
    public static IEnumerator Lerp(LerpFunction onUpdate, float duration)
    {
      float t = 0f;
      while (t <= 1f)
      {
        t += Time.fixedDeltaTime / duration;
        onUpdate(t);
        yield return new WaitForFixedUpdate();
      }
    }
    
    /// <summary>
    /// Invokes a function after specified amount of time
    /// </summary>
    /// <param name="onFinish"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public static IEnumerator Call(System.Action onFinish, float delay)
    {
      yield return new WaitForSeconds(delay);
      onFinish();
    }

    /// <summary>
    /// A sequence of functions. First it will invoke a function before the loop begins, 
    /// a function while the loop runs for the specified duration, and a third function
    /// when the loop has run its course.
    /// </summary>
    /// <param name="onStart"></param>
    /// <param name="onUpdate"></param>
    /// <param name="onEnd"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static IEnumerator Sequence(System.Action onStart, System.Action onUpdate, System.Action onEnd, float duration)
    {
      onStart();

      float timeElapsed = 0f;
      while (timeElapsed <= duration)
      {
        timeElapsed += Time.deltaTime;
        onUpdate();
        yield return new WaitForFixedUpdate();
      }

      onEnd();
    }

    /// <summary>
    /// A sequence of functions. First it will invoke a function before the loop begins, 
    /// a function while the loop runs for the specified duration, and a third function
    /// when the loop has run its course.
    /// </summary>
    /// <param name="onStart"></param>
    /// <param name="onUpdate"></param>
    /// <param name="onEnd"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static IEnumerator Sequence(System.Action onStart, System.Action onEnd, float duration)
    {
      onStart();

      float timeElapsed = 0f;
      while (timeElapsed <= duration)
      {
        timeElapsed += Time.deltaTime;
        yield return new WaitForFixedUpdate();
      }

      onEnd();
    }

    /// <summary>
    /// Toggles a boolean value from one state to another for a given period of time.
    /// </summary>
    /// <param name="boolean">A lambda expression of the form: '(x)=> boolean = x'</param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static IEnumerator Toggle(System.Action<bool> boolean, bool startingValue, bool endingValue, float duration)
    {
      boolean(startingValue);

      float timeElapsed = 0f;
      while (timeElapsed <= duration)
      {
        timeElapsed += Time.deltaTime;
        yield return new WaitForFixedUpdate();
      }

      boolean(endingValue);

      
    }

  }

}
