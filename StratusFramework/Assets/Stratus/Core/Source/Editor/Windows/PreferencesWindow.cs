/******************************************************************************/
/*!
@file   Configuration.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  /// <summary>
  /// The preferences window for the Stratus Framework
  /// </summary>
  [InitializeOnLoad]
  public class PreferencesWindow
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    //System.Action CurrentWindow;
    //static string Title = "Stratus | Preferences";
    static string RepositoryURL = "https://github.com/Azurelol/StratusFramework";
    // Window
    //Rect LeftPanel;
    //Rect RightPanel;
    //float SizeRatio = 0.5f;
    //float Margin = 0.025f;
    // Styles
    GUIStyle ButtonStyle;
    GUIStyle HeaderStyle;

    /// <summary>
    /// Default start for the Stratus Framework.
    /// </summary>
    static PreferencesWindow()
    {
      StratusDebug.Reset();
      Load();
    }

    //------------------------------------------------------------------------/
    // Menu Options
    //------------------------------------------------------------------------/
    private static bool ArePreferencesLoaded = false;
    
    [PreferenceItem("Stratus")]
    private static void PreferencesWindowGUI()
    {
      if (!ArePreferencesLoaded)
      {
        Load();
        ArePreferencesLoaded = true;
      }

      ModifyEvents();
      ModifyTrace();

      if (GUI.changed)
        Save();

    }


    //[MenuItem("Stratus/Preferences")]
    //public static void Open()
    //{
    //  var window = (PreferencesWindow)EditorWindow.GetWindow(typeof(PreferencesWindow), true, Title);
    //  window.Show();
    //  window.ButtonStyle = EditorStyles.toolbarButton;
    //  window.ButtonStyle.margin.left = 5;
    //  window.ButtonStyle.margin.right = 5;
    //  window.HeaderStyle = EditorStyles.whiteLargeLabel;
    //}
    
    
    [MenuItem("Stratus/About")]
    public static void About()
    {
      Application.OpenURL(RepositoryURL);
      //var col = new Color()
    }

    ////------------------------------------------------------------------------/
    //// GUI
    ////------------------------------------------------------------------------/
    //private void OnGUI()
    //{
    //  DrawLeftPanel();
    //  DrawRightPanel();
      
    //}

    //void DrawLeftPanel()
    //{
    //  LeftPanel = new Rect(0, 0, position.width * 0.25f, position.height);
    //  GUILayout.BeginArea(LeftPanel, EditorStyles.helpBox);
    //  GUILayout.BeginVertical();

    //  GUILayout.FlexibleSpace();
    //  if (GUILayout.Button("Trace", ButtonStyle))
    //    CurrentWindow = ModifyTrace;
    //  GUILayout.Space(2.5f);
    //  if (GUILayout.Button("Events", ButtonStyle))
    //    CurrentWindow = ModifyEvents;
    //  GUILayout.FlexibleSpace();

    //  GUILayout.EndVertical();
    //  GUILayout.EndArea();

    //}

    //void DrawRightPanel()
    //{
    //  RightPanel = new Rect((position.width * 0.25f) + (position.width * Margin),
    //                        (position.height * Margin),
    //                        (position.width * 0.75f) - (position.width * (Margin * 2)),
    //                        position.height);
    //  GUILayout.BeginArea(RightPanel);

    //  GUILayout.BeginVertical();

    //  CurrentWindow();

    //  GUILayout.EndVertical();
    //  GUILayout.EndArea();
    //}

    static void ModifyEvents()
    {
      //GUILayout.Label("Events", HeaderStyle);
      GUILayout.Label("Tracing", EditorStyles.boldLabel);
      StratusEvents.logging.connect = EditorGUILayout.Toggle("Connect", StratusEvents.logging.connect);
      StratusEvents.logging.dispatch = EditorGUILayout.Toggle("Dispatch", StratusEvents.logging.dispatch);
      StratusEvents.logging.construction = EditorGUILayout.Toggle("Construction", StratusEvents.logging.construction);
      StratusEvents.logging.register = EditorGUILayout.Toggle("Register", StratusEvents.logging.register);
    }

    static void ModifyTrace()
    {
      //GUILayout.Label("Tracing", HeaderStyle);
      StratusDebug.enabled = EditorGUILayout.Toggle("Enabled", StratusDebug.enabled);
      StratusDebug.timeStamp = EditorGUILayout.Toggle("Timestamp", StratusDebug.timeStamp);
      GUILayout.Label("Colors", EditorStyles.boldLabel);

      StratusDebug.methodColor = EditorGUILayout.ColorField("Method", StratusDebug.methodColor);
      StratusDebug.classColor = EditorGUILayout.ColorField("Class", StratusDebug.classColor);
      StratusDebug.gameObjectColor = EditorGUILayout.ColorField("GameObject", StratusDebug.gameObjectColor);
      StratusDebug.timeStampColor = EditorGUILayout.ColorField("TimeStamp", StratusDebug.timeStampColor);
    }

    ////------------------------------------------------------------------------/
    //// Serialization
    ////------------------------------------------------------------------------/
    /// <summary>
    /// Writes all preferences to disk
    /// </summary>
    public static void Save()
    {
      //Trace.Script("Saving!");
      // Trace
      EditorPrefs.SetBool("Stratus_Trace_Enabled", StratusDebug.enabled);
      EditorPrefs.SetBool("Stratus_Trace_Timestamp", StratusDebug.timeStamp);
      EditorPrefs.SetString("Stratus_Trace_MethodColor", StratusDebug.methodColor.ToHex());
      EditorPrefs.SetString("Stratus_Trace_ClassColor", StratusDebug.classColor.ToHex());
      EditorPrefs.SetString("Stratus_Trace_GameObjectColor", StratusDebug.gameObjectColor.ToHex());
      //Trace.Script("GameObject color = " + Trace.GameObjectColor.ToHex());
      EditorPrefs.SetString("Stratus_Trace_TimestampColor", StratusDebug.timeStampColor.ToHex());
      // Events
      EditorPrefs.SetBool("Stratus_Events_Connect", StratusEvents.logging.connect);
      EditorPrefs.SetBool("Stratus_Events_Dispatch", StratusEvents.logging.dispatch);
      EditorPrefs.SetBool("Stratus_Events_Construction", StratusEvents.logging.construction);
      EditorPrefs.SetBool("Stratus_Events_Register", StratusEvents.logging.register);
    }

    /// <summary>
    /// Loads all preferences from disk
    /// </summary>
    public static void Load()
    {
      //Trace.Script("Loading!");
      // Trace
      StratusDebug.enabled = EditorPrefs.GetBool("Stratus_Trace_Enabled", true);
      StratusDebug.timeStamp = EditorPrefs.GetBool("Stratus_Trace_Timestamp", true);
      StratusDebug.methodColor = Utilities.Rendering.HexToColor(EditorPrefs.GetString("Stratus_Trace_MethodColor", Color.black.ToHex()));
      StratusDebug.classColor = Utilities.Rendering.HexToColor(EditorPrefs.GetString("Stratus_Trace_ClassColor", Color.black.ToHex()));
      StratusDebug.gameObjectColor = Utilities.Rendering.HexToColor(EditorPrefs.GetString("Stratus_Trace_GameObjectColor", Color.blue.ToHex()));
      StratusDebug.timeStampColor = Utilities.Rendering.HexToColor(EditorPrefs.GetString("Stratus_Trace_TimestampColor", Color.green.ToHex()));
      // Events
      StratusEvents.logging.connect = EditorPrefs.GetBool("Stratus_Events_Connect", false);
      StratusEvents.logging.dispatch = EditorPrefs.GetBool("Stratus_Events_Dispatch", false);
      StratusEvents.logging.construction = EditorPrefs.GetBool("Stratus_Events_Construction", false);
      StratusEvents.logging.register = EditorPrefs.GetBool("Stratus_Events_Register", false);
    }    
    

  }
}