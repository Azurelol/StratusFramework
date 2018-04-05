using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  public class GridDrawer
  {
    public struct Grid
    {
      public float spacing;
      public float opacity;
      public Color color;

      public Grid(float spacing, float opacity, Color color)
      {
        this.spacing = spacing;
        this.opacity = opacity;
        this.color = color;
      }
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public Grid outerGrid { get; set; } = new Grid(100f, 0.4f, Color.gray);
    public Grid innerGrid { get; set; } = new Grid(10f, 0.2f, Color.gray);
    public Color background { get; set; } = new Color(169f / 255f, 169f / 255f, 169f / 255f, 0.5f);
    public int zoomLevel { get; set; }
    public int maxZoomLevel { get; set; } = 100;
    public int minZoomLevel { get; set; } = 10;
    public Vector2 offset { get; set; }

    /// <summary>
    /// The current zoom level of this editor (10%,100%)
    /// </summary>
    public float zoom { get { return zoomLevel / 100f; } }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public GridDrawer()
    {
      zoomLevel = maxZoomLevel;
    }

    public void Draw(Rect position, Vector2 drag)
    {
      StratusGUIStyles.DrawBackgroundColor(position, background);
      offset += drag * 0.5f;
      DrawGrid(position, innerGrid);
      DrawGrid(position, outerGrid);
    }

    public void DrawGrid(Rect position, Grid grid)
    {
      float spacing = grid.spacing;
      float opacity = grid.opacity;
      Color color = grid.color;

      spacing = spacing * ((float)zoomLevel / 100f);
      int widthDivs = Mathf.CeilToInt(position.width / spacing);
      int heightDivs = Mathf.CeilToInt(position.height / spacing);

      Handles.BeginGUI();
      Handles.color = new Color(color.r, color.g, color.b, opacity);
      {
        var newOffset = new Vector3(offset.x % spacing, offset.y % spacing, 0);
        for (var w = 0; w < widthDivs; ++w)
          Handles.DrawLine(new Vector3(spacing * w, -spacing, 0) + newOffset,
                           new Vector3(spacing * w, position.height, 0f) + newOffset);
        for (var h = 0; h < heightDivs; ++h)
          Handles.DrawLine(new Vector3(-spacing, spacing * h, 0) + newOffset,
                           new Vector3(position.width, spacing * h, 0f) + newOffset);
      }
      Handles.color = Color.white;
      Handles.EndGUI();
    }

    //public void ProcessEvent()
    //{
    //  var e = UnityEngine.Event.current;
    //  switch (et.type)
    //  {
    //    case EventType.ScrollWheel:
    //      Zoom(e.delta);
    //      e.Use();
    //      break;
    //  }
    //}

    /// <summary>
    /// Handles zoom levels for the editor. This affects how the nodes
    /// and the grid is drawn
    /// </summary>
    /// <param name="mouseWheelDelta"></param>
    public void Zoom(Vector2 mouseWheelDelta)
    {
      bool isZoomingIn = mouseWheelDelta.y < 0 ? true : false;

      if (isZoomingIn && zoomLevel < maxZoomLevel)
      {
        zoomLevel += 10;
      }
      else if (!isZoomingIn && zoomLevel > minZoomLevel)
      {
        zoomLevel -= 10;
      }

    }



  }




}


