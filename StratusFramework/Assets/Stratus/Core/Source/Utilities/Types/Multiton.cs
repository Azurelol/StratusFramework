using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Allows an easy interface for managing multiple instances of a single class,
  /// giving global access to them
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [ExecuteInEditMode]
  public abstract class Multiton<T> : StratusBehaviour where T : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Declarations
    //------------------------------------------------------------------------/
    ///// <summary>
    ///// Utility class used for navigating through all instances of a given multiton
    ///// </summary>
    //public class Navigator : ArrayNavigator<T>
    //{
    //  public ArrayNavigator(List<T> list, bool loop = false)
    //  {
    //  }
    //}

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// All currently active instances, indexed by their labels
    /// </summary>
    public static Dictionary<string, T> available { get; private set; } = new Dictionary<string, T>();
    /// <summary>
    /// Returns the first listed multiton
    /// </summary>
    public static T first => availableList.FirstOrNull() as T;
    /// <summary>
    /// All currently active instances
    /// </summary>
    public static List<T> availableList { get; private set; } = new List<T>();
    /// <summary>
    /// Whether there are available instances
    /// </summary>
    public static bool hasAvailable => availableList.Count > 0;
    /// <summary>
    /// Returns the underlying class for this multiton
    /// </summary>
    public T get { get; private set; }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The identifier for this instance
    /// </summary>
    [Tooltip("The identifier for this instance")]    
    public string label;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      get = this as T;
      OnAwake();
    }

    private void OnEnable()
    {
      if (Application.isPlaying && !string.IsNullOrEmpty(label))
      {
        available.Add(label, this as T);
      }
      availableList.Add(this as T);

      OnMultitonDisable();
    }

    private void OnDisable()
    {
      if (Application.isPlaying && !string.IsNullOrEmpty(label))
      {
        available.Remove(label);
      }

      availableList.Remove(this as T);

      OnMultitonEnable();
    }

    private void Reset()
    {
      label = gameObject.name;
      OnReset();
    }

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void OnAwake();
    protected abstract void OnMultitonEnable();
    protected abstract void OnMultitonDisable();
    protected abstract void OnReset();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Returns a navigator, which allows easy navigation between instances of this class
    /// </summary>
    /// <returns></returns>
    public static ArrayNavigator<T> GetNavigator(bool loop = true)
    {
      var navigator = new ArrayNavigator<T>(availableList, loop);
      return navigator;
    }

  }

}