using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(InputAction), true)]
  public class InputActionDrawer : StratusPropertyDrawer
  {
    protected override float GetPropertyHeight(SerializedProperty property)
    {
      //return lineHeight;
      // ????
      return propertyHeight / 2f;
      //return lineHeight * 4f;
    }
  
    protected override void DrawProperty(Rect position, SerializedProperty property)
    {      
      SerializedProperty inputProperty = property.FindPropertyRelative(nameof(InputAction.input));
      SerializedProperty typeProp = inputProperty.FindPropertyRelative("_type");
      var type = (InputField.Type)typeProp.enumValueIndex;

      SerializedProperty stateProperty= property.FindPropertyRelative(nameof(InputAction.state));

      SerializedProperty callbackProperty = null;
      switch (type)
      {
        case InputField.Type.Key:
        case InputField.Type.MouseButton:
          callbackProperty = property.FindPropertyRelative(nameof(InputAction.onInput));
          break;
        case InputField.Type.Axis:
          callbackProperty = property.FindPropertyRelative(nameof(InputAction.onAxisInput));
          break;
      }

      DrawSingleProperty(ref position, inputProperty);
      DrawSingleProperty(ref position, stateProperty);
      DrawSingleProperty(ref position, callbackProperty);

      //InputField input = inputProperty.Getv as System.Object as InputField;
  
      //switch (input.type)
      //{
      //  case InputField.Type.Key:
      //    break;
      //  case InputField.Type.MouseButton:
      //    SerializedProperty axisType = property.FindPropertyRelative(nameof(InputAction.axisType));
      //    DrawSingleProperty(ref position, axisType);
      //    break;
      //  case InputField.Type.Axis:
      //    break;
      //}   
  
  
  
      
    }
  }
  

}