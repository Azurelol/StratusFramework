/******************************************************************************/
/*!
@file   Combat.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class Combat 
  */
  /**************************************************************************/
  public class Combat
  {
    public enum StatusEffect { Poison, Stunned, Sleep }
    public class StartedEvent : Stratus.Event { public CombatEncounter Encounter; }
    public class EndedEvent : Stratus.Event {}

    /**************************************************************************/
    /*!
    @struct Attack 
    */
    /**************************************************************************/
    public struct Attack
    {
      public int Potency;
      public int Hits;
      // Maybe atributes?
    }






    [Serializable]
    public class Range
    {
      public float Short = 3.0f;
      public float Long = 10.0f;
    }



    //public abstract class Command
    //{
    //  //----------------------------------------------------------------------/
    //  public class ExecuteEvent : Stratus.Event { public Command Command; }
    //  public class CompletedEvent : Stratus.Event { public Command Command; }
    //  public class FailedEvent : Stratus.Event { public Command Command; }
    //  //----------------------------------------------------------------------/
    //  public bool IsFinished = false;      
    //  public abstract void Update();

    //}

  }

}