/******************************************************************************/
/*!
@file   Singleton.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System;
using Stratus.Utilities;

namespace Stratus
{
  /// <summary>
  /// An optional attribute for Stratus singletons, offering more control over its initial setup.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
  public sealed class SingletonAttribute : Attribute
  {
    /// <summary>
    /// Whether the class should only be instantiated during playmode
    /// </summary>
    public bool isPlayOnly { get; set; } = true;
    /// <summary>
    /// If instantiated, the name of the GameObject that will contain the singleton
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// Whether to instantiate an instance of the class if one is not found present at runtime
    /// </summary>
    public bool instantiate { get; set; }
    /// <summary>
    /// Whether the instance is persistent across scene loading
    /// </summary>
    public bool persistent { get; set; }

    /// <param name="name">The name of the GameObject where the singleton will be placed</param>
    /// <param name="persistent">Whether the instance is persistent across scene loading</param>
    /// <param name="instantiate">Whether to instantiate an instance of the class if one is not found present at runtime</param>
    public SingletonAttribute(string name, bool persistent = true, bool instantiate = true)
    {
      this.name = name;
      this.persistent = persistent;
      this.instantiate = instantiate;
    }

    public SingletonAttribute()
    {
    }

  }
  
  /// <summary>
  /// A singleton is a class with only one active instance, instantiated if not present when its
  /// static members are accessed. Use the [Singleton] attribute on the class 
  /// declaration in order to override the default settings.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [DisallowMultipleComponent]
  public abstract class Singleton<T> : StratusBehaviour where T : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this singleton will be persistent whenever its parent scene is destroyed
    /// </summary>
    protected static bool isPersistent => attribute?.GetProperty<bool>("persistent") ?? true;
    /// <summary>
    /// Whether the application is currently quitting play mode
    /// </summary>
    protected static bool isQuitting { get; set; } = false;
    /// <summary>
    /// Returns the current specific attributes for the derived singleton class, if any are present
    /// </summary>
    private static SingletonAttribute attribute => AttributeUtility.FindAttribute<SingletonAttribute>(typeof(T));
    /// <summary>
    /// Whether the class should be instantiated. By default, true.
    /// </summary>
    private static bool shouldInstantiate => attribute?.GetProperty<bool>("instantiate") ?? true;
    /// <summary>
    /// Whether the class should be instantiated while in editor mode
    /// </summary>
    private static bool isPlayerOnly => attribute?.GetProperty<bool>("isPlayOnly") ?? true;
    /// <summary>
    /// What name to use for GameObject this singleton will be instantiated on
    /// </summary>
    private static string ownerName => attribute?.GetProperty<string>("name") ?? typeof(T).GetType().Name;
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
            if (shouldInstantiate == false || (isPlayerOnly && EditorBridge.isEditMode))
            {
              //Trace.Script("Will not instantiate the class " + typeof(T).Name);
              return null;
            }

            //Trace.Script("Creating " + typeof(T).Name);
            var obj = new GameObject();
            obj.name = ownerName;
            instance = obj.AddComponent<T>();            
          }
        }

        return instance;
      }      
    }
    /// <summary>
    /// Whether this singleton has been instantiated
    /// </summary>
    public static bool instantiated => get != null;
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// The singular instance of the class
    /// </summary>
    protected static T instance;

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    protected abstract void OnAwake();
    protected virtual void OnSingletonDestroyed() { }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    //static protected void Instantiate()
    //{
    //  if (isQuitting)
    //    return;
    //
    //  var obj = new GameObject();
    //  var instance = obj.AddComponent<T>();
    //  Singleton<T>.instance = instance;
    //}
   
    void Awake()
    {
      // If the singleton instance hasn't been set, set it to self
      if (!get)
        instance = this as T;

      // If we are the singleton instance that was created (or recently set)
      if (get == this as T)
      {
        if (isPersistent)
        {
          transform.SetParent(null);
          if (!EditorBridge.isEditMode)
            DontDestroyOnLoad(this);
        }

        OnAwake();
        //this.gameObject.name = displayName;
      }
      // If we are not...
      else
      {
        Destroy(gameObject);
      }
    }

    void OnDestroy()
    {
      if (this != instance) return;
      instance = null;
      this.OnSingletonDestroyed();
    }

    void OnApplicationQuit()
    {
      isQuitting = true;
    }

    /// <summary>
    /// Instantiates this singleton if possible
    /// </summary>
    /// <returns></returns>
    public static bool Instantiate()
    {
      if (get != null)
      {
        return true;
      }
      return false;
    }


    ///// <summary>
    ///// Instantiates this singleton, if allowed
    ///// </summary>
    //public static void Instantiate()
    //{
    //  if (shouldInstantiate)
    //    get.
    //}

    protected void Poke()
    {
    }

    //public static void Wake()
    //{
    //  get;
    //}

  }
  

}