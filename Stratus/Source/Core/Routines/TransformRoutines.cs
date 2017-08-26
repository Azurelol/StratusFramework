/******************************************************************************/
/*!
@file   TransformRoutines.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections;
using System;

namespace Stratus
{
  public static partial class Routines
  {
    public class RotateAroundRoutine : TransformRoutine
    {
      private Vector3 pivot;
      private Vector3 axis;
      private float angle;
      private TimeScale timeScale;
      public override TransformationType type => TransformationType.Rotate | TransformationType.Translate;

      public RotateAroundRoutine(Vector3 pivot, Vector3 axis, float angle, TimeScale timeScale = TimeScale.Delta)
      {
        this.pivot = pivot;
        this.axis = axis;
        this.angle = angle;
        this.timeScale = timeScale;
      }

      protected override IEnumerator OnTransform()
      {
        while (true)
        {
          transform.RotateAround(pivot, axis, angle);
          yield return timeScale.Yield();
        }
      }
    }



    /// <summary>
    /// Rotates around a given pivot to a given angle in degrees over a specified duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="pivot"></param>
    /// <param name="axis"></param>
    /// <param name="degrees"></param>
    /// <param name="duration"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator RotateAround(Transform transform, Vector3 pivot, Vector3 axis, float degrees, float duration, TimeScale timeScale = TimeScale.FixedDelta)
    {
      float angularSpeed = degrees / duration;
      float elapsed = 0f;

      System.Action<float> func = (float t) =>
      {
        float time = timeScale.GetTime();
        elapsed += time;
        if (elapsed >= duration)
          time = time - (elapsed - duration);
        float nextAngle = angularSpeed * time;

        transform.RotateAround(pivot, axis, nextAngle);
      };


      if (duration <= 0f)
      {
        transform.RotateAround(pivot, axis, degrees);
      }
      else
        yield return Lerp(func, duration);
    }


    /// <summary>
    /// Rotates around a given pivot until cancelled
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="pivot"></param>
    /// <param name="axis"></param>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static IEnumerator RotateAround(Transform transform, Vector3 pivot, Vector3 axis, float degrees, System.Action onFinished = null, TimeScale timeScale = TimeScale.FixedDelta)
    {
      while (true)
      {
        float step = degrees * timeScale.GetTime();
        transform.RotateAround(pivot, axis, step);
        yield return timeScale.Yield();
      }
    }

    /// <summary>
    /// Moves the transform from its current position to the target position over a specified duration by translation
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="endPos"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static IEnumerator MoveTo(Transform transform, Vector3 endPos, float duration, float distFromTarget = 0f, System.Action onFinished = null, TimeScale timeScale = TimeScale.Delta)
    {
      Vector3 startPos = transform.position;

      if (distFromTarget > 0f)
        endPos = startPos.CalculatePositionAtDistanceFromTarget(endPos, distFromTarget);

      System.Action<float> func = (float t) =>
      {
        Vector3 nextPos = Vector3.Lerp(startPos, endPos, t);
        transform.position = nextPos;
      };

      //IEnumerator lerp = Lerp(func, duration);
      //yield return lerp;
      yield return Lerp(func, duration, timeScale);
      onFinished?.Invoke();
    }

    /// <summary>
    /// Makes the transform follow the specified target at a specified speed and given distance until cancelled.
    /// Optionally, it can be forced to maintain the given distance
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="distance"></param>
    /// <param name="maintainDistance"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator Follow(Transform transform, Transform target, float speed, float distance = 0.0f, bool maintainDistance = false, TimeScale timeScale = TimeScale.Delta)
    {
      while (true)
      {
        FollowProcedure(transform, target, speed, distance, timeScale.GetTime(), maintainDistance);
        yield return timeScale.Yield();
      }
    }    

    /// <summary>
    /// The transform will follow the specified target at a specified speed for a given duration
    /// until the duration elapses
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="duration"></param>
    /// <param name="stopDistance"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator FollowUntil(Transform transform, Transform target, float speed, float duration, float stopDistance = 0.0f, TimeScale timeScale = TimeScale.Delta)
    {
      while (duration > 0f)
      {
        float dt = timeScale.GetTime();
        duration -= dt;
        FollowProcedure(transform, target, speed, stopDistance, dt);
        yield return timeScale.Yield();
      }
    }

    /// <summary>
    /// Follows the specified target while the given condition is true
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="condition"></param>
    /// <param name="stopDistance"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator FollowWhile(Transform transform, Transform target, float speed, System.Func<bool> condition, float stopDistance = 0.0f, TimeScale timeScale = TimeScale.Delta)
    {
      while (condition.Invoke())
      {
        FollowProcedure(transform, target, speed, stopDistance, timeScale.GetTime());
        yield return timeScale.Yield();
      }
    }

    /// <summary>
    /// Rotates the transform to have its forward aligned towards a target position over a specified duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="targetPosition"></param>
    /// <param name="duration"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator LookAt(Transform transform, Vector3 targetPosition, float duration, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Quaternion startingRot = transform.rotation;

      System.Action<float> func = (float t) =>
      {
        Vector3 lookAtVec = targetPosition - transform.position;
        Quaternion nextRot = Quaternion.LookRotation(lookAtVec);
        transform.rotation = Quaternion.Lerp(startingRot, nextRot, t);
      };

      //IEnumerator lerp = Lerp(func, duration);
      //yield return lerp;
      yield return Lerp(func, duration);
    }

    /// <summary>
    /// Rotates the transform to have its forward aligned towards a target over a specified duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="targetPosition"></param>
    /// <param name="duration"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator LookAt(Transform transform, Transform target, float duration, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Quaternion startingRot = transform.rotation;

      System.Action<float> func = (float t) =>
      {
        Vector3 lookAtVec = target.position - transform.position;
        Quaternion nextRot = Quaternion.LookRotation(lookAtVec);
        transform.rotation = Quaternion.Lerp(startingRot, nextRot, t);
      };

      //IEnumerator lerp = Lerp(func, duration);
      //yield return lerp;
      yield return Lerp(func, duration, timeScale);
    }

    /// <summary>
    /// Rotates the transform to have its forward aligned towards a target until cancelled
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator Track(Transform transform, Transform target, float speed, TimeScale timeScale = TimeScale.FixedDelta)
    {
      while (true)
      {
        Vector3 lookAtVec = target.position - transform.position;
        float dt = timeScale.GetTime();
        transform.forward = Vector3.Lerp(transform.forward, lookAtVec, dt * speed);
        yield return timeScale.Yield();
      }
    }

    public static IEnumerator Scale(Transform transform, Vector3 endingVal, float duration, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Vector3 startingVal = transform.localScale;
      System.Action<float> scalingFunc = (float t) => ScaleProcedure(transform, startingVal, endingVal, t);
      yield return Lerp(scalingFunc, duration, timeScale);
    }

    /// <summary>
    /// Applies the given curve to the transform's scale over a given duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="curve"></param>
    /// <param name="duration"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator Scale(Transform transform, AnimationCurve curve, float duration, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Vector3 startingVal = transform.localScale;
      Vector3 endinvgVal = startingVal * curve.Evaluate(duration);
      System.Action<float> scalingFunc = (float t) =>
      {        
        Vector3 nextVal = Vector3.Lerp(startingVal, endinvgVal, curve.Evaluate(t));
        transform.localScale = nextVal;
      };
      yield return Lerp(scalingFunc, duration, timeScale);
    }

    /// <summary>
    /// Applies the scalar to the transform's current scale over a given duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="scalar"></param>
    /// <param name="duration"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator Scale(Transform transform, float scalar, float duration, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Vector3 startingVal = transform.localScale;
      Vector3 endingVal = transform.localScale * scalar;
      System.Action<float> func = (float t) => ScaleProcedure(transform, startingVal, endingVal, t);
      yield return Lerp(func, duration, timeScale);
    }

    /// <summary>
    /// Applies the given scalars to the transform's scale in sequence over a given duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="scalars"></param>
    /// <param name="totalDuration"></param>
    /// <param name="repeat"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator Scale(Transform transform, float[] scalars, float totalDuration, bool repeat = false, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Vector3 originalScale = transform.localScale;
      float durationForEach = totalDuration / scalars.Length;
      do
      {
        foreach (var scalar in scalars)
        {
          Vector3 scale = originalScale * scalar;
          //Trace.Script("Scaling to " + scale);
          yield return Scale(transform, scale, durationForEach, timeScale);
        }
      } while (repeat);
    }

    /// <summary>
    /// Applies the given scales to the transformation in sequence over a given duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="scales"></param>
    /// <param name="totalDuration"></param>
    /// <param name="repeat"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator Scale(Transform transform, Vector3[] scales, float totalDuration, bool repeat = false, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Vector3 originalScale = transform.localScale;
      float durationForEach = totalDuration / scales.Length;
      do
      {
        foreach (var scale in scales)
        {
          Trace.Script("Scaling to " + scale);
          yield return Scale(transform, scale, durationForEach, timeScale);
        }
      } while (repeat);
    }

    /// <summary>
    /// Applies the given scales to the transformation in sequence over a given duration
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="scalingCurves"></param>
    /// <param name="totalDuration"></param>
    /// <param name="repeat"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    public static IEnumerator Scale(Transform transform, AnimationCurve[] scalingCurves, float totalDuration, bool repeat = false, TimeScale timeScale = TimeScale.FixedDelta)
    {
      Vector3 originalScale = transform.localScale;
      float durationForEach = totalDuration / scalingCurves.Length;
      do
      {
        foreach (var scale in scalingCurves)
        {
          Trace.Script("Scaling to " + scale);
          yield return Scale(transform, scale, durationForEach, timeScale);
        }
      } while (repeat);
    }


    //------------------------------------------------------------------------/
    // Procedures
    //------------------------------------------------------------------------/
    private static void FollowProcedure(Transform transform, Transform target, float speed, float stopDistance, float dt, bool maintainDistance = false, float epsilon = 0.2f)
    {
      float step = speed * dt;
      Vector3 targetPos = target.position;
      
      if (stopDistance > 0f)
      {
        float dist = Vector3.Distance(transform.position, target.position);
        bool isWithinStopppingDistance = (dist <= stopDistance);
        if (isWithinStopppingDistance)
        {
          if (maintainDistance)
          {
            float diff = stopDistance - dist;
            bool wontStutter = diff > epsilon;
            if (wontStutter)
              targetPos = -targetPos;
            else
              return;
          }            
          else
          {
            return;
          }
        }
      }

      transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    private static void ScaleProcedure(Transform transform, Vector3 startVal, Vector3 endVal, float t)
    {
      Vector3 nextVal = Vector3.Lerp(startVal, endVal, t);
      transform.localScale = nextVal;
    }

  }
}
