/******************************************************************************/
/*!
@file   AutonomousAgent.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

namespace Stratus
{
  namespace AI
  {

    /// <summary>
    /// An autonomous agent is driven by a set of heuristics and preset behaviours
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AutonomousAgent : Agent
    {
      //------------------------------------------------------------------------/
      // Public Fields
      //------------------------------------------------------------------------/
      [Header("Settings")]
      /// <summary>
      /// How long to wait before deciding on the next action
      /// </summary>
      [Range(0.4f, 2f)]
      public float AssessmentTime = 1.0f;
      /// <summary>
      /// The collection of behaviors to run on this agent (a behavior system such as a BT, Planner, etc)
      /// </summary>
      public BehaviorSystem Behaviors;
      /// <summary>
      /// The blackboard this agent is using
      /// </summary>
      public override Blackboard blackboard { get { return Behaviors.Blackboard; } }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/   
      private ActionSet interruptSequence;
      private ActionSet assessmentSequence;

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/ 
      protected virtual void OnAssessmentStarted() { }
      protected virtual void OnAssessmentCancelled() { }
      protected virtual void OnInterrupt(float duration) { }

      //------------------------------------------------------------------------/
      // Virtual
      //------------------------------------------------------------------------/
      protected override void Subscribe()
      {
        base.Subscribe();
        // Behaviors
        this.gameObject.Connect<Behavior.StartedEvent>(this.OnBehaviorStartedEvent);
        this.gameObject.Connect<Behavior.EndedEvent>(this.OnBehaviorEndedEvent);
      }

      protected override void OnDeath()
      {
        CancelPreviousAssessment();
        if (interruptSequence != null) interruptSequence.Cancel();
      }

      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/
      void OnBehaviorStartedEvent(Behavior.StartedEvent e)
      {
        Behaviors.OnBehaviorStarted(e.Behavior);
      }

      void OnBehaviorEndedEvent(Behavior.EndedEvent e)
      {
        Behaviors.OnBehaviorEnded(e.Behavior);
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Assesses the agent's next action after a given period of time
      /// </summary>
      public void Assess()
      {
        if (assessmentSequence != null && assessmentSequence.isActive)
        {
          return;
        }

        // wat
        if (this == null)
          return;

        if (debug)
          Trace.Script("Assessing the next action in " + this.AssessmentTime + " seconds", this);

        CancelPreviousAssessment();
        assessmentSequence = Actions.Sequence(this);
        Actions.Delay(assessmentSequence, this.AssessmentTime);
        Actions.Call(assessmentSequence, this.OnAssessmentStarted);
      }

      /// <summary>
      /// Interrupts this agent's behavior momentarily
      /// </summary>
      /// <param name="duration"></param>
      public void Interrupt(float duration)
      {
        if (debug)
          Trace.Script("Interrupting this agent for " + duration + " seconds!", this);
        
        interruptSequence = Actions.Sequence(this);
        Actions.Call(interruptSequence, CancelPreviousAssessment);
        Actions.Call(interruptSequence, Pause);
        Actions.Call(interruptSequence, OnAssessmentCancelled);

        if (debug)
          Actions.Trace(interruptSequence, "Previous assessment cancelled");

        Actions.Delay(interruptSequence, duration);
        Actions.Call(interruptSequence, Resume);
        Actions.Call(interruptSequence, this.Assess, duration);

        OnInterrupt(duration);
      }

      /// <summary>
      /// Cancels the previous asssessment sequence
      /// </summary>
      void CancelPreviousAssessment()
      {
        if (assessmentSequence == null)
          return;

        //Trace.Script("Cancelling previous assessment", this);
        assessmentSequence.Cancel();
        assessmentSequence = null;
      }

    }
  }

}