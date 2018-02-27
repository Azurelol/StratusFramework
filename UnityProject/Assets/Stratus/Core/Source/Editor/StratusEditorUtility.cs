using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Stratus
{
  public static partial class StratusEditorUtility
  {
    public static void OnMouseClick(System.Action onLeftClick, System.Action onRightClick)
    {
      var button = UnityEngine.Event.current.button;
      // Left click
      if (button == 0)
        onLeftClick?.Invoke();
      // Right click
      else if (button == 1)
        onRightClick?.Invoke();
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
    public static void SaveOnControlChange(Object obj, System.Action procedure)
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
    /// If a GUI control was changed, saves the state of the object
    /// </summary>
    /// <param name="procedure"></param>
    /// <param name="obj"></param>
    public static void ModifyProperty(Object obj, string propertyName)
    {
      SerializedObject serializedObject = new SerializedObject(obj);
      SerializedProperty prop = serializedObject.FindProperty(propertyName);
      EditorGUI.BeginChangeCheck();
      EditorGUILayout.PropertyField(prop);
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
    public static void ModifyBooleanProperty(Object obj, string booleanPropertyName, string label = null)
    {
      SerializedObject serializedObject = new SerializedObject(obj);
      SerializedProperty prop = serializedObject.FindProperty(booleanPropertyName);
      Toggle(prop, label);
    }

    /// <summary>
    /// If a GUI control was changed, saves the state of the object
    /// </summary>
    /// <param name="procedure"></param>
    /// <param name="obj"></param>
    public static bool Toggle(Object obj, string propertyName, string label = null)
    {
      SerializedProperty property = FindSerializedProperty(obj, propertyName);
      return Toggle(property, label);
    }

    /// <summary>
    /// If a GUI control was changed, saves the state of the object
    /// </summary>
    /// <param name="procedure"></param>
    /// <param name="obj"></param>
    public static bool Toggle(SerializedProperty prop, string label = null)
    {
      EditorGUI.BeginChangeCheck();
      GUIContent content = new GUIContent(label == null ? prop.displayName : label);
      prop.boolValue = GUILayout.Toggle(prop.boolValue, content);
      if (EditorGUI.EndChangeCheck())
      {
        prop.serializedObject.ApplyModifiedProperties();
      }
      return prop.boolValue;
    }

    /// <summary>
    /// Finds the serialized property from a given object
    /// </summary>
    /// <param name="procedure"></param>
    /// <param name="obj"></param>
    public static SerializedProperty FindSerializedProperty(Object obj, string propertyName)
    {
      SerializedObject serializedObject = new SerializedObject(obj);
      SerializedProperty prop = serializedObject.FindProperty(propertyName);
      return prop;
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