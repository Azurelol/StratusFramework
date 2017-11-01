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

namespace Stratus
{
  namespace AI
  {
    [RequireComponent(typeof(Agent))]
    public class Sensor : MonoBehaviour
    {

      //------------------------------------------------------------------------/
      // Events
      //------------------------------------------------------------------------/    
      /// <summary>
      /// Received by objects that can be interacted with. They will
      /// respond with a corresponding event to signal that they are able to be
      /// interacted with.
      /// </summary>
      public class ScanEvent : Stratus.Event
      {
        public Agent Agent;
        public string Context;
      }

      public struct InteractionQuery
      {
        public InteractionTrigger Interactive;
        public float Distance;
        public string Context;
      }

      public class InteractScanResultEvent : Stratus.Event
      {
        public bool HasFoundInteractions;
      }

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
      // Fields
      //------------------------------------------------------------------------/    
      //WorkingMemory Memory = new WorkingMemory();    


      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      [Header("Detection")]
      [Tooltip("How this agent detects enemies")]
      public DetectionMode Mode = DetectionMode.Range;
      /// <summary>
      /// How often this sensor should be scanning
      /// </summary>
      [Range(0.0f, 2.0f)]
      public float scanInterval = 3.0f;
      /// <summary>
      /// The field of view of this sensor
      /// </summary>
      [Range(0.0f, 180.0f)]
      public float fieldOfView = 180.0f;
      /// <summary>
      /// The range at which objects are considered to be in line of sight
      /// </summary>
      public float range = 30.0f;

      [Header("Interaction")]
      [Tooltip("Whether this agent can react to interactives in the environment")]
      public bool isInteracting = false;
      /// <summary>
      /// The radius of the interaction sphere for the agent
      /// </summary>
      public float interactionRadius = 5.0f;
      /// <summary>
      /// The offset for the agents's interaction sphere
      /// </summary>
      public Vector3 offset = new Vector3();

      [Header("Debug")]
      /// <summary>
      /// Whether there is debugging output
      /// </summary>
      public bool isDebugging = false;

      //------------------------------------------------------------------------/
      // Private Members
      //------------------------------------------------------------------------/
      /// <summary>
      /// The position from which the scan will be originated
      /// </summary>
      Vector3 scanPosition { get { return transform.position + offset; } }
      /// <summary>
      /// The current interactives in range
      /// </summary>
      public InteractionQuery[] interactivesInRange { get; private set; }
      /// <summary>
      /// The current object(s) that the agent can interact with
      /// </summary>
      public InteractionTrigger closestInteractive
      {
        get
        {
          if (interactivesInRange == null || interactivesInRange.Length == 0)
            return null;
          return interactivesInRange[0].Interactive;
        }
      }
      
      private Agent agent;      
      private Color detectionColor => new Color(1f, 0.5f, 0f, 0.1f);
      private Color interactionColor => new Color(0f, 1f, 0f, 0.1f);
      private Stopwatch scanTimer;


      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected virtual void OnConfigure() { }
      protected virtual void OnScan() { }


      //------------------------------------------------------------------------/
      // Messages
      //------------------------------------------------------------------------/
      private void Start()
      {
        this.Configure();
      }

      /// <summary>
      /// Updates the sensor
      /// </summary>
      private void Update()
      {
        if (!enabled)
          return;

        if (scanTimer.Update(Time.deltaTime))
        {
          //OnScan();
          Scan();
          scanTimer.Reset();
        }

        

        // @TODO: Add a timer?
      }

      //private void OnGUI()
      //{
      //  GUI.Label(new Rect(0f, 0f, 200f, 200f), "Forward = " + transform.forward);
      //}


      private void OnDrawGizmosSelected()
      {
        //Overlay.Watch(() => transform.forward, "Forward");

        if (isInteracting)
          DrawInteractionSphere();

        switch (this.Mode)
        {
          case DetectionMode.Range:
            DrawDetectionRange();
            break;
          case DetectionMode.FieldOfView:
          case DetectionMode.LineOfSight:
            DrawFIeldOfView();
            break;
        }

      }

      //------------------------------------------------------------------------/
      // Methods: Events
      //------------------------------------------------------------------------/
      /// <summary>
      /// Subscribes to specific events
      /// </summary>
      void Subscribe()
      {
        //this.gameObject.Connect<InteractionAvailableEvent>(this.OnInteractionAvailableEvent);
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Configures the sensor
      /// </summary>
      void Configure()
      {
        this.agent = GetComponent<Agent>();
        this.scanTimer = new Stopwatch(this.scanInterval);
        this.OnConfigure();
      }

      /// <summary>
      /// Scans the world for objects we can interact with.
      /// </summary>    
      public void Scan()
      {
        //this.ScanInteractives();
        if (this.isInteracting) ScanInteractives();
        this.OnScan();
      }

      public bool Detect(Transform target)
      {
        switch (this.Mode)
        {
          case DetectionMode.Range:
            return CheckDistance(target);
          case DetectionMode.FieldOfView:
            return CheckFieldOfView(target);
          case DetectionMode.LineOfSight:
            return CheckFieldOfView(target);
        }
        throw new System.NotImplementedException("Missing detection mode!");
      }

      /// <summary>
      /// Checks whether the target is in range
      /// </summary>
      /// <returns></returns>
      public bool CheckDistance(Transform target)
      {
        return Library.CheckRange(this.transform, target, this.range);
      }

      /// <summary>
      /// Checks whether the target is in the field of view of the owner.
      /// </summary>
      /// <returns></returns>
      public bool CheckFieldOfView(Transform target)
      {
        if (CheckDistance(target))
          return Library.CheckFieldOfView(this.transform, target, fieldOfView);
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
          return Library.CheckLineOfSight(transform, target, range);        
        return false;
      }

      void DrawInteractionSphere()
      {
        Library.DrawRange(transform, interactionRadius, interactionColor);
      }

      void DrawDetectionRange()
      {
        Library.DrawRange(transform, range, detectionColor);
      }

      void DrawFIeldOfView()
      {
        Library.DrawFieldOfView(transform, fieldOfView, range, detectionColor);
      }

      //------------------------------------------------------------------------/
      // Routines
      //------------------------------------------------------------------------/
      void ScanInteractives()
      {
        Collider[] castResults = Physics.OverlapSphere(this.scanPosition, this.interactionRadius);
        var interactions = new List<InteractionQuery>();
        bool foundInteractions = false;

        // The scan event that will be sent
        var scanEvent = new ScanEvent();
        scanEvent.Agent = this.agent;

        if (castResults != null)
        {
          // Performance, ho~!
          //var availableTargets = (from Collider hit in castResults
          //                        where hit.GetComponent<InteractionTrigger>()
          //                        select hit.GetComponent<InteractionTrigger>()).ToArray();
          foreach (var hit in castResults)
          {
            var interactionTrigger = hit.transform.gameObject.GetComponent<InteractionTrigger>();
            if (interactionTrigger != null)
            {
              //if (Debugging) Trace.Script("Can interact!");
              interactionTrigger.gameObject.Dispatch<ScanEvent>(scanEvent);

              // Update the context            
              //scanResult.Context = scan.Context;
              foundInteractions = true;

              // Create a query object
              var query = new InteractionQuery();
              query.Context = scanEvent.Context;
              query.Distance = Vector3.Distance(transform.position, hit.transform.position);
              query.Interactive = interactionTrigger;
              // Now store the interaction
              interactions.Add(query);
            }
          }
        }

        // Sort the interactions by the closest one?
        interactions.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        // Save the current interactions
        interactivesInRange = interactions.ToArray();

        // Now inform the agent of the current results
        var scanResult = new InteractScanResultEvent();
        scanResult.HasFoundInteractions = foundInteractions;
        this.agent.gameObject.Dispatch<InteractScanResultEvent>(scanResult);
      }

    }
  } 
}
