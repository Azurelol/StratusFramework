using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Defines how a character makes progression
	/// </summary>
	public abstract class StratusProgressionModel
	{
		/// <summary>
		/// Adds experience onto this model
		/// </summary>
		/// <param name="experience">The amount of experience earned</param>
		/// <returns></returns>
		public abstract int AddExperience(int experience);
	}

	/// <summary>
	/// A progression model where the character gains levels once enough
	/// experience has been earned
	/// </summary>
	public abstract class StratusLevelProgressionModel : StratusProgressionModel
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/  
		/// <summary>
		/// Signals that a character has gained a level
		/// </summary>
		public class LevelUpEvent : Stratus.StratusEvent
		{
			public StratusCharacterScriptable character;
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/  
		/// <summary>
		/// The current level of this character. Starts at 1.
		/// </summary>
		[Tooltip("The current level of this character")]
		public int level = 1;
		/// <summary>
		/// How much experience is needed for the next level
		/// </summary>
		public int experienceToNextLevel;
		/// <summary>
		/// The current experience accrued (since level 1)
		/// </summary>
		public int currentExperience;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/  
		protected virtual int baseExperienceModifier { get; } = 100;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/  
		protected abstract int GetNextLevelExperience();

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/  
		public StratusLevelProgressionModel()
		{
			this.experienceToNextLevel = GetNextLevelExperience();
		}

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

	namespace Models
	{
		/// <summary>
		/// A level progression model where the experience to gain each level 
		/// remains the same
		/// </summary>
		[Serializable]
		public class StratusLevelLinearProgression : StratusLevelProgressionModel
		{
			protected override int GetNextLevelExperience()
			{
				return this.baseExperienceModifier;
			}
		}

		/// <summary>
		/// A level progression model where the amount of experience needed to gain the next level
		/// is increased exponentially
		/// </summary>
		[Serializable]
		public class StratusLevelExponentialProgression : StratusLevelProgressionModel
		{
			private float experienceExponent = 1.5f;

			protected override int GetNextLevelExperience()
			{
				return (int)Math.Floor(this.baseExperienceModifier * StratusEasing.Power(this.level, this.experienceExponent));
			}
		} 
	}


}