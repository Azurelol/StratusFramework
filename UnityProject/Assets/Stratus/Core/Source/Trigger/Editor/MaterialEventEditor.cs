using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomEditor(typeof(MaterialEvent))]
  public class MaterialEventEditor : TriggerableEditor
  {
    MaterialEvent materialEvent => target as MaterialEvent;

    protected override void OnBaseEditorEnable()
    {

    }

    protected override void DrawDeclaredProperties()
    {
      DrawSerializedProperty(propertyMap["material"], serializedObject);
      DrawSerializedProperty(propertyMap["type"], serializedObject);
      switch (materialEvent.type)
      {
        case MaterialEvent.Type.Color:
          DrawSerializedProperty(propertyMap["color"], serializedObject);
          break;
        case MaterialEvent.Type.SetColor:
          DrawSerializedProperty(propertyMap["propertyName"], serializedObject);
          DrawSerializedProperty(propertyMap["color"], serializedObject);
          break;
        case MaterialEvent.Type.SetFloat:
          DrawSerializedProperty(propertyMap["propertyName"], serializedObject);
          DrawSerializedProperty(propertyMap["floatValue"], serializedObject);
          break;
        case MaterialEvent.Type.SetInteger:
          DrawSerializedProperty(propertyMap["propertyName"], serializedObject);
          DrawSerializedProperty(propertyMap["integerValue"], serializedObject);
          break;
        case MaterialEvent.Type.SetTexture:
          DrawSerializedProperty(propertyMap["propertyName"], serializedObject);
          DrawSerializedProperty(propertyMap["texture"], serializedObject);
          break;
        case MaterialEvent.Type.Lerp:
          DrawSerializedProperty(propertyMap["material2"], serializedObject);
          break;
        default:
          break;
      }
      DrawSerializedProperty(propertyMap["duration"], serializedObject);
    }

  }

}