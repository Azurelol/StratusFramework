using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(StratusBlackboard.Selector))]
  public class BlackboardSelectorDrawer : ObjectPropertySelectorDrawer<StratusBlackboard.Selector>
  {
    protected override string objectPropertyName { get; } = nameof(StratusBlackboard.Selector.blackboard);
    protected override void DrawSelector(Rect position, SerializedProperty property, StratusBlackboard.Selector target)
    {
      // Select the scope
      SerializedProperty scopeProperty = property.FindPropertyRelative(nameof(StratusBlackboard.Selector.scope));
      position.width /= 2f;
      EditorGUI.PropertyField(position, scopeProperty, GUIContent.none);
      position.x += position.width;

      //Types.Symbol[] symbols = null;
      string[] keys = null;
      switch (target.scope)
      {
        case StratusBlackboard.Scope.Local:
          keys = target.blackboard.locals.keys;
          break;
        case StratusBlackboard.Scope.Global:
          keys = target.blackboard.globals.keys;
          break;
      }

      // Select the key
      SerializedProperty keyProperty = property.FindPropertyRelative(nameof(StratusBlackboard.Selector.key));
      int index = keys.FindIndex(keyProperty.stringValue);
      EditorGUI.BeginChangeCheck();
      index = EditorGUI.Popup(position, index, keys);
      if (EditorGUI.EndChangeCheck())
      {
        keyProperty.stringValue = keys[index];
      }
      
    }

  }

}