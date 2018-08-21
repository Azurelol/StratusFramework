using UnityEditor;
using UnityEngine;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(InputField))]
    public class InputFieldDrawer : PropertyDrawer
    {
      float typeWidth { get; } = 0.3f;
      float inputValueWidth { get { return 1f - typeWidth; } }

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        SerializedProperty typeProp = property.FindPropertyRelative("_type");
        var type = (InputField.Type)typeProp.enumValueIndex;


        label = EditorGUI.BeginProperty(position, label, typeProp);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        var width = contentPosition.width;

        //int indent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel = 0;

        // 1. Modify the type
        contentPosition.width = width * typeWidth;
        EditorGUI.PropertyField(contentPosition, typeProp, GUIContent.none);

        // 2. Modify the input depending on the type
        contentPosition.x += contentPosition.width + 4f;
        contentPosition.width = width * inputValueWidth;        
        switch (type)
        {
          case InputField.Type.Key:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("key"), GUIContent.none);
            break;
          case InputField.Type.MouseButton:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("mouseButton"), GUIContent.none);
            break;
          case InputField.Type.Axis:
            SerializedProperty axis = property.FindPropertyRelative("axis");
            int index = InputManagerUtility.GetIndex(axis.stringValue);
            index = EditorGUI.Popup(contentPosition, label.text, index, InputManagerUtility.axesNames);
            axis.stringValue = index >= 0 ? InputManagerUtility.axesNames[index] : "";
            break;
        }

        EditorGUI.EndProperty();

        //EditorGUI.indentLevel = indent;

        // 3. Save, if updated
        if (GUI.changed)
          property.serializedObject.ApplyModifiedProperties();
        //GUILayout.EndHorizontal();


      }

    }

  }
}