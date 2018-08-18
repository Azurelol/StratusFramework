/******************************************************************************/
/*!
@file   KineticEffect.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;

namespace Altostratus 
{
  public abstract class KineticEffectAttribute : EffectAttribute 
  {
    public float Amount = 3.0f;

    public override void OnInspect()
    {
      Amount = EditorBridge.Field("Amount", Amount);
    }

  }
}
