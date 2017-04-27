/******************************************************************************/
/*!
@file   Singleton.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Singleton for components without events.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    protected abstract string Name { get; }
    protected abstract void OnAwake();
    protected static bool Quitting = false;
    protected static bool Tracing = false;
    protected static T SingletonInstance;
    public static T Instance
    {
      get
      {
        // Look for an instance in the scene
        if (!SingletonInstance)
          SingletonInstance = (T)FindObjectOfType(typeof(T));
        // If not found, instantiate
        if (!SingletonInstance)
          Instantiate();

        return SingletonInstance;
      }
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    static protected void Instantiate()
    {
      if (Quitting)
        return;

      var obj = new GameObject();
      var instance = obj.AddComponent<T>();
      //if (!SingletonInstance)
      SingletonInstance = instance;
    }
   
    public void Check()
    {

    }

    void Awake()
    {
      OnAwake();
      this.gameObject.name = Name;
      if (Tracing)
        Trace.Script(Name + " has been created!", this);
    }

    void OnDestroy()
    {
      if (this != SingletonInstance) return;
      SingletonInstance = null;
      if (Tracing)
        Trace.Script(Name + " is being destroyed!", this);
    }

    void OnApplicationQuit()
    {
      Quitting = true;
    }

  }


  /// <summary>
  /// Singleton for components that receive events.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class StratusSingleton<T> : StratusBehaviour where T : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public static T Instance
    {
      get
      {
        // Look for an instance in the scene
        if (!SingletonInstance)
        {
          SingletonInstance = FindObjectOfType<T>();
        }
        // If not found, instantiate
        if (!SingletonInstance)
        {
          var obj = new GameObject();
          SingletonInstance = obj.AddComponent<T>();
        }

        return SingletonInstance;
      }
    }
    protected virtual void OnSingletonDestroyed() {} // Make this abstract later?
    protected static T SingletonInstance;
    protected static bool Tracing = false;

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    protected abstract string Name { get; }
    protected abstract bool IsPersistent { get; }
    protected abstract void OnAwake();

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    void Awake()
    {
      // If the singleton instance hasn't been set, set it to self
      if (!Instance)
        SingletonInstance = this as T;

      // If we are the singleton instance that was created (or recently set)
      if (Instance == this as T)
      {
        if (IsPersistent)
        {
          transform.SetParent(null);
          DontDestroyOnLoad(this);
        }

        OnAwake();
        this.gameObject.name = Name;
        if (Tracing)
          Trace.Script(Name + " has been created!", this);
      }
      // If we are not...
      else
      {
        Destroy(gameObject);
      }
      
    }

    protected override void OnDestroyed()
    {
      if (this != SingletonInstance) return;
      SingletonInstance = null;      
      Trace.Script(Name + " is being destroyed!", this);
      this.OnSingletonDestroyed();
    }

    protected void Poke()
    {
    }





  }

}