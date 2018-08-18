using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Stratus;
using System;

namespace Genitus
{
  /// <summary>
  /// Defines how skills are given targets
  /// </summary>
  public abstract class TargetingModel
  {
    /// <summary>
    /// Evaluates a list of possible targets according to this targeting model
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public abstract CombatController[] EvaluateTargets(CombatController caster, CombatController[] targets, Combat.TargetingParameters type);

    /// <summary>
    /// Given an array of combatants, filters out those who aren't valid targets for the given targeting parameters.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="availableTargets"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static CombatController[] FilterTargets(CombatController user, CombatController[] availableTargets, Combat.TargetingParameters type)
    {
      CombatController[] targets = null;

      // Self
      if (type == Combat.TargetingParameters.Self)
      {
        targets = new CombatController[1] { user }; 
      }

      // Allies
      else if (type == Combat.TargetingParameters.Ally)
      {
        switch (user.faction)
        {
          case CombatController.Faction.Player:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Player select target).ToArray();
            break;

          case CombatController.Faction.Hostile:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Hostile select target).ToArray();
            break;
          case CombatController.Faction.Neutral:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Neutral select target).ToArray();
            break;
        }
      }
      
      // Enemies
      else if (type == Combat.TargetingParameters.Enemy)
      {
        switch (user.faction)
        {
          case CombatController.Faction.Player:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Hostile select target).ToArray();
            break;

          case CombatController.Faction.Hostile:
            targets = (from CombatController target in availableTargets where target.faction == CombatController.Faction.Player select target).ToArray();
            break;
          case CombatController.Faction.Neutral:
            targets = (from CombatController target in availableTargets
                       where (target.faction == CombatController.Faction.Player || target.faction == CombatController.Faction.Hostile)
                       select target).ToArray();
            break;

        }
      }

      // Any
      else if (type == Combat.TargetingParameters.Any)
      {
        targets = availableTargets;
      }

      return (from CombatController controller in targets where controller.currentState == CombatController.State.Active select controller).ToArray();
    }
  }
}

namespace Genitus.Models
{
  [Serializable]
  public class RangeTargeting : TargetingModel 
  {
    /// <summary>
    /// Range required for casting the skill
    /// </summary>
    [Tooltip("Range to the target that is required for casting the skill")]
    [Range(1.0f, 100.0f)]
    public float range = 3.0f;

    public override CombatController[] EvaluateTargets(CombatController caster, CombatController[] targets, Combat.TargetingParameters type)
    {
      return FilterTargets(caster, targets, type).Where(x => Vector3.Distance(x.transform.position, caster.transform.position) <= this.range).ToArray();
    }
  }

}