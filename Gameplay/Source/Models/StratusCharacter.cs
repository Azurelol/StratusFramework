using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Gameplay
{
	/// <summary>
	/// How a character is controlled
	/// </summary>
	public enum StratusCharacterControlMode
	{
		Manual,
		Automatic
	}

	/// <summary>
	/// Base class for a stratus character
	/// </summary>
	public abstract class StratusCharacter
	{
		public abstract string name { get; }
	}

	/// <summary>
	/// Models a character used in combat
	/// </summary>
	/// <typeparam name="DescriptionModel"></typeparam>
	/// <typeparam name="ProgressionModel"></typeparam>
	/// <typeparam name="AttributeModel"></typeparam>
	/// <typeparam name="SkillModel"></typeparam>
	public abstract class StratusCharacter<DescriptionModel,
								  ProgressionModel,
								  AttributeModel>
								  : StratusCharacter

	  where DescriptionModel : StratusDescriptionModel
	  where ProgressionModel : StratusProgressionModel
	  where AttributeModel : IStratusAttributeModel
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/  
		/// <summary>
		/// How the character is described
		/// </summary>
		[Tooltip("How the character is described")]
		public DescriptionModel description;
		/// <summary>
		/// Progression attribute for this character
		/// </summary>
		public ProgressionModel progression;
		/// <summary>
		/// Attributes for a character
		/// </summary>
		public AttributeModel attributes;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The name of this character
		/// </summary>
		public override string name => description.name;

		//------------------------------------------------------------------------/
		// Method
		//------------------------------------------------------------------------/
		public override string ToString()
		{
			return description.name;
		}
	}

	/// <summary>
	/// Data about a given character, ranging from attributes to equipped skills, etc
	/// </summary>
	public abstract class StratusCharacter<DescriptionModel,
							  ProgressionModel,
							  AttributeModel,
							  SkillModel>
							  : StratusCharacter<DescriptionModel, ProgressionModel, AttributeModel>
		where DescriptionModel : StratusDescriptionModel
		where ProgressionModel : StratusProgressionModel
		where AttributeModel : IStratusAttributeModel
		where SkillModel : IStratusCombatSkillModel, new()
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/  
		/// <summary>
		/// The character's equipped skills
		/// </summary>
		public List<SkillModel> skills;
	}

}