/******************************************************************************/
/*!
@file   Space.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/9/2016
@brief  A space is the abstraction for the physical space that the set of all
        objects on the scene resides in.
*/
/******************************************************************************/
using UnityEngine;
using System;
using System.Collections;

namespace Stratus {

  class LogicUpdate : Stratus.Event
  {
    public float Dt;
  }

  public class Space : MonoBehaviour {

    static public bool DoLogicUpdate = false;

    private static Space ActiveSpace;
    public static Space Instance {
      get {
        if (!ActiveSpace) {
          Instantiate();
        }
        return ActiveSpace;
      }
    }

    static void Instantiate()
    {
      ActiveSpace = FindObjectOfType(typeof(Space)) as Space;
    }

    void Start() {
      ActiveSpace = this;
    }
    
    void Update() {

      if (DoLogicUpdate)
      {
        var eventObj = new LogicUpdate();
        eventObj.Dt = Time.deltaTime;
        this.gameObject.Dispatch<LogicUpdate>(eventObj);
      }

    }

    public static void Connect<T>(Action<T> func)
    {
      if (!Instance) Instantiate();
      Stratus.Events.Connect(Instance.gameObject, func);
    }

    public static void Dispatch<T>(T eventObj) where T : Stratus.Event
    {
      if (!Instance) Instantiate();
      Stratus.Events.Dispatch<T>(Instance.gameObject, eventObj);
      //Instance.gameObject.Dispatch<T>(eventObj);
    }
  }

}
