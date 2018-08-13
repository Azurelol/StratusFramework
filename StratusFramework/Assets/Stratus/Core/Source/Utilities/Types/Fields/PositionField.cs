using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// Used to select a target position
  /// </summary>
  [Serializable]
  public class PositionField
  {
    public enum Type { Transform, Vector }
    
    [SerializeField] private Type type;
    [SerializeField] private Transform transform;
    [SerializeField] private Vector3 point;

    public static implicit operator Vector3(PositionField positionField)
    {
      return (positionField.type == Type.Transform) ? positionField.transform.position : positionField.point;
    }

    public void Set(Vector3 point)
    {
      this.point = point;
    }

    public void Set(Transform transform)
    {
      this.transform = transform;
    }

    public override string ToString()
    {
      if (type == Type.Transform && transform)
        return $"Position = {transform.name} {transform.position}";
      else if (type == Type.Vector)
        return $"Position = {point}";

      return base.ToString();
    }
  }

}