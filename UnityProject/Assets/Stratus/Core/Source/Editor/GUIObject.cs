using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  /// <summary>
  /// Provides an object-oriented way of drawing GUI elements
  /// </summary>
  public struct GUIObject
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The current GUI event in Unity
    /// </summary>
    public UnityEngine.Event currentEvent => UnityEngine.Event.current;
    /// <summary>
    /// The current rect used by the object
    /// </summary>
    public Rect rect { get; private set; }
    /// <summary>
    /// The label to be used by this object
    /// </summary>
    public string label { get; set; }
    /// <summary>
    /// Additional information about this object
    /// </summary>
    public string description { get; set; }
    /// <summary>
    /// The tooltip string for this object
    /// </summary>
    public string tooltip { get; set; }
    /// <summary>
    /// Whether to show the description of this button alongside its label
    /// </summary>
    public bool showDescription { get; set; }
    /// <summary>
    /// Whether to concatenate the description with the label
    /// </summary>
    public bool descriptionsWithLabel { get; set; }
    /// <summary>
    /// The method to invoke when this button is left clicked 
    /// </summary>
    public System.Action onLeftClickDown { get; set; }
    /// <summary>
    /// The method to invoke when this button is left clicked, then released
    /// </summary>
    public System.Action onLeftClickUp { get; set; }
    /// <summary>
    /// The method to invoke when this button is clicked by the middle mouse button 
    /// </summary>
    public System.Action onMiddleClickDown { get; set; }
    /// <summary>
    /// The method to invoke when this button is clicked by the middle mouse button, then released
    /// </summary>
    public System.Action onMiddleClickUp { get; set; }
    /// <summary>
    /// The method to invoke when this button is clicked by the right mouse button
    /// </summary>
    public System.Action onRightClickDown { get; set; }
    /// <summary>
    /// The method to invoke when this button is clicked by the right mouse button and released
    /// </summary>
    public System.Action onRightClickUp { get; set; }
    /// <summary>
    /// The method to invoke when this button is being dragged
    /// </summary>
    public System.Action onDrag { get; set; }
    /// <summary>
    /// The method to invoke when an object is drag and dropped onto this button
    /// </summary>
    public System.Action<object> onDrop { get; set; }
    /// <summary>
    /// The string identifier for what type of dragged data to accept
    /// </summary>
    public string dragDataIdentifier { get; set; }
    /// <summary>
    /// The data to be used when this button is dragged
    /// </summary>
    public object dragData { get; set; }
    /// <summary>
    /// The data to be used when this button is dragged
    /// </summary>
    public Func<object, bool> onValidateDrag { get; set; }
    /// <summary>
    /// The background color to use
    /// </summary>
    public Color backgroundColor { get; set; } // = Color.white;
    /// <summary>
    /// Outline color to use
    /// </summary>
    public Color outlineColor { get; set; }
    /// <summary>
    /// Whether this button is currently moused over
    /// </summary>
    public bool isMousedOver
    {
      get
      {
        return rect.Contains(currentEvent.mousePosition);
      }
    }

    public bool isSelected { get; set; }

    /// <summary>
    /// Whether this button is draggable
    /// </summary>
    public bool isDraggable => dragData != null && dragDataIdentifier != null;
    /// <summary>
    /// Whether this button accepts a drop operation from a drag
    /// </summary>
    public bool isDroppable => onDrop != null && dragDataIdentifier != null;
    /// <summary>
    /// Whether one of these buttons is currentl being dragged
    /// </summary>
    public static bool isDragging { get; private set; }
    /// <summary>
    /// What to do on certain key presses
    /// </summary>
    private Dictionary<KeyCode, System.Action> keyMap { get; set; }  // = new Dictionary<KeyCode, System.Action>();
    /// <summary>
    /// Whether this object is checking for keys
    /// </summary>
    public bool hasKeys => keyMap != null;

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/


    //------------------------------------------------------------------------/
    // CTOR
    //------------------------------------------------------------------------/
    //public GUIObject(string label = null)
    //{
    //  this.label = label;
    //  onValidateDrag = null;
    //  dragData = null;
    //  dragDataIdentifier = null;
    //  onDrag = null;
    //  onDrop = null;
    //  onRightClickDown = onRightClickUp = null;
    //  onLeftClickDown = onLeftClickUp= null;
    //  onMiddleClickDown = onMiddleClickUp= null;
    //  showDescription = descriptionsWithLabel = false;
    //  tooltip = description = string.Empty;
    //  backgroundColor = Color.white;
    //}

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Draws this button using Unity's GUILayout system
    /// </summary>
    /// <param name="style"></param>
    /// <param name="options"></param>
    /// <returns>True if the mouse was moused over this control</returns>
    public bool Draw(GUIStyle style = null, params GUILayoutOption[] options)
    {
      GUIContent content = new GUIContent(showDescription ? $"{label}\n{description}" : label, tooltip);

      if (backgroundColor != default(Color)) GUI.backgroundColor = backgroundColor;
      {
        GUILayout.Box(content, style, options);
        rect = GUILayoutUtility.GetLastRect(); 
      }
      if (backgroundColor != default(Color)) GUI.backgroundColor = Color.white;
      if (outlineColor != default(Color) && isSelected)  StratusGUIStyles.DrawOutline(rect, outlineColor);

      //Vector2 style .CalcSize(content)

      // Keyboard
      if (isSelected)
      {        
        if (OnKey())
          return true;
      }

      // Mouse
      if (isMousedOver)
      {
        OnMouse();
        return true;
      }

      return false;
    }

    /// <summary>
    /// Handles mouse events
    /// </summary>
    private void OnMouse()
    {
      if (isMousedOver)
      {
        switch (currentEvent.type)
        {
          case EventType.MouseDown:
            if (isDraggable)
            {
              DragAndDrop.PrepareStartDrag();
              DragAndDrop.objectReferences = new UnityEngine.Object[] { (UnityEngine.Object)dragData };
              DragAndDrop.SetGenericData(dragDataIdentifier, dragData);
            }
            switch (currentEvent.button)
            {
              case 0: onLeftClickDown?.Invoke(); break;
              case 1: onRightClickDown?.Invoke(); break;
              case 2: onMiddleClickDown?.Invoke(); break;
            }
            currentEvent.Use();
            break;

          case EventType.MouseUp:
            switch (currentEvent.button)
            {
              case 0: onLeftClickUp?.Invoke(); break;
              case 1: onRightClickUp?.Invoke(); break;
              case 2: onMiddleClickUp?.Invoke(); break;
            }
            if (isDraggable)
            {
              DragAndDrop.PrepareStartDrag();
              isDragging = false;
            }
            currentEvent.Use();
            //int control = GUIUtility.GetControlID(FocusType.Keyboard);
            //GUIUtility.hotControl = control;
            //Selection.SetActiveObjectWithContext()
            break;

          case EventType.MouseDrag:
            // If the drag was started here
            if (isDraggable)
            {
              object existingDragData = DragAndDrop.GetGenericData(dragDataIdentifier);
              if (existingDragData != null)
              {
                DragAndDrop.StartDrag($"Dragging {label}");
                onDrag?.Invoke();
                currentEvent.Use();
                isDragging = true;
              }
            }
            break;

          case EventType.DragUpdated:
            if (isDraggable)
            {
              if (ValidateDragData())
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
              else
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
            currentEvent.Use();
            break;

          case EventType.DragPerform:
            if (isDroppable)
            {
              DragAndDrop.AcceptDrag();
              object receivedDragData = DragAndDrop.GetGenericData(dragDataIdentifier);
              if (receivedDragData != null)
              {
                onDrop(receivedDragData);
              }
            }
            currentEvent.Use();
            break;

          case EventType.DragExited:
            if (isDraggable)
            {
              isDragging = false;
              DragAndDrop.PrepareStartDrag();
            }
            break;
        }
      }
    }

    /// <summary>
    /// Handles keyboard events
    /// </summary>
    private bool OnKey()
    {
      if (hasKeys && currentEvent.isKey && currentEvent.type == EventType.KeyDown)
      {
        if (keyMap.ContainsKey(currentEvent.keyCode))
        {
          keyMap[currentEvent.keyCode].Invoke();
          return true;
        }
      }
      return false;
    }

    public void AddKey(KeyCode key, System.Action onKey)
    {
      if (keyMap == null)
        keyMap = new Dictionary<KeyCode, System.Action>();
      keyMap.Add(key, onKey);
    }

    /// <summary>
    /// Validates the current dragging data
    /// </summary>
    /// <returns></returns>
    private bool ValidateDragData()
    {
      object receivedDragData = DragAndDrop.GetGenericData(dragDataIdentifier);
      if (receivedDragData != null && onValidateDrag != null && onValidateDrag(receivedDragData))
        return true;
      return false;
    }    

  }
}
