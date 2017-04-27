/******************************************************************************/
/*!
@file   TransformRoutine.cs
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
  /// <summary>
  /// Base class for all transform routines
  /// </summary>
  public abstract class TransformRoutine : MonoBehaviour
  {
    protected UpdateMode Mode = UpdateMode.Normal;
    protected float Duration;
    private IEnumerator Routine;

    protected abstract void OnFrame(float t);

    private void Start()
    {
      Routine = Run(Duration);
      StartCoroutine(Routine);
      Destroy(this, Duration);
    }

    public void Stop()
    {
      if (Routine != null)
        StopCoroutine(Routine);
      Destroy(this, this.Duration);
    }

    IEnumerator Run(float duration)
    {
      // If no time was passed, just apply the change instantly
      if (duration == 0f)
      {
        OnFrame(1f);
        yield break;
      }

      float t = 0f;
      while (t <= 1f)
      {
        switch (Mode)
        {
          case UpdateMode.Normal:
            t += Time.deltaTime / duration;
            break;
          case UpdateMode.Fixed:
            t += Time.fixedDeltaTime / duration;
            break;
          case UpdateMode.Unscaled:
            t += Time.unscaledDeltaTime / duration;
            break;
        }

        //Trace.Script("Fixed = " + Time.fixedDeltaTime + ", Unscaled = " + Time.unscaledDeltaTime);

        //t += Time.fixedDeltaTime / duration;
        OnFrame(t);

        switch (Mode)
        {
          case UpdateMode.Normal:
            yield return null;
            break;
          case UpdateMode.Fixed:
            yield return new WaitForFixedUpdate();
            break;
          case UpdateMode.Unscaled:
            //yield return new WaitForFixedUpdate();
            yield return new WaitForUnscaledUpdate();
            break;
        }

      }
    }
       

  }

  /// <summary>
  /// Base class for all routines that affect the transform's position
  /// </summary>
  public abstract class PositionRoutine : TransformRoutine
  {
    protected abstract void Translate(float t);

    protected override void OnFrame(float t)
    {
      Translate(t);
    }

    /// <summary>
    /// Cancels a currently running position routine if one is active
    /// </summary>
    /// <param name="transform"></param>
    public static void CancelPrevious(Transform transform)
    {
      // Remove a previous routine if already active
      var currentRoutine = transform.gameObject.GetComponent<PositionRoutine>();
      if (currentRoutine != null)
      {
        currentRoutine.Stop();
      }
    }

  }

  /// <summary>
  /// Base class for all routines that affect the transform's scale
  /// </summary>
  public abstract class ScaleRoutine : TransformRoutine
  {
    protected abstract void Scale(float t);

    protected override void OnFrame(float t)
    {
      Scale(t);
    }

    /// <summary>
    /// Cancels a currently running scale routine if one is active
    /// </summary>
    /// <param name="transform"></param>
    public static void CancelPrevious(Transform transform)
    {
      // Remove a previous routine if already active
      var currentRoutine = transform.gameObject.GetComponent<ScaleRoutine>();
      if (currentRoutine != null)
      {
        currentRoutine.Stop();
      }
    }

  }


  /// <summary>
  /// Base class for all routines that affect the transform's rotation
  /// </summary>
  public abstract class RotationRoutine : TransformRoutine
  {
    protected abstract void Rotate(float t);

    protected override void OnFrame(float t)
    {
      Rotate(t);
    }

    /// <summary>
    /// Cancels a currently running rotation routine if one is active
    /// </summary>
    /// <param name="transform"></param>
    public static void CancelPrevious(Transform transform)
    {
      // Remove a previous routine if already active
      var currentRoutine = transform.gameObject.GetComponent<RotationRoutine>();
      if (currentRoutine != null)
      {
        currentRoutine.Stop();
      }
    }

  }


}
