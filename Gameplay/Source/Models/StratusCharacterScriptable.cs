using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;
using Stratus.OdinSerializer;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Data about a given character, ranging from attributes to equipped skills, etc
	/// </summary>
	public abstract class StratusCharacterScriptable : StratusScriptable
	{
	}

	/// <summary>
	/// A scriptable asset for a character
	/// </summary>
	/// <typeparam name="CharacterType"></typeparam>
	public abstract class StratusCharacterScriptable<CharacterType> : StratusScriptable
		where CharacterType : StratusCharacter, new()
	{
		[SerializeReference]
		public CharacterType character = new CharacterType();

		public CharacterType ExportCharacter()
		{
			CharacterType copy = (CharacterType)OdinSerializer.SerializationUtility.CreateCopy(character);
			return copy;
		}
	}


}