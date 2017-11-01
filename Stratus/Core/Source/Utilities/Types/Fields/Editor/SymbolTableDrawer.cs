using UnityEngine;
using Stratus;
using UnityEditor;
using Rotorz.ReorderableList;

namespace Stratus
{
  namespace Types
  {
    [CustomPropertyDrawer(typeof(Symbol.Table), true)]
    public class SymbolTableDrawer : PropertyDrawer
    {
      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
        var symbols = property.FindPropertyRelative("Symbols");
        ReorderableListGUI.Title(label);
        ReorderableListGUI.ListField(symbols);        
      }
    } 
  }

}