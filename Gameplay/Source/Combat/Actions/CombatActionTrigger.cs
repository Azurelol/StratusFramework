/******************************************************************************/
/*!
@file   CombatActionTrigger.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using Stratus.Gameplay;
using System;

namespace AzureCross
{
  public class CombatActionTrigger : StratusTriggerBehaviour
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    [Header("Actions")]
    [Tooltip("The character whose events we are listening to")]
    public StratusCombatController source;
    [Tooltip("On what phase of the combat action will this trigger be fired off")]
    public StratusCombatAction.Phase phase = StratusCombatAction.Phase.Execute;

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      this.Subscribe();
    }

    protected override void OnReset()
    {
      
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    void Subscribe()
    {
      switch (phase)
      {
        case StratusCombatAction.Phase.Queue:
          this.source.gameObject.Connect<StratusCombatAction.QueueEvent>(this.OnCombatActionQueuedEvent);
          break;
        case StratusCombatAction.Phase.Started:
          this.source.gameObject.Connect<StratusCombatAction.StartedEvent>(this.OnCombatActionStartedEvent);
          break;
        case StratusCombatAction.Phase.Trigger:
          this.source.gameObject.Connect<StratusCombatAction.TriggerEvent>(this.OnCombatActionTriggerEvent);
          break;
        case StratusCombatAction.Phase.Execute:
          this.source.gameObject.Connect<StratusCombatAction.ExecuteEvent>(this.OnCombatActionExecuteEvent);
          break;
        case StratusCombatAction.Phase.Ended:
          this.source.gameObject.Connect<StratusCombatAction.EndedEvent>(this.OnCombatActionEndedEvent);
          break;
      }
    }

    /// <summary>
    /// Received when a combat action has been selected
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionQueuedEvent(StratusCombatAction.QueueEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when an action has started casting
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionStartedEvent(StratusCombatAction.StartedEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when an action is ready to be executed
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionTriggerEvent(StratusCombatAction.TriggerEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when a combat action is being executed
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionExecuteEvent(StratusCombatAction.ExecuteEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when a combat action has finished executing
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionEndedEvent(StratusCombatAction.EndedEvent e)
    {
      this.Activate();
    }


  }
}