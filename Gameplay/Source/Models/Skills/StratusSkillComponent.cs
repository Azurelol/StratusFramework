using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Base class for all additional components a skill may have
	/// </summary>
	[Serializable]
	public abstract class StratusSkillComponent
	{
		protected abstract void OnActivation(StratusCombatController user, StratusCombatController target);
	}

	[Serializable]
	public class StratusSkillVFX : StratusSkillComponent
	{
		public GameObject particleSystem;
		public AudioClip sound;

		protected override void OnActivation(StratusCombatController user, StratusCombatController target)
		{
		}
	}

	[Serializable]
	public class StratusSkillTiming : StratusSkillComponent
	{
		[Tooltip("Specific timings for the skill's action")]
		public StratusCombatAction.Timings timings = new StratusCombatAction.Timings();

		protected override void OnActivation(StratusCombatController user, StratusCombatController target)
		{
		}
	}

	[Serializable]
	public class StratusSkillTrigger : StratusSkillComponent
	{
		/// <summary>
		/// Trigger used by the caster when using this skill
		/// </summary>
		[Tooltip("Trigger used by the caster when using this skill")]
		public StratusCombatTrigger onCast;
		/// <summary>
		/// Trigger used by the target when receiving this skill
		/// </summary>
		[Tooltip("Trigger used by the caster when defending from this skill")]
		public StratusCombatTrigger onDefend;
		/// <summary>
		/// Whether this skill has triggers set
		/// </summary>
		/// 
		public bool isTriggered { get { return (onCast.Enabled || onDefend.Enabled); } }

		protected override void OnActivation(StratusCombatController user, StratusCombatController target)
		{
			throw new System.NotImplementedException();
		}
	}

	[Serializable]
	public class StratusSkillTelegraph : StratusSkillComponent
	{
		public int number;
		/// <summary>
		/// Whether the skill is telegraphed
		/// </summary>
		[Tooltip("Whether the skill is telegraphed")]
		public bool telegraphed = true;
		/// <summary>
		/// How the skill is telegraphed.
		/// </summary>
		[Tooltip("How the skill is telegraphed")]
		//[DrawIf("IsTelegraphed", true, ComparisonType.Equals, PropertyDrawingType.DontDraw)]
		public StratusSkillTelegraphBehaviour.Configuration telegraph;

		protected override void OnActivation(StratusCombatController user, StratusCombatController target)
		{
			throw new System.NotImplementedException();
		}
	}

}