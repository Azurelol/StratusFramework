
using UnityEngine;
using System.Collections;
using Stratus;

namespace Genitus
{

  public abstract class ProportionalHealthModificationAttribute : EffectAttribute
  {
    [Range(0, 100)] public int Percentage = 100;

    public override void OnInspect()
    {
      this.Percentage = EditorBridge.Field("Percentage", this.Percentage);
    }

  }

}