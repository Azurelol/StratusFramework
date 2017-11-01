/******************************************************************************/
/*!
@file   InteractiveObject.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  namespace AI
  {
    public abstract class InteractiveObject : MonoBehaviour
    {
      public enum TriggerType
      {
        Interact,
        CollisionEnter,
        CollisionExit,
        //DialogEnded,
        //DialogStarted
      }

      public bool Enabled = true;
      public TriggerType Trigger = TriggerType.Interact;
      public bool Tracing = false;

      protected string Context;
      string Tag = "Player";

      /**************************************************************************/
      /*!
      @brief  Initializes the Script.
      */
      /**************************************************************************/
      void Start()
      {
        if (Enabled)
        {
          if (Trigger == TriggerType.Interact)
          {
            this.gameObject.Connect<Agent.InteractEvent>(this.OnInteractEvent);
            this.gameObject.Connect<Sensor.ScanEvent>(this.OnScanEvent);
          }
          //else if (Trigger == TriggerType.DialogStarted)
          //{
          //  this.gameObject.Connect<ObjectDialog.DialogStartedEvent>(this.OnDialogStartedEvent);
          //}
          //else if (Trigger == TriggerType.DialogEnded)
          //{
          //  this.gameObject.Connect<ObjectDialog.DialogEndedEvent>(this.OnDialogEndedEvent);
          //}
        }

        this.OnInitialize();
      }

      /**************************************************************************/
      /*!
      @brief If its activated when it detects a collision with a target..
      */
      /**************************************************************************/
      void OnCollisionEnter(Collision collision)
      {
        if (!Enabled && !(Trigger == TriggerType.CollisionEnter))
          return;

        if (collision.collider.gameObject.tag.Contains(Tag))
          this.OnInteraction();
      }

      /**************************************************************************/
      /*!
      @brief If its activated when it detects a collision with a target..
      */
      /**************************************************************************/
      void OnCollisionExit(Collision collision)
      {
        if (!Enabled && !(Trigger == TriggerType.CollisionExit))
          return;

        if (collision.collider.gameObject.tag.Contains(Tag))
          this.OnInteraction();
      }

      /**************************************************************************/
      /*!
      @brief  Received when this object is within vicinity of the player.
      */
      /**************************************************************************/
      void OnScanEvent(Sensor.ScanEvent e)
      {
        if (!Enabled)
          return;

        e.Context = this.Context;
      }

      /**************************************************************************/
      /*!
      @brief  Received when there's a request to interact with this object.
      */
      /**************************************************************************/
      void OnInteractEvent(Agent.InteractEvent e)
      {
        if (!Enabled)
          return;

        this.OnInteraction();
      }

      //void OnDialogStartedEvent(ObjectDialog.DialogStartedEvent e)
      //{
      //  if (Enabled)

      //    this.OnInteraction();
      //}

      //void OnDialogEndedEvent(ObjectDialog.DialogEndedEvent e)
      //{
      //  if (Enabled)

      //    this.OnInteraction();
      //}

      protected abstract void OnInitialize();
      protected abstract void OnInteraction();

    }

  } 
}