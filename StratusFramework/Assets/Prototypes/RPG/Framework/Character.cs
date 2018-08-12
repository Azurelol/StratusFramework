using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;

namespace Prototype
{
  /// <summary>
  /// Information about a given character, ranging from attributes to equipped skills, etc
  /// </summary>
  [CreateAssetMenu(fileName = "Character", menuName = "Prototype/Character")]
  public class Character : ScriptableObject
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/  
    /// <summary>
    /// How the character is controlled.
    /// </summary>
    public enum ControlMode { Manual, Automatic }

    public class LevelUpEvent : Stratus.Event
    {
      public Attributes PreviousStats;
      public Attributes CurrentStats;
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    public GameObject model;
    public Material skin;
    public Sprite portrait;
    public CombatController.Faction Faction;
    /// <summary>
    /// The control mode of the character.
    /// </summary>
    public ControlMode Mode = ControlMode.Automatic;
    /// <summary>
    /// Combat attributes of a character
    /// </summary>
    public Attributes Attributes;

    public float staminaRecoveryDelay = 0.5f;

    /// <summary>
    /// The default attack skill.
    /// </summary>
    [Tooltip("The default attack skill")]
    [HideInInspector] public Skill Attack;
    /// <summary>
    /// The character's equipped skills
    /// </summary>
    public List<Skill> Skills = new List<Skill>();

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  

    static bool Tracing = false;
    static Transform Library;
    static Dictionary<string, Character> Characters;
    static string Path = "Characters/";

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Adds a character to the library of available characters
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool Add(string name)
    {
      var character = ScriptableObject.Instantiate(Resources.Load(Path + name)) as Character;
      if (!character)
      {
        Trace.Script(name + " could not be found!");
        return false;
      }

      //character.transform.parent = Library;
      Characters.Add(name, character);
      if (Tracing) Trace.Script("Added " + name + " to the character library!");
      return true;
    }


    /// <summary>
    /// Finds an instance of the character. If none are present, instantiates it.
    /// </summary>
    /// <param name="name">The name of the character</param>
    /// <returns></returns>
    public static Character Find(string name)
    {

      // If the library has not been instantiated yet..
      if (Characters == null)
      {
        // Instantiate the library
        Library = (Object.Instantiate(Resources.Load(Path + "Utilities/CharacterLibrary")) as GameObject).transform;
        DontDestroyOnLoad(Library);
        Characters = new Dictionary<string, Character>();
      }

      // If the character has not been instantiated yet, do so!
      if (!Characters.ContainsKey(name))
      {
        if (!Add(name))
        {
        }
      }

      return Characters[name];
    }

  }


}