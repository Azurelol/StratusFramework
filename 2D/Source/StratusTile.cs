using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Stratus
{
    [CreateAssetMenu(menuName = "Stratus/2D/Tile", fileName = "Stratus Tile")]
    public class StratusTile : Tile, IStratusLogger
    {
        public Vector3Int position { get; private set; }
        //public Vector3 worldPosition { get; private set; }
        public bool hasGameObject => gameObject != null;

        protected virtual void OnRuntimeStartUp() { }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            this.position = position;
            if (Application.isPlaying)
            {
                OnRuntimeStartUp();
            }
            return base.StartUp(position, tilemap, go);
        }

        protected static T CreateFromPalette<T>(Sprite sprite)
            where T : StratusTile
        {
            var tile = ScriptableObject.CreateInstance<T>();
            tile.sprite = sprite;
            return tile;
        }

    }

    public abstract class StratusTileSelection
    {

    }

    public abstract class StratusTileSelection<TileType> 
        : StratusTileSelection
        where TileType : StratusTile
    {
        public class NavigateEvent : StratusEvent
        {
            public NavigateEvent(StratusTileSelection<TileType> tile)
            {
                this.selection = tile;
            }

            public StratusTileSelection<TileType> selection { get; private set; }
        }

        public TileType tile { get; private set; }
        public Vector3Int position { get; private set; }

        public StratusTileSelection()
        {
        }

        public override string ToString()
        {
            return $"{tile} {position}";
        }

        public void Set(TileType tile, Vector3Int position)
        {
            this.tile = tile;
            this.position = position;
        }

        public void Navigate()
        {
            StratusScene.Dispatch<NavigateEvent>(new NavigateEvent(this));
        }

        public T GetTile<T>() where T : TileType
        {
            return tile as T;
        }
    }

}