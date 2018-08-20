using UnityEngine;
using Stratus;
using System;

namespace Genitus.Effects
{
  /// <summary>
  /// Launches the target into the air.
  /// </summary>
  public class LaunchEffect : StunEffect
  {    
    public class StartedEvent { public float height; }
    public class EndedEvent {}

    public float height = 10.0f;

    protected override void OnStarted(CombatController caster, CombatController target)
    {
      base.OnStarted(caster, target);
      // Launch it up?
      target.gameObject.Dispatch<KineticAction.LaunchEvent>(new KineticAction.LaunchEvent(caster.transform, this.height));
      //Trace.Script("Applying launch effect to " + target);
      // Disable gravity?
      //target.GetComponent<Rigidbody>().useGravity = false;
    }

    protected override void OnEnded(CombatController caster, CombatController target)
    {
      base.OnEnded(caster, target);
      target.gameObject.Dispatch<Status.EndedEvent>(new Status.EndedEvent());
      // Down!
      //target.GetComponent<Rigidbody>().useGravity = true;
    }


  }

}