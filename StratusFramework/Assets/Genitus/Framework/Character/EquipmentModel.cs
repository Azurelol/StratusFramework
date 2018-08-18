using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Genitus.Models
{
  [Serializable]
  public class StandardEquipmentModel : Character.EquipmentModel
  {
    public Weapon weapon;
    public Armor armor;
  }

}