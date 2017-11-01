/******************************************************************************/
/*!
@file   Timers.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace Utilities
  {
    /// <summary>
    /// A component which updates all timers registered to it, automatically.
    /// This makes it so you don't have to handle the updating yourself in your
    /// MonoBehaviour's update messages.
    /// </summary>
    public class Timers : Singleton<Timers>
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      private List<Timer> All = new List<Timer>();

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected override void OnAwake()
      {

      }

      private void Update()
      {
        foreach(var timer in All)
        {
          timer.Update(Time.deltaTime);
        }
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public static void Add(Timer timer)
      {
        get.All.Add(timer);
      }

      public static void Remove(Timer timer)
      {
        get.All.Remove(timer);
      }


    }
  }
}
