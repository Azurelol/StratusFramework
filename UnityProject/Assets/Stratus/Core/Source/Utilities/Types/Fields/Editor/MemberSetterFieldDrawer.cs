using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(MemberSetterField))]
  public class MemberSetterFieldDrawer : PropertyDrawer
  {
    private const float lines = 7f;
    private const float padding = 2f;
    private float propertyHeight => (EditorGUIUtility.singleLineHeight + padding) * lines;

    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      if (property.isExpanded)
        return propertyHeight;
      else
        return EditorGUIUtility.singleLineHeight * 2f;

    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var target = property.GetObject<MemberSetterField>(fieldInfo);

      float height = EditorGUIUtility.singleLineHeight + padding;
      var memberProp = property.FindPropertyRelative("member");
      var typeProperty = property.FindPropertyRelative("memberType");
      var type = (ActionProperty.Types)typeProperty.enumValueIndex;
      

      //var sourceType = (UnityMember.SourceType)memberProp.FindPropertyRelative("sourceType").enumValueIndex;

      label = EditorGUI.BeginProperty(position, label, property);
      Rect contentPosition = position;
            

      //EditorGUI.LabelField(contentPosition, new GUIContent(property.displayName)); // property);
      //contentPosition.y += height;
      //EditorGUI.PropertyField(contentPosition, memberProp);

      //contentPosition.x += 10f;
      //EditorGUI.BeginChangeCheck();
      //property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, property.displayName);
      //if (EditorGUI.EndChangeCheck())
      //{
      //  UnityEngine.Event.current.Use();
      //}

      //if (GUILayout.Button(property.displayName, EditorStyles.label))
      //{
      //  property.isExpanded = !property.isExpanded;
      //}

      //contentPosition.y += height;

      EditorGUI.PropertyField(contentPosition, memberProp, new GUIContent(property.displayName));
      contentPosition.height = height;
      //propertyHeight = 2f * height;

      if (target != null)
      {
        target.Validate();
        property.isExpanded = target.member.isAssigned;
      }

      if (property.isExpanded)
      {
        // Indent 
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;
        contentPosition.y += (height * 2f) + 1f;
        
        // Show the value type
        var valueTypeProperty = property.FindPropertyRelative("valueType");
        EditorGUI.PropertyField(contentPosition, valueTypeProperty);

        // Add a suffix to get the dynamic version, if needed
        string suffix = "Static";
        var valueType = (MemberSetterField.ValueType)valueTypeProperty.enumValueIndex;
        if (valueType == MemberSetterField.ValueType.Dynamic)
          suffix = "Dynamic";

        contentPosition.y += height;

        // Set the value
        GUIContent valueLabel = GUIContent.none;
        switch (type)
        {
          case ActionProperty.Types.None:
            EditorGUI.LabelField(contentPosition, valueLabel);
            break;
          case ActionProperty.Types.Integer:            
              EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative($"intValue{suffix}"), valueLabel);
            break;
          case ActionProperty.Types.Float:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative($"floatValue{suffix}"), valueLabel);
            break;
          case ActionProperty.Types.Boolean:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative($"boolValue{suffix}"), valueLabel);
            break;
          case ActionProperty.Types.Vector2:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative($"vector2Value{suffix}"), valueLabel);
            break;
          case ActionProperty.Types.Vector3:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative($"vector3Value{suffix}"), valueLabel);
            break;
          case ActionProperty.Types.Vector4:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative($"vector4Value{suffix}"), valueLabel);
            break;
          case ActionProperty.Types.Color:
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative($"colorValue{suffix}"), valueLabel);
            break;
          default:
            break;
        }


        contentPosition.y += height;
        var durationProp = property.FindPropertyRelative("duration");
        EditorGUI.PropertyField(contentPosition, durationProp);

        contentPosition.y += height + padding;
        var easeProp = property.FindPropertyRelative("ease");
        EditorGUI.PropertyField(contentPosition, easeProp);
        //contentPosition.width /= 2f;

        contentPosition.y += height;
        var toggleprop = property.FindPropertyRelative("toggle");
        EditorGUI.PropertyField(contentPosition, toggleprop);
        //contentPosition.width *= 2f;
        //contentPosition.x += contentPosition.width / 2f;
        EditorGUI.indentLevel = indent;
      }

      EditorGUI.EndProperty();

      if (GUI.changed)
        property.serializedObject.ApplyModifiedProperties();

    }

  }

}