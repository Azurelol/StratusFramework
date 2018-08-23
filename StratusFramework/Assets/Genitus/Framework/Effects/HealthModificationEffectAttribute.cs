using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Genitus.Effects
{
  public abstract class HealthModificationEffectAttribute : EffectAttribute
  {
    [Range(0, 1)]
    public float potency = 100.0f;
  }

}