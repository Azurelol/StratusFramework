using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
  public class ObjectNotesWindow : EditorWindow
  {
    public static bool isAddingNoteOnSceneView { get; private set; }

    private GameObject selection => Selection.activeGameObject;
    private ObjectNote objectNote => selection?.GetComponent<ObjectNote>();

    private Vector2 scrollPos = Vector2.zero;
    static string Title = "Object Notes";
    //private Texture2D icon;
    private const string iconName = "NoteIcon.png";
    private const string iconPath = "/Core/Utilities/";

    //--------------------------------------------------------------------------------------------/
    // Static Methods
    //--------------------------------------------------------------------------------------------/
    [MenuItem("Stratus/Windows/Notes")]
    public static void Open()
    {
      EditorWindow.GetWindow(typeof(ObjectNotesWindow), false, Title);
    }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void OnEnable()
    {
      //var stratusPath = Assets.GetFolderPath("Stratus");
      //icon = AssetDatabase.LoadAssetAtPath(stratusPath + iconPath + iconName, typeof(Texture2D)) as Texture2D;
    }

    private void OnGUI()
    {
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      {
        ShowHeader("Scene");
        ModifyScene();
      }
      {
        ShowHeader("Object");
        ModifyObject(selection);
        if (objectNote) InspectNote(objectNote);
      }
      EditorGUILayout.EndScrollView();

      //if (isAddingNoteOnSceneView)
      //  OnSelectingNotePosition();
      //
      //ProcessEvents(UnityEngine.Event.current);
    }

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void ModifyObject(GameObject go)
    {
      if (!objectNote)
      {
        if (GUILayout.Button("Add Note"))
          AddNote(go);
      }
      else
      {
        if (GUILayout.Button("Remove Note"))
          RemoveNote(go);
      }
    }

    private void InspectNote(ObjectNote note)
    {
      var editor = Editor.CreateEditor(note);
      editor.DrawDefaultInspector();
    }

    private void ModifyScene()
    {
      //if (GUILayout.Button("Add Note At Position")) SelectNotePosition();
      if (GUILayout.Button("Remove all from Scene")) RemoveAllNotes();
    }

    private void RemoveAllNotes()
    {
      ObjectNote[] notes = Scene.GetComponentsInAllActiveScenes<ObjectNote>();
      foreach (var note in notes)
        Undo.DestroyObjectImmediate(note);
      Trace.Script($"Removed {notes.Length} notes!");
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

    void SelectNotePosition()
    {
      isAddingNoteOnSceneView = true;
      //Cursor.SetCursor(icon, Vector2.zero, CursorMode.Auto);
      Trace.Script("Now selecting mouse position");
    }

    void OnSelectingNotePosition()
    {
      var e = UnityEngine.Event.current;
      if (e.type == EventType.MouseUp && e.button == 0)
      {
        OnMouseReleased();
        e.Use();
        isAddingNoteOnSceneView = false;
        Trace.Script("Now releasing mouse");
      }
    }

    void OnMouseReleased()
    {
      var pos = Camera.main.CastRayFromMouseScreenPosition();
      Trace.Script($"pos = {pos.point}");
    }

    private void AddNote(GameObject go) => go.AddComponent<ObjectNote>().hideFlags = HideFlags.HideInInspector | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy;
    private void RemoveNote(GameObject go) => DestroyImmediate(go.GetComponent<ObjectNote>());

    private void ShowHeader(string title)
    {
      GUILayout.Label(title, EditorStyles.centeredGreyMiniLabel);
    }

  }

}