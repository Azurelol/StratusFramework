using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stratus
{
    /// <summary>
    /// Clock-wise rotation around a hex cell with pointy top
    /// </summary>
    public enum StratusHexagonalOddRowDirection
    {
        Right,
        UpperRight,
        UpperLeft,
        Left,
        DownLeft,
        DownRight
    }

    public enum StratusHexOffsetCoordinates
    {
        OddRow,
        EvenRow,
        OddColumn,
        EvenColumn
    }

    /// <summary>
    /// Provides various utility functions for use in grids
    /// </summary>
	public static class StratusGridUtility
	{
        //-------------------------------------------------------------------------/
        // Declarations
        //-------------------------------------------------------------------------/
        public class GridSearch : StratusSearch<Vector3Int>
        {
            private GridSearch()
            {
            }
        }

        //-------------------------------------------------------------------------/
        // Properties
        //-------------------------------------------------------------------------/
        public static readonly StratusHexagonalOddRowDirection[] oddRowDirections
            = (StratusHexagonalOddRowDirection[])Enum.GetValues(typeof(StratusHexagonalOddRowDirection));


        //-------------------------------------------------------------------------/
        // Lerp
        //-------------------------------------------------------------------------/
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static Vector3 CubeLerp(Vector3Int a, Vector3Int b, float t) 
        {
            return new Vector3(Lerp(a.x, b.x, t),
                               Lerp(a.y, b.y, t),
                               Lerp(a.z, b.z, t));
        }

        public static Vector3Int CubeRound(Vector3 cube)
        {
            int rx = (int)Mathf.Round(cube.x);
            int ry = (int)Mathf.Round(cube.y);
            int rz = (int)Mathf.Round(cube.z);

            var x_diff = Mathf.Abs(rx - cube.x);
            var y_diff = Mathf.Abs(ry - cube.y);
            var z_diff = Mathf.Abs(rz - cube.z);

            if (x_diff > y_diff && x_diff > z_diff)
            {
                rx = -ry - rz;
            }
            else if (y_diff > z_diff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new Vector3Int(rx, ry, rz);
        }

        public static Vector3Int OffsetRound(Vector3Int offset)
        {
            var cube = OffsetToCube(offset);
            return CubeToOffset(CubeRound(cube));
        }

        //-------------------------------------------------------------------------/
        // Conversions
        //-------------------------------------------------------------------------/
        public static Vector3Int OffsetToAxialXY(Vector3Int value)
        {
            value.y = value.y - (int)Mathf.Floor(value.x / 2f);
            return value;
        }

        public static Vector3Int AxialToOffsetCoordinatesXY(Vector3Int value)
        {
            value.y = value.y + (int)Mathf.Floor(value.x / 2f);
            return value;
        }

        public static Vector3Int OffsetToCube(Vector3Int value)
        {
            return OffsetOddRowToCube(value);
        }

        public static Vector3Int OffsetOddRowToCube(Vector3Int value)
        {
            int x = value.x - (value.y - (value.y & 1)) / 2;
            int z = value.y;
            int y = -x - z;
            return new Vector3Int(x, y, z);
        }

        public static Vector3Int CubeToOffset(Vector3Int value)
        {
            return CubeToOffsetOddRow(value);
        }

        public static Vector3Int CubeToOffsetOddRow(Vector3Int value)
        {
            int col = value.x + (value.z - (value.z & 1)) / 2;
            int row = value.z;
            return new Vector3Int(col, row, 0);
        }

        public static Vector3Int CubeToAxial(Vector3Int cube)
        {
            var q = cube.x;
            var r = cube.z;
            return new Vector3Int(q, r, 0);
        }

        public static Vector3Int AxialToCube(Vector3Int axial)
        {
            var x = axial.x;
            var z = axial.y;
            var y = -x - z;
            return new Vector3Int(x, y, z);
        }

        //-------------------------------------------------------------------------/
        // Distances
        //-------------------------------------------------------------------------/
        public static float HexCubeDistance(Vector3Int a, Vector3Int b)
        {
            // return (abs(a.x - b.x) + abs(a.y - b.y) + abs(a.z - b.z)) / 2
            float distance = (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2f;
            return distance;
        }
        
        public static float HexOffsetDistance(Vector3Int a, Vector3Int b)
        {
            var ac = OffsetToCube(a);
            var bc = OffsetToCube(b);
            return HexCubeDistance(ac, bc);
        }

        public static float RectangleDistance(Vector3Int a, Vector3Int b, bool diagonal)
        {
            return diagonal ? EuclideanDistance(a, b) : ManhattanDistance(a, b);
        }

        /// <summary>
        /// The square root of the sum of the squares of the differences of the coordinates.
        /// </summary>
        public static float EuclideanDistance(Vector3Int a, Vector3Int b)
        {
            return Vector3Int.Distance(a, b);
        }

        /// <summary>
        /// The sum of the absolute values of the differences of the coordinates.
        /// That is, the number of cells you must travel vertically, plus the number of cells you 
        /// must travel horizontally, much like a taxi driving through a grid of city streets.
        /// </summary>
        public static float ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        //-------------------------------------------------------------------------/
        // Neighbors
        //-------------------------------------------------------------------------/
        private static Vector3Int[][] oddRowDirectionValues = new Vector3Int[][]
        {
            new Vector3Int[] 
            {
                new Vector3Int(1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 0),
                new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0) 
            },
            new Vector3Int[]
            {
                new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 0),
                new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0)
            }
        };

        /// <summary>
        /// Where x = col, y = row
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3Int FindNeighboringCellsOddRow(Vector3Int hex, 
            StratusHexagonalOddRowDirection direction)
		{
            int parity = hex.y & 1;
            Vector3Int offset = oddRowDirectionValues[parity][(int)direction];
            Vector3Int neighbor = new Vector3Int(hex.x + offset.x, hex.y + offset.y, 0);
            return neighbor;
        }

        public static Vector3Int[] FindNeighboringCellsHexOffset(Vector3Int hex)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            for(int i = 0; i < oddRowDirections.Length; ++i)
            {
                result.Add(FindNeighboringCellsOddRow(hex, oddRowDirections[i]));
            }
            return result.ToArray();
        }

        public static Vector3Int[] FindNeighboringCellsRectangle(Vector3Int element)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            result.Add(new Vector3Int(element.x + 1, element.y, element.z));
            result.Add(new Vector3Int(element.x - 1, element.y, element.z));
            result.Add(new Vector3Int(element.x, element.y + 1, element.z));
            result.Add(new Vector3Int(element.x, element.y - 1, element.z));
            return result.ToArray();
        }

        public static Vector3Int[] FindNeighboringCells(Vector3Int element, GridLayout.CellLayout layout)
        {
            Vector3Int[] result = null;
            switch (layout)
            {
                case GridLayout.CellLayout.Rectangle:
                    result = FindNeighboringCellsRectangle(element);
                    break;
                case GridLayout.CellLayout.Hexagon:
                    result = FindNeighboringCellsHexOffset(element);
                    break;
                case GridLayout.CellLayout.Isometric:
                    break;
                case GridLayout.CellLayout.IsometricZAsY:
                    break;
            }
            return result;
        }

        //-------------------------------------------------------------------------/
        // Ranges
        //-------------------------------------------------------------------------/
        /// <summary>
        /// Returns the cell range given an origin
        /// </summary>
        public static Dictionary<Vector3Int, float> GetRange(Vector3Int origin, 
            int n, 
            GridLayout.CellLayout layout,
            Predicate<Vector3Int> predicate)
        {
            Dictionary<Vector3Int, float> result = null;
            switch (layout)
            {
                case GridLayout.CellLayout.Rectangle:
                    result = GetRangeRectangle(origin, n, predicate);
                    break;
                case GridLayout.CellLayout.Hexagon:
                    result = GetRangeHexOffset(origin, n, predicate);
                    break;
                case GridLayout.CellLayout.Isometric:
                    break;
                case GridLayout.CellLayout.IsometricZAsY:
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// Returns the cell range given an origin and a range
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="n"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Dictionary<Vector3Int, float> GetRangeRectangle(Vector3Int origin, int n, Predicate<Vector3Int> predicate = null)
        {
            GridSearch.RangeSearch search
                = new GridSearch.RangeSearch()
                {
                    debug = false,
                    distanceFunction = ManhattanDistance,
                    neighborFunction = FindNeighboringCellsRectangle,
                    traversableFunction = predicate,
                    range = n,
                    startElement = origin
                };
            return search.SearchWithCosts();
        }

        /// <summary>
        /// Returns the cell range for a hexagon
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="n"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Dictionary<Vector3Int, float> GetRangeHexOffset(Vector3Int origin, int n, Predicate<Vector3Int> predicate = null)
        {
            GridSearch.RangeSearch search
            = new GridSearch.RangeSearch()
            {
                debug = false,
                distanceFunction = HexOffsetDistance,
                neighborFunction = FindNeighboringCellsHexOffset,
                traversableFunction = predicate,
                range = n,
                startElement = origin
            };
            var result = search.SearchWithCosts();
            return result;

            //List<Vector3Int> result = new List<Vector3Int>();
            //Vector3Int originc = OffsetToCube(origin);

            //for(int x = -n; x <= n; ++x)
            //{
            //    for(int y = Math.Max(- n, -x-n); y <= Math.Min(n, -x+n); ++y)
            //    {
            //        int z = -x - y;
            //        Vector3Int cellC = originc + new Vector3Int(x, y, z);
            //        Vector3Int cell = CubeToOffset(cellC);
            //        if (predicate == null || predicate(cell))
            //        {
            //            result.Add(cell);
            //        }
            //    }
            //}

            //return result.ToArray();
        }

        //-------------------------------------------------------------------------/
        // Path
        //-------------------------------------------------------------------------/
        public static Vector3Int[] FindPath(Vector3Int origin, Vector3Int target, GridLayout.CellLayout layout,
            Predicate<Vector3Int> traversablePredicate = null)
        {
            Vector3Int[] result = null;
            switch (layout)
            {
                case GridLayout.CellLayout.Rectangle:
                    result = FindRectanglePath(origin, target, traversablePredicate);
                    break;
                case GridLayout.CellLayout.Hexagon:
                    result = FindHexOffsetPath(origin, target, traversablePredicate);
                    break;
                case GridLayout.CellLayout.Isometric:
                    break;
                case GridLayout.CellLayout.IsometricZAsY:
                    break;
            }
            return result;
        }

        public static Vector3Int[] FindRectanglePath(Vector3Int origin, Vector3Int target,
            Predicate<Vector3Int> traversablePredicate = null)
        {
            var pathSearch = new GridSearch.PathSearch()
            {
                startElement = origin,
                targetElement = target,
                distanceFunction = ManhattanDistance,
                neighborFunction = FindNeighboringCellsRectangle,
                traversableFunction = traversablePredicate
            };
            return pathSearch.Search();
        }

        public static Vector3Int[] FindHexOffsetPath(Vector3Int origin, Vector3Int target, 
            Predicate<Vector3Int> traversablePredicate = null)
        {
            var pathSearch = new GridSearch.PathSearch()
            {
                startElement = origin,
                targetElement = target,
                distanceFunction = HexOffsetDistance,
                neighborFunction = FindNeighboringCellsHexOffset,
                traversableFunction = traversablePredicate
            };
            return pathSearch.Search();
        }
    }

    public static class StratusGridExtensions
	{
	}

}