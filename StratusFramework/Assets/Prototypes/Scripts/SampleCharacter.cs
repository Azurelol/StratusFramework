using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Genitus;
using Genitus.Models;
using Stratus;

namespace Prototype
{
  [CreateAssetMenu(fileName = "Sample Character", menuName = "Prototype/Sample Character")]
  public class StandardCharacter : Character<LevelExponentialProgressionModel,
                                             StandardAttributeModel,
                                             ManaModel,
                                             ManaSkill,
                                             StandardEquipmentModel>, 
                                             StandardParameterModel
  {
    public VariableAttribute hitpoints => attributes.hitpoints;
    public float physicalOffense => attributes.attack.current;
    public float physicalDefense => attributes.defense.current;
    public float magicalOffense => attributes.magic.current;
    public float magicalDefense => attributes.resistance.current;
    public float speed => attributes.agility.current;
    public float accuracy => attributes.dexterity.current;
    public float evasion => attributes.agility.current;
    public float movement => attributes.agility.current;
    public float weaponRange => equipment.weapon.Range;
  }

}