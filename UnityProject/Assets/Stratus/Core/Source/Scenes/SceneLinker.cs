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
      [Tooltip("Whether to show a debug display in the editor's scene view")]
      public bool showDisplay = true;
      [Tooltip("Whether the initial scene should be automatically loaded when entering Play")]
      public bool loadInitial = true;
      [Tooltip("Whether to highlight all current scene links in the scene")]
      public bool displayLinks = true;
      [Tooltip("Whether to higlight the boundaries of all scenes")]
      public bool displayBoundaries = false;

      //----------------------------------------------------------------------/
      // Messages
      //----------------------------------------------------------------------/
      protected override void OnAwake()
      {
        if (scenePool.initialScene == null)
          return;

        if (loadInitial)
          LoadInitialScene();
      }

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      /// <summary>
      /// Opens all scenes
      /// </summary>
      public void OpenAll()
      {
        scenePool.OpenAll();
      }

      /// <summary>
      /// Closes all scenes
      /// </summary>
      public void CloseAll()
      {
        scenePool.CloseAll();
      }
      
      /// <summary>
      /// Restarts from the beginning, from the initial scene
      /// </summary>      
      public void Restart(System.Action onFinished = null)
      {        
       scenePool.CloseAll(()=>
       {
         LoadInitialScene(onFinished);
       });
      }

      public void LoadInitialScene(System.Action onFinished = null)
      {
        if (scenePool.initialScene.isLoading)
          return;

        scenePool.initialScene.Load(UnityEngine.SceneManagement.LoadSceneMode.Additive, () =>
        {
          //Trace.Script("Finished loading initial scene");
          onFinished?.Invoke();
        });
      }
    }
  }
}