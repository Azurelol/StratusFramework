using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;

namespace Prototype
{
  /// <summary>
  /// Data about a given character, ranging from attributes to equipped skills, etc
  /// </summary>
  public abstract class Character : StratusScriptable
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/  
    /// <summary>
    /// How the character is controlled.
    /// </summary>
    public enum ControlMode { Manual, Automatic }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    /// <summary>
    /// A portrait for this character
    /// </summary>
    public Sprite portrait;
    /// <summary>
    /// The default control mode of the character.
    /// </summary>
    public ControlMode mode = ControlMode.Automatic;
    /// <summary>
    /// The faction this character belogns to
    /// </summary>
    public CombatController.Faction faction;
    /// <summary>
    /// The default attack skill.
    /// </summary>
    [Tooltip("The default attack skill")]
    [HideInInspector] public Skill defaultSkill;
    /// <summary>
    /// The character's equipped skills
    /// </summary>
    public List<Skill> skills = new List<Skill>();
  }

  /// <summary>
  /// Data about a given character, ranging from attributes to equipped skills, etc
  /// </summary>
  //[CreateAssetMenu(fileName = "Character", menuName = "Prototype/Character")]
  public class Character<AttributesType> : Character where AttributesType : Attributes
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Signals that this character has gained a level
    /// </summary>
    public class LevelUpEvent : Stratus.Event
    {
      public AttributesType previousStats;
      public AttributesType currentStats;
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Combat attributes of a character
    /// </summary>
    public AttributesType attributes;

    //private static bool debug = false;
    //private static Transform Library;
    //private static Dictionary<string, Character<AttributesType>> characters = new Dictionary<string, Character<AttributesType>>();
    //private static string Path = "Characters/";
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  
    ///// <summary>
    ///// Adds a character to the library of available characters
    ///// </summary>
    ///// <param name="name"></param>
    ///// <returns></returns>
    //public static bool Add(string name)
    //{
    //  var character = ScriptableObject.Instantiate(Resources.Load(Path + name)) as Character<AttributesType>;
    //  if (!character)
    //  {
    //    Trace.Script(name + " could not be found!");
    //    return false;
    //  }
    //
    //  //character.transform.parent = Library;
    //  characters.Add(name, character);
    //  if (debug) Trace.Script("Added " + name + " to the character library!");
    //  return true;
    //}


    ///// <summary>
    ///// Finds an instance of the character. If none are present, instantiates it.
    ///// </summary>
    ///// <param name="name">The name of the character</param>
    ///// <returns></returns>
    //public static Character<AttributesType> Find(string name)
    //{
    //
    //  // If the library has not been instantiated yet..
    //  if (characters == null)
    //  {
    //    // Instantiate the library
    //    Library = (Object.Instantiate(Resources.Load(Path + "Utilities/CharacterLibrary")) as GameObject).transform;
    //    DontDestroyOnLoad(Library);
    //    characters = new Dictionary<string, Character>();
    //  }
    //
    //  // If the character has not been instantiated yet, do so!
    //  if (!characters.ContainsKey(name))
    //  {
    //    if (!Add(name))
    //    {
    //    }
    //  }
    //
    //  return characters[name];
    //}

  }


}