using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;
using System.Text;
using Stratus.OdinSerializer;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Defines a skill that is applied by a combat controller
	/// </summary>
	public interface IStratusCombatSkillModel: IStratusModel
	{
	}

	/// <summary>
	/// Defines a skill that is applied by a combat controller, using specified parameters
	/// </summary>
	public interface IStratusCombatSkillModel<Parameters> : IStratusCombatSkillModel
		where Parameters : IStratusCombatParameterModel
	{
	}

	/// <summary>
	/// Base class for all skills used within the framework.
	/// </summary>
	public abstract class StratusCombatSkill<Parameters, Effect> : StratusScriptable, IStratusCombatSkillModel<Parameters>
		where Effect : StratusCombatEffect<Parameters>
		where Parameters : IStratusCombatParameterModel
	{
		public const string menuItemRoot = "Stratus/Gameplay/Models/Skills";

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The targeting parameters of the skill (Self, Ally, Enemy)
		/// </summary>
		public StratusCombatTarget targetType = StratusCombatTarget.Enemy;
		/// <summary>
		/// Effects this skill has
		/// </summary>
		[OdinSerialize]
		public List<Effect> effects = new List<Effect>();
		/// <summary>
		/// Components used by this skill
		/// </summary>
		[OdinSerialize]
		public List<StratusSkillComponent> components = new List<StratusSkillComponent>();

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		/// <summary>
		/// Evaluates a list of possible targets in order to find out which ones are
		/// targetable by this skill.
		/// </summary>
		public abstract StratusCombatController[] EvaluateTargets(StratusCombatController user, StratusCombatController[] availableTargets);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Applies the skill on all valid targets given.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="targets"></param>
		/// <param name="skill"></param>
		public void Apply<CombatController>(CombatController user, CombatController[] targets)
			where CombatController : StratusCombatController, IStratusCombatParametrized<Parameters>
		{
			if (targets == null)
			{
				throw new Exception("No valid targets for this skill!");
			}

			// For each target, apply every effect
			for (int i = 0; i < targets.Length; i++)
			{
				CombatController target = targets[i];
				foreach (var effect in effects)
				{
					effect.Apply(user, target);
				}
			}
		}
	}

	/// <summary>
	/// A skill that has a resource, targeting and description model
	/// </summary>
	/// <typeparam name="Resource"></typeparam>
	/// <typeparam name="Targeting"></typeparam>
	/// <typeparam name="Description"></typeparam>

	public abstract class StratusCombatSkill<Parameters, Effect, Resource, Targeting, Description> 
		: StratusCombatSkill<Parameters, Effect>

		where Parameters : IStratusCombatParameterModel
		where Effect : StratusCombatEffect<Parameters>, new()
		where Resource : StratusResourceModel
		where Targeting : StratusTargetingModel
		where Description : StratusDescriptionModel
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// How the skill is described
		/// </summary>
		[Tooltip("How the character is described")]
		public Description description;

		/// <summary>
		/// The scope of the skill (single, aoe, all)
		/// </summary>
		public Targeting targeting;

		/// <summary>
		/// The cost, in resources, of the skill
		/// </summary>
		[Tooltip("The resource cost of the skill")]
		public Resource cost;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		public override StratusCombatController[] EvaluateTargets(StratusCombatController user, StratusCombatController[] availableTargets)
		{
			return this.targeting.EvaluateTargets(user, availableTargets, this.targetType);
		}
	}

}