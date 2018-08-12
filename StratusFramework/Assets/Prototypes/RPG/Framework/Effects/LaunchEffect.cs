/******************************************************************************/
/*!
@file   LaunchEffect.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /// <summary>
  /// Launches the target into the air.
  /// </summary>
  public class LaunchEffect : StunEffect
  {    
    public class StartedEvent { public float Height; }
    public class EndedEvent {}

    public float Height = 10.0f;

    public override void OnInspect()
    {      
      Height = EditorBridge.Field("Height", Height);
    }

    protected override void OnStarted(CombatController caster, CombatController target)
    {
      base.OnStarted(caster, target);
      // Launch it up?
      target.gameObject.Dispatch<KineticAction.LaunchEvent>(new KineticAction.LaunchEvent(caster.transform, this.Height));
      //Trace.Script("Applying launch effect to " + target);
      // Disable gravity?
      //target.GetComponent<Rigidbody>().useGravity = false;
    }

    protected override void OnEnded(CombatController caster, CombatController target)
    {
      base.OnEnded(caster, target);
      target.gameObject.Dispatch<CombatController.StatusEndedEvent>(new CombatController.StatusEndedEvent());
      // Down!
      //target.GetComponent<Rigidbody>().useGravity = true;
    }


  }

}