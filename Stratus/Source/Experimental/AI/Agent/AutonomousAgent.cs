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
      // Properties
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
      public override Blackboard Blackboard { get { return Behaviors.Blackboard; } }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/    
      /// <summary>
      /// The currently running action assesssment routine
      /// </summary>
      protected IEnumerator AssessmentRoutine;
      /// <summary>
      /// The currently running interrupt sequence
      /// </summary>
      protected ActionSet InterruptSequence;
      /// <summary>
      /// 
      /// </summary>
      protected ActionSet AssessmentSequence;

      protected virtual void OnAssess() {}

      //------------------------------------------------------------------------/
      // Interface
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
        CancelAssessment();
        if (InterruptSequence != null) InterruptSequence.Cancel();
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
        if (AssessmentSequence != null && AssessmentSequence.isActive)
        {
          Trace.Script("Cannot assess!", this);
          return;
        }

        if (this == null)
          return;

        //Trace.Script("Assessing the next action in " + this.AssessmentTime, this);
        //if (AssessmentRoutine != null) StopCoroutine(AssessmentRoutine);
        //AssessmentRoutine = Routines.Call(this.OnAssess, this.AssessmentTime);
        //StartCoroutine(AssessmentRoutine);

        CancelAssessment();
        AssessmentSequence = Actions.Sequence(this);
        Actions.Delay(AssessmentSequence, this.AssessmentTime);
        Actions.Call(AssessmentSequence, this.OnAssess);
      }

      /// <summary>
      /// Interrupts this agent's behavior momentarily
      /// </summary>
      /// <param name="duration"></param>
      public void Interrupt(float duration)
      {
        //Trace.Script("Interrupting this agent for " + duration + " seconds!", this);
        CancelAssessment();
        InterruptSequence = Actions.Sequence(this);
        Actions.Call(InterruptSequence, Pause);
        Actions.Delay(InterruptSequence, duration);
        Actions.Call(InterruptSequence, () => { Active = true; });
        Actions.Call(InterruptSequence, this.Assess, duration);
      }

      void CancelAssessment()
      {
        if (AssessmentSequence != null)
        {
          AssessmentSequence.Cancel();
          AssessmentSequence = null;
          //Trace.Script("Cancelling previous assessment", this);
        }
      }




    }
  }

}