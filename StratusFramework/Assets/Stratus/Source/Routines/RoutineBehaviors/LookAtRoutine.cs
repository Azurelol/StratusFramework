/******************************************************************************/
/*!
@file   LookAtRoutine.cs
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
  public class LookAtRoutine : RotationRoutine
  {
    private Transform Target;

    public void Set(Transform target, float duration, UpdateMode mode)
    {
      this.Target = target;
      this.Duration = duration;
      this.Mode = mode;
    }

    protected override void Rotate(float t)
    {
      var lookAtVec = Target.transform.position - this.transform.position;
      var rot = Quaternion.LookRotation(lookAtVec);
      transform.rotation = Quaternion.Lerp(transform.rotation, rot, t);
      //transform.forward = Vector3.Lerp(transform.forward, lookAtVec, t);
    }
  }
}
