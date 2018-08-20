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
    /// How the character is controlled
    /// </summary>
    public enum ControlMode { Manual, Automatic }

    /// <summary>
    /// Defines the combat attributes of a character
    /// </summary>    
    public abstract class AttributeModel : StratusSerializable {}

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
  }

  /// <summary>
  /// Data about a given character, ranging from attributes to equipped skills, etc
  /// </summary>
  //[CreateAssetMenu(fileName = "Character", menuName = "Prototype/Character")]
  public class Character<Description, Progression, Attributes, Resource, Skills, Equipment> : Character
    where Description : DescriptionModel
    where Progression : Character.ProgressionModel
    where Attributes : Character.AttributeModel
    where Resource : Character.ResourceModel
    where Skills : Skill 
    where Equipment : Character.EquipmentModel
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/ 
    public class Instance
    {
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    /// <summary>
    /// How the character is described
    /// </summary>
    [Tooltip("How the character is described")]
    public Description description;
    /// <summary>
    /// Progression attribute for this character
    /// </summary>
    public Progression progression;
    /// <summary>
    /// Combat attributes of a character
    /// </summary>
    public Attributes attributes;
    /// <summary>
    /// Progression attribute for this character
    /// </summary>
    public Equipment equipment;
    /// <summary>
    /// The character's equipped skills
    /// </summary>
    public List<Skills> skills;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  


  }


}