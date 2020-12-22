using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Clone of an internal Unity class that does stuff like keep track of PropertyHandlers and finding Attributes the inspector uses for drawing
	/// </summary>
	internal static class StratusScriptAttributeUtility
	{


		public struct DrawerKeySet
		{
			public Type drawer;
			public Type type;
		}

		internal static Stack<PropertyDrawer> s_DrawerStack = new Stack<PropertyDrawer>();

		private static Dictionary<Type, DrawerKeySet> s_DrawerTypeForType = null;

		private static Dictionary<string, List<PropertyAttribute>> s_BuiltinAttributes = null;

		private static StratusSerializedPropertyHandler s_SharedNullHandler = new StratusSerializedPropertyHandler();

		private static StratusSerializedPropertyHandler s_NextHandler = new StratusSerializedPropertyHandler();

		private static StratusPropertyHandlerCache s_GlobalCache = new StratusPropertyHandlerCache();

		private static StratusPropertyHandlerCache s_CurrentCache = null;

		//static StratusScriptAttributeUtility()
		//{
		//	BuildDrawerTypeForTypeDictionary();
		//}

		public static StratusSerializedPropertyHandler defaultPropertyHandler => s_SharedNullHandler;
		//public static Dictionary<Type, DrawerKeySet> drawerForType => s_DrawerTypeForType;

		internal static StratusPropertyHandlerCache propertyHandlerCache
		{
			get => s_CurrentCache ?? s_GlobalCache;
			set => s_CurrentCache = value;
		}

		internal static void ClearGlobalCache() // TODO in Unity this is called by InspectorWindow.OnSelectionChange(), need to trigger this outselves from some selection change event
		{
			s_GlobalCache.Clear();
		}

		private static void PopulateBuiltinAttributes()
		{
			s_BuiltinAttributes = new Dictionary<string, List<PropertyAttribute>>();
			AddBuiltinAttribute("GUIText", "m_Text", new MultilineAttribute());
			AddBuiltinAttribute("TextMesh", "m_Text", new MultilineAttribute());
		}

		private static void AddBuiltinAttribute(string componentTypeName, string propertyPath, PropertyAttribute attr)
		{
			string key = componentTypeName + "_" + propertyPath;
			if (!s_BuiltinAttributes.ContainsKey(key))
			{
				s_BuiltinAttributes.Add(key, new List<PropertyAttribute>());
			}
			s_BuiltinAttributes[key].Add(attr);
		}

		private static List<PropertyAttribute> GetBuiltinAttributes(SerializedProperty property)
		{
			if (property.serializedObject.targetObject == null)
			{
				return null;
			}
			Type type = property.serializedObject.targetObject.GetType();
			if (type == null)
			{
				return null;
			}
			string key = type.Name + "_" + property.propertyPath;
			s_BuiltinAttributes.TryGetValue(key, out List<PropertyAttribute> result);
			return result;
		}

		private static void BuildDrawerTypeForTypeDictionary()
		{
			s_DrawerTypeForType = new Dictionary<Type, DrawerKeySet>();
			Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => StratusReflection.GetTypesFromAssembly(x)).ToArray();
			foreach (Type item in SubclassesOf(typeof(GUIDrawer)))
			{
				object[] customAttributes = item.GetCustomAttributes(typeof(CustomPropertyDrawer), true);
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					CustomPropertyDrawer editor = (CustomPropertyDrawer)array[i];
					Type editorType = editor.GetHiddenType();

					s_DrawerTypeForType[editorType] = new DrawerKeySet
					{
						drawer = item,
						type = editorType
					};
					if (editor.GetUseForChildren())
					{
						IEnumerable<Type> enumerable = from x in source
													   where x.IsSubclassOf(editorType)
													   select x;
						foreach (Type item2 in enumerable)
						{
							if (s_DrawerTypeForType.ContainsKey(item2))
							{
								Type type = editorType;
								DrawerKeySet drawerKeySet = s_DrawerTypeForType[item2];
								if (!type.IsAssignableFrom(drawerKeySet.type))
								{
									// Was in unity's decompiled source, so not touching for now
									goto IL_0158;
								}
								continue;
							}
							goto IL_0158;
							IL_0158:
							s_DrawerTypeForType[item2] = new DrawerKeySet
							{
								drawer = item,
								type = editorType
							};
						}
					}
				}
			}
		}

		internal static Type GetDrawerTypeForType(Type type)
		{
			if (s_DrawerTypeForType == null)
			{
				BuildDrawerTypeForTypeDictionary();
			}
			s_DrawerTypeForType.TryGetValue(type, out DrawerKeySet drawerKeySet);
			if (drawerKeySet.drawer != null)
			{
				return drawerKeySet.drawer;
			}
			if (type.IsGenericType)
			{
				s_DrawerTypeForType.TryGetValue(type.GetGenericTypeDefinition(), out drawerKeySet);
			}
			return drawerKeySet.drawer;
		}

		private static List<PropertyAttribute> GetFieldAttributes(FieldInfo field)
		{
			if (field == null)
			{
				return null;
			}
			object[] customAttributes = field.GetCustomAttributes(typeof(PropertyAttribute), true);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				return new List<PropertyAttribute>(from e in customAttributes
												   select e as PropertyAttribute into e
												   orderby -e.order
												   select e);
			}
			return null;
		}

		private static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
		{
			Type scriptTypeFromProperty = GetScriptTypeFromProperty(property);
			if (scriptTypeFromProperty == null)
			{
				type = null;
				return null;
			}
			return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath, out type);
		}

		private static Type GetScriptTypeFromProperty(SerializedProperty property)
		{
			SerializedProperty serializedProperty = property.serializedObject.FindProperty("m_Script");
			if (serializedProperty == null)
			{
				return null;
			}
			MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
			if (monoScript == null)
			{
				return null;
			}
			return monoScript.GetClass();
		}

		private static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path, out Type type)
		{
			FieldInfo fieldInfo = null;
			type = host;
			string[] array = path.Split('.');
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (i < array.Length - 1 && text == "Array" && array[i + 1].StartsWith("data["))
				{
					if (type.IsArrayOrList())
					{
						type = type.GetArrayOrListElementType();
					}
					i++;
				}
				else
				{
					FieldInfo fieldInfo2 = null;
					Type type2 = type;
					while (fieldInfo2 == null && type2 != null)
					{
						fieldInfo2 = type2.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						type2 = type2.BaseType;
					}
					if (fieldInfo2 == null)
					{
						type = null;
						return null;
					}
					fieldInfo = fieldInfo2;
					type = fieldInfo.FieldType;
				}
			}
			return fieldInfo;
		}

		internal static StratusSerializedPropertyHandler GetHandler(SerializedProperty property)
		{
			if (property == null)
			{
				return s_SharedNullHandler;
			}
			if (property.serializedObject.GetInspectorMode() != 0)
			{
				return s_SharedNullHandler;
			}
			StratusSerializedPropertyHandler handler = propertyHandlerCache.GetHandler(property);
			if (handler != null)
			{
				return handler;
			}
			Type type = null;
			List<PropertyAttribute> list = null;
			FieldInfo field = null;
			UnityEngine.Object targetObject = property.serializedObject.targetObject;
			if (targetObject is MonoBehaviour || targetObject is ScriptableObject)
			{
				field = GetFieldInfoFromProperty(property, out type);
				list = GetFieldAttributes(field);
			}
			else
			{
				if (s_BuiltinAttributes == null)
				{
					PopulateBuiltinAttributes();
				}
				if (list == null)
				{
					list = GetBuiltinAttributes(property);
				}
			}
			handler = s_NextHandler;
			if (list != null)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					handler.HandleAttribute(list[num], field, type);
				}
			}
			if (!handler.HasPropertyDrawer && type != null)
			{
				handler.HandleDrawnType(type, type, field, null);
			}
			if (handler.Empty)
			{
				propertyHandlerCache.SetHandler(property, s_SharedNullHandler);
				handler = s_SharedNullHandler;
			}
			else
			{
				propertyHandlerCache.SetHandler(property, handler);
				s_NextHandler = new StratusSerializedPropertyHandler();
			}
			return handler;
		}

		internal static IEnumerable<Type> SubclassesOf(Type parent)
		{
			Type hiddenType = StratusReflection.GetPrivateType("UnityEditor.EditorAssemblies", typeof(CustomEditor));
			return (IEnumerable<Type>)StratusReflection.GetReflectedMethod("SubclassesOf", hiddenType).Invoke(null, new object[] { parent });
		}
	}

	// Stores PropertyHandlers for drawing properties in a dictionary against property hashes 
	internal class StratusPropertyHandlerCache
	{
		protected Dictionary<int, StratusSerializedPropertyHandler> m_PropertyHandlers = new Dictionary<int, StratusSerializedPropertyHandler>();

		internal StratusSerializedPropertyHandler GetHandler(SerializedProperty property)
		{
			int propertyHash = GetPropertyHash(property);
			if (this.m_PropertyHandlers.TryGetValue(propertyHash, out StratusSerializedPropertyHandler result))
			{
				return result;
			}
			return null;
		}

		internal void SetHandler(SerializedProperty property, StratusSerializedPropertyHandler handler)
		{
			int propertyHash = GetPropertyHash(property);
			this.m_PropertyHandlers[propertyHash] = handler;
		}

		private static int GetPropertyHash(SerializedProperty property)
		{
			if (property.serializedObject.targetObject == null)
			{
				return 0;
			}
			int num = property.serializedObject.targetObject.GetInstanceID() ^ property.GetHashCodeForPropertyPathWithoutArrayIndex();
			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				num ^= property.objectReferenceInstanceIDValue;
			}
			return num;
		}

		public void Clear()
		{
			this.m_PropertyHandlers.Clear();
		}
	}

}