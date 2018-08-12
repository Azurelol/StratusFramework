/******************************************************************************/
/*!
@file   EffectAttributeList.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Prototype
{
  [CreateAssetMenu(fileName = "EffectsList", menuName = "Prototype/EffectsList")]
  public class EffectsList : ScriptableObject
  {
    public List<EffectAttribute> Effects = new List<EffectAttribute>();  
  }
}
