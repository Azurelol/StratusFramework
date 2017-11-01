/******************************************************************************/
/*!
@file   Library.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// A library of functions common for game AI
    /// </summary>
    public static class Library
    {
      //------------------------------------------------------------------------/
      // Enumerations
      //------------------------------------------------------------------------/
      public enum RelativeRange { Nearest, Farthest }
      public enum ComparisonOperation { GreaterOrEqualThan, LesserThan }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Checks whether the target is in range
      /// </summary>
      /// <param name="source">The source object</param>
      /// <param name="target">The target object</param>
      /// <param name="range">The range at which we are checking</param>
      /// <returns></returns>
      public static bool CheckRange(Transform source, Transform target, float range)
      {
        return CheckRange(source, target.position, range);
      }

      /// <summary>
      /// Checks whether the target is in range of the source
      /// </summary>
      /// <param name="source">The source object</param>
      /// <param name="target">The target position</param>
      /// <param name="range">The range at which we are checking</param>
      /// <returns></returns>
      public static bool CheckRange(Transform source, Vector3 target, float range)
      {
        float dist = Vector3.Distance(target, source.position);
        bool inRange = (dist <= range);
        //Trace.Script("Distance = " + dist);
        return inRange;
      }

      /// <summary>
      /// Checks whether the target is within the field of view of the source
      /// </summary>
      /// <returns></returns>
      public static bool CheckFieldOfView(Transform source, Transform target, float fov)
      {
        // Compute the vector pointing from the source towards the target
        Vector3 toTarget = source.position - target.position;

        // If the dot product between the source and the target is less than the cosine
        // of the field of view, the target is within it
        var dot = Vector3.Dot(source.forward, toTarget);
        if (dot < Mathf.Cos(fov))
        {
          return true;
        }
        return false;
      }

      /// <summary>
      /// Checks whether there is a direct line of sight between the source and the target
      /// </summary>
      /// <param name="source"></param>
      /// <param name="target"></param>
      /// <param name="range"></param>
      /// <returns></returns>
      public static bool CheckLineOfSight(Transform source, Transform target, float range)
      {
        return Physics.Raycast(source.position, source.position - target.position, range);
      }

      /// <summary>
      /// Draws a field of view using Unity's 3D GUI
      /// </summary>
      /// <param name="source"></param>
      /// <param name="fov"></param>
      /// <param name="range"></param>
      public static void DrawFieldOfView(Transform source, float fov, float range, Color color)
      {
        #if UNITY_EDITOR
        UnityEditor.Handles.color = color;
        Vector3 vec = Quaternion.Euler(0f, -fov / 2f, 0f) * source.forward;
        UnityEditor.Handles.DrawSolidArc(source.position, source.up, vec, fov, range);
        #endif
      }

      /// <summary>
      /// Draws the range of an agent from its center as a disk
      /// </summary>
      /// <param name="source"></param>
      /// <param name="range"></param>
      /// <param name="color"></param>
      public static void DrawRange(Transform source, float range, Color color)
      {
        #if UNITY_EDITOR
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawSolidDisc(source.position, Vector3.up, range);
        #endif
      }


      /// <summary>
      /// Given an array of available targets, finds the nearest or farthest target 
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="source">The source object that is looking for targets</param>
      /// <param name="targets">An array of possible targets</param>
      /// <param name="relativeRange">Whether we are looking for the nearest or farthest target</param>
      /// <param name="range">The maximum range between the source and any given target</param>
      /// <returns></returns>
      public static T FindTarget<T>(Transform source, T[] targets, RelativeRange relativeRange, float range = Mathf.Infinity) where T : MonoBehaviour
      {
        // Calculate distances to all targets
        var distances = new Dictionary<float, T>();
        foreach (var target in targets)
        {
          var dist = Vector3.Distance(source.position, target.transform.position);
          // If the target is within range of the source
          if (dist <= range)
            distances.Add(dist, target);
        }

        // BRANCH:
        // a.) Find the nearest target
        if (relativeRange == RelativeRange.Nearest)
        {
          float nearestDist = Mathf.Infinity;
          T nearestTarget = null;
          foreach (var target in distances)
          {
            if (target.Key < nearestDist)
              nearestTarget = target.Value;
          }
          return nearestTarget;
        }

        // b.) Find the farthest target
        else if (relativeRange == RelativeRange.Farthest)
        {
          float farthestDist = Mathf.NegativeInfinity;
          T farthestTarget = null;
          foreach (var target in distances)
          {
            if (target.Key > farthestDist)
            {
              farthestTarget = target.Value;
            }
          }
          return farthestTarget;
        }

        return null;
      }

      /// <summary>
      /// Finds the nearest target from the source
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="source"></param>
      /// <param name="targets"></param>
      /// <param name="range"></param>
      /// <returns></returns>
      public static T FindNearestTarget<T>(Transform source, T[] targets, float range = Mathf.Infinity) where T : MonoBehaviour
      {
        // Calculate distances to all targets
        var distances = CalculateDistances(source, targets, range);
        // Find the nearest target
        float nearestDist = Mathf.Infinity;
        T nearestTarget = null;
        foreach (var target in distances)
        {
          if (target.Key < nearestDist)
            nearestTarget = target.Value;
        }
        return nearestTarget;
      }

      /// <summary>
      /// Finds the farthest target from the source
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="source"></param>
      /// <param name="targets"></param>
      /// <param name="range"></param>
      /// <returns></returns>
      public static T FindFarthestTarget<T>(Transform source, T[] targets, float range = Mathf.Infinity) where T : MonoBehaviour
      {
        // Calculate distances to all targets
        var distances = CalculateDistances(source, targets, range);
        // Find the farthest target
        float farthestDist = Mathf.NegativeInfinity;
        T farthestTarget = null;
        foreach (var target in distances)
        {
          if (target.Key > farthestDist)
          {
            farthestTarget = target.Value;
          }
        }
        return farthestTarget;

      }


      /// <summary>
      /// Finds the distances between the source and every available target
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="source"></param>
      /// <param name="targets"></param>
      /// <param name="range"></param>
      /// <returns>A dictionary of float/target pairs.</returns>
      public static List<KeyValuePair<float, T>> CalculateDistances<T>(Transform source, T[] targets, float range = Mathf.Infinity) where T : MonoBehaviour
      {
        // Calculate distances to all targets
        var distances = new List<KeyValuePair<float, T>>();
        foreach (var target in targets)
        {
          //Trace.Script("target = " + target.name);
          var dist = Vector3.Distance(source.position, target.transform.position);
          // If the target is within range of the source
          if (dist <= range)
            distances.Add(new KeyValuePair<float, T>(dist, target));
        }
        return distances;
      }


    }
  }

}