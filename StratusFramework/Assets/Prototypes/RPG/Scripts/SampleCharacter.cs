using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Prototype
{
  [SerializeField]
  public class BasicAttributes : Character.Attributes
  {
    [Tooltip("The starting level of this character")]
    public int level = 1;
    public float health = 10;
    public float stamina = 100;
    public float attack = 1;
    public float defense = 1;
    public float speed = 1;
  }

  public class BasicCharacter : Character<BasicAttributes>
  {
  }

}