/******************************************************************************/
/*!
@file   FEAgentDriver.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
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
        [Tooltip("The agent to send commands to")]
        public Agent Agent;

        /// <summary>
        /// Selects an agent to control
        /// </summary>
        /// <param name="hit"></param>
        protected override void OnLeftMouseButtonDown(RaycastHit hit)
        {
          var target = hit.transform.GetComponent<Agent>();
          if (target)
          {
            this.Agent = target;
            Trace.Script("Now controlling " + this.Agent);
            //this.RepositionOnAgent();
          }
        }

        /// <summary>
        /// Orders the selected agent to stop its current action
        /// </summary>
        /// <param name="hit"></param>
        protected override void OnMiddleMouseButtonDown(RaycastHit hit)
        {
          this.Agent.Stop();
        }

        /// <summary>
        /// Orders the selected agent to move to a target location. If there is an enemy at that location, attack it.
        /// </summary>
        /// <param name="hit"></param>
        protected override void OnRightMouseButtonDown(RaycastHit hit)
        {
          if (!this.Agent)
            return;

          var target = hit.transform.GetComponent<Agent>();
          if (target && target != this.Agent)
          {
            this.Agent.Engage(target);
          }
          else
          {
            this.Agent.MoveTo(hit.point);
          }
        }

        void RepositionOnAgent()
        {
          if (!this.Agent)
            return;

          // Find this in a better way?
          //this.transform.SetParent(this.Agent.transform);
          var offSet = 3f;
          var newPos = new Vector3(this.Agent.transform.position.x, offSet, this.Agent.transform.position.z);
          this.transform.position = newPos;
        }

        private void OnValidate()
        {
          if (this.Agent)
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