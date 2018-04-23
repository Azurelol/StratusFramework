using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Abstract class for all game view windows
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class LayoutGameViewWindow<T> : MonoBehaviour where T : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The current anchor for this window
    /// </summary>
    public StratusGUI.Anchor anchor = StratusGUI.Anchor.TopRight;
    /// <summary>
    /// The current size of this window
    /// </summary>
    public Vector2 size = new Vector2(225f, 200f);

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this window should be drawn
    /// </summary>
    protected abstract bool draw { get; }
    /// <summary>
    /// The title for this window
    /// </summary>
    protected abstract string title { get; }
    /// <summary>
    /// The position of the window within the game view
    /// </summary>
    protected Rect position { get; set; }
    /// <summary>
    /// The current scrolling position
    /// </summary>
    protected Vector2 scrollPos { get; set; }
    /// <summary>
    /// The current screen size of the game windoww
    /// </summary>
    public static Vector2 screenSize
    {
      get
      {
        #if UNITY_EDITOR
        string[] res = UnityEditor.UnityStats.screenRes.Split('x');
        Vector2 screenSize = new Vector2(int.Parse(res[0]), int.Parse(res[1]));
        #else
        Vector2 screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        #endif
        return screenSize;
      }
    }
    /// <summary>
    /// The initial, default size for this window
    /// </summary>
    protected virtual Vector2 defaultSize { get; } = new Vector2(225, 200f);
    /// <summary>
    /// What name to use for GameObject this singleton will be instantiated on
    /// </summary>
    private static string ownerName => typeof(T).GetType().Name;
    /// <summary>
    /// Returns a reference to the singular instance of this class. If not available currently, 
    /// it will instantiate it when accessed.
    /// </summary>
    public static T get
    {
      get
      {
        // Look for an instance in the scene
        if (!instance)
        {
          instance = FindObjectOfType<T>();

          // If not found, instantiate
          if (!instance)
          {
            var obj = new GameObject();
            obj.name = ownerName;
            instance = obj.AddComponent<T>();
          }
        }

        return instance;
      }
    }
    /// <summary>
    /// The singular instance of the class
    /// </summary>
    protected static T instance;
    /// <summary>
    /// Whether this singleton has been instantiated
    /// </summary>
    public static bool instantiated => get != null;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      // If the singleton instance hasn't been set, set it to self
      if (!get)
        instance = this as T;

      // If we are the singleton instance that was created (or recently set)
      if (get == this as T)
      {
        DontDestroyOnLoad(this);
        //if (isPersistent)
        //{
        //  transform.SetParent(null);
        //  if (!EditorBridge.isEditMode)
        //}

        OnAwake();
        //this.gameObject.name = displayName;
      }
      // If we are not...
      else
      {
        Destroy(gameObject);
      }
    }

    private void Reset()
    {
      size = defaultSize;
    }

    private void OnGUI()
    {
      if (!draw)
        return;

      Rect layoutPosition = StratusGUI.CalculateAnchoredPositionOnScreen(anchor, size, screenSize);
      GUILayout.BeginArea(layoutPosition, title, StratusGUIStyles.skin.window);
      scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
      OnGUILayout(layoutPosition);
      GUILayout.EndScrollView();
      GUILayout.EndArea();

    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    protected abstract void OnAwake();
    protected abstract void OnGUILayout(Rect position);

    public static bool Instantiate()
    {
      if (get != null)
      {
        return true;
      }
      return false;
    }

  }
}
