/******************************************************************************/
/*!
@file   InteractionTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System;

namespace Stratus
{
  namespace AI
  {
    /// <summary>
    /// Receives interaction events with the player
    /// </summary>
    public class InteractionTrigger : Trigger
    {
      public enum TriggerType { Interaction, Scan }

      [Header("Interaction")]
      public TriggerType Type = TriggerType.Interaction;
      public string Context;

      /// <summary>
      /// Initializes the InteractionTrigger, subscribing to events with the player
      /// </summary>
      protected override void OnAwake()
      {
        this.gameObject.Connect<Sensor.ScanEvent>(this.OnScanEvent);
        this.gameObject.Connect<Agent.InteractEvent>(this.OnInteractEvent);

      }
      

      /// <summary>
      /// Received when this object is within vicinity of the agent.
      /// </summary>
      /// <param name="e"></param>
      void OnScanEvent(Sensor.ScanEvent e)
      {
        if (!enabled)
          return;

        e.Context = this.Context;
        if (Type == TriggerType.Scan)
          this.Activate();
        else if (Type == TriggerType.Interaction)
        {
          var response = new Agent.InteractionAvailableEvent();
          response.Interactive = this;
          response.Context = this.Context;
          e.Agent.gameObject.Dispatch<Agent.InteractionAvailableEvent>(response);
        }
      }

      /// <summary>
      /// Received when there's a request to interact with this object
      /// </summary>
      /// <param name="e"></param>
      void OnInteractEvent(Agent.InteractEvent e)
      {
        if (Type == TriggerType.Interaction)
          this.Activate();
      }


    }

  } 
}