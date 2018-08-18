/******************************************************************************/
/*!
@file   ArenaCombatSystem.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Genitus 
{
  /// <summary>
  /// A combat system based around a dynamically-spawned combat arena
  /// </summary>
  [RequireComponent(typeof(CombatArena))]
  public abstract class ArenaCombatSystem : CombatSystem 
  {
    public float StartupDelay = 0.1f;
    public CombatArena Arena { get { return GetComponent<CombatArena>(); } }

    protected override void Subscribe()
    {
      Scene.Connect<ReferenceEvent>(this.OnReferenceEvent);
      Scene.Connect<RetryEvent>(this.OnRetryEvent);
      Scene.Connect<CombatArena.AllSpawnedEvent>(this.OnCombatArenaAllSpawnedEvent);
    }

    /// <summary>
    /// Received when a reference to the combat system is requested.
    /// </summary>
    /// <param name="e"></param>
    void OnReferenceEvent(ReferenceEvent e)
    {
      e.System = this;
    }

    /// <summary>
    /// Invoked when combat is about to be retried.
    /// </summary>
    /// <param name="e"></param>
    void OnRetryEvent(RetryEvent e)
    {
      //Trace.Script("Retrying!");      
      // Reset the arena
      Arena.gameObject.Dispatch<CombatArena.ResetEvent>(new CombatArena.ResetEvent());
    }

    /// <summary>
    /// Received when all CombatControllers have been spawned.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatArenaAllSpawnedEvent(CombatArena.AllSpawnedEvent e)
    {
      // Configure this system-specific components for each CombatController
      ConfigureCombatControllers();

      // Call the combat system's main start up
      this.OnCombatStart();

      // Combat is now unresolvesd
      IsResolved = false;

      // Enable the combat HUD
      var seq = Actions.Sequence(this);
      Actions.Delay(seq, 2.5f);
      Actions.Call(seq, this.DisplayHUD);

      // Now announce that combat has started to the space!
      var combatStarted = new Combat.StartedEvent();
      combatStarted.Encounter = Arena.encounter;
      Scene.Dispatch<Combat.StartedEvent>(combatStarted);
    }


    /// <summary>
    /// Configures all the CombatControllers in the arena to participate in the
    /// combat system.
    /// </summary>
    void ConfigureCombatControllers()
    {
      foreach (var CombatController in CombatArena.current.combatants)
      {
        OnConfigureCombatController(CombatController);
      }
    }

    void DisplayHUD()
    {
      //Scene.Dispatch<CombatHUD.EnableEvent>(new HUD<CombatHUD>.EnableEvent());
    }


  }  
}
