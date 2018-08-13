using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{  
  /// <summary>
  /// Basic attributes for a character
  /// </summary>
  [Serializable]
  public class Attributes
  {
    [Tooltip("The starting level of this character")]
    public int level = 1;
    public float health = 10;
    public float stamina = 100;
    public float attack = 1;
    public float defense = 1;
    public float speed = 1;
  }
}