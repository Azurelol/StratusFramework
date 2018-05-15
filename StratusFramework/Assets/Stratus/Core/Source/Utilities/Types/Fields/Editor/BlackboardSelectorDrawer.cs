using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
  [CustomPropertyDrawer(typeof(Blackboard.Selector))]
  public class BlackboardSelectorDrawer : ObjectPropertySelectorDrawer<Blackboard.Selector>
  {
    protected override Lines mode { get; } = Lines.Double;
    protected override string objectPropertyName { get; } = nameof(Blackboard.Selector.blackboard);
    protected override void DrawSelector(Rect position, SerializedProperty property, Blackboard.Selector target)
    {
      SerializedProperty scopeProperty = property.FindPropertyRelative(nameof(Blackboard.Selector.scope));
      position.width /= 2f;
      EditorGUI.PropertyField(position, scopeProperty, GUIContent.none);
      position.x += position.width;
      string[] keys = null;
      switch (target.scope)
      {
        case Blackboard.Scope.Local:
          break;
        case Blackboard.Scope.Global:
          break;
      }
      string key;
      //EditorGUI.Popup(position, )
    }

  }

}