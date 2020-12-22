using UnityEditor;
using UnityEngine;

namespace Stratus
{
	[CustomPropertyDrawer(typeof(StratusEventField))]
	public class EventFieldDrawer : StratusPropertyDrawer
	{
		//private float height = 0;

		//protected override float GetPropertyHeight(SerializedProperty property)
		//{
		//	//return base.GetPropertyHeight(property);
		//	return this.height;
		//}

		//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		//{
		//	SerializedProperty typeProp = property.FindPropertyRelative(nameof(EventField.type));
		//	SerializedProperty scopeProperty = property.FindPropertyRelative(nameof(EventField.scope));
		//	StratusEvent.Scope scope = GetEnumValue<StratusEvent.Scope>(scopeProperty);
		//
		//	//SerializedProperty typeProp = property.FindPropertyRelative("Type");
		//	//Type eventType = typeProp.objectReferenceValue as Type;
		//
		//	label = EditorGUI.BeginProperty(position, label, property);
		//	position = EditorGUI.PrefixLabel(position, label);
		//
		//
		//	float initialHeight = position.y;
		//	this.DrawPropertiesVertical(ref position, typeProp, scopeProperty);
		//
		//	// Scope
		//	if (scope == StratusEvent.Scope.GameObject)
		//	{
		//		this.DrawProperty(ref position, property.FindPropertyRelative(nameof(EventField.targets)));
		//		//AddLine(ref position);
		//	}
		//	//this.height = position.y - initialHeight;
		//	EditorGUI.EndProperty();
		//}

		protected override void OnDrawProperty(Rect position, SerializedProperty property)
		{
			SerializedProperty typeProp = property.FindPropertyRelative(nameof(StratusEventField.type));
			SerializedProperty scopeProperty = property.FindPropertyRelative(nameof(StratusEventField.scope));
			StratusEvent.Scope scope = GetEnumValue<StratusEvent.Scope>(scopeProperty);

			//SerializedProperty typeProp = property.FindPropertyRelative("Type");
			//Type eventType = typeProp.objectReferenceValue as Type;
			this.DrawPropertiesVertical(ref position, typeProp, scopeProperty);

			// Scope
			if (scope == StratusEvent.Scope.GameObject)
			{
				DrawProperty(ref position, property.FindPropertyRelative(nameof(StratusEventField.targets)));
			}



		}
	}
}