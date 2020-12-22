using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Defines how a character is described
	/// </summary>
	public abstract class StratusDescriptionModel
	{
		/// <summary>
		/// The default name for a character
		/// </summary>
		[SerializeField]
		private string _name;
		/// <summary>
		/// A short description of the character
		/// </summary>
		[TextArea]
		public string description;
		/// <summary>
		/// A temporary name override for this character
		/// </summary>
		public string nameOverride;
		/// <summary>
		/// The name for this character. (Can be overriden by assigning the name override)
		/// </summary>
		public string name => nameOverride.IsValid() ? nameOverride : _name;

		public override string ToString()
		{
			return $"{name}";
		}
	}

	[Serializable]
	public class StratusCharacterDescription : StratusDescriptionModel
	{
		
	}

}
