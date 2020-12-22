using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Stratus
{
    public interface IStratusTilemap
    {
        void Initialize(Camera camera);
    }

    [Serializable]
    public class StratusTilemap<TileType>: IStratusTilemap, IStratusLogger
        where TileType : StratusTile, new()
    {
        [Serializable]
        public class Selection : StratusTileSelection<TileType>
        {
        }

        //-------------------------------------------------------------------------/
        // Fields
        //-------------------------------------------------------------------------/
        [SerializeField]
        private Tilemap _tilemap;

        //-------------------------------------------------------------------------/
        // Properties
        //-------------------------------------------------------------------------/
        public Tilemap tilemap => _tilemap;
        public Camera camera { get; private set; }
        public TilemapRenderer tilemapRenderer => tilemap.GetComponent<TilemapRenderer>();
        public Vector3Int mousePositionToCell => WorldToCell(camera.GetMousePositionToWorld());
        public BoundsInt bounds => tilemap.cellBounds;
        //public TileType[] tiles { get; private set; }

        //-------------------------------------------------------------------------/
        // Methods
        //-------------------------------------------------------------------------/
        public void Initialize(Camera camera)
        {
            this.camera = camera;
        }

        public void Load(Tilemap tilemap)
        {
            _tilemap = tilemap;
        }

        public Selection GetTile(Vector3Int position)
        {
            Selection selection = null;
            TileType tile = tilemap.GetTile<TileType>(position);
            if (tile != null)
            {
                selection = new Selection();
                selection.Set(tile, position);
            }
            return selection;
        }

        public void ToggleVisibility(bool toggle)
        {
            tilemapRenderer.enabled = toggle;
        }

        //-------------------------------------------------------------------------/
        // Methods: Tiles
        //------------------------------------------------------------------------/
        public Selection GetTileAtMousePosition()
        {
            Vector3 mousePos = camera.GetMousePositionToWorld();
            Vector3Int cellPos = WorldToCell(mousePos);
            return GetTile(cellPos);
        }

        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return tilemap.WorldToCell(worldPosition);
        }

        public Vector3 CellCenterToWorld(Vector3Int cellPosition)
        {
            return tilemap.GetCellCenterWorld(cellPosition);
        }

        public bool HasTile(Vector3Int position) => tilemap.HasTile(position);

        public void MoveToTile(Transform transform, Vector3Int position)
        {
            Selection tile = GetTile(position);
            MoveToTile(transform, tile);
        }

        public void SetTile(Vector3Int position, StratusTile tile)
        {
            tilemap.SetTile(position, tile);
        }

        public void SetTile(Vector3Int position, StratusTile tile, Color color)
        {
            tilemap.SetTile(position, tile);
            SetTileColor(position, color);
        }

        public void RemoveTile(Vector3Int position)
        {
            tilemap.SetTile(position, null);
        }

        public void SetTileColor(Vector3Int position, Color color)
        {
            //this.Log($"Setting tile {position} color to {color}");
            tilemap.SetTileFlags(position, TileFlags.None);
            tilemap.SetColor(position, color);
        }

        public void RemoveTileColor(Vector3Int position)
        {
            tilemap.RemoveTileFlags(position, TileFlags.None);
            tilemap.SetColor(position, Color.white);
        }

        public void MoveToTile(Transform transform, Selection tile)
        {
            var targetPos = tilemap.GetCellCenterWorld(tile.position);
            transform.position = targetPos;
        }

        public void ClearTiles()
        {
            tilemap.ClearAllTiles();
        }
    }
    
    [Serializable]
    public class StratusTilemap : StratusTilemap<StratusTile>
    {
    }

}