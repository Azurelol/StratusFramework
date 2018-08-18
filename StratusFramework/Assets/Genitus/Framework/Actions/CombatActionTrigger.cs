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

namespace Genitus
{
  public class CombatActionTrigger : Trigger
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    [Header("Actions")]
    [Tooltip("The character whose events we are listening to")]
    public CombatController source;
    [Tooltip("On what phase of the combat action will this trigger be fired off")]
    public CombatAction.Phase phase = CombatAction.Phase.Execute;

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
        case CombatAction.Phase.Queue:
          this.source.gameObject.Connect<CombatAction.QueueEvent>(this.OnCombatActionQueuedEvent);
          break;
        case CombatAction.Phase.Started:
          this.source.gameObject.Connect<CombatAction.StartedEvent>(this.OnCombatActionStartedEvent);
          break;
        case CombatAction.Phase.Trigger:
          this.source.gameObject.Connect<CombatAction.TriggerEvent>(this.OnCombatActionTriggerEvent);
          break;
        case CombatAction.Phase.Execute:
          this.source.gameObject.Connect<CombatAction.ExecuteEvent>(this.OnCombatActionExecuteEvent);
          break;
        case CombatAction.Phase.Ended:
          this.source.gameObject.Connect<CombatAction.EndedEvent>(this.OnCombatActionEndedEvent);
          break;
      }
    }

    /// <summary>
    /// Received when a combat action has been selected
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionQueuedEvent(CombatAction.QueueEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when an action has started casting
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionStartedEvent(CombatAction.StartedEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when an action is ready to be executed
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionTriggerEvent(CombatAction.TriggerEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when a combat action is being executed
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionExecuteEvent(CombatAction.ExecuteEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when a combat action has finished executing
    /// </summary>
    /// <param name="e"></param>
    void OnCombatActionEndedEvent(CombatAction.EndedEvent e)
    {
      this.Activate();
    }


  }
}