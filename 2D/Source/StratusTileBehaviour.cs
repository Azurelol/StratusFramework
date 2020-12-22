using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Stratus
{
	public interface IStratusTileBehaviour
	{
		IStratusGridManager manager { get; }
		Vector3Int cellPosition { get; }
		string layer { get; }
		void Initialize(IStratusGridManager grid);
		void Shutdown();
		void MoveToPosition(Vector3Int targetPosition, bool animate, Action onFinished = null);
	}

	public abstract class StratusTileBehaviourEvent : StratusEvent
	{
		protected StratusTileBehaviourEvent(IStratusTileBehaviour behaviour)
		{
			this.behaviour = behaviour;
		}

		public IStratusTileBehaviour behaviour { get; private set; }
	}

	public class StratusTileBehaviourAddEvent : StratusTileBehaviourEvent
	{
		public Action<IStratusGridManager> initializer { get; private set; }
		public StratusTileBehaviourAddEvent(IStratusTileBehaviour behaviour, Action<IStratusGridManager> initializer) : base(behaviour)
		{
			this.initializer = initializer;
		}
	}

	public class StratusTileBehaviourRemovedEvent : StratusTileBehaviourEvent
	{
		public StratusTileBehaviourRemovedEvent(IStratusTileBehaviour behaviour) : base(behaviour)
		{
		}
	}

	public interface IStratusTileBehaviour<T> : IStratusTileBehaviour
		where T : StratusTile
	{
	}


	public abstract class StratusTileBehaviour : StratusBehaviour
	{
	}

	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]

	public abstract class StratusTileBehaviour<TileType>
		: StratusTileBehaviour, IStratusTileBehaviour<TileType>
		where TileType : StratusTile
	{
		//-------------------------------------------------------------------------//
		// Events
		//-------------------------------------------------------------------------//
		public class NavigateEvent : StratusTileBehaviourEvent
		{
			public NavigateEvent(IStratusTileBehaviour behaviour) : base(behaviour)
			{
			}
		}

		public class SelectedEvent : StratusTileBehaviourEvent
		{
			public SelectedEvent(IStratusTileBehaviour behaviour) : base(behaviour)
			{
			}
		}

		public class DeselectedEvent : StratusTileBehaviourEvent
		{
			public DeselectedEvent(IStratusTileBehaviour behaviour) : base(behaviour)
			{
			}
		}

		//-------------------------------------------------------------------------//
		// Virtual
		//-------------------------------------------------------------------------//
		protected abstract void OnTileBehaviourInitialize();
		protected abstract void OnTileBehaviourShutdown();
		protected abstract void OnMoveToPosition(Vector3Int cellPosition);

		//-------------------------------------------------------------------------//
		// Fields
		//-------------------------------------------------------------------------//
		public bool addAtStart = true;
		public bool destroyOnShutdown = true;
		private bool assignedInitialPosition;

		//-------------------------------------------------------------------------//
		// Properties
		//-------------------------------------------------------------------------//
		public SpriteRenderer spriteRenderer => GetComponentCached<SpriteRenderer>();
		public IStratusGridManager manager { get; private set; }
		public Vector3Int cellPosition { get; private set; }
		/// <summary>
		/// Whether this behaviour has been initialized
		/// </summary>
		public bool initialized { get; private set; }
		/// <summary>
		/// Whether this behaviour was registered
		/// </summary>
		public bool registered { get; private set; }
		/// <summary>
		/// Whether this behaviour has been shutdown
		/// </summary>
		public bool shutdown { get; private set; }
		/// <summary>
		/// The layer this behaviour belongs to
		/// </summary>
		public abstract string layer { get; }		

		//-------------------------------------------------------------------------//
		// Events
		//-------------------------------------------------------------------------//
		public event Action<Vector3Int, Vector3Int> onMoved;

		//-------------------------------------------------------------------------//
		// Messages
		//-------------------------------------------------------------------------//
		private void Start()
		{
			if (addAtStart)
			{
				Register();
			}
		}

		public override string ToString()
		{
			return $"{name} cell {cellPosition}";
		}

		//-------------------------------------------------------------------------//
		// Methods: Initialization
		//-------------------------------------------------------------------------//
		protected void Register()
		{
			if (registered)
			{
				return;
			}
			registered = true;
			StratusScene.Dispatch<StratusTileBehaviourAddEvent>(new StratusTileBehaviourAddEvent(this, Initialize));
			if (initialized)
			{
				OnTileBehaviourInitialize();
				ToggleVisibility(true);
				this.Log($"Initialized {this} at {cellPosition}");
			}
			else
			{
				this.LogError("Failed to initialize");
			}
		}

		protected void Unregister()
		{
			if (!registered)
			{
				return;
			}
			registered = false;
			StratusScene.Dispatch<StratusTileBehaviourRemovedEvent>(new StratusTileBehaviourRemovedEvent(this));
			if (shutdown)
			{
				OnTileBehaviourShutdown();
				ToggleVisibility(false);
				this.Log("Shutdown");
				if (destroyOnShutdown)
				{
					this.Log("Destroying...");
					this.gameObject.Destroy(0.1f);
					//this.DestroyGameObject();
				}
			}
		}

		public void Initialize(IStratusGridManager manager)
		{
			this.manager = manager;
			UpdateInternalPosition();
			MoveToPosition(this.cellPosition, false);
			FitToCell();
			initialized = true;
		}

		public void Shutdown()
		{
			if (shutdown)
			{
				return;
			}
			shutdown = true;
		}

		//-------------------------------------------------------------------------//
		// Methods
		//-------------------------------------------------------------------------//
		protected void ToggleVisibility(bool enabled)
		{
			spriteRenderer.enabled = enabled;
		}

		protected void AssignInitialPosition(Vector3Int cellPosition)
		{
			this.cellPosition = cellPosition;
			assignedInitialPosition = true;
		}

		public void UpdateInternalPosition()
		{
			if (!assignedInitialPosition)
			{
				cellPosition = manager.grid.WorldToCell(this.transform.position);
			}
			SnapToPosition(cellPosition);
		}

		public virtual void MoveToPosition(Vector3Int targetPosition, bool animate, Action onFinished = null)
		{
			if (manager.HasBehaviourAtPosition(this.layer, targetPosition))
			{
				this.LogWarning($"A behaviour is already present at {targetPosition} in the same layer ({layer}) as this behaviour");
				return;
			}

			this.Log($"Moving agent to {targetPosition}. Animate ? {animate}");
			StartCoroutine(MoveToPositionRoutine(targetPosition, animate, onFinished));
		}

		protected IEnumerator MoveToPositionRoutine(Vector3Int targetPosition,
			bool animate, Action onFinished = null)
		{
			//this.Log($"Executing routine to move agent to {targetPosition}. Animate ? {animate}");

			Vector3Int sourcePosition = this.cellPosition;

			if (animate)
			{
				// Get the path
				Vector3Int[] path = manager.GetPath(sourcePosition, targetPosition);
				yield return AnimatedMoveToPosition(path);
			}
			else
			{
				SnapToPosition(targetPosition);
			}

			yield return new WaitForEndOfFrame();
			this.cellPosition = targetPosition;
			OnMoveToPosition(targetPosition);
			onMoved?.Invoke(sourcePosition, targetPosition);
			onFinished?.Invoke();
		}

		private void SnapToPosition(Vector3Int cellPosition)
		{
			Vector3 worldPosition = manager.grid.GetCellCenterWorld(cellPosition);
			transform.position = worldPosition;
		}

		protected IEnumerator AnimatedMoveToPosition(Vector3Int[] path)
		{
			const float defaultTimeBetweenPoints = 0.1f;
			foreach (var point in path)
			{
				Vector3 pointWorldPosition = manager.grid.GetCellCenterWorld(point);
				//this.Log($"Moving to point {pointWorldPosition}");
				yield return StratusRoutines.MoveTo(transform, pointWorldPosition, defaultTimeBetweenPoints);
			}
		}

		public virtual void FitToCell()
		{
		}

		public void Navigate()
		{
			//this.Log("Highlighting");
			StratusScene.Dispatch<NavigateEvent>(new NavigateEvent(this));
		}

		public void Select()
		{
			this.Log("Selecting");
			StratusScene.Dispatch<SelectedEvent>(new SelectedEvent(this));
		}

		public void Deselect()
		{
			this.Log("Deselecting");
			StratusScene.Dispatch<DeselectedEvent>(new DeselectedEvent(this));
		}
	}

}