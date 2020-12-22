using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Instantiates a prefab when triggered
	/// </summary>
	public class StratusInstantiateTriggerable : StratusTriggerable
	{
		[Header("Instantiate")]
		public StratusPrefabInstantiateProcedure type = StratusPrefabInstantiateProcedure.Parent;
		public GameObject[] prefabs;
		[Tooltip("Whether to destroy this GameObject after instantiating")]
		public bool destroyOnInstantiate = false;
		[Header("Transform")]
		public StratusPositionField position = new StratusPositionField();
		public Space space = Space.Self;

		private List<GameObject> _instances = new List<GameObject>();

		public GameObject[] instances => _instances.ToArray();

		public override string automaticDescription
		{
			get
			{
				if (prefabs != null)
				{
					string value = $"Instantiate {prefabs.JoinToString()} at {position} ({space})";
					return value;
				}
				return string.Empty;
			}
		}

		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{
		}

		protected override void OnTrigger()
		{
			if (debug)
			{
				this.Log($"Instantiating {prefabs.Length} prefabs...");
			}
			_instances.AddRange(prefabs.ForEachNotNull(x => InstantiatePrefab(x)));
			if (destroyOnInstantiate)
			{
				DestroyGameObjectOnNextFrame();
			}

		}

		private GameObject InstantiatePrefab(GameObject prefab)
		{
			GameObject instance = Instantiate(prefab);

			// Remove the clone prefix
			instance.name.Replace("(Clone)", "");

			// Set position
			if (space == Space.Self)
			{
				instance.transform.localPosition = position;
			}
			else
			{
				instance.transform.position = position;
			}

			// Handle where the instance will be
			if (type == StratusPrefabInstantiateProcedure.Parent)
			{
				instance.transform.SetParent(this.transform);
			}
			else if (type == StratusPrefabInstantiateProcedure.None)
			{
				//instance.transform.SetParent(null);
				//Destroy(this.gameObject);
			}
			else if (type == StratusPrefabInstantiateProcedure.Replace)
			{
				var parent = transform.parent;
				instance.transform.SetParent(parent, false);
				Destroy(this.gameObject);
			}

			return instance;
		}
	}

	public enum StratusPrefabInstantiateProcedure
	{
		None,
		Parent,
		Replace,
	}

}