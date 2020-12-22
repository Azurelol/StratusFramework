using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(StratusSymbol.Reference))]
    public class SymbolReferenceDrawer : PropertyDrawer
    {
      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        var keyProperty = property.FindPropertyRelative(nameof(StratusSymbol.Reference.key));
        var typeProperty = property.FindPropertyRelative(nameof(StratusSymbol.Reference.type));

        label = EditorGUI.BeginProperty(position, label, property);
        {
          Rect contentPosition = EditorGUI.PrefixLabel(position, label);
          var width = contentPosition.width;
          EditorGUI.indentLevel = 0;
          // Key
          contentPosition.width = width * 0.60f;
          EditorGUI.PropertyField(contentPosition, keyProperty, GUIContent.none);
          contentPosition.x += contentPosition.width + 4f;
          // Value
          contentPosition.width = width * 0.40f;
          EditorGUI.PropertyField(contentPosition, typeProperty, GUIContent.none);

        }
        EditorGUI.EndProperty();

      }

    } 
  }
}