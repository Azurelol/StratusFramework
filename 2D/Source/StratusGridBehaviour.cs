using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
    public abstract class StratusGridBehaviour<BaseTileType> : StratusBehaviour
        where BaseTileType : StratusTile, new()
    {
        //-------------------------------------------------------------------------//
        // Fields
        //-------------------------------------------------------------------------//
        [SerializeField]
        private Grid _grid;

        //-------------------------------------------------------------------------//
        // Properties
        //-------------------------------------------------------------------------//
        public Grid grid => _grid;
        public abstract StratusTilemap<BaseTileType> baseLayer { get; }
        public Camera mapCamera { get; private set; }

        //-------------------------------------------------------------------------//
        // Abstract
        //-------------------------------------------------------------------------//
        public abstract IStratusTilemap[] layers { get; }
        protected abstract void OnInitialize();

        public void Initialize(Camera camera)
        {
            this.mapCamera = camera;
            baseLayer.Initialize(mapCamera);
            foreach (var layer in layers)
            {
                layer.Initialize(mapCamera);
            }
            OnInitialize();
        }

        public void NavigateAtPosition(Vector3Int cellPosition)
        {
            var baseTile = baseLayer.GetTile(cellPosition);
            baseTile?.Navigate();
            OnNavigateAtPosition(cellPosition);
        }
        protected abstract void OnNavigateAtPosition(Vector3Int cellPosition);

        public void SelectAtPosition(Vector3Int cellPosition)
        {
            NavigateAtPosition(cellPosition);
            OnSelectAtPosition(cellPosition);
        }
        protected abstract void OnSelectAtPosition(Vector3Int cellPosition);
    }

}