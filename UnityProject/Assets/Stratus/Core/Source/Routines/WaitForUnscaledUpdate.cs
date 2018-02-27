/******************************************************************************/
/*!
@file   WaitForUnscaledUpdate.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  public class WaitForUnscaledUpdate : CustomYieldInstruction
  {
    private float WaitTIme;

    public override bool keepWaiting
    {
      get
      {
        return Time.realtimeSinceStartup < WaitTIme;
      }
    }

    public WaitForUnscaledUpdate()
    {
      //WaitTIme += Time.fixedDeltaTime;
      WaitTIme = Time.realtimeSinceStartup + Time.fixedDeltaTime;
    }
  }
}
