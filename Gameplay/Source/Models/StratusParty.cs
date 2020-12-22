using UnityEngine;
using System.Collections.Generic;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Represents a combat-capable group of characters
	/// </summary>
	[CreateAssetMenu(fileName = "Party", menuName = "Prototype/Party")]
	public class StratusParty : ScriptableObject
	{
		public List<StratusCharacterScriptable> Members;
		public StratusCharacterScriptable Lead;
	}

}