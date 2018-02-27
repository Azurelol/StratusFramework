using UnityEngine;
using System;

namespace Stratus
{
  namespace Editors
  {
    /// <summary>
    /// Our node editor has nodes, it should also be able to connect them. In order
    /// to do this we need two connection points (in and out) on a node and a connection
    /// between them. A connection point has a rectangle (so that it can be drawn),
    /// a type (in or out), a style and references (its parent node).
    /// This class is rather simple: it draws a button at a specific position and performs
    /// an action when it is clicked.
    /// </summary>
    public class ConnectionPoint
    {
      public enum ConnectionType { In, Out }
      public enum OrientationType { Horizontal, Vertical }

      public struct ConnectionSettings
      {
        public ConnectionType Type;
        public OrientationType Orientation;
        public GUIStyle Style;
        public Action<ConnectionPoint> OnClick;
        public ConnectionSettings(ConnectionType type, OrientationType orientation, GUIStyle style, Action<ConnectionPoint> onClick)
        {
          Type = type;
          Orientation = orientation;
          Style = style;
          OnClick = onClick;
        }
      }

      public bool Enabled { get; set; }
      public Rect Rect;
      public Node Node;
      public ConnectionSettings Settings;
            
      private const float OffsetY = 0.25f;

      public ConnectionPoint(Node node, ConnectionSettings settings)
      {
        this.Node = node;
        this.Settings = settings;
        this.Rect = new Rect(0, 0, Node.Size.x * 0.8f, Node.Size.y * 0.25f);
      }

      public void Draw(float scale)
      {
        this.Rect = CalculatePosition(Settings.Type, Settings.Orientation, scale);        
        if (GUI.Button(Rect, GUIContent.none, Settings.Style))
        {
          if (this.Settings.OnClick != null)
          {
            this.Settings.OnClick(this);
          }
        }
      }

      const float FillLength = 0.9f;
      const float FillHeight = 0.15f;

      Rect CalculatePosition(ConnectionType type, OrientationType orientation, float scale)
      {
        var rect = new Rect();        

        if (orientation == OrientationType.Vertical)
        {
          rect.size = new Vector2(Node.Size.x * FillLength, Node.Size.y * FillHeight);          
          rect.x = Node.Rect.x + (Node.Rect.width * 0.5f) - Rect.width * 0.5f;
          switch (this.Settings.Type)
          {
            // Top
            case ConnectionType.In:
              rect.y = Node.Rect.y - Rect.height * 0.5f;
              break;
            // Bottom
            case ConnectionType.Out:
              rect.y = Node.Rect.yMax - (Rect.height * 0.5f); // - Rect.height * 0.25f;
              break;
          }
        }
        else if (orientation == OrientationType.Horizontal)
        {
          rect.size = new Vector2(Node.Size.x * FillHeight, Node.Size.y * FillLength);
          rect.y = Node.Rect.y + (Node.Rect.height * 0.5f) - Rect.height * 0.5f;
          switch (this.Settings.Type)
          {
            // Left
            case ConnectionType.In:
              rect.x = Node.Rect.x - Rect.width * 0.75f;
              break;
            // Right
            case ConnectionType.Out:              
              rect.x = Node.Rect.xMax - Rect.width + (Rect.width * 0.75f); 
              break;
          }
        }

        return rect;
      }


    }


  }
}