using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(MemberVisualizer.MemberVisualizationField))]
  public class MemberVisualizationFieldDrawer : PropertyDrawer
  {
    private const float lines = 5f;
    private const float padding = 2f;
    private float height => EditorGUIUtility.singleLineHeight + padding;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return height * lines;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var memberProp = property.FindPropertyRelative("member");
      var visualizationTypeProperty = property.FindPropertyRelative("visualizationMode");
      var colorProperty = property.FindPropertyRelative("color");
      var prefixProperty = property.FindPropertyRelative("prefix");
      label = EditorGUI.BeginProperty(position, label, property);
    
      EditorGUI.PropertyField(position, memberProp, new GUIContent(property.displayName));
      position.y += height * 2f;
      position.height = height;
    
      EditorGUI.PropertyField(position, visualizationTypeProperty);
      position.y += height;
    
      EditorGUI.PropertyField(position, colorProperty);
      position.y += height;
    
      EditorGUI.PropertyField(position, prefixProperty);
    
      EditorGUI.EndProperty();
    }

  }
}