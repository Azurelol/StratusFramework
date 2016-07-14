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
  /**************************************************************************/
  /*!
  @class Singleton 
  */
  /**************************************************************************/
  public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    protected abstract string getInstanceName();
    protected abstract void Initialize();
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
      var obj = new GameObject();
      var instance = obj.AddComponent<T>();
      SingletonInstance = instance;    
    }

    void Awake()
    {
      Initialize();
      this.gameObject.name = getInstanceName();
    }
    

  }

}