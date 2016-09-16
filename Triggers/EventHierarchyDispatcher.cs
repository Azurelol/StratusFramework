/******************************************************************************/
/*!
@file   EventHierarchyDispatcher.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class EventHierarchyDispatcher 
  */
  /**************************************************************************/
  public abstract class EventHierarchyDispatcher : EventDispatcher
  {
    public enum TargetType { Self, Parent, Children }
    // On what object will this be operating on?
    protected List<GameObject> TargetObjs = new List<GameObject>();
    public TargetType Target = TargetType.Self;

    /**************************************************************************/
    /*!
    @brief  Initializes the EventHierarchyDispatcher.
    */
    /**************************************************************************/
    void Start()
    {
      SetTargets();
      this.gameObject.Connect<EventTrigger.TriggerEvent>(this.OnTriggerEvent);
      this.OnInitialize();
    }

    /**************************************************************************/
    /*!
    @brief Sets the targets for this event dispatcher.
    */
    /**************************************************************************/
    void SetTargets()
    {
      if (Target == TargetType.Self)
      {
        TargetObjs.Add(this.gameObject);
      }
      else if (Target == TargetType.Parent)
      {
        if (this.gameObject.Parent())
          TargetObjs.Add(this.gameObject.Parent());
        else
          Debug.LogError("No parent was found!");
      }
      else if (Target == TargetType.Children)
      {
        foreach (var child in this.gameObject.Children())
        {
          TargetObjs.Add(child);
        }
      }
    }

  }

}