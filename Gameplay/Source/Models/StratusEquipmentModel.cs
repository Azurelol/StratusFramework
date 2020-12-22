using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Defines what equipment a character can use
	/// </summary>
	public abstract class StratusEquipmentModel { }

	namespace Models
	{
		[Serializable]
		public class StratusStandardEquipment : StratusEquipmentModel
		{
		}
	}

}