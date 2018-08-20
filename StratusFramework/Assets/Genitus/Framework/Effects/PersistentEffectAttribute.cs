using UnityEngine;
using Stratus;
using System;

namespace Genitus.Effects
{
  public abstract class PersistentEffectAttribute : EffectAttribute
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public float duration = 1.0f;
    public int ticks = 1;
    public bool stackable = false;
    public int stacks = 3;
    public bool beneficial = false;
    public bool removable = true;

    //------------------------------------------------------------------------/
    // Virtual Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Invoked once when the effect has started.
    /// </summary>    
    protected abstract void OnStarted(CombatController caster, CombatController target);
    /// <summary>
    /// Invoked once when the effect has ended.
    /// </summary>
    protected abstract void OnEnded(CombatController caster, CombatController target);
    /// <summary>
    /// Called on every tick of the effect.
    /// </summary>
    protected abstract void OnPersisted(CombatController caster, CombatController target);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Applies the persistent effect to the target.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    protected override void OnApply(CombatController caster, CombatController target)
    {
      Trace.Script("Applying " + GetType().Name);

      // Adds the effect to the target     
      var effect = new PersistentEffect(this, caster, target);
      var applyEvent = new PersistentEffect.ApplyEvent();
      applyEvent.Effect = effect;
      target.gameObject.Dispatch<PersistentEffect.ApplyEvent>(applyEvent);
      // If the effect is stackable, add it to the stack count to that target's effects instead
      // If not stackable and present, reset its duration (if it can be)       
    }

    public void Started(CombatController caster, CombatController target)
    {
      this.OnStarted(caster, target);
    }

    public void Persisted(CombatController caster, CombatController target)
    {
      this.OnPersisted(caster, target);
    }

    public void Ended(CombatController caster, CombatController target)
    {
      this.OnEnded(caster, target);
    }

  }

  /// <summary>
  /// An instantiation of a persistent effect on a target.
  /// </summary>
  public class PersistentEffect
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public class ApplyEvent : Stratus.Event { public PersistentEffect Effect; }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    CombatController user;
    CombatController target;
    PersistentEffectAttribute effect;
    public float duration;
    public int stacks = 1;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public PersistentEffect(PersistentEffectAttribute effect,
                            CombatController caster,
                            CombatController target)
    {
      this.effect = effect;
      user = caster;
      this.target = target;

      this.duration = this.effect.duration;
    }

    public void Start()
    {
      effect.Started(user, target);
    }
    public void Persist()
    {
      effect.Persisted(user, target);  
    }
    public void End()
    {
      effect.Ended(user, target);
    }

  }

}