using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Stratus
{
  namespace Editor
  {
    /// <summary>
    /// An utility class that can draw multiple columns inside a given rect.
    /// You add columns individually with the 'Add' function, validate when you are done,
    /// then call 'Draw' inside an EditorWindow's OnGUI
    /// </summary>
    public class ColumnDrawer
    {
      public class Column
      {
        public Rect Rect { get; set; }
        [Range(0f, 1f)]
        public float Width;
        public Color Color;
        public System.Action<Rect> OnDraw;

        public Column(float width, System.Action<Rect> onDraw, Color color)
        {
          if (width > 1f) throw new Exception("The width of a column must be normalized!");
          this.Width = width;
          this.OnDraw = onDraw;
        }
      }

      private List<Column> All = new List<Column>();
      private float TotalWidth { get; set; }

      public ColumnDrawer()
      {
        TotalWidth = 0f;
      }

      public void Add(float width, System.Action<Rect> onDraw, Color color)
      {
        TotalWidth += width;
        All.Add(new Column(width, onDraw, color));
      }

      public void Validate()
      {
        if (TotalWidth > 1f)
          throw new Exception("The total width of all columns must equal to a normalized value of 1! A value of 1 represents 100%, or the total width of all columns combined.");
      }

      /// <summary>
      /// Draws all columns
      /// </summary>
      /// <param name="position"></param>
      /// <param name="menuHeight"></param>
      public void Draw(Rect position, float menuHeight)
      {
        // As we draw columns, we keep incrementing some of these values
        // to pass onto the next column
        var x = 0f;
        var y = menuHeight;
        var height = position.height;

        //var defaultColor = GUI.color;

        foreach (var column in All)
        {
          // Calculate its width
          var width = position.width * column.Width;
          column.Rect = new Rect(x, y, width, height);

          // Now draw the column
          GUILayout.BeginArea(column.Rect);
          //GUI.DrawTexture(column.Rect, texture, ScaleMode.StretchToFill);
          GUILayout.BeginVertical();
          column.OnDraw(column.Rect);
          GUILayout.EndVertical();
          GUILayout.EndArea();

          // Increment the x position
          x += position.width * column.Width;
        }
      }



    }
  }

}