using UnityEditor;
using UnityEngine;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(TagField))]
    public class TagFieldDrawer : PropertyDrawer
    {
      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        SerializedProperty tag = property.FindPropertyRelative("tag");

        EditorGUI.BeginProperty(position, label, tag);
        tag.stringValue = EditorGUI.TagField(position, label, tag.stringValue);
        EditorGUI.EndProperty();
      }

    }

  }
}