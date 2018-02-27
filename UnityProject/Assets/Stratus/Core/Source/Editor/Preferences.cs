#define STRATUS_CORE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using Stratus.Utilities;

namespace Stratus
{
  /// <summary>
  /// The main data asset containing all the saved settings present among the Stratus framework's utilities
  /// </summary>
  [SingletonAsset("Assets", "Stratus Preferences")]
  public class Preferences : SingletonAsset<Preferences>
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Saved Settings")]
    /// <summary>
    /// Allows scenes to be bookmarked from the project folder, used by the scene browser
    /// </summary>
    [SerializeField]
    public List<SceneAsset> bookmarkedScenes = new List<SceneAsset>();       

    /// <summary>
    /// Allows the coloring of GameObjects with tags in the hierarchy window
    /// </summary>
    [SerializeField]
    public Internal.TagColors tagColors = new Internal.TagColors();
    
    /// <summary>
    /// Settings for the overlay window
    /// </summary>
    public OverlayWindow.Settings overlaySettings = new OverlayWindow.Settings();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Resets all settings to their default
    /// </summary>
    public void Clear()
    {
      // @TODO: Better way would be to just delete asset then add it again?
      bookmarkedScenes.Clear();
      tagColors.Clear();
    }

    [MenuItem("Edit/Project Settings/Stratus")]
    public static void Open()
    {
      Selection.activeObject = instance;
      
    }


  }

}