//using UnityEngine;
//using Stratus;
//using System.Collections.Generic;
//using System.Linq;
//using System;
//using Stratus.Utilities;

//namespace Stratus.Gameplay
//{
	

//		//------------------------------------------------------------------------/
//		// Methods
//		//------------------------------------------------------------------------/
//		///// <summary>
//		///// Finds all targets of a specified type available to this character within a specified range
//		///// </summary>
//		///// <param name="type"></param>
//		///// <param name="radius"></param>
//		///// <param name="state"></param>
//		///// <returns></returns>
//		//public static StratusCombatController[] FindTargetsOfTypeInWorld(StratusCombatController source, StratusCombat.TargetingParameters type, float radius = 20f, State state = State.Active)
//		//{
//		//	var hits = Physics.OverlapSphere(source.transform.position, radius);
//		//	var availableTargets = (from Collider hit in hits
//		//							where hit.GetComponent<StratusCombatController>()
//		//							select hit.GetComponent<StratusCombatController>()).ToArray();
//		//	return FindTargetsOfTypeInWorld(source, availableTargets, type, state);
//		//}


//		///// <summary>
//		///// Finds all targets of a specified type available to this character
//		///// </summary>
//		///// <param name="availableTargets"></param>
//		///// <param name="type"></param>
//		///// <param name="state"></param>
//		///// <returns></returns>
//		//public static StratusCombatController[] FindTargetsOfTypeInWorld(StratusCombatController source, StratusCombatController[] availableTargets, StratusCombat.TargetingParameters type, State state = State.Active)
//		//{
//		//	StratusCombatController[] targets = null;
//		//	if (type == StratusCombat.TargetingParameters.Self)
//		//	{
//		//		targets = new StratusCombatController[] { source };
//		//	}
//		//	else if (type == StratusCombat.TargetingParameters.Ally)
//		//	{
//		//		switch (source.faction)
//		//		{
//		//			case StratusCombatController.TargetType.Player:
//		//				targets = FilterTargets(availableTargets, TargetType.Player);
//		//				break;

//		//			case StratusCombatController.TargetType.Hostile:
//		//				targets = FilterTargets(availableTargets, TargetType.Hostile);
//		//				break;
//		//			case StratusCombatController.TargetType.Neutral:
//		//				targets = FilterTargets(availableTargets, TargetType.Hostile | TargetType.Player);
//		//				break;
//		//		}
//		//	}
//		//	// ENEMIES0
//		//	else if (type == StratusCombat.TargetingParameters.Enemy)
//		//	{
//		//		switch (source.faction)
//		//		{
//		//			case StratusCombatController.TargetType.Player:
//		//				targets = FilterTargets(availableTargets, TargetType.Hostile);
//		//				break;
//		//			case StratusCombatController.TargetType.Hostile:
//		//				targets = FilterTargets(availableTargets, TargetType.Player);
//		//				break;
//		//			case StratusCombatController.TargetType.Neutral:
//		//				targets = FilterTargets(availableTargets, TargetType.Hostile | TargetType.Player);
//		//				break;
//		//		}
//		//	}

//		//	return (from StratusCombatController controller in targets where controller.currentState == state select controller).ToArray();
//		//}

//		///// <summary>
//		///// 
//		///// </summary>
//		///// <param name="availableTargets"></param>
//		///// <param name="faction"></param>
//		///// <returns></returns>
//		//public static StratusCombatController[] FilterTargets(StratusCombatController[] availableTargets, TargetType faction)
//		//{
//		//	return (from StratusCombatController target in availableTargets
//		//			where target.faction == faction
//		//			select target).ToArray();
//		//}

//		/* 
//		 NOTE: The Combat Controller is purely-event driven. It's action cycle is defined as follows:
//		 * I.   SelectAction:  Asks the player/autonomous agent to pick an action. If an action was
//		 *                    successfully selected, it will be queued. Otherwise we try again.
//		 * II.  QueueEvent:    Adds an action to the character's queue, usually setting it as the current action.
//		 *                    The controller will now start updating that action.
//		 * III. StartedEvent: Once the action has determined the agent is within the specified range of its target,
//		 *                    it will start casting.
//		 * IV.  TriggerEvent:  Once the action has finished casting, it will be triggered. 
//		 *                    After waiting a specified animation duration time, it will execute.
//		 * V.   ExecuteEvent:   At this point the action will send its according event to its specified target.
//		 * VI.  EndedEvent:    The action has finished executing. At this point, we start again from the top.
//		*/

//	}

//}