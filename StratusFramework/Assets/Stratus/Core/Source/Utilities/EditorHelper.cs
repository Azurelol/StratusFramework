/******************************************************************************/
/*!
@file   EditorHelper.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Stratus
{


  /// <summary>
  /// Provides a bridge to functions in the UnityEditor namespace
  /// </summary>
#if UNITY_EDITOR
  [InitializeOnLoad]
#endif
  public static class EditorBridge    
  {
    /// <summary>
    /// Whether the editor is currently in edit mode
    /// </summary>
    public static bool isEditMode
    {
      get
      {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
          return true;
#endif
        return false;
      }
    }

    public static float Field(string label, float value)
    {
#if UNITY_EDITOR
      return EditorGUILayout.FloatField(label, value);
#else
      return 0.0f;
#endif
    }

    public static int Field(string label, int value)
    {
#if UNITY_EDITOR
      return EditorGUILayout.IntField(label, value);
#else
      return 0;
#endif
    }

    public static bool Field(string label, bool value)
    {
#if UNITY_EDITOR
      return EditorGUILayout.Toggle(label, value);
#else
      return false;
#endif
    }

    public static T Enum<T>(string label, object value)
    {
#if UNITY_EDITOR
      return (T)(object)UnityEditor.EditorGUILayout.EnumPopup(label, (System.Enum)value);
#else
      return default(T);
#endif
    }

    public static T Object<T>(string label, T value) where T : Object
    {
#if UNITY_EDITOR
      return (T)(object)UnityEditor.EditorGUILayout.ObjectField(label, value, value.GetType(), true);
#else
      return default(T);
#endif
    }

    public static void BeginHorizontal()
    {
#if UNITY_EDITOR
      EditorGUILayout.BeginHorizontal();
#endif
    }

    public static void EndHorizontal()
    {
#if UNITY_EDITOR
      EditorGUILayout.EndHorizontal();
#endif
    }

    public static void ModifyArray<Class, Type>(Class owner, List<Type> array) 
      where Type : Object
      where Class : Object
    {
      #if UNITY_EDITOR
      // List all added elements, allowing any to be removed
      int indexToRemove = -1;
      for (int i = 0; i < array.Count; ++i)
      {
        var member = array[i];
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        array[i] = EditorGUILayout.ObjectField("", member, typeof(Type), true) as Type;
        if (GUILayout.Button("Remove")) indexToRemove = i;
        EditorGUILayout.EndHorizontal();
      }
      if (indexToRemove > -1) array.RemoveAt(indexToRemove);

      // Add an element
      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("Add"))
      {
        array.Add(null);
        EditorUtility.SetDirty(owner);
      }
      // Clear elements
      if (GUILayout.Button("Clear"))
      {
        array.Clear();
        EditorUtility.SetDirty(owner);
      }
      EditorGUILayout.EndHorizontal();
#endif
    }

    

    //public static T Enum<T>(string label, T value) 
    //{
    //  #if UNITY_EDITOR
    //  return (T)EditorGUILayout.EnumPopup(label, (T)value);
    //  #endif
    //}

  }
}
