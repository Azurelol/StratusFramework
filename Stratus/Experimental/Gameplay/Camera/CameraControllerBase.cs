using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  namespace Gameplay
  {
    /// <summary>
    /// Base camera for all camera controllers, with common functionality and utility functions
    /// that may be used among a wide variety of camera design schemes
    /// </summary>
    public abstract class CameraControllerBase : MonoBehaviour
    {
      //----------------------------------------------------------------------/
      // Event Declarations
      //----------------------------------------------------------------------/
      /// <summary>
      /// Received by this controller, which then forwards it onto the Cinemachine camera
      /// </summary>
      public class InputEvent : Stratus.Event { public Vector2 Axis; }

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      [Header("General")]
      /// <summary>
      /// If active, receives input
      /// </summary>
      [Tooltip("If active, activates input for the cinemachine camera")]
      public bool acceptInput = true;

      /// <summary>
      /// Whether to invert the input along the axis
      /// </summary>
      public bool invertAxisInput = false;

      //----------------------------------------------------------------------/
      // Inheritance
      //----------------------------------------------------------------------/
      protected abstract void OnAwake();
      protected abstract void OnUpdate();
      protected virtual void OnInput(Vector2 axis) {}

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      private void Awake()
      {
        this.OnAwake();
        this.Subscribe();
      }

      private void Update()
      {
        this.OnUpdate();
      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      private void Subscribe()
      {
        this.gameObject.Connect<InputEvent>(this.OnInputEvent);
      }

      private void OnInputEvent(InputEvent e)
      {
        if (invertAxisInput) e.Axis = -e.Axis;
        OnInput(e.Axis);
      }

    } 
  }

}