/******************************************************************************/
/*!
@file   Overlay_Window.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus.Utilities;
using System;

namespace Stratus
{
  public partial class Overlay
  {
    /// <summary>
    /// The base class for all windows
    /// </summary>
    public abstract class AbstractWindow : Element
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// Whether this window is currently active
      /// </summary>
      public abstract bool Active { get; }

      /// <summary>
      /// The anchored position of the overlay
      /// </summary>
      public Anchor Position;

      /// <summary>
      /// The dimensions of this window as ratios between 0 and 1
      /// </summary>
      [Range(0f, 1f)]
      public Vector2 Dimensions;

      /// <summary>
      /// The background texture
      /// </summary>
      public Texture2D Background;

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/        
      /// <summary>
      /// The style of this window's background
      /// </summary>
      protected GUIStyle BackgroundStyle = new GUIStyle();

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnDrawWindow();

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="title">The title of this window</param>
      /// <param name="relativeDimensions">The relative dimensions of this window</param>
      /// <param name="color"></param>
      /// <param name="position"></param>
      public AbstractWindow(string title, Vector2 relativeDimensions, Color color, Anchor position = Anchor.TopLeft) : base(title)
      {
        Position = position;
        Dimensions = FindRelativeDimensions(relativeDimensions.x, relativeDimensions.y);
      }

      /// <summary>
      /// Draws this window
      /// </summary>
      protected override void OnDraw()
      {
        if (!Active)
          return;

        GUILayout.BeginArea(FindAnchoredPosition(Position, Dimensions.x, Dimensions.y), Name, GUI.skin.window);
        OnDrawWindow();
        GUILayout.EndArea();
      }

    }

    /// <summary>
    /// An UI element that contains other elements, which are parented to it
    /// </summary>
    public class Window : AbstractWindow
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// This window is active whenever there are elements on it
      /// </summary>
      public override bool Active { get { return Elements.Count != 0; } }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// All child elements
      /// </summary>
      private List<Element> Elements = new List<Element>();

      /// <summary>
      /// The position of the scrolling bar
      /// </summary>
      protected Vector2 ScrollPosition;

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public Window(string name, Vector2 relativeDimensions, Color color, Anchor position = Anchor.TopLeft)
        : base(name, relativeDimensions, color, position)
      {
      }
      
      /// <summary>
      /// Adds an element to be drawn inside the window
      /// </summary>
      /// <param name="element"></param>
      public void Add(Element element)
      {        
        Elements.Add(element);
      }

      /// <summary>
      /// Removes an element from the window
      /// </summary>
      /// <param name="element"></param>
      public void Remove(Element element)
      {
        Elements.Remove(element);
      }

      /// <summary>
      /// Draws all active elements in the window
      /// </summary>
      protected override void OnDrawWindow()
      {
        //GUILayout.BeginVertical();
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);//, GUILayout.Width(Dimensions.x), GUILayout.Height(Dimensions.y));
        DrawElements();
        GUILayout.EndScrollView();
        //GUILayout.EndVertical();
      }

      /// <summary>
      /// Draws the elements of the window
      /// </summary>
      protected virtual void DrawElements()
      {
        foreach (var element in Elements)
          element.Draw();
      }

    }

  }
}