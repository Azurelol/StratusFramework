/******************************************************************************/
/*!
@file   CombatSystem.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Genitus
{
  /// <summary>
  /// The base class that drives the logic for the game's combat system.
  /// </summary>
  public abstract class CombatSystem : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class ReferenceEvent : Stratus.Event { public CombatSystem System; }
    // Informs that a combat turn has passed
    public class TurnPassedEvent : Stratus.Event { public int Turn; }
    // Informs that time has advanced in combat
    public class TimeStepEvent : Stratus.Event { public float Step; }
    // Informs that the combat system has finished its initialization routine
    public class InitializedEvent : Stratus.Event { }
    // Informs that combat has been resolved
    public class ResolveEvent : Stratus.Event { }
    // Informs that combat has been won by the player
    public class VictoryEvent : Stratus.Event { public CombatEncounter Encounter; }
    // Informs that the player has been defeated
    public class DefeatEvent : Stratus.Event { public CombatEncounter Encounter; }
    // Informs that combat is to be retried
    public class RetryEvent : Stratus.Event { public CombatEncounter Encounter; }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public bool Tracing = false;
    /// <summary>
    /// Whether combat has been resolved.
    /// </summary>
    protected bool IsResolved = false;

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    // Combatants
    protected abstract void OnConfigureCombatController(CombatController CombatController);
    protected abstract void OnCombatControllerSpawn(CombatController controller);
    protected abstract void OnCombatControllerDeath(CombatController controller);
    // System
    protected abstract void OnCombatSystemInitialize();
    protected abstract void OnCombatSystemSubscribe();
    protected abstract void OnCombatSystemTerminate();
    protected abstract void OnCombatSystemUpdate();
    // Combat
    protected abstract void OnCombatStart();
    protected abstract void OnCombatRetry();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the CombatSystem.
    /// </summary>
    void Awake()
    {
      this.Subscribe();
      this.OnCombatSystemSubscribe();
      this.OnCombatSystemInitialize();

      // Announce that the combat system has finished initializing
      Scene.Dispatch<InitializedEvent>(new InitializedEvent());
    }

    /// <summary>
    /// Updates the battle system every frame.
    /// </summary>
    void FixedUpdate()
    {
      if (IsResolved)
        return;

      this.OnCombatSystemUpdate();
    }

    /// <summary>
    /// Subscribe to events.
    /// </summary>
    protected virtual void Subscribe()
    {
      Scene.Connect<CombatController.SpawnEvent>(this.OnCombatControllerSpawnEvent);
      Scene.Connect<CombatController.DeathEvent>(this.OnCombatControllerDeathEvent);
      Scene.Connect<RetryEvent>(this.OnCombatRetryEvent);
    }

    /// <summary>
    /// Registers the CombatControllers depending on their given faction.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatControllerSpawnEvent(CombatController.SpawnEvent e)
    {
      OnCombatControllerSpawn(e.controller);
    }      

    /// <summary>
    /// Received when a combat controller becomes inactive (such as being knocked out)
    /// </summary>
    /// <param name="e">The controller which has gone inactive. </param>
    void OnCombatControllerDeathEvent(CombatController.DeathEvent e)
    {
      OnCombatControllerDeath(e.controller);
    }

    void OnCombatRetryEvent(RetryEvent e)
    {
      this.OnCombatRetry();
    }

    protected void StartCombat()
    {
      //Gamestate.Change(Gamestate.State.Combat);
      Scene.Dispatch<Combat.StartedEvent>(new Combat.StartedEvent());
    }

    protected void EndCombat()
    {
      //Gamestate.Revert();
      Scene.Dispatch<Combat.EndedEvent>(new Combat.EndedEvent());
    }

    


  }

}