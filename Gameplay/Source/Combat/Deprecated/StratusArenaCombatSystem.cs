//using UnityEngine;
//using Stratus;

//namespace Stratus.Gameplay
//{
//	/// <summary>
//	/// A combat system based around a dynamically-spawned combat arena
//	/// </summary>
//	[RequireComponent(typeof(StratusCombatArena))]
//	public abstract class StratusArenaCombatSystem : StratusCombatSystem
//	{
//		public float StartupDelay = 0.1f;
//		public StratusCombatArena Arena { get { return GetComponent<StratusCombatArena>(); } }

//		protected override void Subscribe()
//		{
//			base.Subscribe();
//			Scene.Connect<ReferenceEvent>(this.OnReferenceEvent);
//			Scene.Connect<RetryEvent>(this.OnRetryEvent);
//			Scene.Connect<StratusCombatArena.AllSpawnedEvent>(this.OnCombatArenaAllSpawnedEvent);
//		}

//		/// <summary>
//		/// Received when a reference to the combat system is requested.
//		/// </summary>
//		/// <param name="e"></param>
//		void OnReferenceEvent(ReferenceEvent e)
//		{
//			e.System = this;
//		}

//		/// <summary>
//		/// Invoked when combat is about to be retried.
//		/// </summary>
//		/// <param name="e"></param>
//		void OnRetryEvent(RetryEvent e)
//		{
//			//Trace.Script("Retrying!");      
//			// Reset the arena
//			Arena.gameObject.Dispatch<StratusCombatArena.ResetEvent>(new StratusCombatArena.ResetEvent());
//		}

//		/// <summary>
//		/// Received when all CombatControllers have been spawned.
//		/// </summary>
//		/// <param name="e"></param>
//		void OnCombatArenaAllSpawnedEvent(StratusCombatArena.AllSpawnedEvent e)
//		{
//			// Configure this system-specific components for each CombatController
//			ConfigureCombatControllers();

//			// Call the combat system's main start up
//			this.OnCombatStart();

//			// Combat is now unresolvesd
//			IsResolved = false;

//			// Enable the combat HUD
//			var seq = StratusActions.Sequence(this);
//			StratusActions.Delay(seq, 2.5f);
//			StratusActions.Call(seq, this.DisplayHUD);

//			// Now announce that combat has started to the space!
//			var combatStarted = new StratusCombat.StartedEvent();
//			combatStarted.Encounter = Arena.encounter;
//			Scene.Dispatch<StratusCombat.StartedEvent>(combatStarted);
//		}


//		/// <summary>
//		/// Configures all the CombatControllers in the arena to participate in the
//		/// combat system.
//		/// </summary>
//		void ConfigureCombatControllers()
//		{
//			foreach (var CombatController in StratusCombatArena.current.combatants)
//			{
//				OnConfigureCombatController(CombatController);
//			}
//		}

//		void DisplayHUD()
//		{
//			//Scene.Dispatch<CombatHUD.EnableEvent>(new HUD<CombatHUD>.EnableEvent());
//		}


//	}
//}
