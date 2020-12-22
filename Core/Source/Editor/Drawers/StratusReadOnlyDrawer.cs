using UnityEngine;
using Stratus;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
  public class ReadOnlyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      GUI.enabled = false;
      EditorGUI.PropertyField(position, property, label);
      GUI.enabled = true;
    }
  }

}