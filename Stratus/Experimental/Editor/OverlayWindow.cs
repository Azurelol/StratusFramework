using UnityEngine;
using Stratus;
using UnityEditor;

namespace Stratus
{
  [InitializeOnLoad]
  public class OverlayWindow : EditorWindow
  {
    public class Settings
    {
      public bool enabled;
      public bool showFPS;
    }

    private static Settings settings => Preferences.members.overlaySettings;
    private static OverlayWindow window { get; set; }
    private enum Mode { Play, Edit };
    private static Mode currentMode { get; set; }
     
    static OverlayWindow()
    {
      EditorApplication.playmodeStateChanged += OnPlayModeStateChange;
      //OnPlayModeStateChange();
    }

    //[MenuItem("Stratus/Tools/Overlay")]
    static void Open()
    {
      EditorWindow.GetWindow(typeof(OverlayWindow), true, "Overlay");
    }

    static void OnPlayModeStateChange()
    {
      if (EditorApplication.isPlaying && currentMode != Mode.Play)
      {
        OverlayWindow.OnPlayMode();
        currentMode = Mode.Play;
      }
      else if (currentMode != Mode.Edit)
      {
        OverlayWindow.OnEditMode();
        currentMode = Mode.Edit;
      }
    }

    static void OnPlayMode()
    {
    }

    static void OnEditMode()
    {
    }    

  } 
}
