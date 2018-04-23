using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Stratus
{
  public static partial class StratusEditorUtility 
  {
    public delegate bool DefaultPropertyFieldDelegate(Rect position, SerializedProperty property, GUIContent label);
    public static DefaultPropertyFieldDelegate DefaultPropertyField;

    static StratusEditorUtility()
    {
      var t = typeof(EditorGUI);
      var delegateType = typeof(DefaultPropertyFieldDelegate);
      var m = t.GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.NonPublic);
      DefaultPropertyField = (DefaultPropertyFieldDelegate)System.Delegate.CreateDelegate(delegateType, m);
    }

    public static UnityEngine.Event currentEvent => UnityEngine.Event.current;
    public static bool currentEventUsed => currentEvent.type == EventType.Used;

    public static void OnMouseClick(System.Action onLeftClick, System.Action onRightClick, System.Action onDoubleClick, bool used = false)
    {
      if (!used && !currentEvent.isMouse)
        return;

      //bool clicked = currentEvent.type == EventType.MouseUp || currentEvent.type == EventType.MouseDown;
      //if (!clicked)
      //  return;

      var button = currentEvent.button;
      // Left click
      if (button == 0)
      {
        if (currentEvent.clickCount == 1)
          onLeftClick?.Invoke();
        else if (currentEvent.clickCount > 1)
          onDoubleClick?.Invoke();

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
        return;

      OnMouseClick(onLeftClick, onRightClick, onDoubleClick);
    }

    public static void OnLastControlMouseClick(System.Action onLeftClick, System.Action onRightClick, System.Action onDoubleClick = null)
    {
      if (!IsMousedOver(GUILayoutUtility.GetLastRect()))
        return;

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
        Trace.Script($"Saving change on {obj.name}");
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
      foreach (var name in propertyNames)
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
          foreach (var element in set)
          {
            var matchingElement = subset.Find(x => x.Equals(element));
            if (matchingElement != null)
              continue;

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
            var element = subset[i];
            if (GUILayout.Button(name(element), EditorStyles.miniButton))
            {
              indexToRemove = i;
            }
          }
          if (indexToRemove > -1)
            subset.RemoveAt(indexToRemove);
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

    private static string GetName<T>(T obj) where T : UnityEngine.Object => obj.name;




    public static bool ObjectField(FieldInfo field, object obj, GUIContent content = null)
    {
      object value = field.GetValue(obj);
      EditorGUI.BeginChangeCheck();
      {
        string name = ObjectNames.NicifyVariableName(field.Name); 

        if (value is UnityEngine.Object)
          field.SetValue(obj, EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, field.FieldType, true));
        else if (value is bool)
          field.SetValue(obj, EditorGUILayout.Toggle(name, (bool)value));
        else if (value is int)
          field.SetValue(obj, EditorGUILayout.IntField(name, (int)value));
        else if (value is float)
          field.SetValue(obj, EditorGUILayout.FloatField(name, (float)value));
        else if (value is string)
          field.SetValue(obj, EditorGUILayout.TextField(name, (string)value));
        else if (value is Enum)
          field.SetValue(obj, EditorGUILayout.EnumPopup(name, (Enum)value));
        else if (value is Vector2)
          field.SetValue(obj, EditorGUILayout.Vector2Field(name, (Vector2)value));
        else if (value is Vector3)
          field.SetValue(obj, EditorGUILayout.Vector3Field(name, (Vector3)value));
      }
      if (EditorGUI.EndChangeCheck())
        return true;
      return false;
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