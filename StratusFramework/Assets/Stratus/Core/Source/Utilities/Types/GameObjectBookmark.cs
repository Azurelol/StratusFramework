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
    public static GameObjectInformation[] availableInformation { get; private set; }
    public static bool hasAvailableInformation => availableInformation.Length > 0;
    public static GameObjectInformation.MemberReference[] favorites { get; private set; }
    public static bool hasFavorites => favorites != null && favorites.Length > 0;
    public static System.Action onFavoritesChanged { get; set; } = new System.Action(() => { });
    public static System.Action onInformationChanged{ get; set; } = new System.Action(() => { });
    public static System.Action onUpdate { get; set; } = new System.Action(() => { });

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
            
      //availableInformation.Remove(this._information);
      this._information = (GameObjectInformation)information.CloneJSON();

      GameObjectBookmark.UpdateAvailable();
    }

    //------------------------------------------------------------------------/
    // Methods: Watch
    //------------------------------------------------------------------------/

    /// <summary>
    /// Updates the list of current favorited members
    /// </summary>
    public static void UpdateFavoriteMembers()
    {
      List<GameObjectInformation.MemberReference> values = new List<GameObjectInformation.MemberReference>();
      foreach (var targetInfo in availableInformation)
      {
        values.AddRange(targetInfo.favorites);
      }
      GameObjectBookmark.favorites = values.ToArray();
      GameObjectBookmark.onFavoritesChanged();
    }

    /// <summary>
    /// Updates the list of available information from enabled bookmarks
    /// </summary>
    private static void UpdateInformation()
    {
      List<GameObjectInformation> availableInformation = new List<GameObjectInformation>();
      foreach (var bookmark in GameObjectBookmark.available)
      {
        availableInformation.Add(bookmark.Value.information);
      }
      GameObjectBookmark.availableInformation = availableInformation.ToArray();
      GameObjectBookmark.onInformationChanged();
    }

    /// <summary>
    /// Updates all available data for bookmarks
    /// </summary>
    public static void UpdateAvailable()
    {
      GameObjectBookmark.UpdateInformation();
      GameObjectBookmark.UpdateFavoriteMembers();
      GameObjectBookmark.onUpdate();
    }

  }

}