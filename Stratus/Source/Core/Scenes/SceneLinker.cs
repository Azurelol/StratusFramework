using UnityEngine;

namespace Stratus
{
  namespace Experimental
  {
    /// <summary>
    /// Links multiple scenes together. This component is to be placed inside a GameObject in the
    /// master scene which links all other scenes together dynamically, loading and
    /// unloading them as need fit.
    /// </summary>
    [Singleton("Scene Linker", true, false)]
    public class SceneLinker : Singleton<SceneLinker>
    {
      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      [Tooltip("The pool of scenes being used in the project")]
      public ScenePool scenePool;
      public SceneField[] scenes => scenePool.scenes;

      //----------------------------------------------------------------------/
      // Properties: Public
      //----------------------------------------------------------------------/
      /// <summary>
      /// Whether to show a debug display in the editor's scene view
      /// </summary>
      [Tooltip("Whether to show a debug display in the editor's scene view")]
      public bool showDisplay = true;
      [Tooltip("Whether to close all other open scenes on entering play mode")]
      public bool loadOnlyInitial = true;
      [Tooltip("Whether to highlight all current scene links in the scene")]
      public bool displayLinks = true;
      [Tooltip("Whether to higlight the boundaries of all scenes")]
      public bool displayBoundaries = false;

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      protected override void OnAwake()
      {        
        if (!scenePool.initialScene.isLoading)
        {
          Trace.Script("Loading initial scene");
          scenePool.initialScene.Load(UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        //if (loadOnlyInitial)
        //{
        //  foreach(var scene in Scene.activeScenes)
        //  {
        //    if (scene == scenePool.initialScene)
        //      continue;
        //
        //    // Don't unload the scene this linker is on!
        //    // @TODO: Do something about the equals function?
        //    if (scene.name == Scene.activeScene.name)
        //      continue;
        //
        //    //if (scene.isLoading)
        //    Trace.Script("Unloading " + scene.name);
        //    scene.Close();
        //  }
        //}

      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Opens all scenes in the editor
      /// </summary>
      public void OpenAll()
      {
        scenePool.OpenAll();
      }

      /// <summary>
      /// Closes all scenes in the editor
      /// </summary>
      public void CloseAll()
      {
        scenePool.CloseAll();
      }
    }
  }
}