using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Genitus.Models;

namespace Genitus
{  
  /// <summary>
  /// A skill that consumes mana to be activated
  /// </summary>
  [CreateAssetMenu(menuName = "Prototype/Mana Skill")]
  public class ManaSkill : Skill<ManaModel, RangeTargeting, StandardDescription>
  {
  }

}