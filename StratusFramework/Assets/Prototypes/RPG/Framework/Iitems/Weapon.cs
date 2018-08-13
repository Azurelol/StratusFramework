/******************************************************************************/
/*!
@file   Weapon.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Prototype
{


  /**************************************************************************/
  /*!
  @class Weapon 
  */
  /**************************************************************************/
  [CreateAssetMenu(fileName = "Weapon", menuName = "Prototype/Weapon")]
  public class Weapon : Equipment
  {
    public override Category type { get { return Category.Weapon; } }
    public float Damage = 1.0f;
    public float Range = 2.0f;
    public float Speed = 1.0f;
    public float Penetration = 0.0f;
    //------------------------------------------------------------------------/
    public static List<Weapon> Weapons = new List<Weapon>();
    public static void Register(Weapon weapon)
    {
      //Trace.Script("Added '" + weapon.gameObject.name + "'");
      Weapons.Add(weapon);
    }
    //------------------------------------------------------------------------/
    // Stats
    //public bool Visible { set { Renderer.enabled = value; } }

    //------------------------------------------------------------------------/

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    void Start()
    {
      this.Register();
    }

    public void Attack()
    {
      // Enable the collider

      // Play the swwing animation

      // Disable the collider once the melee attack has finished
    }
    
    void Register()
    {
      Weapon.Register(this);
    }

    public override string Describe()
    {
      throw new NotImplementedException();
    }
  }

}