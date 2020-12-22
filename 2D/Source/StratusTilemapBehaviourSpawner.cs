using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Stratus
{
    public abstract class StratusTilemapBehaviourSpawnParameters
    {
        public Vector3Int cellPosition;

        public StratusTilemapBehaviourSpawnParameters()
        {
        }

        public StratusTilemapBehaviourSpawnParameters(Vector3Int cellPosition)
        {
            this.cellPosition = cellPosition;
        }
    }

    public interface IStratusTilemapSpawnable<Parameters>
    {
        void Assign(Tilemap tilemap, Parameters parameters);
    }

    public class StratusTilemapBehaviourSpawner<BehaviourType, SpawnParameters> : StratusBehaviour
        where BehaviourType : StratusTileBehaviour, IStratusTilemapSpawnable<SpawnParameters>
        where SpawnParameters : StratusTilemapBehaviourSpawnParameters, new()
    {
        public Tilemap tilemap;
        public BehaviourType behaviourPrefab;

        [SerializeReference]
        public List<SpawnParameters> behaviours = new List<SpawnParameters>();

        public void Awake()
        {
            this.Log($"Now spawning {behaviours.Count} behaviours...");
        }

        private void OnGUI()
        {
            foreach(var behaviour in behaviours)
            {

            }
        }


    }

//#if UNITY_EDITOR
//    [UnityEditor.CustomEditor(typeof(StratusTilemapSpawner))]
//    public class StratusTilemapSpawnerEditor : UnityEditor.CustomEditor
//    {

//    }
//#endif
}
