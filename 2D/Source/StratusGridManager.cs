using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

namespace Stratus
{
	public interface IStratusGridManager
	{
		Grid grid { get; }
		Vector3Int currentCellPosition { get; }
		bool HasBehaviourAtPosition(string layer, Vector3Int position);
		bool ContainsCell(Vector3Int position);
		void NavigateToCellPosition(Vector3Int position);
		Vector3Int[] GetPath(Vector3Int start, Vector3Int end);
		T[] GetBehavioursInRange<T>(string layer, Vector3Int position, Vector2Int range)
			where T : class, IStratusTileBehaviour;
		void Highlight(bool on, Vector3Int[] positions, Color color);
		void Highlight(bool on, Vector3Int position, Color color);
	}

	[StratusSingleton(persistent = false)]
	public abstract partial class StratusGridManager<T, BaseTileType, MapType, BehaviourType>
		: StratusSingletonBehaviour<T>, IStratusGridManager
		where T : StratusGridManager<T, BaseTileType, MapType, BehaviourType>
		where BaseTileType : StratusTile, new()
		where MapType : StratusGridBehaviour<BaseTileType>, new()
		where BehaviourType : StratusTileBehaviour<BaseTileType>
	{
		//-------------------------------------------------------------------------/
		// Declarations
		//-------------------------------------------------------------------------/
		public class ClearEvent : StratusEvent
		{
		}

		public class NavigationEvent : StratusEvent
		{
			public NavigationEvent(BehaviourType behaviour)
			{
				this.behaviour = behaviour;
			}

			public BehaviourType behaviour { get; private set; }
		}

		//-------------------------------------------------------------------------/
		// Fields
		//-------------------------------------------------------------------------/
		[SerializeField]
		private Camera _camera;
		[SerializeField]
		private MapType _initialMap;
		[SerializeField]
		private MapType _map;
		[SerializeField]
		private SpriteRenderer _cursor;
		[SerializeField]
		private TextMeshPro tileTextPrefab;
		[SerializeField]
		private bool pushInputLayer = false;

		//-------------------------------------------------------------------------/
		// Property
		//-------------------------------------------------------------------------/
		public Grid grid => currentMap.grid;
		private Dictionary<Vector3Int, GameObject> spawnedGameObjects { get; set; }

		private Dictionary<Vector3Int, TextMeshPro> tileTextInstances = new Dictionary<Vector3Int, TextMeshPro>();
		private BehaviourLayerMap behaviours { get; set; }
		public MapType currentMap { get; private set; }
		public Vector3Int currentCellPosition { get; private set; }
		public Vector3 currentCellWorldPosition => CellToWorld(currentCellPosition);
		public StratusGridNavigationRange navigationRange
		{
			get => _navigationRange;
			set
			{
				_navigationRange = value;
				if (_navigationRange != null)
				{
					NavigateToCellPosition(_navigationRange.firstSelection);
					_navigationRange.Highlight();
					this.Log($"Navigation range set to {value}");
				}
			}
		}
		private StratusGridNavigationRange _navigationRange;
		private StratusInputLayer inputLayer { get; set; }
		public virtual Color rangeColor { get; } = Color.blue.ScaleSaturation(0.5f);

		//-------------------------------------------------------------------------/
		// Virtual
		//-------------------------------------------------------------------------/
		protected abstract void OnManagerAwake();

		public abstract void Highlight(bool on, Vector3Int[] positions, Color color);
		protected abstract string defaultSelectionLayer { get; }
		public abstract void ClearHighlights();
		protected abstract void OnNavigateCellPosition(Vector3Int newPosition, Vector3Int oldPosition);
		public abstract bool IsTraversibleByBehaviour(Vector3Int position, BehaviourType behaviour);
		protected abstract bool IsBaseTileTraversible(BaseTileType baseTile);

		//-------------------------------------------------------------------------/
		// Messages
		//-------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			if (_map != null)
			{
				OnLoadMap(_map);
			}

			spawnedGameObjects = new Dictionary<Vector3Int, GameObject>();
			behaviours = new BehaviourLayerMap();
			inputLayer = GetInputLayer();

			HideCursor();
			SubscribeToEvents();
			OnManagerAwake();
		}

		protected virtual StratusInputUILayer GetInputLayer()
		{
			StratusInputUILayer inputLayer = new StratusInputUILayer(GetType().Name);
			inputLayer.actions.onNavigate = NavigateCellDirection;
			inputLayer.actions.onSubmit = SelectCurrentCell;
			return inputLayer;
		}

		//-------------------------------------------------------------------------/
		// Events
		//-------------------------------------------------------------------------/
		private void SubscribeToEvents()
		{
			StratusScene.Connect<StratusTileBehaviourAddEvent>(OnBehaviourInstancedEvent);
			StratusScene.Connect<StratusTileBehaviourRemovedEvent>(OnBehaviourDestroyedEvent);
			StratusScene.Connect<ClearEvent>(this.OnClearEvent);
		}

		private void OnBehaviourInstancedEvent(StratusTileBehaviourAddEvent e)
		{
			InitializeBehaviourInstance(e.behaviour as BehaviourType);
		}

		private void OnBehaviourDestroyedEvent(StratusTileBehaviourRemovedEvent e)
		{
			RemoveBehaviourInstance(e.behaviour as BehaviourType);
		}

		private void OnClearEvent(ClearEvent e)
		{
			Clear();
		}

		//-------------------------------------------------------------------------/
		// Map Methods
		//-------------------------------------------------------------------------/
		public bool LoadInitialMap()
		{
			return LoadMap(_initialMap);
		}

		public bool LoadMap(MapType map, Action onFinished = null)
		{
			if (this.currentMap != null)
			{
				this.LogError($"Already have a map loaded ({map.name})");
				return false;
			}

			MapType instance = Instantiate(map, this.transform);
			this.currentMap = instance;
			Invoke(() => OnLoadMap(instance, onFinished), 0.1f);
			return true;
		}

		protected void OnLoadMap(MapType map, Action onFinished = null)
		{
			map.Initialize(this._camera);
			map.baseLayer.tilemap.CompressBounds();
			InitializeCursor();
			inputLayer.PushByEvent();
			this.Log($"Initialized map {map.name}");
			onFinished?.Invoke();
		}

		public bool UnloadMap()
		{
			if (currentMap == null)
			{
				this.LogWarning("No map to unload");
				return false;
			}

			this.Log($"Unloading map {currentMap}");
			inputLayer.PopByEvent();
			behaviours.DestroyAndClear();
			Destroy(currentMap.gameObject);
			currentMap = null;
			HideCursor();
			return true;
		}

		//-------------------------------------------------------------------------/
		// Methods
		//-------------------------------------------------------------------------/
		public void Clear()
		{
			HideCursor();
		}

		//-------------------------------------------------------------------------/
		// Navigation Methods
		//-------------------------------------------------------------------------/
		public void NavigateCellDirection(Vector2 direction)
		{
			if (navigationRange != null)
			{
				NavigateToCellPosition(navigationRange.GetNextCellPosition(direction));
				return;
			}

			int x = (int)direction.x;
			int y = (int)direction.y;

			Vector3Int nextCellPosition = currentCellPosition;
			nextCellPosition.x += x;
			nextCellPosition.y += y;

			// For vertical movement...
			if (y != 0)
			{
				// Flip the horizontal direction and try again
				if (!CanNavigateToCell(nextCellPosition))
				{
					nextCellPosition.x += (currentCellPosition.x > 0 ? -1 : 1);
				}
			}

			NavigateToCellPosition(nextCellPosition);
		}

		public void NavigateToCellPosition(Vector3Int position)
		{
			if (CanNavigateToCell(position))
			{
				this.Log($"Navigating cell at {position}");
				var oldCellPosition = currentCellPosition;
				currentCellPosition = position;
				currentMap.NavigateAtPosition(currentCellPosition);
				SetCursor(currentCellPosition);

				OnNavigateCellPosition(position, oldCellPosition);

				BehaviourType behaviourAtPosition = behaviours.GetBehaviour(defaultSelectionLayer, position);
				NavigationEvent navigationEvent = new NavigationEvent(behaviourAtPosition);
				StratusScene.Dispatch<NavigationEvent>(navigationEvent);

				//behaviours.TryNavigate(defaultSelectionLayer, position);

				if (navigationRange != null)
				{
					navigationRange.OnNavigateToCellPosition(currentCellPosition);
				}
			}
		}

		public bool ContainsCell(Vector3Int position)
		{
			return currentMap.baseLayer.bounds.Contains(position);
		}

		public bool CanNavigateToCell(Vector3Int position)
		{
			if (ContainsCell(position))
			{
				if (navigationRange != null && !navigationRange.Contains(position))
				{
					this.LogWarning($"Restricted navigation range does not contain {position}");
					return false;
				}
				return true;
			}
			return false;
		}

		public bool IsTraversible(Vector3Int position)
		{
			if (!ContainsCell(position))
			{
				return false;
			}

			StratusTilemap<BaseTileType>.Selection selection = currentMap.baseLayer.GetTile(position);
			if (selection == null)
			{
				this.LogError($"No base tile at {position}");
				return false;
			}
			return IsBaseTileTraversible(selection.tile);
		}

		public void NavigateToBehaviour(BehaviourType behaviour)
		{
			if (behaviours.HasBehaviour(behaviour))
			{
				NavigateToCellPosition(behaviour.cellPosition);
			}
			else
			{
				this.LogWarning($"Could not find behaviour {behaviour}");
			}
		}

		//-------------------------------------------------------------------------/
		// Game Objects
		//-------------------------------------------------------------------------/
		public GameObject SpawnGameObject(Vector3Int position, GameObject prefab)
		{
			if (spawnedGameObjects.ContainsKey(position))
			{
				this.LogError($"A gameobject has already been spawned at {position}");
				return null;
			}

			GameObject instance = GameObject.Instantiate(prefab, this.transform);
			instance.transform.position = GetWorldPosition(position);
			spawnedGameObjects.Add(position, instance);
			return instance;
		}

		public Vector3 GetWorldPosition(Vector3Int cellPosition)
		{
			return currentMap.baseLayer.CellCenterToWorld(cellPosition);
		}

		public void SetTileText(Vector3Int position, string text)
		{
			if (!tileTextInstances.ContainsKey(position))
			{
				TextMeshPro instance = GameObject.Instantiate(tileTextPrefab, currentMap.baseLayer.tilemap.transform);
				Vector3 worldPosition = currentMap.baseLayer.CellCenterToWorld(position);
				instance.transform.position = worldPosition;
				tileTextInstances.Add(position, instance);
			}

			tileTextInstances[position].gameObject.SetActive(true);
			tileTextInstances[position].text = text;
		}

		public void HideTileText(Vector3Int position)
		{
			if (!tileTextInstances.ContainsKey(position))
			{
				TextMeshPro instance = tileTextInstances[position];
				instance.gameObject.SetActive(false);
			}
		}

		public void ClearTileText(bool destroy = false)
		{
			foreach (var text in tileTextInstances.Values)
			{
				if (destroy)
				{
					Destroy(text.gameObject);
				}
				else
				{
					text.gameObject.SetActive(false);
				}
			}
			if (destroy)
			{
				tileTextInstances.Clear();
			}
		}

		//-------------------------------------------------------------------------/
		// Selection Methods
		//-------------------------------------------------------------------------/
		public void SelectCell(Vector3Int position)
		{
			this.Log("Selecting cell");
			if (behaviours.HasBehaviour(defaultSelectionLayer, position))
			{
				behaviours.GetBehaviour(defaultSelectionLayer, position).Select();
			}
			else
			{
				this.LogWarning($"No cell to select at {position}");
			}
			currentMap.SelectAtPosition(currentCellPosition);
		}

		public void SelectCurrentCell()
		{
			SelectCell(currentCellPosition);
		}

		public Vector3 CellToWorld(Vector3Int cellPosition)
		{
			return grid.CellToWorld(cellPosition);
		}

		public void Highlight(bool on, Vector3Int position)
		{
			Highlight(on, position, rangeColor);
		}

		public void Highlight(bool on, Vector3Int position, Color color)
		{
			Highlight(on, new Vector3Int[] { position }, color);
		}

		public Dictionary<Vector3Int, float> GetRange(Vector3Int center, int n)
		{
			return GetRange(center, new Vector2Int(0, n));
		}

		public Dictionary<Vector3Int, float> GetRange(Vector3Int center, Vector2Int range)
		{
			return GetRange(center, range, IsTraversible);
		}

		/// <summary>
		/// Returns a given range starting from a cell
		/// </summary>
		public Dictionary<Vector3Int, float> GetRange(Vector3Int center, Vector2Int range, Predicate<Vector3Int> traversibleFunction = null)
		{
			this.Log($"Gathering range {range} from center {center}:");

			Dictionary<Vector3Int, float> values = null;
			values = StratusGridUtility.GetRange(center, range.y, grid.cellLayout, traversibleFunction);
			if (values.IsNullOrEmpty())
			{
				this.LogWarning($"No available range found for cell at {center} with range {range}");
			}

			// If the min range is not 0...
			if (range.x > 0)
			{
				// Remove those whose is less than min range
				Dictionary<Vector3Int, float> filtered = new Dictionary<Vector3Int, float>();
				foreach (KeyValuePair<Vector3Int, float> kvp in values)
				{
					float cost = kvp.Value;
					if (cost >= range.x)
					{
						filtered.Add(kvp.Key, kvp.Value);
					}
				}
				return filtered;
			}

			return values;
		}

		/// <summary>
		/// Returns the range for the given behaviour
		/// </summary>
		public Dictionary<Vector3Int, float> GetRange(BehaviourType behaviour, Vector2Int range)
		{
			Vector3Int center = behaviour.cellPosition;

			Dictionary<Vector3Int, float> values = StratusGridUtility.GetRange(center, range.y, grid.cellLayout, ComposeTraversiblePredicate(behaviour));

			if (values.IsNullOrEmpty())
			{
				this.LogWarning($"No available range found for behaviour {behaviour} at {center} with range {range}");
			}

			// If the min range is not 0...
			if (range.x > 0)
			{
				// Remove those whose is less than min range
				Dictionary<Vector3Int, float> filtered = new Dictionary<Vector3Int, float>();
				foreach (KeyValuePair<Vector3Int, float> kvp in values)
				{
					float cost = kvp.Value;
					if (cost >= range.x)
					{
						filtered.Add(kvp.Key, kvp.Value);
					}
				}
				return filtered;
			}

			return values;
		}

		/// <summary>
		/// Returns the path from starting to cell to ending cell
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public Vector3Int[] GetPath(Vector3Int start, Vector3Int end) => GetPath(start, end, IsTraversible);

		/// <summary>
		/// Returns the path from starting to cell to ending cell given a predicate
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public Vector3Int[] GetPath(Vector3Int start, Vector3Int end, Predicate<Vector3Int> isTraversible)
		{
			return StratusGridUtility.FindPath(start, end, grid.cellLayout, isTraversible);
		}

		public Vector3Int[] GetPath(Vector3Int start, Vector3Int end, BehaviourType behaviour)
		{
			return StratusGridUtility.FindPath(start, end, grid.cellLayout, ComposeTraversiblePredicate(behaviour));
		}

		private Predicate<Vector3Int> ComposeTraversiblePredicate(BehaviourType behaviour)
		{
			bool isTraversibleByBehaviour(Vector3Int position)
			{
				if (!IsTraversible(position))
				{
					return false;
				}

				return IsTraversibleByBehaviour(position, behaviour);
			}

			return isTraversibleByBehaviour;
		}

		//-------------------------------------------------------------------------/
		// Cursor
		//-------------------------------------------------------------------------/
		private void InitializeCursor()
		{
			_cursor.transform.localScale = grid.cellSize;
		}

		private void SetCursor(Vector3Int position)
		{
			_cursor.gameObject.SetActive(true);
			_cursor.transform.position = CellToWorld(position);
		}

		private void HideCursor()
		{
			_cursor.gameObject.SetActive(false);
		}
	}

}