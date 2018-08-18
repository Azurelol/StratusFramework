/******************************************************************************/
/*!
@file   EnchantmentAttribute.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Genitus
{
  /**************************************************************************/
  /*!
  @class EnchantmentAttribute 
  */
  /**************************************************************************/
  [CreateAssetMenu(fileName = "Enchantment", menuName = "Prototype")]
  public class EnchantmentAttribute : ScriptableObject
  {
    public enum ProcType { OnDamageDealt, OnDamageTaken }
    public ProcType Type;
    [Range(1.0f, 100.0f)] public float Chance = 1.0f;
    public List<EffectAttribute> Effects = new List<EffectAttribute>();

    public void Apply(CombatController target)
    {
      // Roll on whether this enchantment will proc
    }
  }
  



}