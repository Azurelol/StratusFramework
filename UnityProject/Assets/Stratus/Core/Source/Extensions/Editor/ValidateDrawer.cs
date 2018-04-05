using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(ValidateAttribute))]
  public class ValidateDrawer : PropertyDrawer
  {
    ValidateAttribute validate;
    float propertyHeight;
    GUIStyle helpBoxStyle => EditorStyles.helpBox; // GUI.skin.GetStyle("helpbox");
    bool valid = true;
    string message;


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      //return base.GetPropertyHeight(property, label);
      var validate = attribute as ValidateAttribute;
      return validate.height;
      //return propertyHeight;
      //return property.isExpanded ? EditorGUI.GetPropertyHeight(property) : 0f;
      //return EditorGUI.GetPropertyHeight(property);
    }

    

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var validate = attribute as ValidateAttribute;

      validate.height = propertyHeight = EditorGUI.GetPropertyHeight(property);
      if (!valid)
      {
        // Draw the message
        helpBoxStyle.richText = true;
        float messageHeight = Mathf.Max(25f, helpBoxStyle.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth));
        position.height = messageHeight;
        EditorGUI.HelpBox(position, message, GetMessageType(validate.level));
        // Adjust the size again
        validate.height = propertyHeight += messageHeight;
        position.y += messageHeight;
        position.height = propertyHeight;
      }

      EditorGUI.BeginChangeCheck();
      EditorGUI.PropertyField(position, property, true);
      
      if (EditorGUI.EndChangeCheck())
      {
        MonoBehaviour mb = property.serializedObject.targetObject as MonoBehaviour;
        MethodInfo method = mb.GetType().GetMethod(validate.method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        message = (string)method.Invoke(mb, null);        
        validate.valid = valid = string.IsNullOrEmpty(message);
      }


    }


    private MessageType GetMessageType(ValidateLevel validateLevel)
    {
      switch (validateLevel)
      {
        default:
        case ValidateLevel.Warning: return MessageType.Warning;
        case ValidateLevel.Error: return MessageType.Error;
      }
    }
  }

}