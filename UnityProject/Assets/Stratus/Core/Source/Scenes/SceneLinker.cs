using UnityEngine;

namespace Stratus
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
    [Tooltip("Whether to show a debug display in the editor's scene view")]
    public bool showDisplay = true;
    //[Tooltip("The initial configuration for the scene linker")]
    //public SceneLinkerEvent initial;
    [Tooltip("Whether the initial configuration should be loaded when entering play")]
    public bool loadInitial = true;
    [Tooltip("Whether to highlight all current scene links in the scene")]
    public bool displayLinks = true;
    [Tooltip("Whether to higlight the boundaries of all scenes")]
    public bool displayBoundaries = false;

    //----------------------------------------------------------------------/
    // Properties
    //----------------------------------------------------------------------/
    public static bool isInitialLoaded => Scene.activeScene == get.scenePool.initialScene;

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
    // Methods: Static
    //----------------------------------------------------------------------/
    /// <summary>
    /// Opens all scenes managed by the linker
    /// </summary>
    public static void OpenAll()
    {
      get.scenePool.OpenAll();
    }

    /// <summary>
    /// Closes all scenes managed by the linker
    /// </summary>
    public static void CloseAll()
    {
      get.scenePool.CloseAll();
    }

    /// <summary>
    /// Closes all current scenes, loading the initial one
    /// </summary>      
    public static void Restart(System.Action onFinished = null)
    {
      get.scenePool.CloseAll(() =>
      {
        get.LoadInitialScene(onFinished);
      });
    }

    /// <summary>
    /// Closes all current scenes, loading them again
    /// </summary>
    /// <param name="onFinished"></param>
    public static void RestartAll(System.Action onFinished = null)
    {
      get.scenePool.CloseAll(() =>
      {
        get.scenePool.OpenAll(() => onFinished());
      });
    }

    //----------------------------------------------------------------------/
    // Methods
    //----------------------------------------------------------------------/
    /// <summary>
    /// Loads the initial scene
    /// </summary>
    /// <param name="onFinished"></param>
    private void LoadInitialScene(System.Action onFinished = null)
    {
      if (scenePool.initialScene.isLoading)
        return;

      scenePool.initialScene.Load(UnityEngine.SceneManagement.LoadSceneMode.Additive, () =>
      {
        onFinished?.Invoke();
      });
    }

  }
}