/******************************************************************************/
/*!
@file   AgentDebug.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Provides debugging tools and information for AI agents
    /// </summary>
    [RequireComponent(typeof(Agent))]
    public class AgentDebug : MonoBehaviour
    {
      private Agent Agent { get { return GetComponent<Agent>(); } }

      void Start()
      {
        //Overlay.Watch(() => Agent.Target, "Target", this);      
        StratusGUI.Watch(() => Agent.currentState, "Behavior", this);
      }


    }
  }

}