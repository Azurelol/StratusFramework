using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Stratus
{
  namespace Editors
  {
    /// <summary>
    /// A generic node-based editor
    /// </summary>
    public abstract class NodeBasedEditor<DerivedNode>
      where DerivedNode : Node, new()
    {
      //------------------------------------------------------------------------/
      // Classes
      //------------------------------------------------------------------------/
      public class Grid
      {
        public class Settings
        {
          public float Spacing;
          public float Opacity;
          public Color Color;

          public Settings(float spacing, float opacity, Color color)
          {
            this.Spacing = spacing;
            this.Opacity = opacity;
            this.Color = color;
          }
        }

      }

      public class Settings
      {
        public Grid.Settings OuterGrid { get; set; }
        public Grid.Settings InnerGrid { get; set; }
        public Color Background { get; set; }
        
      }

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The currently selected node
      /// </summary>
      public DerivedNode SelectedNode
      {
        get
        {
          if (SelectedNodes.Count > 1 || SelectedNodes.Count == 0)
            return null;
          return SelectedNodes[0];
        }
      }

      /// <summary>
      /// The current zoom level of this editor (10%,100%)
      /// </summary>
      public float Zoom { get { return CurrentZoomLevel / 100f; } }

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The window this editor belongs to
      /// </summary>
      protected EditorWindow Window { get; private set; }

      /// <summary>
      /// The style settings for this editor
      /// </summary>
      private Settings settings { get; set; }
      protected ConnectionPoint SelectedInPoint { private set; get; }
      protected ConnectionPoint SelectedOutPoint { private set; get; }
      protected bool IsDragging { private set; get; }
      protected bool IsMultiSelecting { private set; get; }
      protected bool HasMultipleSelected { get { return SelectedNodes.Count > 1; } }
      private bool HasSetStyles { get; set; }
      protected Vector2 ViewportOffset { get; private set; }
      protected Vector2 ViewportCenter { get; private set; }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/        
      // Drag nodes
      private Vector2 Drag;
      private Vector2 GridOffset;

      // Select multiple nodes
      private List<DerivedNode> SelectedNodes = new List<DerivedNode>();
      private Rect SelectionBoxArea;
      private Vector2 StartingMousePosition;

      private Vector2 NodeSize = new Vector2(250, 100);

      private int CurrentZoomLevel { get; set; }
      private const int MaximumZoomLevel = 100, MinimumZoomLevel = 10;

      private GUIStyle DefaultNodeStyle;
      private GUIStyle SelectedNodeStyle = new GUIStyle();
      private GUIStyle InPointStyle;
      private GUIStyle OutPointStyle;      

      private List<DerivedNode> Nodes = new List<DerivedNode>();
      private List<Connection> Connections = new List<Connection>();

      // Keybindings
      private KeyCode DeleteKey = KeyCode.Delete;
      private KeyCode AutoArrangeKey = KeyCode.F;
      private KeyCode CancelKey = KeyCode.Escape;

      //------------------------------------------------------------------------/
      // Configuration
      //------------------------------------------------------------------------/
      /// <summary>
      /// Configures the node-based editor with initial values
      /// </summary>
      public void Initialize(EditorWindow window, Settings settings)
      {
        this.Window = window;
        this.settings = settings;
        this.LoadStyles();
        this.CurrentZoomLevel = MaximumZoomLevel;
      }

      //------------------------------------------------------------------------/
      // Interface
      //------------------------------------------------------------------------/
      protected abstract void OnContextMenu(GenericMenu menu, Vector2 mousePos);
      protected abstract void OnMultiSelectContextMenu(GenericMenu menu, Vector2 mousePos);

      //------------------------------------------------------------------------/
      // GUI: Draw
      //------------------------------------------------------------------------/
      public void Draw(Rect position)
      {
        // Calculate the current offset based on the position
        ViewportOffset = new Vector2(position.width / 2f, position.height / 2f);
        //ViewportCenter = new Vector2(0f, 0f);

        //GUILayout.BeginArea(position);
        Styles.DrawBackgroundColor(position, settings.Background);
        DrawGrid(position, settings.InnerGrid.Spacing, settings.InnerGrid.Opacity, settings.InnerGrid.Color);
        DrawGrid(position, settings.OuterGrid.Spacing, settings.OuterGrid.Opacity, settings.OuterGrid.Color);
        DrawNodes();
        DrawConnections();
        DrawMultiSelection();

        // Now handle GUI events
        var currentEvent = UnityEngine.Event.current;

        DrawConnectionLine(currentEvent);
        ProcessNodeEvents(currentEvent);
        ProcessEvents(currentEvent);

        if (GUI.changed)
        {
          Window.Repaint();
          //Trace.Script("Repainting");
        }

        GUILayout.Label("Viewport Center: " + ViewportCenter + " Zoom: " + CurrentZoomLevel + "%" + " Mouse: " + currentEvent.mousePosition);
       //GUILayout.EndArea();
      }

      private void DrawNodes()
      {
        foreach (var node in Nodes)
        {
          node.Draw(Zoom);
        }
      }

      private void DrawConnections()
      {
        foreach (var connection in Connections)
        {
          connection.Draw();
        }
      }

      private void DrawConnectionLine(UnityEngine.Event e)
      {
        ConnectionPoint startingPoint = null;
        int sign = 1;

        if (SelectedInPoint != null && SelectedOutPoint == null)
        {
          startingPoint = SelectedInPoint;
        }
        else if (SelectedOutPoint != null && SelectedInPoint == null)
        {
          startingPoint = SelectedOutPoint;
          sign = -1;
        }

        if (startingPoint != null)
        {
          Handles.DrawBezier(
            startingPoint.Rect.center,
            e.mousePosition,
            startingPoint.Rect.center + (Vector2.left * sign) * 50f,
            e.mousePosition - (Vector2.left * sign) * 50f,
            Color.white,
            null,
            2f);

          GUI.changed = true;
        }

      }

      private void DrawGrid(Rect position, float spacing, float opacity, Color color)
      {
        spacing = spacing * ((float)CurrentZoomLevel / 100f);
        int widthDivs = Mathf.CeilToInt(position.width / spacing);
        int heightDivs = Mathf.CeilToInt(position.height / spacing);
        
        Handles.BeginGUI();
        Handles.color = new Color(color.r, color.g, color.b, opacity);
        {
          GridOffset += Drag * 0.5f;
          var newOffset = new Vector3(GridOffset.x % spacing, GridOffset.y % spacing, 0);
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

      private void DrawMultiSelection()
      {
        if (!IsMultiSelecting)
          return;
        
        GUI.Box(SelectionBoxArea, GUIContent.none, EditorStyles.helpBox);
        GUI.changed = true;
        //Window.Repaint();
      }

      //------------------------------------------------------------------------/
      // GUI: Event Processing
      //------------------------------------------------------------------------/
      private void ProcessEvents(UnityEngine.Event e)
      {
        Drag = Vector2.zero;

        switch (e.type)
        {
          case EventType.MouseDown:

            // -- Left click --
            if (e.button == 0)
            {
              ClearConnectionSelection();
              //Trace.Script("Multiselecting!");
              IsMultiSelecting = true;
              StartingMousePosition = e.mousePosition;
              //e.Use();
            }

            // -- Right cick --
            if (e.button == 1)
            {
              ProcessContextMenu(e.mousePosition);
            }
            break;

          case EventType.MouseUp:

            // -- Left click --
            if (e.button == 0)
            {
              //Trace.Script("No longer multiselecting!");
              IsMultiSelecting = false;
              SelectionBoxArea = new Rect();
            }
            break;

          case EventType.MouseDrag:

            // Left click: Select multiple
            if (e.button == 0)
            {
              if (IsMultiSelecting)
              {
                OnMultiSelect(e);
              }
            }

            // Middle click: Pan the canvas
            if (e.button == 2)
            {
              OnPan(e.delta);
            }
            break;

          case EventType.ScrollWheel:
            OnZoom(e.delta);
            e.Use();
            break;

          case EventType.KeyDown:
            if (e.keyCode == CancelKey)
            {
              DeselectAll();
            }
            else if (e.keyCode == AutoArrangeKey)
            {
              AutoArrange();
            }
            break;
        }
      }

      private void ProcessNodeEvents(UnityEngine.Event e)
      {
        if (HasMultipleSelected && !IsMultiSelecting)
        {
          ProcessMultiSelectionEvents(e);
          return;
        }

        // We traverse the node list backwards, because the last node is drawn at the top.
        // We should process its events first.
        for (var i = Nodes.Count - 1; i >= 0; i--)
        {
          var node = Nodes[i];
          bool guiCHanged = node.ProcessEvents(e);
          if (guiCHanged)
            GUI.changed = true;          
        }
      }

      bool RecentlyDragged;



      private void ProcessMultiSelectionEvents(UnityEngine.Event e)
      {
        //Trace.Script("Processing for multiselect");

        switch(e.type)
        {
          case EventType.MouseDown:

            // -- Left click --
            // If we clicked on top of a node, we can start dragging
            // Otherwise, we need to release multiselect and send the event down
            // to normal processing
            if (e.button == 0)
            {
              if (HasSelectedNode(e.mousePosition))
                IsDragging = true;
              else
              {
                OnReleaseMultiSelect();
                ProcessEvents(e);
                return;
              }
            }

            // -- Right click --
            // Display multi select context menu
            else if (e.button == 1)
            {
              ProcessMultiSelectContextMenu(e.mousePosition);
            }

            GUI.changed = true;
            e.Use();
            break;

          case EventType.MouseUp:

            // -- Left click --
            // If there was nothing dragged, release all nodes
            if (e.button == 0)
            {
              if (!RecentlyDragged)
                OnReleaseMultiSelect();

              IsDragging = false;
              RecentlyDragged = false;
              GUI.changed = true;
            }
            break;

          case EventType.MouseDrag:

            // -- Left click --
            // Start dragging nodes if we previously pressed left click
            if (e.button == 0 && IsDragging)
            {
              foreach(var node in SelectedNodes)
                node.Drag(e.delta);
              e.Use();
              RecentlyDragged = true; 
            }
            break;

          case EventType.KeyDown:
            if (e.keyCode == CancelKey)
            {
              DeselectAll();              
            }
            else if (e.keyCode == DeleteKey)
            {
              DeleteAll();
            }            
            e.Use();
            break;
        }
      }
      
      private void ProcessContextMenu(Vector2 mousePos)
      {
        var genericMenu = new GenericMenu();
        //genericMenu.AddItem(new GUIContent("Add Node"), false, () => CreateNode(mousePos));
        OnContextMenu(genericMenu, mousePos);
        genericMenu.ShowAsContext();
      }

      private void ProcessMultiSelectContextMenu(Vector2 mousePos)
      {
        var genericMenu = new GenericMenu();
        OnMultiSelectContextMenu(genericMenu, mousePos);
        genericMenu.AddItem(new GUIContent("Remove All"), false, DeleteAll);
        genericMenu.ShowAsContext();
      }

      //------------------------------------------------------------------------/
      // GUI: Callbacks
      //------------------------------------------------------------------------/
      /// <summary>
      /// Pans the viewport, moving all nodes by the mouse delta
      /// </summary>
      /// <param name="delta"></param>
      private void OnPan(Vector2 delta)
      {
        ViewportCenter += delta;
        Drag = delta;
        foreach (var node in Nodes)
          node.Drag(delta);
        GUI.changed = true;
      }

      /// <summary>
      /// Allows selection of multiple nodes
      /// </summary>
      private void OnMultiSelect(UnityEngine.Event e)
      /// <param name="position"></param>
      {
        SelectionBoxArea = new Rect(StartingMousePosition.x,
          StartingMousePosition.y,
          e.mousePosition.x - StartingMousePosition.x,
          e.mousePosition.y - StartingMousePosition.y);

        // Select any nodes inside the bounding box
        SelectedNodes.Clear();
        foreach(var node in Nodes)
        {
          if (SelectionBoxArea.Contains(new Vector2(node.Rect.x, node.Rect.y), true))
          {
            node.Select();
            SelectedNodes.Add(node);
          }
          else
          {
            node.Deselect();
          }
        }
      }

      /// <summary>
      /// Handles zoom levels for the editor. This affects how the nodes
      /// and the grid is drawn
      /// </summary>
      /// <param name="mouseWheelDelta"></param>
      void OnZoom(Vector2 mouseWheelDelta)
      {
        bool isZoomingIn = mouseWheelDelta.y < 0 ? true : false;

        if (isZoomingIn && CurrentZoomLevel < MaximumZoomLevel)
        {
          CurrentZoomLevel += 10;
        }
        else if (!isZoomingIn && CurrentZoomLevel > MinimumZoomLevel)
        {          
          CurrentZoomLevel -= 10;
        }
        
      }

      void AutoArrange()
      {
        Trace.Script("Resetting the viewport back to (0,0)");
        ViewportCenter = Vector2.zero;
      }

      bool HasSelectedNode(Vector2 mousePos)
      {
        foreach (var node in SelectedNodes)
        {
          if (node.Rect.Contains(mousePos))
          {
            return true;
          }
        }

        return false;
      }

      void OnReleaseMultiSelect()
      {
        //Trace.Script("Releasing multi-select");
        // If there's a node under the mouse when we let go, select it
        foreach (var node in SelectedNodes)
        {
          if (SelectionBoxArea.Contains(new Vector2(node.Rect.x, node.Rect.y), true))
          {
            node.Select();
            break;
          }
          else
          {
            node.Deselect();
          }
        }
        SelectedNodes.Clear();
      }

      /// <summary>
      /// When creating a new node, does the base class boilerplate then calls upon the subclass
      /// to modify it as it needs
      /// </summary>
      /// <param name="mousePos"></param>
      protected void AddNode(Vector2 mousePos, System.Action<DerivedNode> onAddNode)
      {
        // Create the node, initialize it
        var newNode = new DerivedNode();
        newNode.Initialize(new Rect(mousePos, this.NodeSize), this.DefaultNodeStyle, 
                           this.SelectedNodeStyle, OnNodeEvent);
        // Create its endpoints
        var orientation = ConnectionPoint.OrientationType.Vertical;
        var inPointSettings = new ConnectionPoint.ConnectionSettings(ConnectionPoint.ConnectionType.In,
         orientation, InPointStyle, OnClickInPoint);
        var outPointSettings = new ConnectionPoint.ConnectionSettings(ConnectionPoint.ConnectionType.Out,
          orientation, OutPointStyle, OnClickOutPoint);
        newNode.AddConnection(inPointSettings);
        newNode.AddConnection(outPointSettings);

        // Have the in-point enabled by default
        newNode.InPoint.Enabled = true;
                
        // Add it to our current list of nodes
        Nodes.Add(newNode);

        // Now pass the newly-created node to the callback function
        onAddNode(newNode);        
      }
      
      //------------------------------------------------------------------------/
      // Node: Events
      //------------------------------------------------------------------------/
      /// <summary>
      /// Handles node events
      /// </summary>
      /// <param name="node"></param>
      /// <param name="e"></param>
      void OnNodeEvent(Node node, Node.Event e)
      {
        switch (e)
        {
          case Node.Event.Select:
            OnSelectNode(node);
            break;

          case Node.Event.Deselect:
            OnDeselectNode(node);
            break;

          case Node.Event.Remove:
            OnRemoveNode(node);
            break;
        }
      }

      private void OnSelectNode(Node node)
      {
        SelectedNodes.Clear();
        SelectedNodes.Add(node as DerivedNode);
      }

      private void OnDeselectNode(Node node)
      {
        SelectedNodes.Clear();
      }

      /// <summary>
      /// When an user removes a node, we need not only remove it from the node list
      /// but because it might have connections to other nodes we need to remove those connections
      /// as well
      /// </summary>
      /// <param name="node"></param>
      private void OnRemoveNode(Node node)
      {
        var toRemove = new List<Connection>();

        foreach (var connection in Connections)
        {
          if (connection.InPoint == node.InPoint || connection.OutPoint == node.OutPoint)
            toRemove.Add(connection);
        }
        foreach (var connection in toRemove)
          Connections.Remove(connection);

        Nodes.Remove(node as DerivedNode);
      }

      private void OnClickInPoint(ConnectionPoint point)
      {
        SelectedInPoint = point;
        //Trace.Script("SelectedInPoint = " + SelectedInPoint.Node)
        if (SelectedOutPoint != null)
        {
          if (SelectedOutPoint.Node != SelectedInPoint.Node)
            CreateConnection();
          ClearConnectionSelection();
        }
      }

      private void OnClickOutPoint(ConnectionPoint point)
      {
        SelectedOutPoint = point;
        if (SelectedInPoint != null)
        {
          if (SelectedOutPoint.Node != SelectedInPoint.Node)
            CreateConnection();
          ClearConnectionSelection();
        }
      }


      private void OnClickRemoveConnection(Connection connection)
      {
        Connections.Remove(connection);
      }

      private void DeselectAll()
      {
        Trace.Script("Deselecting all nodes )" + SelectedNodes.Count + ")");
        foreach (var node in SelectedNodes)
          node.Deselect();
        SelectedNodes.Clear();
      }

      private void DeleteAll()
      {
        Trace.Script("Deleting all selected nodes (" + SelectedNodes.Count + ")");        
        foreach (var node in SelectedNodes)
        {
          node.Deselect();
          node.Delete();
        }
        SelectedNodes.Clear();
      }

      //------------------------------------------------------------------------/
      // GUI: Utility
      //------------------------------------------------------------------------/
      private void CreateConnection()
      {
        Connections.Add(new Connection(SelectedInPoint, SelectedOutPoint, OnClickRemoveConnection));
        Trace.Script("Creating connection between " + SelectedInPoint.Node.Name + " and " + SelectedOutPoint.Node.Name);
      }

      private void ClearConnectionSelection()
      {
        SelectedInPoint = null;
        SelectedOutPoint = null;
      }

      private void LoadStyles()
      {
        this.DefaultNodeStyle = GUI.skin.box;

        //const int NodeBorderWidth = 12;
        //
        //this.DefaultNodeStyle = new GUIStyle();
        //this.DefaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        //this.DefaultNodeStyle.border = new RectOffset(NodeBorderWidth, NodeBorderWidth, NodeBorderWidth, NodeBorderWidth);        
        //
        //this.SelectedNodeStyle = new GUIStyle();
        //this.SelectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        //this.SelectedNodeStyle.border = new RectOffset(NodeBorderWidth, NodeBorderWidth, NodeBorderWidth, NodeBorderWidth);

        //this.InPointStyle = GUI.skin.toggle;
        //this.OutPointStyle = GUI.skin.toggle;

        this.InPointStyle = new GUIStyle();
        this.InPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        this.InPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        this.InPointStyle.border = new RectOffset(4, 4, 12, 12);

        this.OutPointStyle = new GUIStyle();
        this.OutPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        this.OutPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        this.OutPointStyle.border = new RectOffset(4, 4, 12, 12);

        //this.NodeSettings = new Node.Settings(this.DefaultNodeStyle, this.SelectedNodeStyle, this.NodeWidth, this.NodeHeight, this.OnClickRemoveNode);
      }


    } 
  }

}