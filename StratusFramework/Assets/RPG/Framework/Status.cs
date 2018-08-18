/******************************************************************************/
/*!
@file   Status.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;

namespace Altostratus
{
  /// <summary>
  /// A status is a persistent condition on a target.
  /// </summary>
  [CreateAssetMenu(fileName = "Status", menuName = "Prototype/Status")]
  public class Status : ScriptableObject
  {
    //------------------------------------------------------------------------/
    public abstract class StatusEvent : Stratus.Event { public Status Status; }
    public class StartedEvent : StatusEvent {}
    public class EndedEvent : StatusEvent {}
    //------------------------------------------------------------------------/
    public string Name;
    public string Description;
    public float Duration;
    /// <summary>
    /// The number of maximum stacks possible on this status.
    /// </summary>
    public int Stacks = 1;
    /// <summary>
    /// Whether this status can be stacked or not.
    /// </summary>
    public bool IsStackable { get { if (Stacks > 1) return true; return false; } }
    //public bool Diminishing;
    public List<PersistentEffectAttribute> Effects = new List<PersistentEffectAttribute>();
    //------------------------------------------------------------------------/
    //protected abstract void OnStarted(CombatController target);
    //protected abstract void OnEnded(CombatController target);
    //protected abstract void OnPersist(CombatController target);
    //------------------------------------------------------------------------/
    /// <summary>
    /// Adds the status onto the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="timeStep"></param>
    /// <returns>True if the status has ended, false otherwise. </returns>
    public void Apply(CombatController caster, CombatController target)
    {      
      var applyEvent = new CombatController.StatusAppliedEvent();
      applyEvent.Status = new Instance(this, caster, target);
      target.gameObject.Dispatch<CombatController.StatusAppliedEvent>(applyEvent);
    }
    
    /// <summary>
    /// Applies every effect's initial condition to the target
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    public void Started(CombatController caster, CombatController target)
    {
      foreach(var effect in Effects)
      {
        effect.Started(caster, target);
      }
    }

    /// <summary>
    /// Applies every effect's persistent condition to the target
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    public void Persisted(CombatController caster, CombatController target)
    {
      foreach (var effect in Effects)
      {
        effect.Persisted(caster, target);
      }
    }

    /// <summary>
    /// Applies every effect's final condition to the target
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    public void Ended(CombatController caster, CombatController target)
    {
      foreach (var effect in Effects)
      {
        effect.Ended(caster, target);
      }
    }

    /**************************************************************************
     **************************************************************************/

    /// <summary>
    /// A runtime instance of an active status condition, present on a target.
    /// </summary>
    public class Instance
    {
      public Status Status;
      public string Name { get { return Status.Name; } }
      public float Duration;
      public int Stacks; public int MaximumStacks;
      public CombatController Caster;
      public CombatController Target;
      //----------------------------------------------------------------------/
      public Instance(Status status, CombatController caster, CombatController target)
      {
        Status = status;
        Duration = Status.Duration;
        MaximumStacks = Status.Stacks;
        Caster = caster;
        Target = target;
      }

      /// <summary>
      /// Starts this status, applying all its initial effects.
      /// </summary>
      public void Start()
      {
        this.Status.Started(Caster, Target);
      }

      /// <summary>
      /// Persists this status, applying all its persistent effects.
      /// </summary>
      /// <param name="step"></param>
      public bool Persist(float step)
      {
        this.Status.Persisted(Caster, Target);

        // Run its duration
        this.Duration -= step;
        Trace.Script(this.Name + ": Duration left = " + this.Duration);
        if (this.Duration <= 0.0f)
        {
          // The status has run its duration
          return true;
          //this.End();
        }

        return false;
      }

      /// <summary>
      /// Ends this status, applying all its final effects.
      /// </summary>
      public void End()
      {
        this.Status.Ended(Caster, Target);
      }

      /// <summary>
      /// Adds stacks to this status.
      /// </summary>
      public void Stack(int stacks = 1)
      {
        if (Stacks >= MaximumStacks)
          return;

        Stacks += stacks;
      }

      /// <summary>
      /// Resets the duration of this status.
      /// </summary>
      public void Reset()
      {
        Duration = Status.Duration;
      }

    }

  }

}