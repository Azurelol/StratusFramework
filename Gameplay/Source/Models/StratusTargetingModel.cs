using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Stratus;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Determines how a character is being controlled
	/// </summary>
	public enum StratusAgentControlType
	{
		Player,
		AI
	}

	/// <summary>
	/// The relationship with the faction the target belongs to
	/// </summary>
	[Flags]
	public enum StratusCombatTargetRelation
	{
		/// <summary>
		/// Own faction
		/// </summary>
		Self,
		/// <summary>
		/// Faction is friendly to self
		/// </summary>
		Friendly,
		/// <summary>
		/// Faction is neutral to self
		/// </summary>
		Neutral,
		/// <summary>
		/// Faction is hostile to self
		/// </summary>
		Hostile
	}

	/// <summary>
	/// Targeting parameters for a given command
	/// </summary>
	public enum StratusCombatTarget 
	{
		Self, 
		Ally, 
		Enemy, 
		Any 
	}

	public class StratusCombatFactionMatrix<FactionType>
	: StratusEnumRelationshipMatrix<FactionType, StratusCombatTargetRelation>
	where FactionType : Enum
	{
		public StratusCombatFactionMatrix()
		{
		}

		public StratusCombatFactionMatrix(IEnumerable<RelationValue> values) : base(values)
		{
		}

		protected override StratusCombatTargetRelation selfValue => StratusCombatTargetRelation.Self;
		protected override StratusCombatTargetRelation defaultValue => StratusCombatTargetRelation.Hostile;
	}

	/// <summary>
	/// Defines how skills are given targets
	/// </summary>
	public abstract class StratusTargetingModel
	{
		/// <summary>
		/// Evaluates a list of possible targets according to this targeting model
		/// </summary>
		/// <param name="caster"></param>
		/// <param name="target"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public abstract StratusCombatController[] EvaluateTargets(StratusCombatController caster, StratusCombatController[] targets, StratusCombatTarget type);

		/// <summary>
		/// Given an array of combatants, filters out those who aren't valid targets for the given targeting parameters.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="availableTargets"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static StratusCombatController[] FilterTargets(StratusCombatController source, StratusCombatController[] availableTargets, StratusCombatTarget type)
		{
			StratusCombatController[] targets = null;

			switch (type)
			{
				case StratusCombatTarget.Self:
					targets = new StratusCombatController[1] { source };
					break;
				case StratusCombatTarget.Ally:
					targets = (from StratusCombatController target in availableTargets where source.GetTargetRelation(target) == StratusCombatTargetRelation.Friendly select target).ToArray();
					break;
				case StratusCombatTarget.Enemy:
					targets = (from StratusCombatController target in availableTargets where source.GetTargetRelation(target) == StratusCombatTargetRelation.Hostile select target).ToArray();
					break;
				case StratusCombatTarget.Any:
					targets = availableTargets;
					break;
				default:
					break;
			}

			return (from StratusCombatController controller in targets where controller.state == StratusCombatController.State.Active select controller).ToArray();
		}
	}

	[Serializable]
	public class StratusRangeTargeting : StratusTargetingModel
	{
		/// <summary>
		/// Range required for casting the skill
		/// </summary>
		[Tooltip("Range to the target that is required for casting the skill")]
		[Range(1.0f, 100.0f)]
		public float range = 3.0f;

		public override StratusCombatController[] EvaluateTargets(StratusCombatController caster, StratusCombatController[] targets, StratusCombatTarget type)
		{
			return FilterTargets(caster, targets, type).Where(x => Vector3.Distance(x.transform.position, caster.transform.position) <= this.range).ToArray();
		}
	}

}