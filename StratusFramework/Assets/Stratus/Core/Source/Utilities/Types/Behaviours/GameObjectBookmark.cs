using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public class GameObjectBookmark : StratusEditorBehaviour<GameObjectBookmark>
  {
    //#if UNITY_EDITOR
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// Information about this GameObject
    /// </summary>    
    [SerializeField]
    private GameObjectInformation _information;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public GameObjectInformation information => this._information;
    public static GameObjectInformation[] availableInformation { get; private set; } = new GameObjectInformation[0];
    public static bool hasAvailableInformation => availableInformation != null && availableInformation.Length > 0;
    public static ComponentInformation.MemberReference[] watchList { get; private set; } = new ComponentInformation.MemberReference[0];
    public static bool hasWatchList => watchList != null && watchList.Length > 0;
    public static System.Action onUpdate { get; set; } = new System.Action( () => { });

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnReset()
    {
      this._information = new GameObjectInformation(this.gameObject);
    }

    protected override void OnStratusEditorBehaviourEnable()
    {
      if (this._information == null)
        this._information = new GameObjectInformation(gameObject);
      GameObjectBookmark.UpdateAvailable();
    }

    protected override void OnStratusEditorBehaviourDisable()
    {
      GameObjectBookmark.UpdateAvailable();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public static GameObjectBookmark Add(GameObject gameObject)
    {
      GameObjectBookmark bookmark = gameObject.AddComponent<GameObjectBookmark>();
      return bookmark;
    }

    public static void Remove(GameObject gameObject)
    {
      gameObject.RemoveComponent<GameObjectBookmark>();
    }

    public static void Toggle(GameObject gameObject)
    {
      if (gameObject.HasComponent<GameObjectBookmark>())
      {
        gameObject.RemoveComponent<GameObjectBookmark>();
      }
      else
      {
        gameObject.AddComponent<GameObjectBookmark>();
      }
    }

    public void SetInformation(GameObjectInformation information)
    {
      if (information.target != this.gameObject)
        return;
            
      this._information = (GameObjectInformation)information.CloneJSON();
      this._information.CacheReferences();
      GameObjectBookmark.UpdateAvailable();
    }

    //------------------------------------------------------------------------/
    // Methods: Watch
    //------------------------------------------------------------------------/

    /// <summary>
    /// Updates the list of current favorited members
    /// </summary>
    public static void UpdateWatchList(bool invokeDelegate = false)
    {
      List<ComponentInformation.MemberReference> values = new List<ComponentInformation.MemberReference>();
      foreach (var targetInfo in availableInformation)
      {
        // Update the watch list cache first
        targetInfo.CacheWatchList();
        // Now add that object's members onto this
        values.AddRange(targetInfo.watchList);
      }
      GameObjectBookmark.watchList = values.ToArray();

      if (invokeDelegate)
        GameObjectBookmark.onUpdate();
    }

    /// <summary>
    /// Updates the list of available information from enabled bookmarks
    /// </summary>
    private static void UpdateInformation(bool invokeDelegate = false)
    {
      List<GameObjectInformation> availableInformation = new List<GameObjectInformation>();
      foreach (var bookmark in GameObjectBookmark.available)
      {
        availableInformation.Add(bookmark.Value.information);
      }
      GameObjectBookmark.availableInformation = availableInformation.ToArray();

      if (invokeDelegate)
        GameObjectBookmark.onUpdate();
    }

    /// <summary>
    /// Updates all available data for bookmarks
    /// </summary>
    public static void UpdateAvailable()
    {
      GameObjectBookmark.UpdateInformation();
      GameObjectBookmark.UpdateWatchList();

      GameObjectBookmark.onUpdate();
    }

  }

}