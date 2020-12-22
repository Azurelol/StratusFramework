using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// A helper for locking the cursor in-game
	/// </summary>
	[Serializable]
	public class StratusCursorLock
	{
		/// <summary>
		/// Whether to lock the cursor
		/// </summary>
		[Tooltip("Whether to lock the cursor initially")]
		public CursorLockMode mode = CursorLockMode.None;
		/// <summary>
		/// Whether the cursor should be visible
		/// </summary>
		public bool visible = true;

		public StratusCursorLock()
		{
		}

		public StratusCursorLock(CursorLockMode mode, bool visible)
		{
			this.mode = mode;
			this.visible = visible;
		}

		/// <summary>
		/// Whether the cursor is currently locked
		/// </summary>
		public static bool locked => Cursor.lockState == CursorLockMode.Locked;
		private static CursorLockMode previousMode { get; set; }
		private static bool previouslyVisible { get; set; }
		public static bool verbose { get; set; } = true;

		/// <summary>
		/// Enables the lock
		/// </summary>
		public void Enable()
		{
			LockCursor(mode, visible);
		}

		/// <summary>
		/// Disables the lock
		/// </summary>
		public void Disable()
		{
			RevertLock();
		}

		/// <summary>
		/// Locks the cursor, storing the current state
		/// </summary>
		/// <param name="state"></param>
		/// <param name="visible"></param>
		/// <returns></returns>
		public static bool LockCursor(CursorLockMode state, bool visible)
		{
			if (Cursor.lockState == state && Cursor.visible == visible)
			{
				return false;
			}

			previousMode = Cursor.lockState;
			previouslyVisible = Cursor.visible;

			Cursor.lockState = state;
			Cursor.visible = visible;

			if (verbose)
			{
				StratusDebug.Log($"Cursor lockstate = {Cursor.lockState}, Visible = " + Cursor.visible);
			}

			return true;
		}

		/// <summary>
		/// Reverts the cursor lock to its previous state
		/// </summary>
		public static void RevertLock()
		{
			LockCursor(previousMode, previouslyVisible);
		}

		/// <summary>
		/// Releases the cursor lock
		/// </summary>
		public static void ReleaseLock()
		{
			LockCursor(CursorLockMode.None, true);
		}

		/// <summary>
		/// Locks the cursor using given arguments
		/// </summary>
		/// <param name="cursorLock"></param>
		public static void LockCursor(StratusCursorLock cursorLock)
		{
			LockCursor(cursorLock.mode, cursorLock.visible);
		}
	}

}