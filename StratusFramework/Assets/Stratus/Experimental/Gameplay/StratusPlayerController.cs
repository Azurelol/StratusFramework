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
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(NavMeshAgent))]
  public class StratusPlayerController : ExtensibleBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    public enum MovementOffset
    {
      PlayerForward,
      CameraUp
    }


    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [Tooltip("Whether to print debug output")]
    public bool debug = false;
    [Header("Input")]
    public InputField movementX = new InputField();
    public InputField movementY = new InputField();
    [Tooltip("The camera used to orient this movement by")]
    public new Camera camera;

    [Header("Movement")]
    public float movementThreshold = 0.2f;
    public float rotationSpeed = 1f;
    public bool faceDirection = true;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public MovementOffset movementOffset { get; set; } = MovementOffset.PlayerForward;
    public NavMeshAgent navigation { get; private set; }
    public new Rigidbody rigidbody { get; private set; }
    public Func<Vector3> calculateDirectionFunction { get; private set; }
    public bool isMoving => Math.Abs(rigidbody.velocity.x) > movementThreshold || Math.Abs(rigidbody.velocity.z) > movementThreshold;
    public Vector3 heading { get; private set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      heading = transform.forward;
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

      //if (faceDirection)
      //  transform.forward = Vector3.Lerp(transform.forward, heading, Time.deltaTime * 2f);
    }

    //--------------------------------------------------------------------------------------------/
    // Methods: Movement
    //--------------------------------------------------------------------------------------------/
    private void Move()
    {
      Vector2 axis = new Vector2(movementX.value, movementY.value);
      Vector3 dir = CalculateDirection(axis, movementOffset);
      heading = dir;
      //Trace.Script($"Heading = {heading}");
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
        case MovementOffset.CameraUp:
          dir = (axis.y * camera.transform.up) + (axis.x * camera.transform.right);
          dir.y = 0f;
          break;
        default:
          break;
      }
      return dir;
    }


  }
}
