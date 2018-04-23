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
  public class CursorLock 
  {
    /// <summary>
    /// Whether this lock is allowed to update on input
    /// </summary>
    [Tooltip("Whether this lock is allowed to update on input")]
    public bool update = true;
    /// <summary>
    /// What input to use in order to release the cursor lock
    /// </summary>
    [Tooltip("What input to use in order to release the cursor lock")]
    public InputField releaseCursor = new InputField(KeyCode.Escape);
    /// <summary>
    /// Whether to lock the cursor is locked
    /// </summary>
    [Tooltip("Whether to lock the cursor initially")]
    public bool isLocked = false;
    /// <summary>
    /// Whether input is being polled
    /// </summary>
    protected bool pollInput = true;

    /// <summary>
    /// Enables the lock
    /// </summary>
    public virtual void Enable()
    {
      LockCursor(isLocked);
    }

    /// <summary>
    /// Disables the lock
    /// </summary>
    public virtual void Disable()
    {   
      LockCursor(false);
    }

    /// <summary>
    /// Updates the lock, based on current input
    /// </summary>
    public void Update()
    {
      if (update)
        UpdateCursorLock();
    }

    /// <summary>
    /// Locks the cursor
    /// </summary>
    /// <param name="isLocked"></param>
    public void LockCursor(bool lockCursor)
    {
      isLocked = lockCursor;

      //Cursor.lockState = CursorLockMode.None;
      //if (isLocked)
      //  Cursor.lockState = CursorLockMode.Locked;
      
      Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
      Cursor.visible = !isLocked;
      pollInput = isLocked;
      //Trace.Script($"lockCursor = {lockCursor}, Cursor lockstate = {Cursor.lockState}, Visible = " + Cursor.visible);
    }

    /// <summary>
    /// Locks Cinemachine camera input
    /// </summary>
    /// <param name="isLocked"></param>
    public void LockInput(bool isLocked)
    {
      pollInput = !isLocked;
    }

    private void UpdateCursorLock()
    {
      if (releaseCursor.isDown)
      {
        pollInput = isLocked = false;

      }
      else if (Input.GetMouseButtonUp(0))
      {
        pollInput = isLocked = true;
      }

      LockCursor(isLocked);
    }

  }

}