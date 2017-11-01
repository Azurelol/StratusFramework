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
    public abstract partial class Agent : MonoBehaviour
    {
      //------------------------------------------------------------------------/
      // Event Declarations
      //------------------------------------------------------------------------/
      /// <summary>
      /// Base class for all status events
      /// </summary>
      public abstract class StatusEvent : Stratus.Event
      {
        public Agent Agent;
      }
      /// <summary>
      /// Signals that the agent has spawned
      /// </summary>
      public class SpawnEvent : StatusEvent
      {
        public SpawnEvent(Agent agent) { Agent = agent; }
      }
      /// <summary>
      /// Signals that the agent has died
      /// </summary>
      public class DeathEvent : StatusEvent
      {
        public DeathEvent(Agent agent) { Agent = agent; }
      }

      /// <summary>
      /// Base class for all engagement events
      /// </summary>
      public abstract class EngagementEvent : Stratus.Event
      {
        public Agent Agent;
      }
      /// <summary>
      /// Signals the the agent has engaged the target
      /// </summary>
      public class EngagedEvent : EngagementEvent
      {
        //public EngagedEvent(Agent agent) { Agent = agent; }
      }
      /// <summary>
      /// Signals that the agent has disengaged from the target
      /// </summary>
      public class DisengagedEvent : EngagementEvent
      {
        //public DisengagedEvent(Agent agent) { Agent = agent; }
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
      public class MoveToEvent : Stratus.Event
      {
        public Vector3 Point;
        public MoveToEvent(Vector3 point)
        {
          Point = point;
        }
      }

      /// <summary>
      /// Signals that the agent should stop its current action
      /// </summary>
      public class StopEvent : Stratus.Event
      {
      }

      /// <summary>
      /// Signals an interactive object that the agent wants to interact with it
      /// </summary>
      public class InteractEvent : Stratus.Event
      {
        public GameObject Object;
      }

      /// <summary>
      /// Signals the agent that the given object can be interacted with. 
      /// </summary>
      public class InteractionAvailableEvent : Stratus.Event
      {
        public InteractionTrigger Interactive;
        public string Context;
      }

    }
  }

}