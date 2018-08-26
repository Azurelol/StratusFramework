using UnityEngine;
using System;
using Stratus.Prototyping;

namespace Stratus
{
  namespace AI
  {
    namespace Testing
    {
      public class AgentController : MouseDrivenController
      {
        //public EventField eventField;
        [Tooltip("The agent to send commands to")]
        public Agent agent;

        /// <summary>
        /// Selects an agent to control
        /// </summary>
        /// <param name="hit"></param>
        protected override void OnLeftMouseButtonDown(RaycastHit hit)
        {
          var target = hit.transform.GetComponent<Agent>();
          if (target)
          {
            this.agent = target;
            Trace.Script("Now controlling " + this.agent);
            //this.RepositionOnAgent();
          }
        }

        /// <summary>
        /// Orders the selected agent to stop its current action
        /// </summary>
        /// <param name="hit"></param>
        protected override void OnMiddleMouseButtonDown(RaycastHit hit)
        {
          this.agent.Stop();
        }

        /// <summary>
        /// Orders the selected agent to move to a target location. If there is an enemy at that location, attack it.
        /// </summary>
        /// <param name="hit"></param>
        protected override void OnRightMouseButtonDown(RaycastHit hit)
        {
          if (!this.agent)
            return;

          var otherAgent = hit.transform.GetComponent<Agent>();
          if (otherAgent)
          {
            this.agent.Target(otherAgent);
          }
          else
          {
            this.agent.MoveTo(hit.point);
          }
        }

        void RepositionOnAgent()
        {
          if (!this.agent)
            return;

          // Find this in a better way?
          //this.transform.SetParent(this.Agent.transform);
          var offSet = 3f;
          var newPos = new Vector3(this.agent.transform.position.x, offSet, this.agent.transform.position.z);
          this.transform.position = newPos;
        }

        private void OnValidate()
        {
          if (this.agent)
          {
            this.RepositionOnAgent();
          }
        }

        protected override void OnUpdate()
        {
          this.RepositionOnAgent();
        }
      } 
    }
  }

}