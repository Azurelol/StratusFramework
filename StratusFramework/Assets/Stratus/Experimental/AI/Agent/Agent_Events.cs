/******************************************************************************/
/*!
@file   Agent_Events.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  namespace AI
  {
    public abstract partial class Agent : StratusBehaviour
    {
      //------------------------------------------------------------------------/
      // Event Declarations
      //------------------------------------------------------------------------/
      /// <summary>
      /// Base class for all status events
      /// </summary>
      public abstract class StatusEvent : Stratus.Event
      {
        public Agent agent { get; internal set; }
      }

      /// <summary>
      /// Signals that the agent has spawned
      /// </summary>
      public class SpawnEvent : StatusEvent
      {
      }

      /// <summary>
      /// Signals that the agent has died
      /// </summary>
      public class DeathEvent : StatusEvent
      {
        public float Delay;
        public DeathEvent(float delay = 0) { Delay = delay; }
      }

      /// <summary>
      /// Base class for all engagement events
      /// </summary>
      public abstract class EngagementEvent : StatusEvent
      {
      }

      /// <summary>
      /// Signals the the agent has engaged the target
      /// </summary>
      public class EngagedEvent : EngagementEvent
      {
      }

      /// <summary>
      /// Signals that the agent has disengaged from the target
      /// </summary>
      public class DisengagedEvent : EngagementEvent
      {
      }

      /// <summary>
      /// Base class for all engagement events
      /// </summary>
      public abstract class TargetingEvent : Stratus.Event
      {
        public Agent Target;
      }
      /// <summary>
      /// Signals to the agent to engage the target in combat
      /// </summary>
      public class EngageTargetEvent : TargetingEvent
      {
        public EngageTargetEvent(Agent target) { Target = target; }
      }
      /// <summary>
      /// Signals to the agent to switch the given target in combat
      /// </summary>
      public class SwitchTargetEvent : TargetingEvent
      {
        public SwitchTargetEvent(Agent target) { Target = target; }
      }

      /// <summary>
      /// Signals the agent to start considering its next action
      /// </summary>
      public class AssessEvent : StatusEvent { }

      /// <summary>
      /// Signals to the agent that it should be disabled for a set amount of time
      /// </summary>
      public class DisableEvent : Stratus.Event
      {
        public float Duration = 0f;
        public DisableEvent(float duration) { Duration = duration; }
      }

      /// <summary>
      /// Signals the agent to move to a specified position
      /// </summary>
      public class MoveEvent : StatusEvent
      {
        public Vector3 position;
        public MoveEvent(Vector3 point)
        {
          position = point;
        }
      }

      /// <summary>
      /// Signals that the agent should stop its current action
      /// </summary>
      public class StopEvent : Stratus.Event
      {
      }
 
      /// <summary>
      /// Signals the agent that the given object can be interacted with. 
      /// </summary>
      public class InteractionAvailableEvent : Stratus.Event
      {
        public InteractableTrigger Interactive;
        public string Context;
      }

    }
  }

}