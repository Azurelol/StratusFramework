using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Stratus.Experimental
{
  /// <summary>
  /// A simple, modular player controller
  /// </summary>
  [RequireComponent(typeof(Collider))]
  [RequireComponent(typeof(NavMeshAgent))]
  public class StratusPlayerController : ExtensibleBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public enum MovementOffset
    {
      PlayerForward,
      CameraForward
    }
    

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Tooltip("Whether to print debug output")]
    public bool debug = false;
    [Header("Input")]
    public InputAxisField movementX = new InputAxisField();
    public InputAxisField movementY = new InputAxisField();
    [Tooltip("The camera used to orient this movement by")]
    public new Camera camera;
    public bool pollInput = true;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public MovementOffset movementOffset { get; set; } = MovementOffset.PlayerForward;
    public NavMeshAgent navigation { get; private set; }
    public new Rigidbody rigidbody { get; private set; }
    public Func<Vector3> calculateDirectionFunction { get; private set; }    

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      rigidbody = GetComponent<Rigidbody>();
      navigation = GetComponent<NavMeshAgent>();
      navigation.Warp(transform.position);
    }

    protected override void OnStart()
    {
      
    }

    void Update()
    {
      if (!movementX.isNeutral || !movementY.isNeutral)
        Move();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Movement
    //--------------------------------------------------------------------------------------------/
    private void Move()
    {
      Vector2 axis = new Vector2(movementX.value, movementY.value);
      Vector3 dir = CalculateDirection(axis, movementOffset);
      rigidbody.velocity = dir * navigation.speed;
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public Vector3 CalculateDirection(Vector2 axis, MovementOffset offset)
    {
      Vector3 dir = Vector3.zero;
      switch (offset)
      {
        case MovementOffset.PlayerForward:
          dir = new Vector3(axis.x, 0f, axis.y);
          break;
        case MovementOffset.CameraForward:
          dir = (axis.y * camera.transform.forward) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;
        default:
          break;
      }
      return dir;
    }


  }
}
