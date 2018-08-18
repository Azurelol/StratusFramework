using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Altostratus
{
  [CreateAssetMenu(fileName = "Weapon", menuName = "Prototype/Weapon")]
  public class Weapon : Equipment
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/  
    public override Category type { get { return Category.Weapon; } }
    public float Damage = 1.0f;
    public float Range = 2.0f;
    public float Speed = 1.0f;
    public float Penetration = 0.0f;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/  
    public override string Describe()
    {
      throw new NotImplementedException();
    }
  }

}