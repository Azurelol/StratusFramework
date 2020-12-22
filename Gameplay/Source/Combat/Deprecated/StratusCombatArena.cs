//using UnityEngine;
//using System.Collections.Generic;
//using Stratus;

//namespace Stratus.Gameplay
//{
//	/// <summary>
//	/// The component responsible for spawning all combatants and serving
//	/// as a common boundary.
//	/// </summary>
//	public class StratusCombatArena : MonoBehaviour
//	{
//		//------------------------------------------------------------------------/
//		// Declarations
//		//------------------------------------------------------------------------/
//		public abstract class ArenaEvent : Stratus.StratusEvent { public StratusCombatEncounter encounter; }
//		public class InitializedEvent : ArenaEvent { public StratusCombatArena arena; }
//		public class ResetEvent : ArenaEvent { }
//		public class AllSpawnedEvent : ArenaEvent { }

//		//------------------------------------------------------------------------/
//		// Fields
//		//------------------------------------------------------------------------/
//		public bool debug = false;
//		public StratusCombatEncounter encounter;
//		[HideInInspector] public Dictionary<StratusCombatController.Faction, List<StratusCombatController>> combatantsByFaction;
//		[HideInInspector] public List<StratusCombatController> combatants;

//		//------------------------------------------------------------------------/
//		// Properties
//		//------------------------------------------------------------------------/
//		public static StratusCombatArena current { get; private set; }
//		private Dictionary<StratusCombatController.Faction, Stack<Vector3>> availableSpawnPositions { get; set; }

//		//------------------------------------------------------------------------/
//		// Messages
//		//------------------------------------------------------------------------/
//		/// <summary>
//		/// Constructs a combat arena of the specified size at the given position.
//		/// </summary>
//		/// <param name="position">The position of the arena.</param>
//		/// <param name="size">The size of the arena.</param>
//		/// <returns></returns>
//		public static StratusCombatArena Construct(Vector3 position, int size)
//		{
//			//Trace.Script("Constructing combat arena!");
//			string prefabName = "Combat/CombatArena";
//			var arenaObj = Instantiate(Resources.Load(prefabName)) as GameObject;
//			arenaObj.transform.position = position;
//			arenaObj.transform.localScale = new Vector3(size, size, size);
//			return arenaObj.GetComponent<StratusCombatArena>();
//		}

//		/// <summary>
//		/// Initializes the CombatArena script.
//		/// </summary>
//		void Awake()
//		{
//			current = this;

//			combatantsByFaction = new Dictionary<StratusCombatController.Faction, List<StratusCombatController>>();
//			combatantsByFaction.Add(StratusCombatController.Faction.Player, new List<StratusCombatController>());
//			combatantsByFaction.Add(StratusCombatController.Faction.Hostile, new List<StratusCombatController>());

//			// Subscribe to events
//			this.gameObject.Connect<ResetEvent>(this.OnResetEvent);
//			this.gameObject.Connect<StratusCombat.StartedEvent>(this.OnCombatStartedEvent);
//			Scene.Connect<StratusCombat.EndedEvent>(this.OnCombatEndedEvent);
//			Scene.Connect<StratusCombatController.SpawnEvent>(this.OnCombatControllerSpawnEvent);
//		}

//		//------------------------------------------------------------------------/
//		// Events
//		//------------------------------------------------------------------------/
//		/// <summary>
//		/// Invoked when combat is to be restarted.
//		/// </summary>
//		/// <param name="e"></param>
//		void OnResetEvent(ResetEvent e)
//		{
//			this.Reset();
//		}

//		/// <summary>
//		/// Initializes the combat arena.
//		/// </summary>
//		/// <param name="e"></param>
//		void OnCombatStartedEvent(StratusCombat.StartedEvent e)
//		{
//			this.Initialize(e.Encounter);
//		}

//		/// <summary>
//		/// Cleans up the combat arena.
//		/// </summary>
//		/// <param name="e"></param>
//		void OnCombatEndedEvent(StratusCombat.EndedEvent e)
//		{
//			this.End();
//		}


//		/// <summary>
//		/// Registers the CombatControllers depending on their given faction.
//		/// </summary>
//		/// <param name="e"></param>
//		void OnCombatControllerSpawnEvent(StratusCombatController.SpawnEvent e)
//		{
//			combatantsByFaction[e.controller.faction].Add(e.controller);
//			combatants.Add(e.controller);

//			if (debug)
//			{
//				if (e.controller.faction == StratusCombatController.Faction.Player)
//				{
//					StratusDebug.Log(e.controller.gameObject.name + " has registered as FRIENDLY");
//				}
//				else
//				{
//					StratusDebug.Log(e.controller.gameObject.name + " has registered as HOSTILE");
//				}
//			}
//		}

//		/// <summary>
//		/// Checks if the combat controllers of the specified faction are active.
//		/// </summary>
//		/// <param name="controllers"> A list of controllers of a specific faction. </param>
//		/// <returns> True if any of them are active, false otherwise. </returns>
//		public bool AreCombatControllersActive(List<StratusCombatController> controllers)
//		{
//			// TODO: Better way to do this with linq?
//			foreach (var controller in controllers)
//			{
//				if (controller.currentState == StratusCombatController.State.Active)
//					return true;
//			}

//			return false;
//		}

//		/// <summary>
//		/// Initializes the combat arena, calculating spawn positions and spawning all combatants.
//		/// </summary>
//		/// <param name="encounter"></param>
//		public void Initialize(StratusCombatEncounter encounter)
//		{
//			this.encounter = encounter;
//			this.Begin();
//		}

//		/// <summary>
//		/// Begins combat.
//		/// </summary>
//		void Begin()
//		{
//			// Start playing the combat theme
//			if (encounter.Theme)
//			{
//				throw new System.NotImplementedException("Hey man, do the audio right!");
//				//var settings = new Audio.PlaybackSettings();
//				//settings.Volume = 0.2f;
//				//Music.Dispatch.PlayTrack(Encounter.Theme, settings);
//			}

//			// Calculate the spawn positions
//			this.CalculateSpawnPositions();
//			// Spawn all combat actors (within the boundaries of the arena next to each other?)
//			Spawn();
//			// Announce that all CombatControllers have been spawned
//			this.gameObject.Dispatch<AllSpawnedEvent>(new AllSpawnedEvent());

//		}

//		/// <summary>
//		/// Ends combat, following player victory.
//		/// </summary>
//		void End()
//		{
//			StratusDebug.Log("Ending combat!");

//			// Remove all remaining CombatControllers
//			Clear();

//			current = null;

//			// Destroy this object
//			Destroy(this.gameObject);

//			//var combatEnded = new Combat.EndedEvent();
//			//Scene.Dispatch<Combat.EndedEvent>(combatEnded);
//			//Scene.Dispatch<Combat.EndedEvent>(combatEnded);     
//		}

//		/// <summary>
//		/// Resets the encounter.
//		/// </summary>
//		void Reset()
//		{
//			//Trace.Script("Resetting arena!?");
//			// Fade out, stop music?
//			//Music.
//			// Despawn all actors,
//			Clear();
//			// Fade in

//			// Begin combat again?
//			Begin();
//			// Reset the music



//		}

//		/// <summary>
//		/// Clears all active combat controllers.
//		/// </summary>
//		void Clear()
//		{
//			//Trace.Script("Clearing all combat controllers");
//			foreach (var controller in combatants)
//			{
//				DestroyImmediate(controller.gameObject);
//			}

//			combatants.Clear();
//			combatantsByFaction[StratusCombatController.Faction.Player].Clear();
//			combatantsByFaction[StratusCombatController.Faction.Hostile].Clear();
//		}

//		/// <summary>
//		/// Spawns all registered combatants for this arena.
//		/// </summary>
//		void Spawn()
//		{
//			// Spawn all actors in the player party's active roster around the player
//			foreach (var member in StratusPlayerPartyBehaviour.current.combatRoster)
//			{
//				Spawn(member.character, StratusCharacterControlMode.Manual);
//			}

//			// Spawn all enemy actors around the encounter's actor
//			this.SpawnEnemyGroup(this.encounter.Group);

//			// Announce that all CombatControllers have been spawned into the arena
//			Scene.Dispatch<AllSpawnedEvent>(new AllSpawnedEvent());
//		}

//		/// <summary>
//		/// Spawns the combatants for the given party, placing them on the arena.
//		/// </summary>
//		/// <param name="CombatControllers">A party of combatants.</param>
//		void SpawnEnemyGroup(List<StratusCharacterScriptable> CombatControllers)
//		{
//			foreach (var character in CombatControllers)
//			{
//				Spawn(character, StratusCharacterControlMode.Automatic);
//			}
//		}

//		/// <summary>
//		/// Spawns the specified character in the arena.
//		/// </summary>
//		/// <param name="character">The character to spawn.</param>
//		void Spawn(StratusCharacterScriptable character, StratusCharacterControlMode control)
//		{
//			var combatController = StratusCombatController.Construct(character, control);
//			combatController.transform.parent = transform;
//			combatController.transform.position = availableSpawnPositions[combatController.faction].Pop();
//			// Announce that it has been spawwned (this event will be first received by this arena immediately)
//			var spawnEvent = new StratusCombatController.SpawnEvent();
//			spawnEvent.controller = combatController;
//			Scene.Dispatch<StratusCombatController.SpawnEvent>(spawnEvent);
//		}

//		/// <summary>
//		/// Calculates all possible spawn positions in the given boundary.
//		/// </summary>
//		void CalculateSpawnPositions()
//		{
//			availableSpawnPositions = new Dictionary<StratusCombatController.Faction, Stack<Vector3>>();
//			availableSpawnPositions.Add(StratusCombatController.Faction.Player, new Stack<Vector3>());
//			availableSpawnPositions.Add(StratusCombatController.Faction.Hostile, new Stack<Vector3>());

//			this.CalculatePositionsAround(StratusPlayerPartyBehaviour.current.transform.position, StratusCombatController.Faction.Player);
//			this.CalculatePositionsAround(encounter.gameObject.transform.position, StratusCombatController.Faction.Hostile);
//		}

//		/// <summary>
//		/// Calculates the grid positions for the given party faction.
//		/// </summary>
//		/// <param name="position">The position of the lead from the party.</param>
//		/// <param name="faction">The faction it belongs to. </param>
//		void CalculatePositionsAround(Vector3 position, StratusCombatController.Faction faction)
//		{
//			var margin = 3f;
//			// 1 2 3
//			availableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z - margin));
//			availableSpawnPositions[faction].Push(new Vector3(position.x, position.y, position.z - margin));
//			availableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z - margin));
//			// 4 5 6
//			availableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z));
//			availableSpawnPositions[faction].Push(position);
//			availableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z));
//			// 7 8 9
//			availableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z + margin));
//			availableSpawnPositions[faction].Push(new Vector3(position.x, position.y, position.z + margin));
//			availableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z + margin));
//		}

//		/// <summary>
//		/// Gets a list of all possible targets, those not belonging to the given
//		/// faction.
//		/// </summary>
//		/// <param name="faction"></param>
//		/// <returns></returns>
//		List<StratusCombatController> getTargets(StratusCombatController.Faction faction)
//		{
//			var targets = new List<StratusCombatController>();
//			foreach (var CombatControllerGroup in combatantsByFaction)
//			{
//				if (CombatControllerGroup.Key != faction)
//				{
//					foreach (var target in CombatControllerGroup.Value)
//					{
//						targets.Add(target);
//					}
//				}
//			}

//			return targets;
//		}

//	}
//}