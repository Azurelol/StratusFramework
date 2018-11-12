using UnityEngine;
using System.Collections.Generic;
using Stratus;

namespace Genitus
{
  /// <summary>
  /// A status is a persistent condition on a target.
  /// </summary>
  [CreateAssetMenu(fileName = "Status", menuName = "Prototype/Status")]
  public class Status : StratusScriptable
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public abstract class StatusEvent : Stratus.StratusEvent { public Status.Instance status; }
    public class StartedEvent : StatusEvent {}
    public class EndedEvent : StatusEvent {}

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public string descriptiion;
    public float duration;
    public int stacks = 1;
    public List<Effects.PersistentEffectAttribute> effects = new List<Effects.PersistentEffectAttribute>();

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this status can be stacked or not.
    /// </summary>
    public bool IsStackable { get { if (stacks > 1) return true; return false; } }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Adds the status onto the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="timeStep"></param>
    /// <returns>True if the status has ended, false otherwise. </returns>
    public void Apply(CombatController caster, CombatController target)
    {      
      var startedEvent = new StartedEvent();
      startedEvent.status = new Instance(this, caster, target);
      target.gameObject.Dispatch<StartedEvent>(startedEvent);
    }
    
    /// <summary>
    /// Applies every effect's initial condition to the target
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    public void Started(CombatController caster, CombatController target)
    {
      foreach(var effect in effects)
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
      foreach (var effect in effects)
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
      foreach (var effect in effects)
      {
        effect.Ended(caster, target);
      }
    }

    /// <summary>
    /// A runtime instance of an active status condition, present on a target.
    /// </summary>
    public class Instance
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      public Status status;
      public string name { get { return status.name; } }
      public float duration;
      public int stacks;
      public int maximumStacks;
      public CombatController user;
      public CombatController target;

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public Instance(Status status, CombatController caster, CombatController target)
      {
        this.status = status;
        this.duration = this.status.duration;
        this.maximumStacks = this.status.stacks;
        user = caster;
        this.target = target;
      }

      /// <summary>
      /// Starts this status, applying all its initial effects.
      /// </summary>
      public void Start()
      {
        this.status.Started(user, target);
      }

      /// <summary>
      /// Persists this status, applying all its persistent effects.
      /// </summary>
      /// <param name="step"></param>
      public bool Persist(float step)
      {
        this.status.Persisted(user, target);

        // Run its duration
        this.duration -= step;
        Trace.Script(this.name + ": Duration left = " + this.duration);
        if (this.duration <= 0.0f)
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
        this.status.Ended(user, target);
      }

      /// <summary>
      /// Adds stacks to this status.
      /// </summary>
      public void Stack(int stacks = 1)
      {
        if (this.stacks >= this.maximumStacks)
          return;

        this.stacks += stacks;
      }

      /// <summary>
      /// Resets the duration of this status.
      /// </summary>
      public void Reset()
      {
        duration = status.duration;
      }

    }

  }

}