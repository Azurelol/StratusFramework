/******************************************************************************/
/*!
@file   SceneFieldDrawer.cs
@author Fredrik Ludvigsen
@note:  http://wiki.unity3d.com/index.php/SceneField
*/
/******************************************************************************/
using UnityEngine;
using System;


namespace Stratus
{
  /// <summary>
  /// Allows you to refer to any scene by reference, instead of name or index.
  /// </summary>
  [Serializable]
  public class SceneField : ISerializationCallbackReceiver
  {

    #if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    #endif

    #pragma warning disable 414
    [SerializeField, HideInInspector]
    private string SceneName = "";
    #pragma warning restore 414
    
    /// <summary>
    /// Makes it work with the existing Unity methods (LoadLevel/LoadScene) 
    /// </summary>
    /// <param name="sceneField"></param>
    public static implicit operator string(SceneField sceneField)
    {
    #if UNITY_EDITOR
      return System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(sceneField.SceneAsset));
    #else
        return sceneField.SceneName;
    #endif
    }

    public void OnBeforeSerialize()
    {
    #if UNITY_EDITOR
      SceneName = this;
    #endif
    }
    public void OnAfterDeserialize() { }
  }
}