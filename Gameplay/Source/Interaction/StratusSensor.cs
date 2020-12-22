using UnityEngine;
using System.Collections.Generic;
using Stratus.Utilities;
using System;

namespace Stratus.Gameplay
{
	public interface IStratusInteractable
	{
		void Interact();
	}

	/// <summary>
	/// Handles sensing objects for an agent
	/// </summary>
	[DisallowMultipleComponent]
	public class StratusSensor : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/    
		/// <summary>
		/// Base class for events by this sensor
		/// </summary>
		public abstract class BaseEvent : StratusEvent
		{
			/// <summary>
			/// The sensor that is outputting the scan
			/// </summary>
			public StratusSensor source { get; private set; }

			public BaseEvent(StratusSensor source)
			{
				this.source = source;
			}

		}

		/// <summary>
		/// Received by objects that can be interacted with. They will
		/// respond with a corresponding event to signal that they are able to be
		/// interacted with.
		/// </summary>
		public class InteractableDetectedEvent : BaseEvent
		{
			/// <summary>
			/// Any relevant data from the object that's been scanned to the sensor
			/// </summary>
			public object[] scanData;

			public InteractableDetectedEvent(StratusSensor source) : base(source)
			{
			}
		}

		/// <summary>
		/// Signals an interactable object that the sensor wants to interact with it
		/// </summary>
		public class InteractEvent : BaseEvent
		{
			/// <summary>
			/// Any relevant data sent from the sensor that is interacting to the object
			/// </summary>
			public object[] interactionData;

			public InteractEvent(StratusSensor source) : base(source)
			{
			}
		}

		/// <summary>
		/// Contains information about a successful object detectio
		/// </summary>
		public struct InteractionQuery
		{
			public StratusInteractable interactable;
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
		// Properties
		//------------------------------------------------------------------------/        
		/// <summary>
		/// The position from which the scan will be originated
		/// </summary>
		public Vector3 scanPosition => root.position + offset;
		/// <summary>
		/// The current interactives in range
		/// </summary>
		public InteractionQuery[] interactivesInRange { get; private set; }
		/// <summary>
		/// The current object(s) that the agent can interact with
		/// </summary>
		public StratusInteractable closestInteractable
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
		// Events
		//------------------------------------------------------------------------/
		public event Action<bool> onScan;

		//------------------------------------------------------------------------/
		// Private Fields
		//------------------------------------------------------------------------/
		private StratusStopwatch scanTimer;
		private InteractEvent interactEvent;

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/		
		protected virtual void OnAwake()
		{
		}

		protected virtual void OnInteractablesFound()
		{
		}

		protected virtual void OnInteractablesOutOfRange()
		{
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			if (!root)
			{
				StratusDebug.LogErrorBreak("No root transform selected for sensing!", this);
				return;
			}

			this.interactEvent = new InteractEvent(this);
			this.scanTimer = new StratusStopwatch(this.scanInterval);
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
		/// Scans the world for objects we can interact with (automatically if set, on a given interval
		/// </summary>    
		public void Scan()
		{
			bool hadInteractions = (closestInteractable != null);
			bool foundInteractables = ScanForInteractables();

			// If we detected something
			if (foundInteractables)
			{
				if (showDebug)
					StratusDebug.Log($"Interactives in range = {interactivesInRange.Length}, closest is '{closestInteractable}'", this);
				OnInteractablesFound();
			}
			// Else if we didn't detect anything and there was previously something detected
			else if (!foundInteractables && hadInteractions)
			{
				if (showDebug)
					StratusDebug.Log($"No interactives in range.", this);
				OnInteractablesOutOfRange();
			}

		}

		/// <summary>
		/// Interacts with the given interactable if within interaction radius
		/// </summary>
		/// <param name="interactable"></param>
		public void Interact(StratusInteractable interactable)
		{
			if (!CanInteract(interactable.transform))
			{
				if (showDebug)
					StratusDebug.Log($"Out of interaction range from {interactable.name}", this);
				return;
			}

			interactable.gameObject.Dispatch<InteractEvent>(this.interactEvent);

			if (showDebug)
				StratusDebug.Log($"Interacted with {interactable.name}", this);
		}

		/// <summary>
		/// Returns true if this sensor can detect the target
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
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
			return StratusDetection.CheckRange(this.root, target, this.range);
		}

		/// <summary>
		/// Checks whether the target is in the field of view of the owner.
		/// </summary>
		/// <returns></returns>
		public bool CheckFieldOfView(Transform target)
		{
			if (CheckDistance(target))
				return StratusDetection.CheckFieldOfView(this.root, target, selectedFov);
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
				return StratusDetection.CheckLineOfSight(this.root, target, range);
			return false;
		}

		/// <summary>
		/// Checks whether this sensor can interact with the target
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool CanInteract(Transform target)
		{
			return StratusDetection.CheckRange(this.root, target, this.interactionRadius);
		}

		//------------------------------------------------------------------------/
		// Routines
		//------------------------------------------------------------------------/
		/// <summary>
		/// The main procedure for the sensor
		/// </summary>
		/// <returns></returns>
		private bool ScanForInteractables()
		{
			Collider[] castResults = Physics.OverlapSphere(this.scanPosition, this.range);
			List<InteractionQuery> interactions = new List<InteractionQuery>();
			bool foundInteractions = false;

			// The scan event that will be sent
			InteractableDetectedEvent detectEvent = new InteractableDetectedEvent(this);

			if (castResults != null)
			{
				foreach (var hit in castResults)
				{
					// Check whether it can be detected, depending on the scanmode
					bool detected = Detect(hit.transform);
					if (!detected)
						continue;

					// Check whether it is an interactable
					StratusInteractable interactable = hit.transform.gameObject.GetComponent<StratusInteractable>();
					if (interactable == null)
						continue;

					// Scan it
					interactable.gameObject.Dispatch<InteractableDetectedEvent>(detectEvent);

					// Create a query object, filling with scanned data
					InteractionQuery query = new InteractionQuery();
					query.data = detectEvent.scanData;
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
			onScan?.Invoke(foundInteractions);
			return foundInteractions;
		}

		private void DrawInteractionSphere()
		{
			StratusDetection.DrawRange(root, interactionRadius, interactionColor);
		}

		private void DrawDetectionRange()
		{
			StratusDetection.DrawRange(root, range, detectionColor);
		}

		private void DrawFieldOfView()
		{
			StratusDetection.DrawFieldOfView(root, selectedFov, range, detectionColor, true);
		}
	}

}
