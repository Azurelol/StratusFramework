using Stratus;
using UnityEditor;
using UnityEngine;

namespace Stratus.Gameplay
{
	[CustomPropertyDrawer(typeof(StratusScopeTargeting))]
	public class ScopeTargetingDrawer : StratusPropertyDrawer
	{
		// Draw the property inside the given rect
		//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		//{
		//  // Using BeginProperty / EndProperty on the parent property means that
		//  // prefab override logic works on the entire property.
		//  EditorGUI.BeginProperty(position, label, property);
		//
		//  // Draw label
		//  //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		//
		//  // Draw the navigation property
		//  var propertyScope = property.FindPropertyRelative(nameof(TargetingScope.scope));
		//  EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.range)));
		//  EditorGUILayout.PropertyField(propertyScope, new GUIContent("Scope"));
		//
		//  //EditorGUI.indentLevel = 1;
		//
		//  // Draw fields
		//  var scope = (TargetingScope.Type)propertyScope.enumValueIndex;
		//  switch (scope)
		//  {
		//    case TargetingScope.Type.Single:
		//      break;
		//    case TargetingScope.Type.Radius:
		//      EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.length)), new GUIContent("Radius"));
		//      break;
		//    case TargetingScope.Type.Line:
		//      EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.length)), new GUIContent("Length"));
		//      EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(TargetingScope.width)), new GUIContent("Width"));
		//      break;
		//  }
		//
		//  //EditorGUI.indentLevel = 0;
		//
		//  EditorGUI.EndProperty();
		//}

		protected override void OnDrawProperty(Rect position, SerializedProperty property)
		{
			SerializedProperty scopeProperty = property.FindPropertyRelative(nameof(StratusScopeTargeting.scope));
			SerializedProperty rangeProperty = property.FindPropertyRelative(nameof(StratusScopeTargeting.range));
			DrawPropertiesHorizontal(ref position, scopeProperty, rangeProperty);

			//EditorGUI.indentLevel = 1;

			// Draw fields
			StratusScopeTargeting.Type scope = (StratusScopeTargeting.Type)scopeProperty.enumValueIndex;
			switch (scope)
			{
				case StratusScopeTargeting.Type.Single:
					break;
				case StratusScopeTargeting.Type.Radius:
					EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(StratusScopeTargeting.length)), new GUIContent("Radius"));
					break;
				case StratusScopeTargeting.Type.Line:
					EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(StratusScopeTargeting.length)), new GUIContent("Length"));
					EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(StratusScopeTargeting.width)), new GUIContent("Width"));
					break;
			}
		}
	}

}