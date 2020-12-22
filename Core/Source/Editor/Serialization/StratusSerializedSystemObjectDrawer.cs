using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	public partial class StratusSerializedEditorObject
	{
		/// <summary>
		/// Base class for all drawers
		/// </summary>
		public abstract class Drawer
		{
			//------------------------------------------------------------------------/
			// Declarations
			//------------------------------------------------------------------------/
			public struct Arguments
			{
				public object target;
				public bool isChild;
			}

			//------------------------------------------------------------------------/
			// Properties
			//------------------------------------------------------------------------/
			public string name { get; protected set; }
			public string displayName { get; protected set; }
			public Type type { get; protected set; }
			public abstract bool DrawEditorGUILayout(object target, bool isChild = false);
			public abstract bool DrawEditorGUI(Rect position, object target);
			//public abstract bool DrawGUILayout(Rect rectobject target, bool isChild = false);
			public bool isDrawable { get; protected set; }
			public bool isPrimitive { get; protected set; }
			public float height { get; protected set; }
			public static float lineHeight => StratusEditorUtility.lineHeight;
			public static float labelWidth => StratusEditorUtility.labelWidth;

			//------------------------------------------------------------------------/
			// Methods
			//------------------------------------------------------------------------/
			internal void SetDisplayName(string name)
			{
				this.displayName = ObjectNames.NicifyVariableName(name);
			}
		}

		/// <summary>
		/// Base class for all drawers
		/// </summary>
		public abstract class ObjectDrawer : Drawer
		{
			public FieldInfo[] fields { get; private set; }
			public Dictionary<string, FieldInfo> fieldsByName { get; private set; } = new Dictionary<string, FieldInfo>();
			public int fieldCount => this.fields.Length;
			public bool hasFields => this.fields.NotEmpty();
			public bool hasDefaultConstructor { get; private set; }

			public ObjectDrawer(Type type)
			{
				this.type = type;
				MemberInfo[] members = OdinSerializer.FormatterUtilities.GetSerializableMembers(type, OdinSerializer.SerializationPolicies.Unity);
				this.fields = members.OfType<FieldInfo>().ToArray();
				this.fieldsByName.AddRange((FieldInfo field) => field.Name, this.fields);
				this.hasDefaultConstructor = (type.GetConstructor(Type.EmptyTypes) != null) || type.IsValueType;
			}
		}

		public struct DrawCommand
		{
			public Drawer drawer;
			public FieldInfo field;
			public bool isField;

			public DrawCommand(Drawer drawer, FieldInfo field, bool isField)
			{
				this.drawer = drawer;
				this.field = field;
				this.isField = isField;
			}
		}

		//------------------------------------------------------------------------/
		// Properties: Static
		//------------------------------------------------------------------------/
		private static Dictionary<Type, ObjectDrawer> objectDrawers { get; set; } = new Dictionary<Type, ObjectDrawer>();
		private static Dictionary<FieldInfo, FieldDrawer> fieldDrawers { get; set; } = new Dictionary<FieldInfo, FieldDrawer>();
		private static Type[] customObjectDrawers { get; set; } = StratusReflection.GetSubclass<CustomObjectDrawer>(false);

		//------------------------------------------------------------------------/
		// Methods: Static
		//------------------------------------------------------------------------/
		/// <summary>
		/// Gets the object drawer for the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ObjectDrawer GetDrawer(Type type)
		{
			if (!objectDrawers.ContainsKey(type))
			{
				objectDrawers.Add(type, new DefaultObjectDrawer(type));
			}

			return objectDrawers[type];
		}

		/// <summary>
		/// Gets the object drawer for the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ObjectDrawer GetDrawer(FieldInfo field)
		{
			Type type = field.FieldType;
			if (!objectDrawers.ContainsKey(type))
			{
				objectDrawers.Add(type, new DefaultObjectDrawer(field, type));
			}

			ObjectDrawer drawer = objectDrawers[type];
			//drawer.SetDisplayName(field.Name);
			return drawer;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public bool DrawEditorGUILayout()
		{
			return this.drawer.DrawEditorGUILayout(this.target);
		}

		public bool DrawEditorGUI(Rect position)
		{
			return this.drawer.DrawEditorGUI(position, this.target);
		}

	}

}