using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;

namespace Genitus
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
    public abstract class AttributeModel : StratusSerializable
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
    public abstract class ResourceModel
    {
      public abstract class Component
      {
        public abstract void OnUsage();
        public abstract void OnUpdate(float step);
      }

      public abstract void Use();
    }

    /// <summary>
    /// Defines what equipment a character can use
    /// </summary>
    public abstract class EquipmentModel {}

    /// <summary>
    /// Parameters determine how a character performs in combat
    /// </summary>
    public interface ParameterModel
    {
      VariableAttribute hitpoints { get; }
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    /// <summary>
    /// A portrait for this character
    /// </summary>
    public Sprite portrait;
    /// <summary>
    /// The faction this character belogns to
    /// </summary>
    public CombatController.Faction faction;
  }

  /// <summary>
  /// Data about a given character, ranging from attributes to equipped skills, etc
  /// </summary>
  //[CreateAssetMenu(fileName = "Character", menuName = "Prototype/Character")]
  public class Character<Progression, Attributes, Resource, Skill, Equipment> : Character
    where Progression : Character.ProgressionModel
    where Attributes : Character.AttributeModel
    where Resource : Character.ResourceModel
    where Skill : Skill<Resource>
    where Equipment : Character.EquipmentModel
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Progression attribute for this character
    /// </summary>
    public Progression progression;
    /// <summary>
    /// Combat attributes of a character
    /// </summary>
    public Attributes attributes;
    /// <summary>
    /// Combat resources for the character, consumed by skills and abilities
    /// </summary>
    //public Resource resources;
    /// <summary>
    /// Progression attribute for this character
    /// </summary>
    public Equipment equipment;
    /// <summary>
    /// The default attack skill.
    /// </summary>
    [Tooltip("The default attack skill")]
    [HideInInspector] public Skill defaultSkill;
    /// <summary>
    /// The character's equipped skills
    /// </summary>
    public List<Skill> skills = new List<Skill>();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  


  }


}