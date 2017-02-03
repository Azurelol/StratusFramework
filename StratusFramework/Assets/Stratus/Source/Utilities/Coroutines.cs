/******************************************************************************/
/*!
@file   Coroutines.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{
  public static class Routines
  {
    public static IEnumerator WaitForRealSeconds(float time)
    {
      float start = Time.realtimeSinceStartup;
      while (Time.realtimeSinceStartup < start + time)
      {
        yield return null;
      }
    }
  }

}
