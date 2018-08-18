﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Genitus.Models
{
  [Serializable]
  public class StandardEquipment : Character.EquipmentModel
  {
    public Weapon weapon;
    public Armor armor;
  }

}