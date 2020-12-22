using System;
using Stratus;
using UnityEngine;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Skills consume a finite amount of mana
	/// </summary>
	public class StratusManaComponent : StratusResourceModel.Component
	{
		public int mana;

		public override void OnUpdate(float step)
		{
		}

		public override void OnUsage()
		{
		}
	}

	[Serializable]
	public class StratusManaModel : StratusResourceModel
	{
		public int mana;

		public override void Use()
		{
		}
	}


	/// <summary>
	/// Skills, once activated, cannot be used until a cooldown period has passed
	/// </summary>
	public class StratusCooldownComponent : StratusResourceModel.Component
	{
		/// <summary>
		/// Time required before a skill can be used again after activation
		/// </summary>
		[Tooltip("Time required before the skill can be used again after activation")]
		public StratusVariableAttribute cooldown = new StratusVariableAttribute();

		private StratusCooldown timer;

		public override void OnUsage()
		{
			this.timer.Activate();
		}

		public override void OnUpdate(float timeStep)
		{
			this.timer.Update(timeStep);
		}
	}

}