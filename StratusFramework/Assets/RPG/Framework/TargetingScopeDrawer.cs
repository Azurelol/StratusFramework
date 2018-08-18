/******************************************************************************/
/*!
@file   TargetingScopeDrawer.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using Stratus;
using UnityEditor;

namespace Altostratus
{
  /**************************************************************************/
  /*!
  @class TargetingScopeDrawer 
  */
  /**************************************************************************/
  [CustomPropertyDrawer(typeof(TargetingScope))]
  public class TargetingScopeDrawer : PropertyDrawer
  {
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Using BeginProperty / EndProperty on the parent property means that
      // prefab override logic works on the entire property.
      EditorGUI.BeginProperty(position, label, property);

      // Draw label
      //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

      // Draw the navigation property
      var propertyScope = property.FindPropertyRelative("Scope");
      EditorGUILayout.PropertyField(propertyScope, new GUIContent("Scope"));

      EditorGUI.indentLevel = 1;

      // Draw fields
      var scope = (TargetingScope.Type)propertyScope.enumValueIndex;
      switch (scope)
      {
        case TargetingScope.Type.Single:
          break;
        case TargetingScope.Type.Radius:
          EditorGUILayout.PropertyField(property.FindPropertyRelative("Length"), new GUIContent("Radius"));
          break;
        case TargetingScope.Type.Line:
          EditorGUILayout.PropertyField(property.FindPropertyRelative("Length"), new GUIContent("Length"));
          EditorGUILayout.PropertyField(property.FindPropertyRelative("Width"), new GUIContent("Width"));
          break;
      }

      EditorGUI.indentLevel = 0;

      EditorGUI.EndProperty();
    }

  }

} 
#endif