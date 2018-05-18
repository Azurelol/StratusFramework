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
    private Type[] extensionTypes;
    private string[] extensionTypeNames;
    private StratusEditor extensionEditor;
    private string[] extensionsNames;
    private int extensionIndex = 0;
    private SerializedProperty extensionsProperty;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/    
    public ExtensibleBehaviour.Extension selectedExtension => target.hasExtensions ? target.extensions[extensionIndex] : null;

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/    
    protected override void OnStratusEditorEnable()
    {
      extensionTypes = Reflection.GetSubclass<ExtensibleBehaviour.Extension>();
      extensionTypeNames = Reflection.GetSubclassNames<ExtensibleBehaviour.Extension>();
      extensionsProperty = serializedObject.FindProperty("extensionsField");
      RefreshExtensions();
      drawGroupRequests.Add(new DrawGroupRequest(SelectExtension, () => { return target.hasExtensions; }));
      drawGroupRequests.Add(new DrawGroupRequest(DrawExtension, () => { return target.hasExtensions; }));
      drawGroupRequests.Add(new DrawGroupRequest(AddExtension));
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/    
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
      //extensionEditor = UnityEditor.Editor.CreateEditor(selectedExtension) as StratusEditor;
      //extensionEditor.backgroundStyle = EditorStyles.helpBox;
    }

    private void DrawExtension(Rect rect)
    {
      // Now draw the selected extension
      EditorGUI.indentLevel = 1;
      EditorGUILayout.PropertyField(extensionsProperty.GetArrayElementAtIndex(extensionIndex));
      //extensionEditor.OnInspectorGUI();
      EditorGUI.indentLevel = 0;
    }

    private void RefreshExtensions()
    {
      if (!target.hasExtensions)
        return;

      extensionsNames = target.extensions.TypeNames();

      //availableExtensionTypeNames = extensionTypeNames.Filter(extensionsNames);
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
        ExtensibleBehaviour.Extension extension = (ExtensibleBehaviour.Extension)Utilities.Reflection.Instantiate(extensionType);
        target.Add(extension);
        Undo.RecordObject(target, extensionType.Name);
        serializedObject.ApplyModifiedProperties();
        RefreshExtensions();
      }
    }

    private void RemoveExtension(ExtensibleBehaviour.Extension extension)
    {
      endOfFrameRequests.Add(() =>
      {
        extensionEditor = null;
        extensionIndex = 0;
        target.Remove(extension);
        //Undo.DestroyObjectImmediate(extension);
        Undo.RecordObject(target, extension.GetType().Name);
        serializedObject.ApplyModifiedProperties();
        RefreshExtensions();
      });
    }

  }

}