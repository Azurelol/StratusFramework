/******************************************************************************/
/*!
@file   TransformExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;

namespace Stratus
{
  public static partial class Extensions
  {
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
    /// Calculates a position on a given normalized direction vector from the transform's position.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="normalizedDirVec"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector3 CalculatePositionAtDirection(this Transform transform, Vector3 normalizedDirVec, float distance)
    {
      return transform.position + (normalizedDirVec * distance);
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

    /// <summary>
    /// Returns a container of all the children of this transform.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns>A container of all the children of this transform.</returns>
    public static Transform[] Children(this Transform transform)
    {
      var children = new List<Transform>();
      ListChildren(transform, children);
      return children.ToArray();
    }

    /// <summary>
    /// Returns a container of all the children of this transform.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns>A container of all the children of this transform.</returns>    
    public static Transform[] Children(this Transform transform, int depth)
    {
      var children = new List<Transform>();
      ListChildren(transform, children, ref depth);
      return children.ToArray();
    }

    //public static Transform[] ImmediateChildren(this Transform transform)
    //{
    //  var children = new List<Transform>();
    //  foreach (Transform child in transform)
    //  {
    //    children.Add(child);
    //  }
    //  return children.ToArray();
    //}

    static void ListChildren(Transform obj, List<Transform> children)
    {
      foreach (Transform child in obj.transform)
      {
        children.Add(child);
        ListChildren(child, children);
      }
    }

    static void ListChildren(Transform obj, List<Transform> children, ref int depth)
    {
      foreach (Transform child in obj.transform)
      {
        children.Add(child);
        if (depth > 0)
        {
          depth--;
          ListChildren(child, children, ref depth);
        }
      }
    }

  }
}
