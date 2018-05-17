using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(Symbol))]
    public class SymbolDrawer : PropertyDrawer
    {
      //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
      //{
      //  return EditorGUIUtility.singleLineHeight;
      //}

      bool ShowSingleLine = true;

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        var valueProperty = property.FindPropertyRelative(nameof(Symbol.value));
        var typeProperty = valueProperty.FindPropertyRelative("type");
        var type = (Variant.VariantType)typeProperty.enumValueIndex;
        
        label = EditorGUI.BeginProperty(position, label, property);

        if (ShowSingleLine)
        {
          Rect contentPosition = EditorGUI.PrefixLabel(position, label);
          var width = contentPosition.width;
          EditorGUI.indentLevel = 0;
          // Key
          contentPosition.width = width * 0.40f;
          EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative(nameof(Symbol.key)), GUIContent.none);
          contentPosition.x += contentPosition.width + 4f;
          // Value
          contentPosition.width = width * 0.60f;
          EditorGUI.PropertyField(contentPosition, valueProperty, GUIContent.none);
        }
        else
        {
          EditorGUI.LabelField(position, label);
          //EditorGUI.indentLevel = 1;
          position.y += EditorGUIUtility.singleLineHeight;
          EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(Symbol.key)));
          position.y += EditorGUIUtility.singleLineHeight;
          EditorGUI.PropertyField(position, valueProperty);

        }


        EditorGUI.EndProperty();
      }


      
    }
  }

}