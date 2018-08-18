/******************************************************************************/
/*!
@file   PersistentEffectAttribute.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Altostratus
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class PersistentEffectAttribute : EffectAttribute
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public float Duration = 1.0f;
    public int Ticks = 1;
    public bool Stackable = false;
    public int Stacks = 3;
    public bool Beneficial = false;
    public bool Removable = true;

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
    public override void OnInspect()
    {
      EditorBridge.BeginHorizontal();
      Duration = EditorBridge.Field("Duration", Duration);
      Ticks = EditorBridge.Field("Ticks", Ticks);
      EditorBridge.EndHorizontal();
    }

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
    public class ApplyEvent : Stratus.Event { public PersistentEffect Effect; }
    //------------------------------------------------------------------------/
    CombatController Caster;
    CombatController Target;
    PersistentEffectAttribute Effect;
    public float Duration;
    public int Stacks = 1;
    //------------------------------------------------------------------------/
    public PersistentEffect(PersistentEffectAttribute effect,
                            CombatController caster,
                            CombatController target)
    {
      Effect = effect;
      Caster = caster;
      Target = target;

      Duration = Effect.Duration;
    }

    public void Start()
    {
      Effect.Started(Caster, Target);
    }
    public void Persist()
    {
      Effect.Persisted(Caster, Target);  
    }
    public void End()
    {
      Effect.Ended(Caster, Target);
    }

  }

}