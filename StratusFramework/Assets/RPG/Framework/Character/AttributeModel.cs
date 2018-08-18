using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Altostratus.Models
{
  [Serializable]
  public class StandardAttributeModel : Character.AttributeModel
  {
    public Combat.Attribute hitpoints = new Combat.Attribute(10);
    public Combat.Attribute mPoints;

    public Combat.Attribute attack;
    public Combat.Attribute defense;

    public Combat.Attribute magic;
    public Combat.Attribute resistance;

    public Combat.Attribute dexterity;
    public Combat.Attribute agility;
  }
}