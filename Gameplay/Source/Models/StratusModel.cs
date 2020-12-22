using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	public interface IStratusModel
	{
	}

	/// <summary>
	/// Defines what resources a character consumes to perform actions
	/// </summary>
	public abstract class StratusResourceModel
	{
		public abstract class Component
		{
			public abstract void OnUsage();
			public abstract void OnUpdate(float step);
		}

		public abstract void Use();
	}



	namespace Models
	{
		public class StratusStandardResourceModel : StratusResourceModel
		{
			public override void Use()
			{
			}
		}
	}


}