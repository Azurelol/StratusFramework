/******************************************************************************/
/*!
@file   RigidBodyRoutines.cs
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
    public static IEnumerator AddForce(Rigidbody rigidBody, Vector3 force, ForceMode mode, float duration)
    {
      float timeElapsed = 0f;
      while (timeElapsed <= duration)
      {
        timeElapsed += Time.deltaTime;
        rigidBody.AddForce(force, mode);
        yield return new WaitForFixedUpdate();
      }
    }

  }
}
