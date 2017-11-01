using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(RuntimeMethodField))]
  public class MethodFieldDrawer : PropertyDrawer
  {
    private int numLines;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (property == null)
        return;

      var methodField = fieldInfo.GetValue(property.serializedObject.targetObject) as RuntimeMethodField;

      
      // Disabled during editor mode
      if (!EditorApplication.isPlaying || methodField == null || methodField.methods == null)
      {
        numLines = 0;
        return;
        //GUI.enabled = false;
      }


      numLines = methodField.methods.Length + 1;      

      //const float scaleModifier = 0.5f;
      //position.x += position.width * scaleModifier;
      //position.width *= scaleModifier;
      //GUI.Label(position, label);

      position.height /= numLines;
      EditorGUI.LabelField(position, $"{label.text}", EditorStyles.centeredGreyMiniLabel);
      position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

      foreach (var method in methodField.methods)
      {
        if (GUI.Button(position, $"Invoke {method.Method.Name}"))
        {
          method.Invoke();
        }
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
      }

      //GUI.enabled = true;      
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      if (!EditorApplication.isPlaying)
        return 0f;

      return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines;
    }

  } 
}
