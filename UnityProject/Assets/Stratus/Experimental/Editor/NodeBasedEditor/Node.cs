using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace Editor
  {
    /// <summary>
    /// A node will be responsible for drawing itself and processing its own events.
    /// </summary>
    public abstract partial class Node
    {
      //------------------------------------------------------------------------/
      // Classes
      //------------------------------------------------------------------------/
      public class Settings
      {
        public GUIStyle DefaultStyle;
        public GUIStyle SelectedStyle;
        public Vector2 Size;
        public Action<Node> OnRemove;

        public Settings(GUIStyle defaultStyle, GUIStyle selectedStyle, Vector2 size, Action<Node> onRemove)
        {
          this.DefaultStyle = defaultStyle;
          this.SelectedStyle = selectedStyle;
          this.Size = size;
          this.OnRemove = onRemove;
        }
      }

      /// <summary>
      /// A node event (such as the node being selected, deselected, etc)
      /// </summary>
      public enum Event
      {
        Select,
        Deselect,
        Remove
      }
            

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      public GUIStyle CurrentStyle;
      public GUIStyle DefaultStyle;
      public GUIStyle SelectedStyle;

      private GUIStyle HeaderStyle;
      private GUIStyle DescriptionStyle;

      private List<ContentElement> Content = new List<ContentElement>();
      private Vector2 ContentSize { get; set; }
      public Action<Node, Node.Event> OnEvent;
      private const KeyCode DeleteKey = KeyCode.Delete;

      //------------------------------------------------------------------------/
      // Properties: Public
      //------------------------------------------------------------------------/      
      public string Name { get; set; }
      //public ContentElement Description { get; set; }
      public Rect Rect { get; private set; }
      public Vector2 Position { get { return Rect.position; } }
      public Vector2 Size { get { return Rect.size; } }
      public ConnectionPoint InPoint { get; private set; }
      public ConnectionPoint OutPoint { get; private set; }
      public bool IsDragged { get; private set; }
      public bool IsSelected { get; private set; }
      public Node Parent { get; private set; }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnProcessContextMenu(GenericMenu menu);

      //------------------------------------------------------------------------/
      // Constructor
      //------------------------------------------------------------------------/
      public void Initialize(Rect position, GUIStyle defaultStyle, GUIStyle selectedStyle, Action<Node, Node.Event> onEvent)
      {
        this.Rect = position;
        this.OnEvent = onEvent;        

        this.DefaultStyle = defaultStyle;
        this.SelectedStyle = selectedStyle;
        this.CurrentStyle = defaultStyle;
      }

      //------------------------------------------------------------------------/
      // Methods: Public
      //------------------------------------------------------------------------/
      /// <summary>
      /// Draws this node, as well as its connection points
      /// </summary>
      public void Draw(float scale)
      {
        GUI.Box(this.Rect, GUIContent.none, this.CurrentStyle);        
        DrawContent();
        if (InPoint.Enabled) InPoint.Draw(scale);
        if (OutPoint.Enabled) OutPoint.Draw(scale);
        //var center = this.Rect.center;
        //GUI.Label(new Rect(center, new Vector2(Rect.width, Rect.height)), Name);
      }

      /// <summary>
      /// Drags this node, modifying its position
      /// </summary>
      /// <param name="delta"></param>
      public void Drag(Vector2 delta)
      {
        this.Rect = new Rect(this.Rect.position + delta, this.Rect.size);
        //this.Rect.position += delta;
      }

      /// <summary>
      /// Selects this node
      /// </summary>
      public void Select()
      {
        IsSelected = true;
        
        //CurrentStyle = SelectedStyle;        
        this.OnEvent(this, Event.Select);
      }

      /// <summary>
      /// Deselects this node
      /// </summary>
      public void Deselect()
      {
        IsSelected = false;
        //CurrentStyle = DefaultStyle;
        this.OnEvent(this, Event.Deselect);
      }

      /// <summary>
      /// Deletes this node
      /// </summary>
      public void Delete()
      {
        this.OnEvent(this, Event.Remove);
      }

      //------------------------------------------------------------------------/
      // Methods: Modification
      //------------------------------------------------------------------------/
      public void AddConnection(ConnectionPoint.ConnectionSettings settings)
      {
        if (settings.Type == ConnectionPoint.ConnectionType.In)
          this.InPoint = new ConnectionPoint(this, settings);
        else if (settings.Type == ConnectionPoint.ConnectionType.Out)
          this.OutPoint = new ConnectionPoint(this, settings);
      }

      //------------------------------------------------------------------------/
      // Methods: Events
      //------------------------------------------------------------------------/
      /// <summary>
      /// Returns a boolean which we can check to know whether we should
      /// repaint the GUI or not.
      /// </summary>
      /// <param name="e"></param>
      /// <returns></returns>
      public bool ProcessEvents(UnityEngine.Event e)
      {
        switch (e.type)
        {
          case EventType.MouseDown:

            // User attempts to select the node
            if (e.button == 0)
            {
              if (Rect.Contains(e.mousePosition))
              {
                Select();

                // If we clicked on a context element
                // that has a callback set, have it process.
                // Otherwise, select this node and begin to drag!
                if (!CheckContentElements(e.mousePosition))
                {
                  IsDragged = true;
                }

                e.Use();
              }
              else
              {
                Deselect();
              }

              GUI.changed = true;
            }

            // User attempts to modify the node
            if (e.button == 1 && IsSelected && Rect.Contains(e.mousePosition))
            {
              ProcessContextMenu();
              e.Use();
            }
            break;

          // If we release the mouse
          case EventType.MouseUp:
            IsDragged = false;
            break;

          // We use the 'drag' event so it stops event bubbling (otherwise we would end up
          // dragging this node and the canvas behind it)
          case EventType.MouseDrag:
            if (e.button == 0 && IsDragged)
            {
              Drag(e.delta);
              e.Use();

              return true;
            }
            break;

          case EventType.KeyDown:

            // Delete this node
            if (e.keyCode == DeleteKey)
            {
              Delete();
            }
            e.Use();
            break;

        }

        return false;
      }
      
      private void ProcessContextMenu()
      {
        var menu = new GenericMenu();

        // Allow the subclass to Add more entries
        OnProcessContextMenu(menu);

        // Finally, allow the removal of node        
        menu.AddItem(new GUIContent("Remove Node"), false, Delete);
        menu.ShowAsContext();
      }

      private bool CheckContentElements(Vector2 mousePos)
      {
        foreach(var element in Content)
        {
          if (element.Position.Contains(mousePos))
          {
            if (element.OnClick != null)
            {
              Trace.Script("Clicked on " + element.Title);
              element.OnClick();
              return true;
            }

          }
        }
        return false;
      }

      //------------------------------------------------------------------------/
      // Methods: Content
      //------------------------------------------------------------------------/
      public void AddContent(ContentElement content)
      {
        Content.Add(content);
        UpdateContentSize();
        const float padding = 1.2f;
        this.Rect = new Rect(this.Rect.position, this.ContentSize * padding);
      }

      private void UpdateContentSize()
      {
        var width = 0f;
        var height = 0f;
        foreach (var element in Content)
        {
          if (element.Width > width) width = element.Width;
          height += element.Height;
        }
        ContentSize = new Vector2(width, height); 
      }

      private void DrawContent()
      {
        var currentPosition = Rect.position;
        currentPosition += (Rect.size * 0.1f);
        var width = ContentSize.x;

        foreach (var element in Content)
        {
          var rect = new Rect(currentPosition, new Vector2(width, element.Height));
          element.Draw(rect);

          // Now increment the position for the next element
          currentPosition.y += element.Size.y;
        }
      }


    }
  }

}