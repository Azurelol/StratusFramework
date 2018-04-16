using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  public abstract class BaseEditorWindow<T> : EditorWindow
  {
    //------------------------------------------------------------------------/
    // Static
    //------------------------------------------------------------------------/
    protected static T instance;

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void OnEditorAwake();
    protected abstract void OnEditorGUI();

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      OnEditorAwake();
    }
    private void OnGUI()
    {
      OnEditorGUI();
    }

    private void OnEnable()
    {
      
    }

    private void OnDisable()
    {
      
    }

    //public static void OpenWindow<T>() 
    //{
    //  //instance = EditorWindow.GetWindow<T>();
    //}

    


  }

}