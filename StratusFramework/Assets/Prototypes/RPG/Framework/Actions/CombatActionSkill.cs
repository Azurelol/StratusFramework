/******************************************************************************/
/*!
@file   CombatActionSkill.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using System;


namespace Prototype
{

  public class CombatActionSkill : CombatAction
  { 

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The selected skill
    /// </summary>
    public Skills.EquippedSkill Skill;
    /// <summary>
    /// The active instance of the telegraph
    /// </summary>
    private Telegraph Telegraph;
    /// <summary>
    /// When a telegraph is stopped, we record its last position
    /// </summary>
    private Vector3 LastTelegraphPosition;

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    /// <summary>
    /// Constructor that takes an equipped skill; used by the player.
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public CombatActionSkill(Skills.EquippedSkill skill, CombatController user, CombatController target)
  : base(user, target, skill.Data.Range, skill.Data.Timings)
    {
      this.Skill = skill;
    }

    public override string description { get { return this.Skill.Name; } }

    public override string name { get { return this.Skill.Name; } }
    /// <summary>
    /// Starts casting the skill
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    protected override void OnStart(CombatController user, CombatController target)
    {
      if (!Skill.Data.IsTelegraphed)
        return;
      
      this.StartTelegraph();
    }

    /// <summary>
    /// What to do while the skill is casting
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="step"></param>
    protected override void OnCasting(CombatController user, CombatController target, float step)
    {
      //Trace.Script("Casting!");
      Telegraph.Place(CalculatePlacement());
      //if (Skill.Data.Telegraphing.Delivery == Telegraph.Delivery.Projection)
      //{
        //Trace.Script("Placing telegraph!", user);
      //}

    }

    protected override void OnTrigger(CombatController user, CombatController target)
    {
      //Trace.Script("Stopping telegraph!");
      this.StopTelegraph(false);
    }

    /// <summary>
    /// Executes the specified skill on the given target. This is called
    /// after the action has finished its cast time.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    protected override void OnExecute(CombatController user, CombatController target)
    {
      if (Skill != null)
        this.Skill.Cast(user, target, this.Telegraph);

      this.SpawnParticles();
    }

    protected override void OnEnd()
    {
      if (!Telegraph)
        return;

      //Trace.Script("Ending telegraph!");
      this.StopTelegraph(true);
      this.Telegraph = null;
    }
    
    /// <summary>
    /// If the skill is cancelled, stop telegraphing it
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    protected override void OnCancel()
    {
      //this.StopTelegraph(true);
      //if (!Telegraph)
      //  return;
      //
      //Telegraph.End(0f, 0.1f);
      //this.Telegraph = null;
    }

    //------------------------------------------------------------------------/
    // Routines
    //------------------------------------------------------------------------/
    void StartTelegraph()
    {
      Telegraph = Telegraph.Construct(this.Skill.Data.Telegraphing, CalculatePlacement());
      Telegraph.Start(this.Timers.Cast);
      //Trace.Script("Starting telegraph");
    }

    void StopTelegraph(bool instant)
    {
      if (!Telegraph)
        return;

      LastTelegraphPosition = Telegraph.transform.position;

      if (instant)
        Telegraph.End(0f, 0.1f);
      else
        Telegraph.End(Timers.Trigger, Timers.Trigger + Timers.Execute + Timers.End);
      
      //Telegraph = null;
    }

    Telegraph.Placement CalculatePlacement()
    {
      var placement = new Telegraph.Placement();      
      placement.Source = User.transform;
      placement.Target = Target.transform.position;
      placement.Rotation = new Vector3(User.transform.forward.x, 0.0f, User.transform.forward.z);
      return placement;
    }

    /// <summary>
    /// Spawns particles for the skill if there's any
    /// </summary>
    void SpawnParticles()
    {
      //if (Skill.Data.particles != null)
      //{
      //  AudioVisualEffect.Spawn(Skill.Data.Particles, LastTelegraphPosition);
      //}
    }
    
  }

}