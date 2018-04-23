using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;
using System;

namespace Stratus.Experimental
{
  [CustomEditor(typeof(StratusPlayerController))]
  public class StratusPlayerControllerEditor : BehaviourEditor<StratusPlayerController>
  {
    private Type[] extensionTypes;
    private string[] extensionTypeNames;
    //private string[] availableExtensionTypeNames;

    private StratusEditor extensionEditor;
    private string[] extensionsNames;
    private int extensionIndex = 0;

    public StratusPlayerControllerExtension selectedExtension => target.extensions != null ? target.extensions[extensionIndex] : null;

    protected override void OnStratusEditorEnable()
    {
      extensionTypes = Reflection.GetSubclass<StratusPlayerControllerExtension>();
      extensionTypeNames = Reflection.GetSubclassNames<StratusPlayerControllerExtension>();
      RefreshExtensions();
      drawGroupRequests.Add(new DrawGroupRequest(SelectExtension, () => { return target.hasExtensions; }));
      drawGroupRequests.Add(new DrawGroupRequest(DrawExtension, () => { return target.hasExtensions; }));
      drawGroupRequests.Add(new DrawGroupRequest(AddExtensions));
    }

    private void AddExtensions(Rect rect)
    {
      int index = -1;
      index = EditorGUILayout.Popup("Add Extension", index, extensionTypeNames);
      if (index > -1)
      {
        //string name = availableExtensionTypeNames[index];
        //Type extension = extensionTypes.FindFirst((Type type) => { return type.Name == name; });
        Type extension = extensionTypes[index];
        target.gameObject.GetOrAddComponent(extension);
        RefreshExtensions();
      }
    }

    private void SelectExtension(Rect rect)
    {
      EditorGUILayout.BeginHorizontal();
      {
        EditorGUILayout.LabelField("Extensions", EditorStyles.whiteLargeLabel);
        bool changed = StratusEditorUtility.CheckControlChange(() => 
        {          
          extensionIndex = EditorGUILayout.Popup(extensionIndex, extensionsNames);
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

    private void DrawExtension(Rect rect)
    {
      // Now draw the selected extension
      EditorGUI.indentLevel = 1;
      extensionEditor.OnInspectorGUI();
      EditorGUI.indentLevel = 0;
    }

    private void RefreshExtensions()
    {
      if (!target.hasExtensions)
        return;

      extensionsNames = target.extensions.TypeNames();
      //availableExtensionTypeNames = extensionTypeNames.Filter(extensionsNames);
    }

    private void CreateExtensionEditor()
    {
      extensionEditor = UnityEditor.Editor.CreateEditor(selectedExtension) as StratusEditor;
      extensionEditor.backgroundStyle = EditorStyles.helpBox;
    }

    private void RemoveExtension(StratusPlayerControllerExtension extension)
    {
      endOfFrameRequests.Add(() =>
      {
        extensionEditor = null;
        extensionIndex = 0;
        target.Remove(extension);
        Undo.DestroyObjectImmediate(extension);
        RefreshExtensions();        
      });
    }


  }

}