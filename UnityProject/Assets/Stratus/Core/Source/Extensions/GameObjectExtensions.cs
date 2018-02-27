using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;

namespace Stratus
{
  public static partial class Extensions
  {

    static void ListChildren(GameObject obj, List<GameObject> children)
    {
      foreach (Transform child in obj.transform)
      {
        children.Add(child.gameObject);
        ListChildren(child.gameObject, children);
      }
    }

    /// <summary>
    /// Returns a container of all the children of this GameObject.
    /// </summary>
    /// <param name="gameObj"></param>
    /// <returns>A container of all the children of this GameObject.</returns>
    public static List<GameObject> Children(this GameObject gameObj)
    {
      var children = new List<GameObject>();
      ListChildren(gameObj, children);
      return children;
    }

    /// <summary>
    /// Returns the parent GameObject of this GameObject.
    /// </summary>
    /// <param name="gameObj"></param>
    /// <returns>A reference to the GameObject parent of this GameObject.</returns>
    public static GameObject Parent(this GameObject gameObj)
    {
      return gameObj.transform.parent.gameObject;
    }

    /// <summary>
    /// Adds a component to this GameObject, through copying an existing one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObj"></param>
    /// <param name="componentToCopy">The component to copy.</param>
    /// <returns>A reference to the newly added component.</returns>
    public static T AddComponent<T>(this GameObject gameObj, T componentToCopy) where T : Component
    {
      return gameObj.AddComponent<T>().CopyFrom(componentToCopy);
    }

    /// <summary>
    /// Checks whether this GameObject has the specified component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObj"></param>
    /// <returns>True if the component was present, false otherwise.</returns>
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
    /// Destroys the GameObject.
    /// </summary>
    /// <param name="go"></param>
    public static void Destroy(this GameObject go)
    {
      GameObject.DestroyImmediate(go);
    }

    /// <summary>
    /// Gets or if not present, adds the specified component to the GameObject.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
      T component = go.GetComponent<T>();
      if (component == null)
        component = go.gameObject.AddComponent<T>();      
      return component;
    }

    /// <summary>
    /// Gets or if not present, adds the specified component to the GameObject.
    /// </summary>
    public static Component GetOrAddComponent(this GameObject go, Type type)
    {
      Component component = go.GetComponent(type);
      if (component == null)
        component = go.gameObject.AddComponent(type);
      return component;
    }

    /// <summary>
    /// Duplicates the given component on this GameObject
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    public static Component DuplicateComponent(this GameObject gameObject, Component component, bool copy = false)
    {
      Type type = component.GetType();
      Component newComponent = gameObject.AddComponent(type);
      if (copy)
        newComponent.CopyFrom(component);
      return newComponent;
    }

    /// <summary>
    /// Duplicates the given component onto this GameObject. All its values will be copied over into the copy.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    public static T DuplicateComponent<T>(this GameObject gameObject, T component) where T : Component
    {
      T newComponent = gameObject.AddComponent<T>();
      newComponent.CopyFrom(component);
      return newComponent;
    }

    /// <summary>
    /// Returns true if the GameObject has been properly destroyed by the engine
    /// </summary>
    /// <param name="go"></param>
    public static bool IsDestroyed(this GameObject go)
    {
      return go == null && !ReferenceEquals(go, null);
    }

    /// <summary>
    /// Finds and invokes the method with the given name among all components
    /// attached to this GameObject
    /// </summary>
    /// <param name="go"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static void FindAndInvokeMethod(this GameObject go, string methodName)
    {
      var components = go.GetComponents<MonoBehaviour>();
      MethodInfo mInfo = components.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
      if (mInfo != null)
      {
        mInfo.Invoke(components, null);
      }
    }

  }
}