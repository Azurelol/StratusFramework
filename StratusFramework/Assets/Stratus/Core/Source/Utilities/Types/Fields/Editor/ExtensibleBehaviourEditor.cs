using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Stratus.Utilities;

namespace Stratus
{
  [CustomEditor(typeof(ExtensibleBehaviour), true)]
  public class ExtensibleBehaviourEditor : BehaviourEditor<ExtensibleBehaviour>
  {
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/    
    private static Type extensibleBehaviourType { get; } = typeof(ExtensibleBehaviour);
    private Type[] extensionTypes;
    private string[] extensionTypeNames;
    private StratusEditor extensionEditor;
    private string[] extensionsNames;
    private int extensionIndex = 0;
    //private SerializedProperty extensionsProperty;
    private HideFlags extensionFlags = HideFlags.HideInInspector;
    private Type extensibleType;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/    
    public ExtensionBehaviour selectedExtension => target.hasExtensions ? (ExtensionBehaviour)target.extensions[extensionIndex] : null;
    public string selectedExtensionName => extensionsNames[extensionIndex];

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/    
    protected override void OnStratusEditorEnable()
    {
      extensibleType = target.GetType();
      //extensionsProperty = serializedObject.FindProperty("extensionsField");
      GetMatchingExtensionTypes();
      RefreshExtensions();
      drawGroupRequests.Add(new DrawGroupRequest(DrawExtensions, () => { return target.hasExtensions; }));
      drawGroupRequests.Add(new DrawGroupRequest(AddExtension));
      //drawGroupRequests.Add(new DrawGroupRequest(DrawExtension, () => { return target.hasExtensions; }));
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/    
    private void DrawExtensions(Rect rect)
    {
      SelectExtension(rect);
      if (target.hasExtensions)
        DrawExtension(rect);
    }

    private void SelectExtension(Rect rect)
    {
      EditorGUILayout.BeginHorizontal();
      {
        EditorGUILayout.LabelField("Extensions", EditorStyles.whiteLargeLabel);
        bool changed = StratusEditorUtility.CheckControlChange(() =>
        {
          extensionIndex = EditorGUILayout.Popup(extensionIndex, extensionTypeNames);
          if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
            RemoveExtension(selectedExtension);
        });

        if (changed || extensionEditor == null)
        {
          CreateExtensionEditor();
        }

      }
      EditorGUILayout.EndHorizontal();
    }

    private void CreateExtensionEditor()
    {
      extensionEditor = UnityEditor.Editor.CreateEditor(selectedExtension) as StratusEditor;
      extensionEditor.backgroundStyle = EditorStyles.helpBox;
    }

    private void DrawExtension(Rect rect)
    {
      DrawEditor(extensionEditor, selectedExtensionName);
      // Now draw the selected extension
      //EditorGUI.indentLevel = 1;
      //extensionEditor.OnInspectorGUI();
      //EditorGUI.indentLevel = 0;
    }

    private void RefreshExtensions()
    {
      if (!target.hasExtensions)
        return;

      extensionsNames = target.extensions.TypeNames();
      foreach (var extension in target.extensions)
        extension.hideFlags = extensionFlags;
    }

    private void AddExtension(Rect rect)
    {
      int index = -1;
      index = EditorGUILayout.Popup("Add Extension", index, extensionTypeNames);
      if (index > -1)
      {
        Type extensionType = extensionTypes[index];
        //ExtensibleBehaviour.Extension extension = target.gameObject.GetOrAddComponent(extensionType);
        //ExtensibleBehaviour.Extension extension = UnityEngine.Object.Instantiate(extensionType);
        //ExtensionBehaviour extension = (ExtensionBehaviour)Utilities.Reflection.Instantiate(extensionType);
        //target.Add(extension);
        ExtensionBehaviour extension = target.gameObject.GetOrAddComponent(extensionType) as ExtensionBehaviour;
        target.Add(extension);
        extension.hideFlags = extensionFlags;
        Trace.Script($"Adding {extensionType.Name}");
        Undo.RecordObject(target, extensionType.Name);
        serializedObject.ApplyModifiedProperties();
        RefreshExtensions();
      }
    }

    private void RemoveExtension(ExtensionBehaviour extension)
    {
      endOfFrameRequests.Add(() =>
      {
        extensionEditor = null;
        extensionIndex = 0;
        target.Remove(extension);
        Undo.DestroyObjectImmediate(extension);
        Undo.RecordObject(target, extension.GetType().Name);
        serializedObject.ApplyModifiedProperties();
        RefreshExtensions();
      });
    }

    private void GetMatchingExtensionTypes()
    {
      List<Type> matchingTypes = new List<Type>();

      var allExtensionTypes  = Reflection.GetSubclass<ExtensionBehaviour>();
      foreach(var type in allExtensionTypes)
      {
        CustomExtension attribute = type.GetAttribute<CustomExtension>();
        if (attribute != null && attribute.extensibleType == extensibleType)
          matchingTypes.Add(type);
      }

      extensionTypes = matchingTypes.ToArray();
      extensionTypeNames = matchingTypes.Names(x => x.Name);
    }

  }

}