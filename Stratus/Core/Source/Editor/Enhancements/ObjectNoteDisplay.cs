using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Stratus.Utilities;
using UnityEngine.UI;

namespace Stratus
{
  public class ObjectNoteDisplay : BehaviourSceneViewDisplay<ObjectNote>, IHierarchyWindowItemOnGUI
  {
    protected override bool isValid => (EditorApplication.isPlaying == false);

    public Texture2D icon;
    private const string iconName = "NoteIcon.png";
    private const float iconWidth = 15f;

    private const string path = "/Core/Utilities/";
    public bool isAddingNote => ObjectNotesWindow.isAddingNoteOnSceneView;

    protected override void OnInitialize()
    {
      base.OnInitialize();
      var stratusPath = Assets.GetFolderPath("Stratus");
      icon = AssetDatabase.LoadAssetAtPath(stratusPath + path + iconName, typeof(Texture2D)) as Texture2D;
    }

    protected override void OnSceneGUI(SceneView view)
    {
      base.OnSceneGUI(view);
      OnAddingNote(view);
      //ProcessEvents(UnityEngine.Event.current);
    }

    public void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
      GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
      if (go == null)
        return;

      ObjectNote note = go.GetComponent<ObjectNote>();
      if (note == null)
        return;

      if (note.text == null)
        return;

      DrawTooltip(note, selectionRect);
    }

    protected override void OnInspect(ObjectNote note)
    {
      if (note == null)
        return;

      switch (note.drawMode)
      {
        case ObjectNote.DrawMode.Always:
          Draw(note);
          break;
        case ObjectNote.DrawMode.Selected:
          if (Selection.activeTransform == note.transform)
            Draw(note);
          break;
        case ObjectNote.DrawMode.Unselected:
          if (Selection.activeTransform != note.transform)
            Draw(note);
          break;
        default:
          break;
      }
    }

    private void Draw(ObjectNote note)
    {
      Transform transform = note.transform;
      float dist = HandleUtility.GetHandleSize(transform.position);
      Vector3 offset = new Vector3(note.offset.x * dist, note.offset.y * dist, note.offset.z * dist);
      GUI.backgroundColor = note.color;
      if (!note.hasStyle)
        note.ConstructStyle();
      Handles.Label(transform.position + offset, note.text, note.style);
    }

    private void OnAddingNote(SceneView view)
    {
      //EditorGUIUtility.AddCursorRect(view.position, MouseCursor.Link);
    }

    private void DrawIcon(ObjectNote note, Rect selectionRect)
    {
      EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
      var padding = new Vector2(5, 0);
      var iconDrawRect = new Rect(
                             selectionRect.xMax - (iconWidth + padding.x),
                             selectionRect.yMin,
                             selectionRect.width,
                             selectionRect.height);
      var iconGUIContent = new GUIContent(icon);
      EditorGUI.LabelField(iconDrawRect, iconGUIContent);
      EditorGUIUtility.SetIconSize(Vector2.zero);

      //var e = UnityEngine.Event.current;
      //var mousePos = e.mousePosition;
      //if (iconDrawRect.Contains(mousePos))
      //  OnMouseOver(note, e);
    }

    private void DrawTooltip(ObjectNote note, Rect selectionRect)
    {
      if (!note.hasStyle)
        return;

      Vector2 size = note.style.CalcSize(new GUIContent(note.text));
      //float width = size.x;
      //width = CalculateLengthOfMessage(note.style, note.text);
      //note.style.CalcMinMaxWidth(new GUIContent(note.text), out minWidth, out maxWidth);
      //float modifier = size.x * 0.20f;
      selectionRect.x = selectionRect.xMax - size.x;
      GUI.backgroundColor = note.color;
      EditorGUI.HelpBox(selectionRect, note.text, MessageType.None);
      //GUI.Box(selectionRect, note.text);
      GUI.backgroundColor = Color.white;

      //var e = UnityEngine.Event.current;
      //var mousePos = e.mousePosition;
      //if (selectionRect.Contains(mousePos))
      //  OnMouseOver(note, e);
    }


    private void OnMouseOver(ObjectNote note, UnityEngine.Event e)
    {
      Trace.Script("Moused over " + note.name);

      if (e.isMouse && e.button == 0)
      {
        Trace.Script("Clicked on " + note.name);
        e.Use();
      }

    }

    private void ProcessEvents(UnityEngine.Event e)
    {
      switch (e.type)
      {
        case EventType.MouseDown:
          if (e.button == 0)
            Trace.Script("Left mouse down");
          break;

        case EventType.MouseUp:
          if (e.button == 0)
            Trace.Script("Left mouse up!");
          break;
      }
    }



  }

}