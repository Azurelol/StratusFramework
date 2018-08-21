using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Genitus
{
  /// <summary>
  /// Base class for all additional components a skill may have
  /// </summary>
  [Serializable]
  public abstract class SkillComponent 
  {
    protected abstract void OnActivation(CombatController user, CombatController target);
  }


  //public interface SkillAudioVisualEffects
  //{    
  //}
  //
  ///// <summary>
  ///// A component that handles target acquisition for a skill
  ///// </summary>
  //public interface SkillTargetingComponent
  //{
  //  CombatController[] GetTargets(CombatController user, CombatController target);
  //}

  [Serializable]
  public class StandardSkillVFX : SkillComponent
  {
    public GameObject particleSystem;
    public AudioClip sound;

    protected override void OnActivation(CombatController user, CombatController target)
    {      
    }
  }

  [Serializable]
  public class SkillTiming : SkillComponent
  {
    [Tooltip("Specific timings for the skill's action")]
    public CombatAction.Timings timings = new CombatAction.Timings();

    protected override void OnActivation(CombatController user, CombatController target)
    {      
    }
  }

  [Serializable]
  public class SkillTrigger : SkillComponent
  {
    /// <summary>
    /// Trigger used by the caster when using this skill
    /// </summary>
    [Tooltip("Trigger used by the caster when using this skill")]
    public CombatTrigger onCast;
    /// <summary>
    /// Trigger used by the target when receiving this skill
    /// </summary>
    [Tooltip("Trigger used by the caster when defending from this skill")]
    public CombatTrigger onDefend;
    /// <summary>
    /// Whether this skill has triggers set
    /// </summary>
    /// 
    public bool isTriggered { get { return (onCast.Enabled || onDefend.Enabled); } }

    protected override void OnActivation(CombatController user, CombatController target)
    {
      throw new System.NotImplementedException();
    }
  }
  
  [Serializable]
  public class SkillTelegraph : SkillComponent
  {
    public int number;
    /// <summary>
    /// Whether the skill is telegraphed
    /// </summary>
    [Tooltip("Whether the skill is telegraphed")]
    public bool telegraphed = true;
    /// <summary>
    /// How the skill is telegraphed.
    /// </summary>
    [Tooltip("How the skill is telegraphed")]
    //[DrawIf("IsTelegraphed", true, ComparisonType.Equals, PropertyDrawingType.DontDraw)]
    public Telegraph.Configuration telegraph;

    protected override void OnActivation(CombatController user, CombatController target)
    {
      throw new System.NotImplementedException();
    }
  }

}