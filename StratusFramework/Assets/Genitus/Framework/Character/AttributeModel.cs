using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Stratus;

namespace Genitus.Models
{
  [Serializable]
  public class StandardAttributes : Character.AttributeModel
  {
    public VariableAttribute hitpoints;
    public VariableAttribute mana;

    public VariableAttribute attack;
    public VariableAttribute defense;

    public VariableAttribute magic;
    public VariableAttribute resistance;

    public VariableAttribute dexterity;
    public VariableAttribute agility;
  }
}