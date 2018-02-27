using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

using Stratus.Editors;

namespace Stratus
{
  namespace Editors
  {
    /// <summary>
    /// Handles splitting a given GUI rect into multiple rects that can be
    /// resized and moved by an user
    /// </summary>
    public class GUISplitter
    {
      //----------------------------------------------------------------------/
      // Classes
      //----------------------------------------------------------------------/
      /// <summary>
      /// A split represents a drawable area
      /// </summary>
      public class Split
      {
        //--------------------------------------------------------------------/
        // Properties
        //--------------------------------------------------------------------/
        public Vector2 Position { get; set; }
        public float Scale { get; private set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 ScrollPosition { get; set; }
        public Vector2 Size
        {
          get { return new Vector2(Width, Height); }
          set { Width = value.x; Height = value.y; }
        }
        public Vector2 MinimumSize { get; private set; }
        public float MinimumScale { get { return 0.4f; } }

        //--------------------------------------------------------------------/
        // Fields 
        //--------------------------------------------------------------------/
        public System.Action<Rect> OnDraw;
        private bool IsInitialized { get; set; }

        //--------------------------------------------------------------------/
        // Methods
        //--------------------------------------------------------------------/
        public Split(float scale, Action<Rect> onDraw)
        {
          this.Scale = scale;
          this.OnDraw = onDraw;
        }

        public void Initialize(Vector2 position, OrientationType orientation)
        {
          if (IsInitialized)
            throw new Exception("This split has already been initialized!");

          this.Width = (orientation == OrientationType.Horizontal ? Scale * position.x : position.x);
          this.Height = (orientation == OrientationType.Vertical ? Scale * position.y : position.y);
          this.MinimumSize = new Vector2(Width * MinimumScale, Height * MinimumScale);

          IsInitialized = true;
        }
      }

      // I can do immediate mode if I ask the user to pass in a struct
      // containing the splits; that way they can save them on their side?

      //public class SplitGroup
      //{
      //  private List<Split> Splits;
      //  public void Add(float scale, Action<Rect> onDraw)
      //}

      /// <summary>
      /// A splitter manages the sizes (w/h) of adjacent splits
      /// </summary>

      private class Splitter
      {
        public bool Dragging { get; set; }      
      }

      public enum OrientationType
      {
        Horizontal,
        Vertical
      }

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// The window this splitter belongs to
      /// </summary>
      private EditorWindow Window { get; set; }

      /// <summary>
      /// The initial position of the editor window (used for keeping
      /// scale relative as the window size changes.
      /// </summary>
      private Vector2 InitialWindowSize { get; set; }

      /// <summary>
      /// The initial position of the splitter area within the window
      /// (before being scaled)
      /// </summary>
      private Rect InitialPosition { get; set; }

      /// <summary>
      /// Keeps the scale of the splits a constant relative by comparing
      /// the initial window scale with the current one
      /// </summary>
      private Vector2 SizeModifier
      {
        get
        {
          float width = Window.position.width / InitialWindowSize.x;
          float height = Window.position.height / InitialWindowSize.y;
          return new Vector2(width, height);
        }
      }

      /// <summary>
      /// How much of the splitter's total rect has been filled
      /// by all the splits In the initialization step, after
      /// all splits have been added, this is asserted to be 100% (1f)
      /// </summary>
      private float FillPercentage { get; set; }

      /// <summary>
      /// Whether the Spliter has been initialized. It does the initialization
      /// procedure on the very first update (rather than construction) for
      /// reasons
      /// </summary>
      private bool IsInitialized { get; set; }

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      // Lists, ho!
      private OrientationType Orientation;
      private List<Split> Splits = new List<Split>();
      private List<Splitter> Splitters = new List<Splitter>();
      private static float SplitterWidth = 5f;

      // Functions
      private System.Action<GUILayoutOption[]> BeginControlGroupFunction;
      private System.Action EndControlGroupFunction;
      private Func<float, GUILayoutOption> DefaultScaleControlFunction, MinScaleControlFunction, MaxScaleControlFunction;
      //private Func<bool, GUILayoutOption> ExpandControlFunction;
      MouseCursor CursorType;

      //----------------------------------------------------------------------/
      // Methods: Constructor
      //----------------------------------------------------------------------/
      public GUISplitter(EditorWindow window, OrientationType type)
      {
        this.Window = window;
        this.Orientation = type;

        switch (this.Orientation)
        {
          case OrientationType.Horizontal:
            BeginControlGroupFunction = GUILayout.BeginHorizontal;
            EndControlGroupFunction = GUILayout.EndHorizontal;
            DefaultScaleControlFunction = GUILayout.Width;
            MinScaleControlFunction = GUILayout.MinWidth;
            MaxScaleControlFunction = GUILayout.MaxWidth;
            //ExpandControlFunction = GUILayout.ExpandHeight;
            CursorType = MouseCursor.ResizeHorizontal;
            break;

          case OrientationType.Vertical:
            BeginControlGroupFunction = GUILayout.BeginVertical;
            EndControlGroupFunction = GUILayout.EndVertical;
            DefaultScaleControlFunction = GUILayout.Height;
            MinScaleControlFunction = GUILayout.MinHeight;
            MaxScaleControlFunction = GUILayout.MaxHeight;
            //ExpandControlFunction = GUILayout.ExpandWidth;
            CursorType = MouseCursor.ResizeVertical;
            break;
        }

      }

      private void Initialize(Rect position)
      {
        InitialWindowSize = Window.position.size;
        InitialPosition = position;

        //Trace.Script("Splitter position = " + position);
        // If the percentages alloted to all splits is not equal to 1 (100%),
        // throw an error!
        if (FillPercentage != 1f)
        {
          Window.Close();
          throw new Exception("The scale percentages for all splits must all add up to 1");
        }

        // Set the intiial size of all splits
        foreach (var split in Splits)
          split.Initialize(position.size, this.Orientation);

        // Add a splitter for every 2 splits
        var numSplitters = Splits.Count - 1;
        for (var i = 0; i < numSplitters; ++i)
        {
          // Use the initial position of the second split to start the splitter
          var secondSplit = Splits[i + 1];
          var initialPosition = secondSplit.Size;
          Splitters.Add(new Splitter());

          //Trace.Script("Splitter at " + initialPosition);
        }
        IsInitialized = true;

      }
      //----------------------------------------------------------------------/
      // Methods: ...
      //----------------------------------------------------------------------/
      /// <summary>
      /// Adds a split, given an initial possition and a function that will be
      /// called to draw its elements inside it
      /// </summary>
      /// <param name="initialPosition"></param>
      /// <param name="scale">The initial scale of this split, a value between 0 and 1</param>
      /// <param name="onDraw"></param>
      public void Add(float scale, System.Action<Rect> onDraw)
      {
        // Keep track of the percentages allotted to all splits
        FillPercentage += scale;

        var split = new Split(scale, onDraw);               
        Splits.Add(split); 

      }

      //----------------------------------------------------------------------/
      // Methods: GUI
      //----------------------------------------------------------------------/
      public void Draw(Rect position)
      {
        if (!IsInitialized)
          Initialize(position);

        //var info = "Window Position = " + Window.position + ", Splitter Position = " + position;
        //Trace.Script(info);

        // Grab the current event: we will check if the user
        // wants to drag one of the splits
        var currentEvent = UnityEngine.Event.current;
        var currentMousePosition = currentEvent.mousePosition;

        // Record the current positions for each split
        var currentPos = new Vector2();

        GUILayout.BeginArea(position);
        BeginControlGroupFunction(null);

        // For every splitter, draw the splits to its sides
        for (var i = 0; i < Splitters.Count; ++i)
        {
          var currentSplitter = Splitters[i];
          var currentSplit = Splits[i];
          var nextSplit = Splits[i + 1];

          // Draw the current split
          DrawSplit(currentSplit, currentPos);

          // Update the position for the next split
          if (Orientation == OrientationType.Horizontal)
            currentPos.x += currentSplit.Width;
          else if (Orientation == OrientationType.Vertical)
            currentPos.y += currentSplit.Height;

          // Draw the splitter
          //var splitterRect = new Rect(currentPos, CalculateSplitterSize(position));           
          var splitterRect = DrawSplitter(currentSplitter, currentPos, position);
          splitterRect.y += position.y;
          //EditorGUIUtility.AddCursorRect(splitterRect, CursorType);

          // Now handle events for this splitter
          if (currentEvent != null && currentEvent.type != EventType.Used)
          {
            switch (currentEvent.type)
            {
              case EventType.MouseDown:
                //Trace.Script("Clicked on " + currentMousePosition);
                if (splitterRect.Contains(currentMousePosition))
                {
                  currentSplitter.Dragging = true;
                  currentEvent.Use();
                }
                break;

              case EventType.MouseDrag:
                if (currentSplitter.Dragging)
                {
                  // If the size of the splits was modified (by the splitter)
                  // we will repaint the window next frame
                  if (ResizeSplitter(currentSplit, nextSplit, currentEvent.delta))
                    Window.Repaint();
                }
                break;

              case EventType.MouseUp:
                currentSplitter.Dragging = false;
                break;
            }
          }
        }

        // At the end, draw the last split
        var lastSplit = Splits[Splits.Count - 1];
        DrawSplit(lastSplit, currentPos);        

        EndControlGroupFunction();
        GUILayout.EndArea();
      }

      private Vector2 CalculateSplitterSize(Vector2 position)
      {
        var width = (this.Orientation == OrientationType.Horizontal ? SplitterWidth : position.x);
        var height = (this.Orientation == OrientationType.Vertical ? SplitterWidth : position.y);
        return new Vector2(width, height);
      }

      private void DrawSplit(Split split, Vector2 position) 
      {
        var scaledPosition = new Rect(position.x * SizeModifier.x,
                                      position.y * SizeModifier.y,
                                      split.Width * SizeModifier.x,
                                      split.Height * SizeModifier.y);

        split.Position = position;
        split.ScrollPosition = GUILayout.BeginScrollView(split.ScrollPosition,
                                   ScaleControl(DefaultScaleControlFunction, scaledPosition.size),
                                   ScaleControl(MaxScaleControlFunction, scaledPosition.size),
                                   ScaleControl(MinScaleControlFunction, scaledPosition.size)
                                   );
        split.OnDraw(scaledPosition);
        GUILayout.EndScrollView();
      }
      
      private Rect DrawSplitter(Splitter splitter, Vector2 nextSplitPosition, Rect position)
      {
        var x = (this.Orientation == OrientationType.Horizontal ? nextSplitPosition.x - (SplitterWidth / 2f) : nextSplitPosition.x);
        var y = (this.Orientation == OrientationType.Vertical ? nextSplitPosition.y - (SplitterWidth / 2f): nextSplitPosition.y);
        var width = (this.Orientation == OrientationType.Horizontal ? SplitterWidth : position.width);
        var height = (this.Orientation == OrientationType.Vertical ? SplitterWidth : position.height);
        x *= SizeModifier.x;
        y *= SizeModifier.y;

        if (Orientation == OrientationType.Horizontal)          
          GUILayout.Box(GUIContent.none, StratusGUIStyles.editorLine, GUILayout.Width(1f), GUILayout.Height(height));
        else if (Orientation == OrientationType.Vertical)
          GUILayout.Box(GUIContent.none, StratusGUIStyles.editorLine, GUILayout.Width(width), GUILayout.Height(1f));

        //GUILayout.Box(GUIContent.none, Styles.EditorLine, GUILayout.Width(width), GUILayout.Height(1f));
        //var splitterRect = GUILayoutUtility.GetLastRect();
        var splitterRect = new Rect(x, y, width, height);

        EditorGUIUtility.AddCursorRect(splitterRect, CursorType);
        return splitterRect;
      }

      GUILayoutOption ScaleControl(Func<float, GUILayoutOption> func, Vector2 size)
      {
        if (this.Orientation == OrientationType.Horizontal)
          return func(size.x);
        else
          return func(size.y);
      }

      private bool ResizeSplitter(Split currentSplit, Split nextSplit, Vector2 mouseDelta)
      { 
        if (this.Orientation == OrientationType.Horizontal)
        {
          mouseDelta.y = 0f;
        }
        else if (this.Orientation == OrientationType.Vertical)
        {
          mouseDelta.x = 0f;
        }
        
        // Maintain a minimum size for this split and the next
        var currentSplitModifiedSize = currentSplit.Size + mouseDelta;
        if (currentSplitModifiedSize.x < currentSplit.MinimumSize.x ||
            currentSplitModifiedSize.y < currentSplit.MinimumSize.y)
          return false;

        var nextSplitModifiedSize = nextSplit.Size - mouseDelta;
        if (nextSplitModifiedSize.x < nextSplit.MinimumSize.x ||
            nextSplitModifiedSize.y < nextSplit.MinimumSize.y)
          return false;
        
        // Modify the width of this split, and the next one
        currentSplit.Size = currentSplitModifiedSize;
        nextSplit.Size = nextSplitModifiedSize;

        return true;
      }

      /// <summary>
      /// How the split allocation will be determined
      /// </summary>
      public enum FillType
      {
        Relative,
        Absolute
      }

      private class SplitterFrame
      {
        public SplitterFrame(GUISplitter splitter, Rect position)
        {
          this.Splitter = splitter;
          this.Position = position;
        }

        public GUISplitter Splitter { get; private set; }
        public Rect Position { get; private set; }
      }

      private static Stack<SplitterFrame> FrameStack = new Stack<SplitterFrame>();
      private static SplitterFrame CurrentFrame { get { return FrameStack.Peek(); } }
      //private static Rect CurrentPosition { get; set; }

      public static void BeginHorizontalSplitGroup(EditorWindow window, Rect position)
      {
        FrameStack.Push(new SplitterFrame(new GUISplitter(window, OrientationType.Horizontal), position));
      }

      public static float AddSplit(float scale, System.Action<Rect> onDraw)
      {
        throw new NotImplementedException();
        //CurrentFrame.Splitter.Add(scale, onDraw);
      }

      public static void EndHorizontalSplitGroup()
      {
        CurrentFrame.Splitter.Draw(CurrentFrame.Position);
        FrameStack.Pop();      
      }

      

    }
  }

}

//public class GUISplitterExampleTwo : EditorWindow
//{
//  GUISplitter Splitter;

//  // Set the splitter position within the window
//  Rect SplitterPosition
//  {
//    get
//    {
//      var splitterX = 0f; // this.position.width / 2f; ;
//      var splitterY = this.position.height / 2f;
//      var splitterWidth = this.position.width; // / 2f;
//      var splitterHeight = this.position.height / 2f;
//      return new Rect(splitterX, splitterY, splitterWidth, splitterHeight);
//    }
//  }

//  [MenuItem("GUI/GUISplitter Two")]
//  static void Init()
//  {
//    GUISplitterExampleTwo window = GetWindow<GUISplitterExampleTwo>();
//  }

//  private void OnEnable()
//  {
//    this.position = new Rect(Screen.currentResolution.width / 2,
//                                  Screen.currentResolution.height / 2,
//                                  600,
//                                  400);


//    Splitter = new GUISplitter(this, GUISplitter.OrientationType.Horizontal);
//    Splitter.Add(0.25f, Split);
//    Splitter.Add(0.25f, Split);
//    Splitter.Add(0.5f, Split);
//  }

//  private void OnGUI()
//  {
//    Splitter.Draw(SplitterPosition);
//  }

//  void Split(Rect position)
//  {
//    GUILayout.Box("Rect = " + position,
//        GUI.skin.box,
//        GUILayout.ExpandWidth(true),
//        GUILayout.ExpandHeight(true));
//  }

//}