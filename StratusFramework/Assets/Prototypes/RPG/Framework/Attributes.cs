using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{  
  [Serializable]
  public class Attributes
  {
    [Serializable]
    public class Statistics
    {
      public float health = 10;
      public float stamina = 100;
      public float attack = 1;
      public float defense = 1;
      public float speed = 1;

      //public float staminaRecovery

      public Statistics Copy()
      {
        var copy = new Statistics();
        copy.health = health;
        copy.stamina = stamina;
        copy.attack = attack;
        copy.defense = defense;
        copy.speed = speed;
        return copy;
      }
    }

    [Tooltip("The starting level of this character")]
    public int level = 1;
    [Tooltip("The statistics of this character")]
    public Statistics statistics = new Statistics();



  }
}