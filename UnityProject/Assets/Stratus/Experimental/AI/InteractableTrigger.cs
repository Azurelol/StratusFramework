/******************************************************************************/
/*!
@file   InteractionTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System;

namespace Stratus.AI
{
  /// <summary>
  /// Receives interaction events with the player
  /// </summary>
  public class InteractableTrigger : Trigger
  {
    public enum TriggerType { Interaction, Detection }

    [Header("Interaction")]
    public TriggerType type = TriggerType.Interaction;    

    /// <summary>
    /// Initializes the InteractionTrigger, subscribing to events with the player
    /// </summary>
    protected override void OnAwake()
    {
      switch (type)
      {
        case TriggerType.Interaction:
          this.gameObject.Connect<Sensor.InteractEvent>(this.OnInteractEvent);
          break;
        case TriggerType.Detection:
          this.gameObject.Connect<Sensor.DetectionEvent>(this.OnDetection);
          break;
      }
    }

    protected override void OnReset()
    {

    }

    /// <summary>
    /// Received when this object is within vicinity of the agent.
    /// </summary>
    /// <param name="e"></param>
    void OnDetection(Sensor.DetectionEvent e)
    {
      this.Activate();
    }

    /// <summary>
    /// Received when there's a request to interact with this object
    /// </summary>
    /// <param name="e"></param>
    void OnInteractEvent(Sensor.InteractEvent e)
    {
      this.Activate();
    }


  }
}