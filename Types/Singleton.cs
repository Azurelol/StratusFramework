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
    protected abstract string Name { get; }
    protected abstract void OnAwake();
    protected static bool Quitting = false;
    static bool Tracing = false;
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
    protected abstract string Name { get; }
    protected abstract void OnAwake();
    protected static T SingletonInstance;
    static bool Tracing = false;
    public static T Instance
    {
      get
      {
        // Look for an instance in the scene
        //if (!SingletonInstance)
        //  SingletonInstance = (T)FindObjectOfType(typeof(T));
        // If not found, instantiate
        if (!SingletonInstance)
          Instantiate();

        return SingletonInstance;
      }
    }

    static protected void Instantiate()
    {
      var obj = new GameObject();
      var instance = obj.AddComponent<T>();
      SingletonInstance = instance;
    }

    void Awake()
    {
      OnAwake();
      this.gameObject.name = Name;
      if (Tracing)
        Trace.Script(Name + " has been created!", this);
    }

    protected override void OnDestroyed()
    {
      if (this != SingletonInstance) return;
      SingletonInstance = null;
      if (Tracing)
        Trace.Script(Name + " is being destroyed!", this);
    }




  }

}