using UnityEngine;
using UnityEditor;
using System;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(EventField))]
  public class EventFieldDrawer : PropertyDrawer
  {
    private static int index;

    void ListAllEvents()
    {

    }
    

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      //SerializedProperty typeProp = property.FindPropertyRelative("Type");
      SerializedProperty typeProp = property.FindPropertyRelative("type");
      //Type eventType = typeProp.objectReferenceValue as Type;

      //EditorGUI.BeginProperty(position, label, typeProp);
      EditorGUI.PropertyField(position, typeProp);
      //EditorGUI.BeginChangeCheck();
      //index = EditorGUI.Popup(position, index, Library.eventTypeNames);
      //if (EditorGUI.EndChangeCheck())
      //  typeProp.objectReferenceValue = Library.eventTypes[index] as Object;
      //EditorGUI.EndProperty();
    }
  }
}