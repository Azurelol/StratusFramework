using UnityEditor;
using UnityEngine;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(LayerField))]
    public class LayerFieldDrawer : PropertyDrawer
    {
      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        SerializedProperty layer = property.FindPropertyRelative("layer");

        EditorGUI.BeginProperty(position, label, layer);
        layer.intValue = EditorGUI.LayerField(position, label, layer.intValue);
        EditorGUI.EndProperty();
      }

    }

  }
}
