using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Stratus
{
	/// <summary>
	/// Base class for navigation ranges where the selection is overriden
	/// </summary>
	public abstract class StratusGridNavigationRange
	{
		public IStratusGridManager grid { get; private set; }
		public Vector3Int origin { get; private set; }

		public StratusGridNavigationRange(Vector3Int origin, IStratusGridManager grid)
		{
			this.grid = grid;
			this.origin = origin;
		}

		public virtual Vector3Int firstSelection => origin;

		public abstract bool Contains(Vector3Int position);
		public abstract void Highlight();
		public abstract void OnNavigateToCellPosition(Vector3Int position);
		public virtual Vector3Int GetNextCellPosition(Vector2 direction)
		{
			int x = (int)direction.x;
			int y = (int)direction.y;

			Vector3Int nextCellPosition = grid.currentCellPosition;
			nextCellPosition.x += x;
			nextCellPosition.y += y;
			return nextCellPosition;
		}
	}

	/// <summary>
	/// Base classs for navigation ranges
	/// </summary>
	public class StratusGridNavigationSelectionRange : StratusGridNavigationRange
	{
		public HashSet<Vector3Int> cells { get; private set; }
		private StratusArrayNavigator<Vector3Int> cellNavigator { get; set; }
		public Color highlightColor { get; set; }
		public Color navigationColor { get; set; }

		public StratusGridNavigationSelectionRange(Vector3Int origin, 
			IStratusGridManager grid, 
			IEnumerable<Vector3Int> cells,
			Color highlightColor,
			Color navigationColor)

			: base(origin, grid)
		{
			this.cells = new HashSet<Vector3Int>(cells);
			cellNavigator = new StratusArrayNavigator<Vector3Int>(cells.ToArray(), true);
			this.highlightColor = highlightColor;
			this.navigationColor = navigationColor;
		}

		public override bool Contains(Vector3Int position)
		{
			return cells.Contains(position);
		}

		public override Vector3Int firstSelection => cells.First();

		public override void OnNavigateToCellPosition(Vector3Int position)
		{
			Highlight();
			grid.Highlight(true, position, navigationColor);
		}

		public override Vector3Int GetNextCellPosition(Vector2 direction)
		{
			if (direction.x > 0 || direction.y < 0)
			{
				cellNavigator.Next();
			}
			else if (direction.x < 0 || direction.y > 0)
			{
				cellNavigator.Previous();
			}
			return cellNavigator.current;
		}

		public override void Highlight()
		{
			foreach (var cell in cells)
			{
				grid.Highlight(true, cell, highlightColor);
			}
		}
	}

	public class StratusGridMovementNavigationRange : StratusGridNavigationRange
	{
		public Dictionary<Vector3Int, float> movementRange { get; private set; }
		public Color movementRangeColor { get; set; }
		public Vector3Int[] currentPath { get; set; }
		public Color pathColor { get; set; }

		public StratusGridMovementNavigationRange(Vector3Int origin, IStratusGridManager grid,
			Dictionary<Vector3Int, float> movementRange) : base(origin, grid)
		{
			this.movementRange = movementRange;
		}

		public override bool Contains(Vector3Int position) => movementRange.ContainsKey(position);

		public Vector3Int OnNavigateDirection(Vector2 direction)
		{
			return default;
		}

		public override void Highlight()
		{
			grid.Highlight(true, movementRange.Keys.ToArray(), movementRangeColor);
			//foreach (var cell in movementRange)
			//{
			//	Vector3Int position = cell.Key;
			//	grid.Highlight(true, position, movementRangeColor);
			//}
		}

		protected virtual Vector3Int[] GetPath(Vector3Int position)
		{
			return grid.GetPath(origin, position);
		}

		public override void OnNavigateToCellPosition(Vector3Int position)
		{
			Vector3Int[] path = GetPath(position);

			// Dehighlight previous path
			if (currentPath != null)
			{
				// Revert previous color
				grid.Highlight(true, currentPath, movementRangeColor);
				//foreach (var cell in currentPath)
				//{
				//	grid.Highlight(true, cell, movementRangeColor);
				//}
			}
			currentPath = path;
			grid.Highlight(true, path, pathColor);
			//foreach (var cell in path)
			//{
			//	grid.Highlight(true, cell, pathColor);
			//}
		}
	}
}