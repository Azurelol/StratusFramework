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
      Trace.Reset();
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
      Events.logging.Connect = EditorGUILayout.Toggle("Connect", Events.logging.Connect);
      Events.logging.Dispatch = EditorGUILayout.Toggle("Dispatch", Events.logging.Dispatch);
      Events.logging.Construction = EditorGUILayout.Toggle("Construction", Events.logging.Construction);
      Events.logging.Register = EditorGUILayout.Toggle("Register", Events.logging.Register);
    }

    static void ModifyTrace()
    {
      //GUILayout.Label("Tracing", HeaderStyle);
      Trace.Enabled = EditorGUILayout.Toggle("Enabled", Trace.Enabled);
      Trace.TimeStamp = EditorGUILayout.Toggle("Timestamp", Trace.TimeStamp);
      GUILayout.Label("Colors", EditorStyles.boldLabel);

      Trace.MethodColor = EditorGUILayout.ColorField("Method", Trace.MethodColor);
      Trace.ClassColor = EditorGUILayout.ColorField("Class", Trace.ClassColor);
      Trace.GameObjectColor = EditorGUILayout.ColorField("GameObject", Trace.GameObjectColor);
      Trace.TimeStampColor = EditorGUILayout.ColorField("TimeStamp", Trace.TimeStampColor);
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
      EditorPrefs.SetBool("Stratus_Trace_Enabled", Trace.Enabled);
      EditorPrefs.SetBool("Stratus_Trace_Timestamp", Trace.TimeStamp);
      EditorPrefs.SetString("Stratus_Trace_MethodColor", Trace.MethodColor.ToHex());
      EditorPrefs.SetString("Stratus_Trace_ClassColor", Trace.ClassColor.ToHex());
      EditorPrefs.SetString("Stratus_Trace_GameObjectColor", Trace.GameObjectColor.ToHex());
      //Trace.Script("GameObject color = " + Trace.GameObjectColor.ToHex());
      EditorPrefs.SetString("Stratus_Trace_TimestampColor", Trace.TimeStampColor.ToHex());
      // Events
      EditorPrefs.SetBool("Stratus_Events_Connect", Events.logging.Connect);
      EditorPrefs.SetBool("Stratus_Events_Dispatch", Events.logging.Dispatch);
      EditorPrefs.SetBool("Stratus_Events_Construction", Events.logging.Construction);
      EditorPrefs.SetBool("Stratus_Events_Register", Events.logging.Register);
    }

    /// <summary>
    /// Loads all preferences from disk
    /// </summary>
    public static void Load()
    {
      //Trace.Script("Loading!");
      // Trace
      Trace.Enabled = EditorPrefs.GetBool("Stratus_Trace_Enabled", true);
      Trace.TimeStamp = EditorPrefs.GetBool("Stratus_Trace_Timestamp", true);
      Trace.MethodColor = Utilities.Graphical.HexToColor(EditorPrefs.GetString("Stratus_Trace_MethodColor", Color.black.ToHex()));
      Trace.ClassColor = Utilities.Graphical.HexToColor(EditorPrefs.GetString("Stratus_Trace_ClassColor", Color.black.ToHex()));
      Trace.GameObjectColor = Utilities.Graphical.HexToColor(EditorPrefs.GetString("Stratus_Trace_GameObjectColor", Color.blue.ToHex()));
      Trace.TimeStampColor = Utilities.Graphical.HexToColor(EditorPrefs.GetString("Stratus_Trace_TimestampColor", Color.green.ToHex()));
      // Events
      Events.logging.Connect = EditorPrefs.GetBool("Stratus_Events_Connect", false);
      Events.logging.Dispatch = EditorPrefs.GetBool("Stratus_Events_Dispatch", false);
      Events.logging.Construction = EditorPrefs.GetBool("Stratus_Events_Construction", false);
      Events.logging.Register = EditorPrefs.GetBool("Stratus_Events_Register", false);
    }    
    

  }
}