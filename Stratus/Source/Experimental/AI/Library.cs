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
      public static bool CheckDistance(Transform source, Transform target, float range)
      {
        return CheckDistance(source, target.position, range);
      }

      /// <summary>
      /// Checks whether the target is in range
      /// </summary>
      /// <param name="source">The source object</param>
      /// <param name="target">The target position</param>
      /// <param name="range">The range at which we are checking</param>
      /// <returns></returns>
      public static bool CheckDistance(Transform source, Vector3 target, float range)
      {
        float dist = Vector3.Distance(target, source.position);
        bool inRange = (dist <= range);
        //Trace.Script("Distance = " + dist);
        return inRange;
      }

      /// <summary>
      /// Checks whether the target is in the field of view of the owner.
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