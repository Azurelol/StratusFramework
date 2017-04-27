/******************************************************************************/
/*!
@file   MoveToRoutine.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Stratus
{
  public class MoveToRoutine : PositionRoutine
  {
    public Vector3 Target;

    public void Set(Vector3 target, float duration, UpdateMode mode)
    {
      this.Target = target;
      this.Duration = duration;
      this.Mode = mode;
    }

    protected override void Translate(float t)
    {
      transform.position = Vector3.MoveTowards(transform.position, this.Target, t);
    }
  }
}
