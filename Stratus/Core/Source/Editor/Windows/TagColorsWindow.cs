using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [InitializeOnLoad]
  public class TagColorsWindow : EditorWindow
  {
    /// <summary>
    /// Defines how we draw the tag colors in the hierarchy window
    /// </summary>
    public enum TagColorDrawMode
    {
      Prefix,
      Whole
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// How the tag colors are being drawn
    /// </summary>
    public static TagColorDrawMode tagColorsDrawMode { get; } = TagColorDrawMode.Prefix;
    
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    private static Internal.TagColors tagColors { get { return Preferences.members.tagColors; } }
    private static float tagColorPrefixWidth { get; } = 15f;

    //------------------------------------------------------------------------/
    // Methods: Static
    //------------------------------------------------------------------------/
    [MenuItem("Stratus/Windows/Configure Tag Colors")]
    static void Open()
    {
      EditorWindow.GetWindow(typeof(TagColorsWindow), true, "Tag Colors");
    }

    static TagColorsWindow()
    {
      EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
    }

    //------------------------------------------------------------------------/
    // Methods: Callbacks
    //------------------------------------------------------------------------/
    static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
      GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

      if (go == null)
        return;
      
      InspectGameObject(selectionRect, go);
    }

    static void InspectGameObject(Rect selectionRect, GameObject gameObject)
    {
      var tag = gameObject.tag;

      // If it has no tag, do nothing
      if (tag.Contains("Untagged"))
        return;

      var color = tagColors.GetColor(gameObject.tag); ;

      // Don't use the default color
      if (color == Color.clear)
        return;
      
      var fullWidth = selectionRect.width;        

      // Draw the prefix
      selectionRect.x = 0f; // selectionRect.width;
      selectionRect.width = tagColorPrefixWidth;
      GUI.backgroundColor = color;
      GUI.Box(selectionRect, string.Empty);

      // Draw behind the gameobject label. Desaturate the color
      //selectionRect.x += selectionRect.width + 15f;
      //selectionRect.width = fullWidth;
      //GUI.backgroundColor = color.Saturate(0.2f);      
      //GUI.Box(selectionRect, string.Empty);
      //GUI.Label(selectionRect, gameObject.name);

      // Now reset the background color back to default
      GUI.backgroundColor = Color.white;
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnEnable()
    {
      Refresh();      
    }
    private void OninspectorUpdate()
    {
      Refresh();
    }

    private void OnGUI()
    {
      EditTagColors();
    }

    private static void Refresh()
    {
      tagColors.Validate();
      Preferences.Save();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void EditTagColors()
    {
      EditorGUILayout.LabelField("Tag Colors", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
      EditorGUILayout.Separator();
      foreach (var pair in tagColors.coloredTags)
      {
        pair.Color = EditorGUILayout.ColorField(new GUIContent(pair.Tag), pair.Color);
      }

      if (GUILayout.Button("Reset"))
      {
        tagColors.Reset();
      }

      if (GUI.changed)
      {
        Preferences.Save();
      }
    }
  }

}