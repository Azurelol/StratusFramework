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

namespace Prototype
{  
  /// <summary>
  /// The component responsible for spawning all combatants and serving
  /// as a common boundary.
  /// </summary>
  public class CombatArena : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    public abstract class ArenaEvent : Stratus.Event { public CombatEncounter Encounter; }
    public class InitializedEvent : ArenaEvent { public CombatArena Arena; }
    public class ResetEvent : ArenaEvent { }
    public class AllSpawnedEvent : ArenaEvent { }
    //------------------------------------------------------------------------/    
    public static CombatArena Current;
    [HideInInspector] public Dictionary<CombatController.Faction, List<CombatController>> Combatants;
    [HideInInspector] public List<CombatController> AllCombatControllers;
    public bool Tracing = false;        
    //------------------------------------------------------------------------/
    Dictionary<CombatController.Faction, Stack<Vector3>> AvailableSpawnPositions;
    public CombatEncounter Encounter;
    //Events.Setup Setup = new Events.Setup(this);

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
      Current = this;

      Combatants = new Dictionary<CombatController.Faction, List<CombatController>>();
      Combatants.Add(CombatController.Faction.Player, new List<CombatController>());
      Combatants.Add(CombatController.Faction.Hostile, new List<CombatController>());

      // Subscribe to events
      this.gameObject.Connect<ResetEvent>(this.OnResetEvent);
      this.gameObject.Connect<Combat.StartedEvent>(this.OnCombatStartedEvent);
      Scene.Connect<Combat.EndedEvent>(this.OnCombatEndedEvent);
      Scene.Connect<CombatController.SpawnEvent>(this.OnCombatControllerSpawnEvent);

      // Announce that the arena has been initialized
      //var initEvent = new InitializedEvent();
      //initEvent.

    }

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
      Combatants[e.controller.character.Faction].Add(e.controller);
      AllCombatControllers.Add(e.controller);

      if (Tracing)
      {
        if (e.controller.character.Faction == CombatController.Faction.Player)
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
      Encounter = encounter;      
      this.Begin();
    }
    
    /// <summary>
    /// Begins combat.
    /// </summary>
    void Begin()
    {
      // Start playing the combat theme
      if (Encounter.Theme)
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

      Current = null;

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
      foreach (var controller in AllCombatControllers)
      {
        DestroyImmediate(controller.gameObject);
      }

      AllCombatControllers.Clear();
      Combatants[CombatController.Faction.Player].Clear();
      Combatants[CombatController.Faction.Hostile].Clear();
    }

    /// <summary>
    /// Spawns all registered combatants for this arena.
    /// </summary>
    void Spawn()
    {
      // Spawn all actors in the player party's active roster around the player
      foreach (var member in PlayerParty.current.combatRoster)
      {
        Spawn(member.character);
      }
      // Spawn all enemy actors around the encounter's actor
      this.Spawn(this.Encounter.Group);
      // Announce that all CombatControllers have been spawned into the arena
      Scene.Dispatch<AllSpawnedEvent>(new AllSpawnedEvent());
    }
    
    /// <summary>
    /// Spawns the combatants for the given party, placing them on the arena.
    /// </summary>
    /// <param name="CombatControllers">A party of combatants.</param>
    void Spawn(List<Character> CombatControllers)
    {
      foreach (var character in CombatControllers)
      {
        Spawn(character);
      }
    }

    /// <summary>
    /// Spawns the specified character in the arena.
    /// </summary>
    /// <param name="character">The character to spawn.</param>
    void Spawn(Character character)
    {
      var combatController = CombatController.Construct(character);
      combatController.transform.parent = transform;
      combatController.transform.position = AvailableSpawnPositions[combatController.character.Faction].Pop();
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
      AvailableSpawnPositions = new Dictionary<CombatController.Faction, Stack<Vector3>>();
      AvailableSpawnPositions.Add(CombatController.Faction.Player, new Stack<Vector3>());
      AvailableSpawnPositions.Add(CombatController.Faction.Hostile, new Stack<Vector3>());
      
      this.CalculatePositionsAround(PlayerParty.current.transform.position, CombatController.Faction.Player);
      this.CalculatePositionsAround(Encounter.gameObject.transform.position, CombatController.Faction.Hostile);
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
      AvailableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z - margin));
      AvailableSpawnPositions[faction].Push(new Vector3(position.x, position.y, position.z - margin));
      AvailableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z - margin));
      // 4 5 6
      AvailableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z));
      AvailableSpawnPositions[faction].Push(position);
      AvailableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z));
      // 7 8 9
      AvailableSpawnPositions[faction].Push(new Vector3(position.x - margin, position.y, position.z + margin));
      AvailableSpawnPositions[faction].Push(new Vector3(position.x, position.y, position.z + margin));
      AvailableSpawnPositions[faction].Push(new Vector3(position.x + margin, position.y, position.z + margin));
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
      foreach (var CombatControllerGroup in Combatants)
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