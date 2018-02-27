using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Allows a broader targeting method for GameObjects
  /// </summary>
  [Serializable]
  public class GameObjectField
  {
    /// <summary>
    /// Specifies what targeting mechanism to use
    /// </summary>
    public enum Type
    {
      [Tooltip("Check against a specific GameObject")]
      GameObject,
      [Tooltip("Check against a specific layer")]
      Layer,
      [Tooltip("Check against a specific tag")]
      Tag,
      [Tooltip("Check against a specific name")]
      Name
    }

    //[SerializeField]
    [Tooltip("How to pick the target")]
    public Type type = Type.GameObject;

    // Different targets
    [SerializeField]
    private GameObject gameObject;
    [SerializeField]
    private TagField tag;
    [SerializeField]
    private LayerField layer;
    [SerializeField]
    private string name;

    /// <summary>
    /// Checks whether the given GameObject is the given target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsTarget(GameObject target)
    {
      bool isTarget = false;
      switch (type)
      {
        case Type.GameObject: 
          if (target == gameObject) isTarget = true;
          break;
        case Type.Layer:
          if (layer.Matches(target)) isTarget = true;
          break;
        case Type.Tag:
          if (tag.Matches(target)) isTarget = true;
          break;
        case Type.Name:
          if (target.name == name) isTarget = true;
          break;
        default:
          break;
      }
      return isTarget;
    }

    /// <summary>
    /// Gets the layer of the current target, if possible. If not possible, returns -1.
    /// </summary>
    /// <returns></returns>
    public bool GetLayer(ref int layer)
    {
      if (type == Type.GameObject && gameObject != null)
      {
        layer = gameObject.layer;
        return true;
      }
      else if (type == Type.Layer)
      {
        layer = this.layer;
        return true;
      }

      return false;
    }
    


  }

}