
using UnityEngine;
using System.Collections;
using Stratus;

namespace Genitus.Effects
{

  public abstract class ProportionalHealthModificationAttribute : EffectAttribute
  {
    [Range(0, 100)]
    public int percentage = 100;

  }

}