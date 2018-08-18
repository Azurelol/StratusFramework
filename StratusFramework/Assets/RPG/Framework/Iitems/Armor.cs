/******************************************************************************/
/*!
@file   Armor.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Altostratus
{
  /**************************************************************************/
  /*!
  @class Armor 
  */
  /**************************************************************************/
  [CreateAssetMenu(fileName = "Armor", menuName = "Prototype/Armor")]
  public class Armor : Equipment
  {
    public override Category type { get { return Category.Armor; } }
    public float Defense = 0.0f;

    public override string Describe()
    {
      throw new NotImplementedException();
    }
  }

}