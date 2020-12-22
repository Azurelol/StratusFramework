using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Defines the combat attributes of a character
	/// </summary>    
	public interface IStratusAttributeModel  
	{
	}

	namespace Models
	{
		/// <summary>
		/// Standard attributes for an FPS
		/// </summary>
		public abstract class StratusFPSAttributeModel : IStratusAttributeModel
		{
			public StratusVariableAttribute health = new StratusVariableAttribute();
		}

		/// <summary>
		/// Based on DND 5th Edition ruleset: 
		/// </summary>
		public abstract class StratusStandardCRPGAttributeModel : IStratusAttributeModel
		{
			public StratusVariableAttribute strength = new StratusVariableAttribute(defaultAbilityScore, 0, abilityScoreCeiling);
			public StratusVariableAttribute dexterity = new StratusVariableAttribute(defaultAbilityScore, 0, abilityScoreCeiling);
			public StratusVariableAttribute constitution = new StratusVariableAttribute(defaultAbilityScore, 0, abilityScoreCeiling);
			public StratusVariableAttribute intellect = new StratusVariableAttribute(defaultAbilityScore, 0, abilityScoreCeiling);
			public StratusVariableAttribute wisdom = new StratusVariableAttribute(defaultAbilityScore, 0, abilityScoreCeiling);
			public StratusVariableAttribute charisma = new StratusVariableAttribute(defaultAbilityScore, 0, abilityScoreCeiling);

			public const int defaultAbilityScore = 10;
			public const int abilityScoreCeiling = 30;
		}


	}



}