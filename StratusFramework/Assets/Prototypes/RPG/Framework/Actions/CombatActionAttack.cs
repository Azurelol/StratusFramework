/******************************************************************************/
/*!
@file   CombatActionAttack.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  ///**************************************************************************/
  ///*!
  //@class CombatActionAttack 
  //*/
  ///**************************************************************************/
  //public class CombatActionAttack : CombatAction
  //{
  //  public Skill AttackSkill; // = (Skill)ScriptableObject.CreateInstance(typeof(Skill));

  //  static float AttackCastTime = 0.1f;
  //  public override string Description { get { return "Attack"; } }

  //  public CombatActionAttack(CombatController user, CombatController target) : base(user, target, AttackCastTime, user.Range.Current) 
  //  {
  //    // Construct the attack skill
  //    this.AttackSkill = (Skill)ScriptableObject.CreateInstance(typeof(Skill));
  //    // Apply damage
  //    this.AttackSkill.Effects.Add((DamageEffect)ScriptableObject.CreateInstance(typeof(DamageEffect)));
  //    // Apply a slight knockback
  //    this.AttackSkill.Effects.Add((PushEffect)ScriptableObject.CreateInstance(typeof(PushEffect)));
  //  }

  //  protected override void OnExecute(CombatController caster, CombatController target)
  //  {
  //    AttackSkill.Cast(caster, target);
      
    
      
  //  }
  //}

}