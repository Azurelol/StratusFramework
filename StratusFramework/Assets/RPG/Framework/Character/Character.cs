using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;

namespace Altostratus
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

    /// <summary>
    /// Basic attributes for a character
    /// </summary>    
    public abstract class AttributeModel
    {
    }

    /// <summary>
    /// Defines how a character makes progression
    /// </summary>
    public abstract class ProgressionModel
    {
      /// <summary>
      /// Adds experience onto this model
      /// </summary>
      /// <param name="experience">The amount of experience earned</param>
      /// <returns></returns>
      public abstract int AddExperience(int experience);
    }

    /// <summary>
    /// Defines what resources a character consumes to perform actions
    /// </summary>
    public abstract class ResourceModel {}

    /// <summary>
    /// Defines what equipment a character can use
    /// </summary>
    public abstract class EquipmentModel {}

    /// <summary>
    /// Parameters determine how a character performs in combat
    /// </summary>
    public interface ParameterModel
    {
      Combat.Attribute hitpoints { get; }
    }

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
  public class Character<Attributes, Equipment, Progression> : Character
    where Attributes : Character.AttributeModel
    where Equipment : Character.EquipmentModel
    where Progression : Character.ProgressionModel
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Combat attributes of a character
    /// </summary>
    public Attributes attributes;
    /// <summary>
    /// Progression attribute for this character
    /// </summary>
    public Progression progression;
    /// <summary>
    /// Progression attribute for this character
    /// </summary>
    public Equipment equipment;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  


  }


}