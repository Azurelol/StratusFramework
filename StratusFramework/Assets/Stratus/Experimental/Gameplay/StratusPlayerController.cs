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
  public class StratusPlayerController : StratusBehaviour
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
    public bool debug = false;
    [Header("Input")]
    public InputAxisField movementX = new InputAxisField();
    public InputAxisField movementY = new InputAxisField();
    [Tooltip("The camera used to orient this movement by")]
    public new Camera camera;
    public bool pollInput = true;
    [SerializeField]
    private List<StratusPlayerControllerExtension> extensionsField = new List<StratusPlayerControllerExtension>();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public MovementOffset movementOffset { get; set; } = MovementOffset.PlayerForward;
    public StratusPlayerControllerExtension[] extensions => extensionsField.ToArray();
    public NavMeshAgent navigation { get; private set; }
    public new Rigidbody rigidbody { get; private set; }
    public bool hasExtensions => extensionsField.Count > 0;
    public Func<Vector3> calculateDirectionFunction { get; private set; }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      rigidbody = GetComponent<Rigidbody>();
      navigation = GetComponent<NavMeshAgent>();
      navigation.Warp(transform.position);      
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
    public void Add(StratusPlayerControllerExtension extension)
    {
      extensionsField.Add(extension);
    }

    public void Remove(StratusPlayerControllerExtension extension)
    {
      extensionsField.Remove(extension);
    }    

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
