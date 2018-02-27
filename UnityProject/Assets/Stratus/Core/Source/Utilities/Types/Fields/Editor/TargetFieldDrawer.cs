using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(GameObjectField))]
  public class GameObjectFieldDrawer : PropertyDrawer
  {
    float typeWidth { get; } = 0.3f;
    float inputValueWidth { get { return 1f - typeWidth; } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      SerializedProperty typeProp = property.FindPropertyRelative("type");
      var type = (GameObjectField.Type)typeProp.enumValueIndex;

      label = EditorGUI.BeginProperty(position, label, typeProp);
      Rect contentPosition = EditorGUI.PrefixLabel(position, label);
      var width = contentPosition.width;
      EditorGUI.indentLevel = 0;

      // 1. Modify the type
      contentPosition.width = width * typeWidth;
      EditorGUI.PropertyField(contentPosition, typeProp, GUIContent.none);

      // 2. Modify the input depending on the type
      contentPosition.x += contentPosition.width + 4f;
      contentPosition.width = width * inputValueWidth;
      SerializedProperty inputProp = null;
      switch (type)
      {
        case GameObjectField.Type.GameObject:
          inputProp = property.FindPropertyRelative("gameObject");
          break;
        case GameObjectField.Type.Layer:
          inputProp = property.FindPropertyRelative("layer");
          break;
        case GameObjectField.Type.Tag:
          inputProp = property.FindPropertyRelative("tag");
          break;
        case GameObjectField.Type.Name:
          inputProp = property.FindPropertyRelative("name");
          break;
      }

      EditorGUI.PropertyField(contentPosition, inputProp, GUIContent.none);
      EditorGUI.EndProperty();

      // 3. Save, if updated
      if (GUI.changed)
        property.serializedObject.ApplyModifiedProperties();


    }


  }
}