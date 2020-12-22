/******************************************************************************/
/*!
@file   SceneFieldDrawer.cs
@author Fredrik Ludvigsen
*/
/******************************************************************************/
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(StratusSceneField))]
  public class SceneFieldPropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      SerializedProperty sceneAssetProp = property.FindPropertyRelative(nameof(StratusSceneField.sceneAsset));

      EditorGUI.BeginProperty(position, label, sceneAssetProp);
      EditorGUI.PropertyField(position, sceneAssetProp, label);
      EditorGUI.EndProperty();
    }
  } 
}