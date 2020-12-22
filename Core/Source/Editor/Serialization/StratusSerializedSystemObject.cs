using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Edits System.Object types in a completely generic way
	/// </summary>
	public partial class StratusSerializedEditorObject : StratusSerializedObject
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public ObjectDrawer drawer { get; private set; }

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusSerializedEditorObject(object target) : base(target)
		{
			this.drawer = GetDrawer(target.GetType());
		}

		static StratusSerializedEditorObject()
		{
			foreach (Type drawerType in customObjectDrawers)
			{
				ObjectDrawer drawer = (ObjectDrawer)StratusReflection.Instantiate(drawerType);
				Type objectType = drawer.type;
				objectDrawers.Add(objectType, drawer);
			}
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Serialize(UnityEngine.Object targetObject, SerializedProperty stringProperty)
		{
			string data = JsonUtility.ToJson(this.target);
			stringProperty.stringValue = data;
			Undo.RecordObject(targetObject, stringProperty.displayName);
			stringProperty.serializedObject.ApplyModifiedProperties();
		}

		public void Serialize(SerializedProperty stringProperty)
		{
			string data = JsonUtility.ToJson(this.target);
			stringProperty.stringValue = data;
			Undo.RecordObject(stringProperty.serializedObject.targetObject, stringProperty.displayName);
			stringProperty.serializedObject.ApplyModifiedProperties();
		}

		public void Deserialize(SerializedProperty stringProperty)
		{
			string data = stringProperty.stringValue;
			JsonUtility.FromJsonOverwrite(data, this.target);
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		public static ObjectDrawer GetObjectDrawer(object element)
		{
			Type elementType = element.GetType();
			if (!objectDrawers.ContainsKey(elementType))
			{
				objectDrawers.Add(elementType, new StratusSerializedEditorObject.DefaultObjectDrawer(elementType));
			}

			ObjectDrawer drawer = objectDrawers[elementType];
			return drawer;
		}

		public static ObjectDrawer GetObjectDrawer(Type elementType)
		{
			if (!objectDrawers.ContainsKey(elementType))
			{
				objectDrawers.Add(elementType, new StratusSerializedEditorObject.DefaultObjectDrawer(elementType));
			}

			ObjectDrawer drawer = objectDrawers[elementType];
			return drawer;
		}

		public static FieldDrawer GetFieldDrawer(FieldInfo field)
		{
			if (!fieldDrawers.ContainsKey(field))
			{
				fieldDrawers.Add(field, new StratusSerializedEditorObject.FieldDrawer(field));
			}

			return fieldDrawers[field];
		}		
	}
}
