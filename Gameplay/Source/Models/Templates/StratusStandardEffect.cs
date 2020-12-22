using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay.Models
{
	public abstract class StratusStandardEffect<Parameters> : StratusCombatEffect<StratusStandardCharacterParameters<Parameters>>
		where Parameters : StratusCharacter
	{
	}

	public abstract class StratusStandardHealthModificationEffect<Parameters> : StratusHealthModificationEffect<Parameters>
		where Parameters : IStratusCombatParameterModel
	{
	}

	public abstract class StratusStandardDamageEffect<Parameters> : StratusHealthModificationEffect<Parameters>
		where Parameters : IStratusCombatParameterModel
	{
	}

}