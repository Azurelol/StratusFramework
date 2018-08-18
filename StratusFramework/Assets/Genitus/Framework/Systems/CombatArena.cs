/******************************************************************************/
/*!
@file   CombatArena.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;

namespace Genitus
{  
  /// <summary>
  /// The component responsible for spawning all combatants and serving
  /// as a common boundary.
  /// </summary>
  public class CombatArena : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    public abstract class ArenaEvent : Stratus.Event { public CombatEncounter encounter; }
    public class InitializedEvent : ArenaEvent { public CombatArena arena; }
    public class ResetEvent : ArenaEvent { }
    public class AllSpawnedEvent : ArenaEvent { }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    public bool debug = false;
    public CombatEncounter encounter;
    [HideInInspector] public Dictionary<CombatController.Faction, List<CombatController>> combatantsByFaction;
    [HideInInspector] public List<CombatController> combatants;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public static CombatArena current { get; private set; }
    private Dictionary<CombatController.Faction, Stack<Vector3>> availableSpawnPositions { get; set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    /// <summary>
    /// Constructs a combat arena of the specified size at the given position.
    /// </summary>
    /// <param name="position">The position of the arena.</param>
    /// <param name="size">The size of the arena.</param>
    /// <returns></returns>
    public static CombatArena Construct(Vector3 position, int size)
    {
      //Trace.Script("Constructing combat arena!");
      string prefabName = "Combat/CombatArena";
      var arenaObj = Instantiate(Resources.Load(prefabName)) as GameObject;
      arenaObj.transform.position = position;
      arenaObj.transform.localScale = new Vector3(size, size, size);
      return arenaObj.GetComponent<CombatArena>();
    }

    /// <summary>
    /// Initializes the CombatArena script.
    /// </summary>
    void Awake()
    {
      current = this;

      combatantsByFaction = new Dictionary<CombatController.Faction, List<CombatController>>();
      combatantsByFaction.Add(CombatController.Faction.Player, new List<CombatController>());
      combatantsByFaction.Add(CombatController.Faction.Hostile, new List<CombatController>());

      // Subscribe to events
      this.gameObject.Connect<ResetEvent>(this.OnResetEvent);
      this.gameObject.Connect<Combat.StartedEvent>(this.OnCombatStartedEvent);
      Scene.Connect<Combat.EndedEvent>(this.OnCombatEndedEvent);
      Scene.Connect<CombatController.SpawnEvent>(this.OnCombatControllerSpawnEvent);
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    /// <summary>
    /// Invoked when combat is to be restarted.
    /// </summary>
    /// <param name="e"></param>
    void OnResetEvent(ResetEvent e)
    {
      this.Reset();
    }
    
    /// <summary>
    /// Initializes the combat arena.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatStartedEvent(Combat.StartedEvent e)
    {
      this.Initialize(e.Encounter);
    }

    /// <summary>
    /// Cleans up the combat arena.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatEndedEvent(Combat.EndedEvent e)
    {
      this.End();
    }

    
    /// <summary>
    /// Registers the CombatControllers depending on their given faction.
    /// </summary>
    /// <param name="e"></param>
    void OnCombatControllerSpawnEvent(CombatController.SpawnEvent e)
    {
      combatantsByFaction[e.controller.faction].Add(e.controller);
      combatants.Add(e.controller);

      if (debug)
      {
        if (e.controller.faction == CombatController.Faction.Player)
        {
          Trace.Script(e.controller.gameObject.name + " has registered as FRIENDLY");
        }
        else
        {
          Trace.Script(e.controller.gameObject.name + " has registered as HOSTILE");
        }
      }      
    }

    /// <summary>
    /// Checks if the combat controllers of the specified faction are active.
    /// </summary>
    /// <param name="controllers"> A list of controllers of a specific faction. </param>
    /// <returns> True if any of them are active, false otherwise. </returns>
    public bool AreCombatControllersActive(List<CombatController> controllers)
    {
      // TODO: Better way to do this with linq?
      foreach(var controller in controllers)
      {
        if (controller.currentState == CombatController.State.Active)
          return true;
      }
      
      return false;
    }
        
    /// <summary>
    /// Initializes the combat arena, calculating spawn positions and spawning all combatants.
    /// </summary>
    /// <param name="encounter"></param>
    public void Initialize(CombatEncounter encounter)
    {
      this.encounter = encounter;      
      this.Begin();
    }
    
    /// <summary>
    /// Begins combat.
    /// </summary>
    void Begin()
    {
      // Start playing the combat theme
      if (encounter.Theme)
      {
        throw new System.NotImplementedException("Hey man, do the audio right!");
        //var settings = new Audio.PlaybackSettings();
        //settings.Volume = 0.2f;
        //Music.Dispatch.PlayTrack(Encounter.Theme, settings);
      }

      // Calculate the spawn positions
      this.CalculateSpawnPositions();
      // Spawn all combat actors (within the boundaries of the arena next to each other?)
      Spawn();
      // Announce that all CombatControllers have been spawned
      this.gameObject.Dispatch<AllSpawnedEvent>(new AllSpawnedEvent());

    }
    
    /// <summary>
    /// Ends combat, following player victory.
    /// </summary>
    void End()
    {
      Trace.Script("Ending combat!");

      // Remove all remaining CombatControllers
      Clear();

      current = null;

      // Destroy this object
      Destroy(this.gameObject);

      //var combatEnded = new Combat.EndedEvent();
      //Scene.Dispatch<Combat.EndedEvent>(combatEnded);
      //Scene.Dispatch<Combat.EndedEvent>(combatEnded);     
    }
    
    /// <summary>
    /// Resets the encounter.
    /// </summary>
    void Reset()
    {
      //Trace.Script("Resetting arena!?");
      // Fade out, stop music?
      //Music.
      // Despawn all actors,
      Clear();
      // Fade in

      // Begin combat again?
      Begin();
      // Reset the music

      

    }

    /// <summary>
    /// Clears all active combat controllers.
    /// </summary>
    void Clear()
    {
      //Trace.Script("Clearing all combat controllers");
      foreach (var controller in combatants)
      {
        DestroyImmediate(controller.gameObject);
      }

      combatants.Clear();
      combatantsByFaction[CombatController.Faction.Player].Clear();
      combatantsByFaction[CombatController.Faction.Hostile].Clear();
    }

    /// <summary>
    /// Spawns all registered combatants for this arena.
    /// </summary>
    void Spawn()
    {
      // Spawn all actors in the player party's active roster around the player
      foreach (var member in PlayerParty.current.combatRoster)
      {
        Spawn(member.character, Character.ControlMode.Manual);
      }

      // Spawn all enemy actors around the encounter's actor
      this.SpawnEnemyGroup(this.encounter.Group);

      // Announce that all CombatControllers have been spawned into the arena
      Scene.Dispatch<AllSpawnedEvent>(new AllSpawnedEvent());
    }
    
    /// <summary>
    /// Spawns the combatants for the given party, placing them on the arena.
    /// </summary>
    /// <param name="CombatControllers">A party of combatants.</param>
    void SpawnEnemyGroup(List<Character> CombatControllers)
    {
      foreach (var character in CombatControllers)
      {
        Spawn(character, Character.ControlMode.Automatic);
      }
    }

    /// <summary>
    /// Spawns the specified character in the arena.
    /// </summary>
    /// <param name="character">The character to spawn.</param>
    void Spawn(Character character, Character.ControlMode control)
    {
      var combatController = CombatController.Construct(character, control);
      combatController.transform.parent = transform;
      combatController.transform.position = availableSpawnPositions[combatController.faction].Pop();
      // Announce that it has been spawwned (this event will be first received by this arena immediately)
      var spawnEvent = new CombatController.SpawnEvent();
      spawnEvent.controller = combatController;
      Scene.Dispatch<CombatController.SpawnEvent>(spawnEvent);
    }

    /// <summary>
    /// Calculates all possible spawn positions in the given boundary.
    /// </summary>
    void CalculateSpawnPositions()
    {
      availableSpawnPositions = new Dictionary<CombatController.Faction, Stack<Vector3>>();
      availableSpawnPositions.Add(CombatController.Faction.Player, new Stack<Vector3>());
      availableSpawnPositions.Add(CombatController.Faction.Hostile, new Stack<Vector3>());
      
      this.CalculatePositionsAround(PlayerParty.current.transform.position, CombatController.Faction.Player);
      this.CalculatePositionsAround(encounter.gameObject.transform.position, CombatController.Faction.Hostile);
    }
    
    /// <summary>
    /// Calculates the grid positions for the given party faction.
    /// </summary>
    /// <param name="position">The position of the lead from the party.</param>
    /// <param name="faction">The faction it belongs to. </param>
    void CalculatePositionsAround(Vector3 position, CombatController.Faction faction)
    {
      var margin = 3f;
      // 1 2 3
      availableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z - margin));
      availableSpawnPositions[faction].Push(new Vector3(position.x, position.y, position.z - margin));
      availableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z - margin));
      // 4 5 6
      availableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z));
      availableSpawnPositions[faction].Push(position);
      availableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z));
      // 7 8 9
      availableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z + margin));
      availableSpawnPositions[faction].Push(new Vector3(position.x, position.y, position.z + margin));
      availableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z + margin));
    }
    
    /// <summary>
    /// Gets a list of all possible targets, those not belonging to the given
    /// faction.
    /// </summary>
    /// <param name="faction"></param>
    /// <returns></returns>
    List<CombatController> getTargets(CombatController.Faction faction)
    {
      var targets = new List<CombatController>();
      foreach (var CombatControllerGroup in combatantsByFaction)
      {
        if (CombatControllerGroup.Key != faction)
        {
          foreach (var target in CombatControllerGroup.Value)
          {
            targets.Add(target);
          }
        }
      }

      return targets;
    }    

  }
}