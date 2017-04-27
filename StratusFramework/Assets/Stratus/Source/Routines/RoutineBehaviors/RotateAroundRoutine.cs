/******************************************************************************/
/*!
@file   RotateAroundRoutine.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using System;
using UnityEngine;
using System.Collections;

namespace Stratus
{
  /// <summary>
  /// Rotates around a given pivot
  /// </summary>
  public class RotateAroundRoutine : RotationRoutine
  {
    private Vector3 Pivot;
    private Vector3 Axis;
    private float Angle;
    private float AngularSpeed;
    private float AngleTraveled;
    private float elapsedT;

    public void Set(Vector3 pivot, Vector3 axis, float angle, float duration, UpdateMode mode)
    {
      this.Pivot = pivot;
      this.Axis = axis;
      this.Angle = angle;
      this.Duration = duration;
      this.AngularSpeed = (this.Angle / this.Duration);
      this.Mode = mode;
      this.elapsedT = 0;
    }
    

    protected override void Rotate(float t)
    {
      //var nextAngle = 1f - (Angle * t);
      //Trace.Script("Next Angle = " + nextAngle);
      //transform.RotateAround(this.Pivot.position, Vector3.up, this.AngularSpeed);
      
      float time = Time.fixedDeltaTime;
      this.elapsedT += time;
      if (this.elapsedT >= this.Duration)
        time = time - (this.elapsedT - this.Duration);
      var nextAngle = this.AngularSpeed * time;
      
      //Trace.Script("Next Angle = " + nextAngle + ", Traveled = " + this.AngleTraveled);
      transform.RotateAround(this.Pivot, this.Axis, nextAngle);
    }
  }

  //public static partial class Routines
  //{
  //  //public static IEnumerator 
  //  //
  //  public static IEnumerator RotateAround(Transform transform, Transform pivot, float angle, float duration)
  //  {
  //  
  //  }
  //}

}
