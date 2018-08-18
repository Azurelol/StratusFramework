/******************************************************************************/
/*!
@file   Equipment.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Altostratus
{
  /// <summary>
  /// Base class for all equipment (weapons, armor, accessories)
  /// </summary>
  public abstract class Equipment : Item
  {
    [Serializable]
    public class AttributeScaling
    {
      [Range(0.0f, 1.0f)]
      public float Strength = 1.0f;
      [Range(0.0f, 1.0f)]
      public float Agility = 1.0f;
      [Range(0.0f, 1.0f)]
      public float Intelligence = 1.0f;
    }

    [Serializable]
    public class DamageResistances
    {
      [Range(0.0f, 100.0f)]
      public float Physical = 0.0f;
      [Range(0.0f, 100.0f)]
      public float Magical = 0.0f;
    }

    public AttributeScaling Scaling;
    public DamageResistances Resistances;
    public List<EnchantmentAttribute> Enchantments = new List<EnchantmentAttribute>();
  }




}