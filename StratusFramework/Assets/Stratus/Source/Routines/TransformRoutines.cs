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

namespace Stratus
{
  public static partial class Routines
  {
    public static IEnumerator RotateAround(Transform transform, Transform pivot, float angle, float duration)
    {
      float t = 0f;
      while (t <= 1f)
      {
        t += Time.fixedDeltaTime / duration;

        
        yield return new WaitForFixedUpdate();
      }
    }
  }
}
