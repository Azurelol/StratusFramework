/******************************************************************************/
/*!
@file   GameObjectExtensions.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;

/**************************************************************************/
/*!
@class GameObjectExtensions 
*/
/**************************************************************************/
public static class GameObjectExtensions {
  
  static void ListChildren(GameObject obj, List<GameObject> children)
  {
    foreach (Transform child in obj.transform)
    {
      children.Add(child.gameObject);
      ListChildren(child.gameObject, children);
    }
  }

  /**************************************************************************/
  /*!
  @brief  Returns a container of all the children of this GameObject.
  @return A container of all the children of this GameObject.
  */
  /**************************************************************************/
  public static List<GameObject> Children(this GameObject gameObj)
  {
    var children = new List<GameObject>();
    ListChildren(gameObj, children);
    return children;
  }

  /**************************************************************************/
  /*!
  @brief  Returns the parent of this GameObject.
  @return A reference to the GameObject parent of this GameObject.
  */
  /**************************************************************************/
  public static GameObject Parent(this GameObject gameObj)
  {
    return gameObj.transform.parent.gameObject;
  }

  /**************************************************************************/
  /*!
  @brief  Adds a component to this GameObject, through copying an existing one.
  @oaram componentToCopy The component to copy.
  @return A reference to the newly added Component.
  */
  /**************************************************************************/
  public static T AddComponent<T>(this GameObject gameObj, T componentToCopy) where T : Component
  {
    return gameObj.AddComponent<T>().Copy(componentToCopy);
  }

  /**************************************************************************/
  /*!
  @brief  Checks whether this GameObject has the specified component.
  @return True if the component was present, false otherwise.
  */
  /**************************************************************************/
  public static bool HasComponent<T>(this GameObject gameObj) where T : Component
  {
    if (gameObj.GetComponent<T>())
      return true;
    return false;
  }

  /// <summary>
  /// Finds the child of this GameObject with a given name
  /// </summary>
  /// <param name="gameObj"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static GameObject FindChild(this GameObject gameObj, string name)
  {
    return FindChildBFS(gameObj.transform, name).gameObject;
  }

  /// <summary>
  /// Finds the child of this transform, using Breadth-first search
  /// </summary>
  /// <param name="parent"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static Transform FindChildBFS(this Transform parent, string name)
  {
    var result = parent.Find(name);
    if (result != null)
      return result;
    foreach (Transform child in parent)
    {
      result = child.FindChildBFS(name);
      if (result != null)
        return result;
    }
    return null;
  }
  
  /// <summary>
  /// Destroys the GameObject.
  /// </summary>
  /// <param name="go"></param>
  public static void Destroy(this GameObject go)
  {
    GameObject.DestroyImmediate(go);
  }

  /**************************************************************************/
  /*!
  @brief  Checks whether this GameObject has the specified component.
  @return True if the component was present, false otherwise.
  */
  /**************************************************************************/
  //public static Transform FindChildByName(this Transform transform, string name)
  //{
  //  return transform.ch.Children().Find(x => x.name == name);
  //}

}
