/******************************************************************************/
/*!
@file   Skills.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System;
using Stratus;

namespace Prototype
{
  /// <summary>
  /// The collection of equipped skills on a combat controller
  /// </summary>
  [Serializable]
  public class Skills
  {
    /// <summary>
    /// A real-time representation of a skill, including its cooldown and
    /// ways to validate whether it can be cast.
    /// </summary>
    [Serializable]
    public class EquippedSkill
    {
      //----------------------------------------------------------------------/
      public Skill Data;
      public float CooldownLeft;
      public string Name { get { return Data.Name; } }
      //----------------------------------------------------------------------/
      /// <summary>
      /// Reduces the cooldown of this skill.
      /// </summary>
      /// <param name="timeStep"></param>
      public void ReduceCooldown(float timeStep)
      {
        if (CooldownLeft == 0.0f)
          return;
        
        //Trace.Script("Cooldown left on " + Data.Name + " = " + CooldownLeft);
        CooldownLeft -= timeStep;
        if (CooldownLeft < 0.0f) CooldownLeft = 0.0f;
      }

      /// <summary>
      /// Validates whether the skill can be cast by the caster.
      /// </summary>
      /// <param name="caster"></param>
      /// <returns></returns>
      public bool Validate(CombatController caster)
      {
        // If it's on cooldown...
        if (CooldownLeft > 0.0f)
        {
          //if (caster.Tracing) Trace.Script(Data.Name + " still on cooldown for " + CooldownLeft + " seconds!", caster);
          return false;
        }
        // If the caster does not have stamina to cast it...
        if (caster.stamina.current < Data.Cost)
        {
          if (caster.logging) Trace.Script("Not enough stamina to cast " + Data.Name, caster);
          return false;
        }

        // Otherwise, it can be cast!
        return true;
      }

      /// <summary>
      /// Casts the equipped skill on the target.
      /// </summary>
      /// <param name="caster">The user of this skill.</param>
      /// <param name="target">The target of this skill.</param>
      public void Cast(CombatController caster, CombatController target, Telegraph telegraph)
      {
        //Trace.Script("Casting + " + Skill.Name);
        this.CooldownLeft = this.Data.Cooldown;
        caster.stamina.Reduce(this.Data.Cost);
        Data.Cast(caster, target, telegraph);
      }

      /// <summary>
      /// Describes this skill.
      /// </summary>
      /// <returns></returns>
      public string Describe()
      {
        return Data.Describe();
      }

    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class SkillSelectedEvent : Stratus.Event { public EquippedSkill Skill; }
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The default attack skill.
    /// </summary>
    public EquippedSkill Attack;
    /// <summary>
    /// All available skills.
    /// </summary>
    public List<EquippedSkill> All = new List<EquippedSkill>();
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/ 
    /// <summary>
    /// Sets the default attack skill.
    /// </summary>
    /// <param name="attackSkill"></param>
    public void SetAttack(Skill attackSkill)
    {
      Attack = new EquippedSkill();
      Attack.Data = attackSkill;
    }
              
    /// <summary>
    /// Adds a skill.
    /// </summary>
    /// <param name="skill"></param>
    public void Add(Skill skill)
    {
      var equipped = new EquippedSkill();
      equipped.Data = skill;
      All.Add(equipped);
    }

    /// <summary>
    /// Validates whether a skill can be cast, by checking both its cooldown and
    /// whether the combatcontroller has enough stamina to cast it.
    /// </summary>
    /// <param name="skill">The skill which to validate.</param>
    /// <param name="caster">The caster of the skill.</param>
    /// <returns>True if the skill is both present and can be cast by the user,
    ///          other wise false.</returns>
    public bool Validate(CombatController caster, Skill skill)
    {
      var equippedSkill = All.Find( x=> x.Data == skill);
      
      // If the skill could not be found..
      if (equippedSkill == null)
        return false;

      return equippedSkill.Validate(caster);
    }

    /// <summary>
    /// Looks for the skill among the list of available skills.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public EquippedSkill Find(CombatController user, Skill skill)
    {
      return All.Find(x => x.Data == skill);
    }

    /// <summary>
    /// Finds the skill among available skills by name.
    /// </summary>
    /// <param name="name">The name of the skill.</param>
    /// <returns></returns>
    public EquippedSkill Find(string name)
    {
      return All.Find(x => x.Name == name);
    }

    /// <summary>
    /// Prints all available skills to the console.
    /// </summary>
    public void Print()
    {
      foreach (var equippedSkill in All)
      {
        Trace.Script(equippedSkill.Data.Name + " is available!");
      }
    }

    /// <summary>
    /// Updates every equipped skill. This will reduce cooldowns.
    /// </summary>
    /// <param name="step"></param>
    public void Update(float step)
    {
      foreach (var entry in All)
      {
        entry.ReduceCooldown(step);
      }
    }

  }

}