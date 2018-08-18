using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus;

namespace Genitus.Models
{
  /// <summary>
  /// A progression model where the character gains levels once enough
  /// experience has been earned
  /// </summary>
  public abstract class LevelProgressionModel : Character.ProgressionModel
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Signals that a character has gained a level
    /// </summary>
    public class LevelUpEvent : Stratus.Event
    {
      public Character character;
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    [Tooltip("The current level of this character")]
    public int level;
    public int experienceToNextLevel;
    public int currentExperience;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    protected virtual int baseExperience { get; } = 100;

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/  
    protected abstract int GetNextLevelExperience();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/  
    /// <summary>
    /// Adds a given amount of experience. If enough experience has been earned,
    /// the character will gain a level.
    /// </summary>
    /// <param name="experience"></param>
    /// <returns>The number of levels gained</returns>
    public override int AddExperience(int experience)
    {
      this.currentExperience += experience;

      // Now let's gain up to 1+ levels
      int levelsGained = 0;
      while (experience > 0)
      {
        if (experience > this.experienceToNextLevel)
        {
          experience -= this.experienceToNextLevel;          
          this.experienceToNextLevel = this.GetNextLevelExperience();
          levelsGained++;

          this.OnLevelUp();
        }
        else
        {
          this.experienceToNextLevel -= experience;
        }
      }

      return levelsGained;
    }

    protected virtual void OnLevelUp()
    {
      this.level++;
    }
  }
  
  

  /// <summary>
  /// A level progression model where the amount of experience needed to gain the next level
  /// is increased exponentially
  /// </summary>
  [Serializable]
  public class LevelExponentialProgressionModel : LevelProgressionModel
  {
    private float experienceExponent = 1.5f;

    protected override int GetNextLevelExperience()
    {
      return (int)Math.Floor(this.baseExperience * Easing.Power(this.level, this.experienceExponent));
    }
  }


}