using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Stratus
{
    public abstract partial class StratusGridManager<T, BaseTileType, MapType, BehaviourType>
    {
		//-------------------------------------------------------------------------/
		// Declarations
		//-------------------------------------------------------------------------/
		public class BehaviourLayerMap
		{
			private Dictionary<string, StratusBictionary<Vector3Int, BehaviourType>> map = new Dictionary<string, StratusBictionary<Vector3Int, BehaviourType>>();
			public string[] layers => map.Keys.ToArray();

			public bool AddBehaviour(string layer, BehaviourType behaviour, Vector3Int position)
			{
				if (!map.ContainsKey(layer))
				{
					map.Add(layer, new StratusBictionary<Vector3Int, BehaviourType>());
				}
				if (map[layer].Contains(behaviour))
				{
					return false;
				}
				if (map[layer].Contains(position))
				{
					return false;
				}
				map[layer].Add(position, behaviour);
				return true;
			}

			public bool RemoveBehaviour(string layer, BehaviourType behaviour)
			{
				if (!map.ContainsKey(layer))
				{
					return false;
				}
				if (!map[layer].Contains(behaviour))
				{
					return false;
				}
				map[layer].Remove(behaviour);
				return true;
			}

			public bool RemoveBehaviour(string layer, Vector3Int cellPosition)
			{
				if (!map.ContainsKey(layer))
				{
					return false;
				}
				if (!map[layer].Contains(cellPosition))
				{
					return false;
				}
				map[layer].Remove(cellPosition);
				return true;
			}

			public bool RemoveBehaviour(BehaviourType behaviour) => RemoveBehaviour(behaviour.layer, behaviour);

			public bool UpdateBehaviourPosition(string layer, BehaviourType behaviour, Vector3Int position)
			{
				if (!map.ContainsKey(layer))
				{
					map.Add(layer, new StratusBictionary<Vector3Int, BehaviourType>());
				}
				if (!map[layer].Contains(behaviour))
				{
					return false;
				}
				if (map[layer].Contains(position))
				{
					return false;
				}
				map[layer].Remove(behaviour);
				map[layer].Add(position, behaviour);
				return true;
			}

			public bool TryNavigate(string layer, Vector3Int position)
			{
				if (HasBehaviour(layer, position))
				{
					map[layer][position].Navigate();
					return true;
				}
				return false;
			}

			public bool HasBehaviour(string layer, Vector3Int position)
			{
				return map.ContainsKey(layer) && map[layer].Contains(position);
			}

			public bool HasBehaviour(string layer, BehaviourType behaviour)
			{
				return map.ContainsKey(layer) && map[layer].Contains(behaviour);
			}
			public bool HasBehaviour(BehaviourType behaviour)
			{
				return map.ContainsKey(behaviour.layer) && map[behaviour.layer].Contains(behaviour);
			}

			public DerivedType GetBehaviour<DerivedType>(string layer, Vector3Int position)
				where DerivedType : BehaviourType
			{
				if (!HasBehaviour(layer, position))
				{
					return null;
				}
				return map[layer][position] as DerivedType;
			}

			public BehaviourType GetBehaviour(string layer, Vector3Int position)
			{
				if (!HasBehaviour(layer, position))
				{
					return null;
				}
				return map[layer][position];
			}

			public void DestroyAndClear()
			{
				foreach (var layer in map.Values)
				{
					foreach (var behaviour in layer)
					{
						StratusDebug.Log($"Destroying {behaviour.Value.gameObject}");
						behaviour.Value.DestroyGameObject();
					}
				}
				map.Clear();
			}
		}

		//-------------------------------------------------------------------------/
		// Behaviour Methods
		//-------------------------------------------------------------------------/
		public BehaviourType GetBehaviourAtPosition(string layer, Vector3Int position)
		{
			if (!behaviours.HasBehaviour(layer, position))
			{
				return null;
			}
			return behaviours.GetBehaviour(layer, position);
		}

		public DerivedBehaviourType GetBehaviourAtPosition<DerivedBehaviourType>(string layer, Vector3Int position)
			where DerivedBehaviourType : BehaviourType
		{
			return behaviours.GetBehaviour<DerivedBehaviourType>(layer, position);
		}

		public T[] GetBehavioursInRange<T>(BehaviourType behaviour, Vector2Int range)
			where T : BehaviourType
		{
			if (!HasBehaviour(behaviour))
			{
				return null;
			}
			return GetBehavioursInRange<T>(behaviour.layer, behaviour.cellPosition, range);
		}

		public T[] GetBehavioursInRange<T>(string layer, Vector3Int position, Vector2Int range)
			where T : class, IStratusTileBehaviour
		{
			List<T> behaviours = new List<T>();
			Dictionary<Vector3Int, float> cells = GetRange(position, range);
			foreach (KeyValuePair<Vector3Int, float> cell in cells)
			{
				if (HasBehaviourAtPosition(layer, cell.Key))
				{
					BehaviourType behaviour = GetBehaviourAtPosition(layer, cell.Key);
					if (behaviour is T)
					{
						behaviours.Add(behaviour as T);
					}
				}
			}
			return behaviours.ToArray();
		}

		public bool HasBehaviour(BehaviourType behaviour)
		{
			return behaviours.HasBehaviour(behaviour);
		}

		public bool HasBehaviourAtPosition(string layer, Vector3Int position)
		{
			return behaviours.HasBehaviour(layer, position);
		}

		protected void InitializeBehaviourInstance(BehaviourType behaviour)
		{
			if (behaviour == null)
			{
				return;
			}

			behaviour.Initialize(this);
			if (!behaviours.AddBehaviour(behaviour.layer, behaviour, behaviour.cellPosition))
			{
				this.LogError($"Failed to add behaviour {behaviour} at layer {behaviour.layer}");
				return;
			}

			// Set the move callback
			behaviour.onMoved += (sourcePosition, targetPosition) =>
			{
				UpdateBehaviourPosition(behaviour, targetPosition);
			};

			this.Log($"Added behaviour {behaviour} at layer {behaviour.layer}");
		}

		protected void RemoveBehaviourInstance(BehaviourType behaviour)
		{
			if (behaviour == null)
			{
				return;
			}

			behaviour.Shutdown();
			if (behaviours.RemoveBehaviour(behaviour.layer, behaviour.cellPosition))
			{
				this.Log($"Removed behaviour {behaviour} at layer {behaviour.layer}");
			}
			else
			{
				this.Log($"Failed to remove behaviour {behaviour} at layer {behaviour.layer}");
			}
		}

		public void UpdateBehaviourPosition(BehaviourType behaviour, Vector3Int position)
		{
			behaviours.UpdateBehaviourPosition(behaviour.layer, behaviour, position);
			this.Log($"Updated {behaviour} position to {position}");
		}

	}


}