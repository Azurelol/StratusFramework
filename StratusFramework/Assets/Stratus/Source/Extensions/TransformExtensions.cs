/******************************************************************************/
/*!
@file   TransformExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  public static partial class Extensions
  {

    /// <summary>
    /// Rotates the transform so that the forward vector looks the target's position over a specified amount of time.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="duration"></param>
    public static void LookAt(this Transform transform, Transform target, float duration, UpdateMode mode = UpdateMode.Normal)
    {
      // Remove a previous routine if already active
      RotationRoutine.CancelPrevious(transform);

      // Set the new routine
      //var newRoutine = LookAtRoutine.Construct(transform, target, duration);
      var newRoutine = transform.gameObject.AddComponent<LookAtRoutine>();
      newRoutine.Set(target, duration, mode);
    }

    /// <summary>
    /// Rotates the transform so that the forward vector looks the target's position over a specified amount of time.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    public static void MoveTo(this Transform transform, Vector3 target, float speed, UpdateMode mode = UpdateMode.Normal)
    {
      PositionRoutine.CancelPrevious(transform);      
      var newRoutine = transform.gameObject.AddComponent<MoveToRoutine>();
      newRoutine.Set(target, speed, mode);
    }

    /// <summary>
    /// Rotates the transform around a given pivot
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="pivot"></param>
    /// <param name="speed"></param>
    public static void RotateAround(this Transform transform, Vector3 pivot, Vector3 axis, float angle, float speed, UpdateMode mode = UpdateMode.Fixed)
    {
      if (speed == 0f)
      {
        transform.RotateAround(pivot, axis, angle);
      }
      else
      {
        RotationRoutine.CancelPrevious(transform);
        var newRoutine = transform.gameObject.AddComponent<RotateAroundRoutine>();
        newRoutine.Set(pivot, axis, angle, speed, mode);
      }

    }

    /// <summary>
    /// Calculates a position in front of the transform at a given distance
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector3 CalculateForwardPosition(this Transform transform, float distance)
    {
      return transform.position + (transform.forward * distance);
    }


    /// <summary>
    /// Centers this transform on the parent
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="parent"></param>
    public static void Center(this Transform transform, Transform parent)
    {
      transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Finds the child of this transform, using Breadth-first search
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChildBFS(this Transform parent, string name)
    {
      var result = parent.Find(name);
      if (result != null)
        return result;
      foreach (Transform child in parent)
      {
        result = child.FindChildBFS(name);
        if (result != null)
          return result;
      }
      return null;
    }

  }
}
