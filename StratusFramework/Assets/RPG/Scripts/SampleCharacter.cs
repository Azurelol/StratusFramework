using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Altostratus;
using Altostratus.Models;

namespace Prototype
{
  [CreateAssetMenu(fileName = "Sample Character", menuName = "Prototype/Sample Character")]
  public class StandardCharacter : Character<StandardAttributeModel, StandardEquipmentModel, LevelExponentialProgressionModel>, StandardParameterModel
  {
    public Combat.Attribute hitpoints => attributes.hitpoints;
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