/******************************************************************************/
/*!
@file   StunEffect.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class StunEffect 
  */
  /**************************************************************************/
  public class StunEffect : PersistentEffectAttribute
  {

    protected override void OnStarted(CombatController caster, CombatController target)
    {
      target.gameObject.Dispatch<CombatController.PauseEvent>(new CombatController.PauseEvent());
    }

    protected override void OnEnded(CombatController caster, CombatController target)
    {
      target.gameObject.Dispatch<CombatController.ResumeEvent>(new CombatController.ResumeEvent());
    }

    protected override void OnPersisted(CombatController caster, CombatController target)
    {
      
    }
  
  }
}