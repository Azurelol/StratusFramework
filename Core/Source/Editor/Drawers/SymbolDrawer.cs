using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(StratusSymbol))]
    public class SymbolDrawer : PropertyDrawer
    {
      //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
      //{
      //  return EditorGUIUtility.singleLineHeight;
      //}

      bool ShowSingleLine = true;

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        var valueProperty = property.FindPropertyRelative(nameof(StratusSymbol.value));
        var typeProperty = valueProperty.FindPropertyRelative("type");
        var type = (StratusVariant.VariantType)typeProperty.enumValueIndex;
        
        label = EditorGUI.BeginProperty(position, label, property);

        if (ShowSingleLine)
        {
          Rect contentPosition = EditorGUI.PrefixLabel(position, label);
          var width = contentPosition.width;
          EditorGUI.indentLevel = 0;
          // Key
          contentPosition.width = width * 0.40f;
          EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative(nameof(StratusSymbol.key)), GUIContent.none);
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
          EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(StratusSymbol.key)));
          position.y += EditorGUIUtility.singleLineHeight;
          EditorGUI.PropertyField(position, valueProperty);

        }
        EditorGUI.EndProperty();
      }      
    }

    //public class SymbolDrawerTwo : SerializedSystemObject.CustomObjectDrawer<Symbol>
    //{
    //  protected override float GetHeight(Symbol value)
    //  {
    //    return lineHeight;
    //  }
    //
    //  protected override void OnDrawEditorGUI(Rect position, Symbol value)
    //  {
    //    throw new System.NotImplementedException();
    //  }
    //
    //  protected override void OnDrawEditorGUILayout(Symbol value)
    //  {
    //    throw new System.NotImplementedException();        
    //  }
    //}

    public class SymbolReferenceDrawer2 : StratusSerializedEditorObject.CustomObjectDrawer<StratusSymbol.Reference>
    {
      protected override float GetHeight(StratusSymbol.Reference value)
      {
        return lineHeight;
      }

      protected override void OnDrawEditorGUI(Rect position, StratusSymbol.Reference value)
      {
        //EditorGUI.LabelField(position, value.key);
        StratusEditorGUI.DrawGUI(position, "Key", ref value.key);
      }      

      protected override void OnDrawEditorGUILayout(StratusSymbol.Reference value)
      {
        StratusEditorGUI.DrawGUILayout("Key", ref value.key);
      }
    }



  }

}