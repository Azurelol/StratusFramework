using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stratus.Utilities;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Stratus
{
	public static partial class StratusEditorUtility
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public enum ContextMenuType
		{
			Add,
			Validation,
			Options
		}

		public delegate bool DefaultPropertyFieldDelegate(Rect position, SerializedProperty property, GUIContent label);

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static UnityEngine.Event currentEvent => UnityEngine.Event.current;
		public static bool currentEventUsed => currentEvent.type == EventType.Used;
		public static bool onRepaint => currentEvent.type == EventType.Repaint;
		public static Rect lastRect => GUILayoutUtility.GetLastRect();
		public static float lineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		public static float verticalSpacing => EditorGUIUtility.standardVerticalSpacing;
		public static float labelWidth => EditorGUIUtility.labelWidth;
		public static DefaultPropertyFieldDelegate defaultPropertyField { get; private set; }
		private static Dictionary<int, float> abstractListHeights { get; set; } = new Dictionary<int, float>();
		public static Rect lastEditorGUILayoutRect
		{
			get => StratusReflection.GetField<Rect>("s_LastRect", typeof(UnityEditor.EditorGUILayout));
			set => StratusReflection.SetField<Rect>("s_LastRect", typeof(UnityEditor.EditorGUILayout), value);
		}

		private static Assembly _unityEditorAssembly;
		public static Assembly UnityEditorAssembly
		{
			get
			{
				if (_unityEditorAssembly == null)
				{
					_unityEditorAssembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
				}

				return _unityEditorAssembly;
			}
		}

		private static Type _scriptAttributeUtility;
		public static Type scriptAttributeUtility
		{
			get
			{
				if (_scriptAttributeUtility == null)
				{
					_scriptAttributeUtility = UnityEditorAssembly.GetType("UnityEditor.ScriptAttributeUtility");
				}

				return _scriptAttributeUtility;
			}
		}

		private static StratusTypeReflection _scriptAttributeReflection;
		public static StratusTypeReflection scriptAttributeReflection
		{
			get
			{
				if (_scriptAttributeReflection == null)
				{
					_scriptAttributeReflection = new StratusTypeReflection(scriptAttributeUtility);
				}
				return _scriptAttributeReflection;
			}
		}

		private static StratusTypeReflection editorGUIReflection { get; } = new StratusTypeReflection(typeof(EditorGUI));

		private static Stack<PropertyDrawer> _propertyDrawerStack;
		public static Stack<PropertyDrawer> propertyDrawerStack
		{
			get
			{
				if (_propertyDrawerStack == null)
				{
					_propertyDrawerStack = (Stack<PropertyDrawer>)scriptAttributeUtility.GetField("s_DrawerStack",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
				}
				return _propertyDrawerStack;
			}
		}

		private static IDictionary _propertyDrawerTypeForType = null;
		public static IDictionary propertyDrawerTypeForType
		{
			get
			{
				if (_propertyDrawerTypeForType == null)
				{
					_propertyDrawerTypeForType = (IDictionary)scriptAttributeReflection.fieldsByName.GetValueOrNull("s_DrawerTypeForType").GetValue(null);
				}

				return _propertyDrawerTypeForType;
			}
		}

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		static StratusEditorUtility()
		{
			Type t = typeof(EditorGUI);
			Type delegateType = typeof(DefaultPropertyFieldDelegate);
			MethodInfo m = t.GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.NonPublic);
			defaultPropertyField = (DefaultPropertyFieldDelegate)System.Delegate.CreateDelegate(delegateType, m);
		}


		public static void UseDefaultDrawer(Rect position, SerializedProperty property, GUIContent label, Type type)
		{
			//var drawer =  StratusScriptAttributeUtility.s_DrawerStack.Pop();
			//EditorGUI.MultiPropertyField
			//var drawer = StratusScriptAttributeUtility.drawerForType[type];
			//editorGUIReflection.ToString();
			defaultPropertyField(position, property, label);
			//StratusSerializedPropertyHandler handler = StratusScriptAttributeUtility.GetHandler(property);
			//EditorGUI.ProgressBar()
			//handler.OnGUI(position, property, label, true);
			//StratusScriptAttributeUtility.s_DrawerStack.Push(drawer);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static T Instantiate<T>()
		{
			return Utilities.StratusReflection.Instantiate<T>();
		}

		public static object Instantiate(Type type)
		{
			return Utilities.StratusReflection.Instantiate(type);
		}

		public static void OnMouseClick(System.Action onLeftClick, System.Action onRightClick, System.Action onDoubleClick, bool used = false)
		{
			if (!used && !currentEvent.isMouse)
			{
				return;
			}

			//bool clicked = currentEvent.type == EventType.MouseUp || currentEvent.type == EventType.MouseDown;
			//if (!clicked)
			//  return;

			int button = currentEvent.button;
			// Left click
			if (button == 0)
			{
				if (currentEvent.clickCount == 1)
				{
					onLeftClick?.Invoke();
				}
				else if (currentEvent.clickCount > 1)
				{
					onDoubleClick?.Invoke();
				}

				currentEvent.Use();
			}
			// Right click
			else if (button == 1)
			{
				onRightClick?.Invoke();
				currentEvent.Use();
			}
		}

		public static void OnMouseClick(Rect rect, System.Action onLeftClick, System.Action onRightClick, System.Action onDoubleClick = null)
		{
			if (!IsMousedOver(rect))
			{
				return;
			}

			OnMouseClick(onLeftClick, onRightClick, onDoubleClick);
		}

		public static void OnLastControlMouseClick(System.Action onLeftClick, System.Action onRightClick, System.Action onDoubleClick = null)
		{
			if (!IsMousedOver(GUILayoutUtility.GetLastRect()))
			{
				return;
			}

			OnMouseClick(onLeftClick, onRightClick, onDoubleClick);
		}

		/// <summary>
		/// Checks whether the mouse was within the boundaries of the last control
		/// </summary>
		/// <returns></returns>
		public static bool IsLastControlMousedOver()
		{
			Rect rect = GUILayoutUtility.GetLastRect();
			return IsMousedOver(rect);
		}

		/// <summary>
		/// Checks whether the mouse was within the boundaries of the last control
		/// </summary>
		/// <returns></returns>
		public static bool IsMousedOver(Rect rect)
		{
			return rect.Contains(UnityEngine.Event.current.mousePosition);
		}

		/// <summary>
		/// Returns true if a GUI control was changed within the procedure
		/// </summary>
		/// <param name="procedure"></param>
		/// <returns></returns>
		public static bool CheckControlChange(System.Action procedure)
		{
			EditorGUI.BeginChangeCheck();
			procedure();
			return EditorGUI.EndChangeCheck();
		}

		/// <summary>
		/// If a GUI control was changed, saves the state of the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static void SaveOnControlChange(UnityEngine.Object obj, System.Action procedure)
		{
			EditorGUI.BeginChangeCheck();
			procedure();
			if (EditorGUI.EndChangeCheck())
			{
				SerializedObject serializedObject = new SerializedObject(obj);
				serializedObject.UpdateIfRequiredOrScript();
				serializedObject.ApplyModifiedProperties();
				UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
				StratusDebug.Log($"Saving change on {obj.name}");
			}
		}

		/// <summary>
		/// Modifies the given property on the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool ModifyProperty(UnityEngine.Object obj, string propertyName, params GUILayoutOption[] options)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty prop = serializedObject.FindProperty(propertyName);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(prop, options);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Modifies the given property on the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool ModifyProperty(UnityEngine.Object obj, string propertyName, GUIContent label, params GUILayoutOption[] options)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty prop = serializedObject.FindProperty(propertyName);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(prop, label, options);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Modifies all the given properties on the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static void ModifyProperties(UnityEngine.Object obj, string[] propertyNames, params GUILayoutOption[] options)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			EditorGUI.BeginChangeCheck();
			foreach (string name in propertyNames)
			{
				SerializedProperty prop = serializedObject.FindProperty(name);
				EditorGUILayout.PropertyField(prop, options);
			}
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		/// <summary>
		/// If a GUI control was changed, saves the state of the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool Toggle(UnityEngine.Object obj, string propertyName, string label = null)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty property = serializedObject.FindProperty(propertyName);
			//SerializedProperty property = FindSerializedProperty(obj, propertyName);
			return Toggle(serializedObject, property, label);
		}

		/// <summary>
		/// If a GUI control was changed, saves the state of the object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static bool Toggle(SerializedObject serializedObject, SerializedProperty prop, string label = null)
		{
			EditorGUI.BeginChangeCheck();
			GUIContent content = new GUIContent(label == null ? prop.displayName : label);
			prop.boolValue = GUILayout.Toggle(prop.boolValue, content);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
			return prop.boolValue;
		}

		/// <summary>
		/// Finds the serialized property from a given object
		/// </summary>
		/// <param name="procedure"></param>
		/// <param name="obj"></param>
		public static SerializedProperty FindSerializedProperty(UnityEngine.Object obj, string propertyName)
		{
			SerializedObject serializedObject = new SerializedObject(obj);
			SerializedProperty prop = serializedObject.FindProperty(propertyName);
			return prop;
		}

		/// <summary>
		/// Disables mouse selection behind the given rect
		/// </summary>
		/// <param name="rect"></param>
		public static void DisableMouseSelection(Rect rect)
		{
			if (IsMousedOver(rect))
			{
				//int currentControl = GUIUtility.hotControl;
				int control = GUIUtility.GetControlID(FocusType.Passive);
				GUIUtility.hotControl = control;
				//if (currentEvent.type == EventType.MouseDrag)
				//  currentEvent.Use();
			}
		}

		/// <summary>
		/// Add define symbols as soon as Unity gets done compiling.
		/// </summary>
		public static void AddDefineSymbols(string[] symbols)
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = new List<string>(definesString.Split(';'));
			allDefines.AddRange(symbols.Except(allDefines));
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				EditorUserBuildSettings.selectedBuildTargetGroup,
				string.Join(";", allDefines.ToArray()));
		}

		/// <summary>
		/// Selects the subset of a given set of elements, organized vertically
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="set"></param>
		/// <param name="subset"></param>
		public static void SelectSubset<T>(T[] set, List<T> subset, Func<T, string> name)
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Available", EditorStyles.centeredGreyMiniLabel);
					foreach (T element in set)
					{
						T matchingElement = subset.Find(x => x.Equals(element));
						if (matchingElement != null)
						{
							continue;
						}

						if (GUILayout.Button(name(element), EditorStyles.miniButton))
						{
							subset.Add(element);
						}
					}
				}
				EditorGUILayout.EndVertical();

				// Selected scenes
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Selected", EditorStyles.centeredGreyMiniLabel);
					int indexToRemove = -1;
					for (int i = 0; i < subset.Count; ++i)
					{
						T element = subset[i];
						if (GUILayout.Button(name(element), EditorStyles.miniButton))
						{
							indexToRemove = i;
						}
					}
					if (indexToRemove > -1)
					{
						subset.RemoveAt(indexToRemove);
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Selects the subset of a given set of elements, organized vertically
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="set"></param>
		/// <param name="subset"></param>
		public static void SelectSubset<T>(T[] set, List<T> subset) where T : UnityEngine.Object
		{
			SelectSubset(set, subset, GetName);
		}

		public static void DrawHeader(string text)
		{
			EditorGUILayout.LabelField("Effects", StratusGUIStyles.header);
		}

		private static string GetName<T>(T obj) where T : UnityEngine.Object
		{
			return obj.name;
		}

		public static bool ObjectField(FieldInfo field, object obj, GUIContent content = null)
		{
			object value = field.GetValue(obj);
			EditorGUI.BeginChangeCheck();
			{
				string name = ObjectNames.NicifyVariableName(field.Name);

				if (value is UnityEngine.Object)
				{
					field.SetValue(obj, EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, field.FieldType, true));
				}
				else if (value is bool)
				{
					field.SetValue(obj, EditorGUILayout.Toggle(name, (bool)value));
				}
				else if (value is int)
				{
					field.SetValue(obj, EditorGUILayout.IntField(name, (int)value));
				}
				else if (value is float)
				{
					field.SetValue(obj, EditorGUILayout.FloatField(name, (float)value));
				}
				else if (value is string)
				{
					field.SetValue(obj, EditorGUILayout.TextField(name, (string)value));
				}
				else if (value is Enum)
				{
					field.SetValue(obj, EditorGUILayout.EnumPopup(name, (Enum)value));
				}
				else if (value is Vector2)
				{
					field.SetValue(obj, EditorGUILayout.Vector2Field(name, (Vector2)value));
				}
				else if (value is Vector3)
				{
					field.SetValue(obj, EditorGUILayout.Vector3Field(name, (Vector3)value));
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				return true;
			}

			return false;
		}

		public static void DrawContextMenu(GenericMenu menu, ContextMenuType style)
		{
			Texture texture = null;
			switch (style)
			{
				case ContextMenuType.Add:
					texture = StratusGUIStyles.addIcon;
					break;
				case ContextMenuType.Validation:
					texture = StratusGUIStyles.validateIcon;
					break;
				case ContextMenuType.Options:
					texture = StratusGUIStyles.optionsIcon;
					break;
			}
			if (GUILayout.Button(texture, StratusGUIStyles.smallLayout))
			{
				menu.ShowAsContext();
			}
		}

		public static void DrawContextMenu(Func<GenericMenu> menuFunction, ContextMenuType context)
		{
			Texture texture = null;
			switch (context)
			{
				case ContextMenuType.Add:
					texture = StratusGUIStyles.addIcon;
					break;
				case ContextMenuType.Validation:
					texture = StratusGUIStyles.validateIcon;
					break;
				case ContextMenuType.Options:
					texture = StratusGUIStyles.optionsIcon;
					break;
			}

			if (GUILayout.Button(texture, StratusGUIStyles.editorStyles.button, StratusGUIStyles.smallLayout))
			{
				GenericMenu menu = menuFunction();
				menu.ShowAsContext();
			}
		}

		public static void DrawFadeGroup(AnimBool show, string label, System.Action drawFunction)
		{
			show.target = EditorGUILayout.Foldout(show.target, label);
			if (EditorGUILayout.BeginFadeGroup(show.faded))
			{
				drawFunction();
			}
			EditorGUILayout.EndFadeGroup();
		}

		public static void DrawVerticalFadeGroup(AnimBool show, string label, System.Action drawFunction, GUIStyle verticalStyle = null, bool validate = true)
		{
			show.target = EditorGUILayout.Foldout(show.target, label) && validate;
			if (EditorGUILayout.BeginFadeGroup(show.faded))
			{
				EditorGUILayout.BeginVertical(verticalStyle);
				drawFunction();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFadeGroup();
		}

		public static void DrawVerticalFadeGroup<T>(AnimBool show, string label, System.Action<T> drawFunction, T argument, GUIStyle verticalStyle = null, bool validate = true)
		{
			show.target = EditorGUILayout.Foldout(show.target, label) && validate;
			if (EditorGUILayout.BeginFadeGroup(show.faded))
			{
				EditorGUILayout.BeginVertical(verticalStyle);
				drawFunction(argument);
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFadeGroup();
		}

		public static void DrawListView<T>(IEnumerable<T> list, Func<T, GUIContent> leftContent, Func<T, string> rightContent,
										   GUILayoutOption leftWidth, GUILayoutOption rightWidth, GUILayoutOption height)
		{
			foreach (T element in list)
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(leftContent(element), StratusGUIStyles.listViewLabel, leftWidth, height);
					EditorGUILayout.SelectableLabel(rightContent(element), StratusGUIStyles.textField, rightWidth, height);
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		public static void DrawAligned(System.Action drawFunction, TextAlignment alignment)
		{
			GUILayout.BeginHorizontal();

			switch (alignment)
			{
				case TextAlignment.Left:
					drawFunction();
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Center:
					GUILayout.FlexibleSpace();
					drawFunction();
					GUILayout.FlexibleSpace();
					break;

				case TextAlignment.Right:
					GUILayout.FlexibleSpace();
					drawFunction();
					break;
			}
			GUILayout.EndHorizontal();
		}

		public static Rect Pad(Rect rect)
		{
			float padding = StratusEditorGUI.standardPadding;
			return Pad(rect, padding);
		}

		public static Rect Pad(Rect rect, float padding)
		{
			rect.y += padding;
			rect.height -= padding;
			rect.x += padding;
			rect.width -= padding;
			return rect;
		}

		public static Rect PadVertical(Rect rect)
		{
			float padding = StratusEditorGUI.standardPadding;
			return PadVertical(rect, padding);
		}

		public static Rect PadVertical(Rect rect, float padding)
		{
			rect.y += padding;
			rect.height -= padding;
			return rect;
		}

		public static Rect RaiseVertical(Rect rect, float padding)
		{
			rect.y -= padding;
			rect.height += padding;
			return rect;
		}

		public static Rect PadHorizontal(Rect rect)
		{
			float padding = StratusEditorGUI.standardPadding;
			rect.x += padding;
			rect.width -= padding;
			return rect;
		}

		public static bool LabelHasContent(GUIContent label)
		{
			if (label == null)
			{
				return true;
			}
			return label.text != string.Empty || label.image != null;
		}

		public static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (float)StratusReflection.GetReflectedMethod("GetSinglePropertyHeight", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { property, label });
		}

		internal static bool HasVisibleChildFields(SerializedProperty property)
		{
			return (bool)StratusReflection.GetReflectedMethod("HasVisibleChildFields", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { property });
		}

		internal static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
		{
			return (bool)StratusReflection.GetReflectedMethod("DefaultPropertyField", typeof(UnityEditor.EditorGUI)).Invoke(null, new object[] { position, property, label });
		}

		internal static Rect GetToggleRect(bool hasLabel, params GUILayoutOption[] options)
		{
			return (Rect)StratusReflection.GetReflectedMethod("GetToggleRect", typeof(UnityEditor.EditorGUILayout)).Invoke(null, new object[] { hasLabel, options });
		}

		internal static GUIContent TempContent(string t)
		{
			BindingFlags bindflags = BindingFlags.NonPublic | BindingFlags.Static;
			MethodInfo method = typeof(UnityEditor.EditorGUIUtility).GetMethod("TempContent", bindflags, null, new[] { typeof(string) }, null);

			return (GUIContent)method.Invoke(null, new[] { t });
		}



		///// <summary>
		///// Adds the given define symbols to PlayerSettings define symbols.
		///// Just add your own define symbols to the Symbols property at the below.
		///// </summary>
		//[InitializeOnLoad]
		//public class AddDefineSymbols : Editor
		//{
		//  /// <summary>
		//  /// Symbols that will be added to the editor
		//  /// </summary>
		//  public static readonly string[] Symbols = new string[] {
		//     "MYCOMPANY",
		//     "MYCOMPANY_MYPACKAGE"
		// };
		//
		//  /// <summary>
		//  /// Add define symbols as soon as Unity gets done compiling.
		//  /// </summary>
		//  static AddDefineSymbols()
		//  {
		//
		//  }
		//
		//}



	}

}