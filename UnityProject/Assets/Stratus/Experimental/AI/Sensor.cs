/******************************************************************************/
/*!
@file   Sensor.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus.Utilities;

namespace Stratus.AI
{
  /// <summary>
  /// Handles sensing objects for an agent
  /// </summary>
  //[RequireComponent(typeof(Agent))]
  public abstract class Sensor : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/    
    /// <summary>
    /// Received by objects that can be interacted with. They will
    /// respond with a corresponding event to signal that they are able to be
    /// interacted with.
    /// </summary>
    public class DetectionEvent : Stratus.Event
    {
      /// <summary>
      /// The sensor that is outputting the scan
      /// </summary>
      public Sensor sensor;
      /// <summary>
      /// Any relevant data from the object that's been scanned to the sensor
      /// </summary>
      public object[] scanData;
    }

    /// <summary>
    /// Signals an interactive object that the sensor wants to interact with it
    /// </summary>
    public class InteractEvent : Stratus.Event
    {
      /// <summary>
      /// The sensor that wants to interact
      /// </summary>
      public Sensor sensor;
      /// <summary>
      /// Any relevant data sent from the sensor that is interacting to the object
      /// </summary>
      public object[] interactionData;
    }

    /// <summary>
    /// The results of a given scan
    /// </summary>
    public class DetectionResultEvent : Stratus.Event
    {
      /// <summary>
      /// Whether interactions were found by this sensor
      /// </summary>
      public bool hasFoundInteractions;
    }

    /// <summary>
    /// Contains information about a successful scan result
    /// </summary>
    public struct InteractionQuery
    {
      public Interactable interactable;
      public float distance;
      public object[] data;
    }

    /// <summary>
    /// How the sensor runs its scans
    /// </summary>
    public enum DetectionMode
    {
      [Tooltip("The target must be in range")]
      Range,
      [Tooltip("The target must be in range and within field of view")]
      FieldOfView,
      [Tooltip("The target must be in range, within field of view and a valid raycast target")]
      LineOfSight,
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/        
    /// <summary>
    /// The position from which the scan will be originated
    /// </summary>
    public Vector3 scanPosition { get { return root.position + offset; } }
    /// <summary>
    /// The current interactives in range
    /// </summary>
    public InteractionQuery[] interactivesInRange { get; private set; }
    /// <summary>
    /// The current object(s) that the agent can interact with
    /// </summary>
    public Interactable closestInteractable
    {
      get
      {
        if (interactivesInRange == null || interactivesInRange.Length == 0)
          return null;
        return interactivesInRange[0].interactable;
      }
    }
    /// <summary>
    /// Whether the root object for sensing is a camera
    /// </summary>
    public bool isCamera => root.GetComponent<Camera>() != null;
    /// <summary>
    /// If the root object is a camera
    /// </summary>
    public Camera rootCamera { get; private set; }
    /// <summary>
    /// The color used to show the detection volume
    /// </summary>
    private Color detectionColor => new Color(1f, 0.5f, 0f, 0.1f);
    /// <summary>
    /// The color used to show the interaction volume
    /// </summary>
    private Color interactionColor => new Color(0f, 1f, 0f, 0.1f);
    /// <summary>
    /// The currently seleceted field of view, depending on whether the root object is using a camera
    /// </summary>
    private float selectedFov => isCamera ? rootCamera.GetHorizontalFOV() : fieldOfView;

    //------------------------------------------------------------------------/
    // Public Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether there is debugging output
    /// </summary>
    [Tooltip("Whether to show debug output")]
    public bool showDebug = false;

    [Header("Detection")]
    /// <summary>
    /// The root object from which we are detecting from
    /// </summary>
    [Tooltip("The root object from which we are detecting from")]
    public Transform root;
    /// <summary>
    /// How this agent detects enemies
    /// </summary>
    [Tooltip("How this agent detects enemies")]
    public DetectionMode mode = DetectionMode.Range;
    /// <summary>
    /// The field of view of this sensor
    /// </summary>
    [Range(0.0f, 180.0f)]
    [Tooltip("The field of view for the sensor")]
    public float fieldOfView = 90f;
    /// <summary>
    /// The range at which objects are considered to be in line of sight
    /// </summary>
    [Tooltip("The range at which objects are considered to be in line of sight")]
    public float range = 30.0f;
    /// <summary>
    /// How often this sensor should be scanning
    /// </summary>
    [Range(0.0f, 3.0f)]
    [Tooltip("How often the sensor performs a scan")]
    public float scanInterval = 0.5f;

    [Header("Interaction")]
    /// <summary>
    /// The range at which the sensor can interact physically with the environment
    /// </summary>
    [Tooltip("The range at which the sensor can interact physically with the environment")]
    public float interactionRadius = 5.0f;
    /// <summary>
    /// The offset for the agents's interaction sphere
    /// </summary>
    public Vector3 offset = new Vector3();

    //------------------------------------------------------------------------/
    // Private Fields
    //------------------------------------------------------------------------/
    private Stopwatch scanTimer;
    private InteractEvent interactEvent;

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void OnAwake();
    protected abstract void OnInteractablesFound();
    protected abstract void OnInteractablesOutOfRange();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      if (!root)
        Trace.Error("No root transform selected for sensing!", this, true);

      this.interactEvent = new InteractEvent() { sensor = this };
      this.scanTimer = new Stopwatch(this.scanInterval);
      this.OnAwake();
    }

    /// <summary>
    /// Updates the sensor
    /// </summary>
    private void Update()
    {
      if (scanTimer.Update(Time.deltaTime))
      {
        Scan();
        scanTimer.Reset();
      }
    }

    private void Reset()
    {
      root = GetComponent<Transform>();
    }

    private void OnValidate()
    {
      this.rootCamera = this.root.GetComponent<Camera>();
    }

    private void OnDrawGizmosSelected()
    {
      // Can't draw if we have no root!
      if (!root)
        return;

      DrawInteractionSphere();

      switch (this.mode)
      {
        case DetectionMode.Range:
          DrawDetectionRange();
          break;
        case DetectionMode.FieldOfView:
        case DetectionMode.LineOfSight:
          DrawFieldOfView();
          break;
      }

    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Scans the world for objects we can interact with, on a given interval
    /// </summary>    
    public void Scan()
    {
      bool hadInteractions = (closestInteractable != null);
      bool foundInteractables = ScanForInteractables();

      // If we detected something
      if (foundInteractables)
      {
        if (showDebug)
          Trace.Script($"Interactives in range = {interactivesInRange.Length}, closest is '{closestInteractable}'", this);
        OnInteractablesFound();
      }
      // Else if we didn't detect anything and there was previously something detected
      else if (!foundInteractables && hadInteractions)
      {
        if (showDebug)
          Trace.Script($"No interactives in range.", this);
        OnInteractablesOutOfRange();
      }

    }

    /// <summary>
    /// Interacts with the given interactable if within interaction radius
    /// </summary>
    /// <param name="interactable"></param>
    public void Interact(Interactable interactable)
    {
      if (!CanInteract(interactable.transform))
      {
        if (showDebug)
          Trace.Script($"Out of interaction range from {interactable.name}", this);
        return;
      }

      interactable.gameObject.Dispatch<InteractEvent>(this.interactEvent);

      if (showDebug)
        Trace.Script($"Interacted with {interactable.name}", this);
    }

    public bool Detect(Transform target)
    {
      switch (this.mode)
      {
        case DetectionMode.Range:
          return CheckDistance(target);
        case DetectionMode.FieldOfView:
          return CheckFieldOfView(target);
        case DetectionMode.LineOfSight:
          return CheckLineOfSight(target);
      }
      throw new System.NotImplementedException("Missing detection mode!");
    }

    /// <summary>
    /// Checks whether the target is in range
    /// </summary>
    /// <returns></returns>
    public bool CheckDistance(Transform target)
    {
      return Library.CheckRange(this.root, target, this.range);
    }

    /// <summary>
    /// Checks whether the target is in the field of view of the owner.
    /// </summary>
    /// <returns></returns>
    public bool CheckFieldOfView(Transform target)
    {
      if (CheckDistance(target))
        return Library.CheckFieldOfView(this.root, target, selectedFov);
      return false;
    }

    /// <summary>
    /// Checks line of sight with the given target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckLineOfSight(Transform target)
    {
      if (CheckFieldOfView(target))
        return Library.CheckLineOfSight(this.root, target, range);
      return false;
    }

    /// <summary>
    /// Checks whether this sensor can interact with the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CanInteract(Transform target)
    {
      return Library.CheckRange(this.root, target, this.interactionRadius);
    }

    void DrawInteractionSphere()
    {
      Library.DrawRange(root, interactionRadius, interactionColor);
    }

    void DrawDetectionRange()
    {
      Library.DrawRange(root, range, detectionColor);
    }

    void DrawFieldOfView()
    {
      Library.DrawFieldOfView(root, selectedFov, range, detectionColor, true);
    }

    //------------------------------------------------------------------------/
    // Routines
    //------------------------------------------------------------------------/
    bool ScanForInteractables()
    {
      Collider[] castResults = Physics.OverlapSphere(this.scanPosition, this.range);
      var interactions = new List<InteractionQuery>();
      bool foundInteractions = false;

      // The scan event that will be sent
      var scanEvent = new DetectionEvent();
      scanEvent.sensor = this;

      if (castResults != null)
      {
        foreach (var hit in castResults)
        {
          // Check whether it can be detected, depending on the scanmode
          bool detected = Detect(hit.transform);
          if (!detected)
            continue;

          // Check whether it is an interactable
          var interactable = hit.transform.gameObject.GetComponent<Interactable>();
          if (interactable == null)
            continue;

          // Scan it
          interactable.gameObject.Dispatch<DetectionEvent>(scanEvent);

          // Create a query object, filling with scanned data
          var query = new InteractionQuery();
          query.data = scanEvent.scanData;
          query.distance = Vector3.Distance(root.position, hit.transform.position);
          query.interactable = interactable;

          // Now store the interaction
          interactions.Add(query);

          // Note that we have scanned something        
          foundInteractions = true;
        }
      }

      // Sort the interactions by the closest one
      interactions.Sort((a, b) => a.distance.CompareTo(b.distance));
      // Save the current interactions
      interactivesInRange = interactions.ToArray();

      // Now inform the agent of the current results
      var scanResult = new DetectionResultEvent();
      scanResult.hasFoundInteractions = foundInteractions;
      this.gameObject.Dispatch<DetectionResultEvent>(scanResult);
      return foundInteractions;
    }

  }

}
