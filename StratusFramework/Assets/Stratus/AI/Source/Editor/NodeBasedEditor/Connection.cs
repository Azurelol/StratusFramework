using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  namespace Editor
  {
    /// <summary>
    /// A connection has two connection points an an action to remove it.
    /// </summary>
    public class Connection
    {
      public ConnectionPoint InPoint;
      public ConnectionPoint OutPoint;
      public Action<Connection> OnClickRemove;
      private float Width = 2f;

      public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> onClickRemove)
      {
        this.InPoint = inPoint;
        this.OutPoint = outPoint;
        this.OnClickRemove = onClickRemove;
      }

      public void Draw()
      {
        Handles.DrawBezier(
          this.InPoint.Rect.center,
          this.OutPoint.Rect.center,
          this.InPoint.Rect.center + Vector2.left * 50f,
          this.OutPoint.Rect.center - Vector2.left * 50f,
          Color.white,
          null,
          Width);


        if (Handles.Button((InPoint.Rect.center + OutPoint.Rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
          if (OnClickRemove != null)
            OnClickRemove(this);
        }

      }


    } 
  }

}