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
    private HideFlags extensionFlags = HideFlags.HideInInspector;
    private Type extensibleType, extensionType;
    private SerializedProperty selectedExtensionIndexProperty;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/    
    public IExtensionBehaviour selectedExtension => target.hasExtensions ? target.extensions[extensionIndex] : null;
    //public MonoBehaviour selectedExtensionAsMonoBehaviour => selectedExtension as MonoBehaviour;
    public string selectedExtensionName => extensionsNames[extensionIndex];

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/    
    protected override void OnStratusEditorEnable()
    {
      //target.selectedExtension = (MonoBehaviour)selectedExtension;
      selectedExtensionIndexProperty = propertyMap["_selectedExtensionIndex"];
      extensionIndex = selectedExtensionIndexProperty.intValue;
      extensibleType = target.GetType();
      extensionType = typeof(IExtensionBehaviour);
      GetMatchingExtensionTypes();
      RefreshExtensions();
      drawGroupRequests.Add(new DrawGroupRequest(DrawExtensions, () => { return target.hasExtensions; }));
      //drawGroupRequests.Add(new DrawGroupRequest(AddExtension));
      //foreach(var extension in this.target.extensionBehaviours)
      //{
      //  extension.hideFlags = HideFlags.None;
      //}

      //foreach (var extension in target.extensions)
      //  (extension as MonoBehaviour).hideFlags = extensionFlags;
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/    
    private void DrawExtensions(Rect rect)
    {
      SelectExtension(rect);
      if (target.hasExtensions)
        DrawExtension(rect);
      AddExtension(rect);
    }

    private void SelectExtension(Rect rect)
    {
      EditorGUILayout.LabelField("Extensions", StratusGUIStyles.header);
      EditorGUILayout.Separator();
      EditorGUILayout.BeginHorizontal();
      {
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

    private void CreateExtensionEditor()
    {     
      selectedExtensionIndexProperty.intValue = extensionIndex;
      selectedExtensionIndexProperty.serializedObject.ApplyModifiedProperties();

      extensionEditor = UnityEditor.Editor.CreateEditor((MonoBehaviour)selectedExtension) as StratusEditor;
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
    }

    private void AddExtension(Rect rect)
    {
      int index = -1;
      index = EditorGUILayout.Popup("Add Extension", index, extensionTypeNames);
      if (index > -1)
      {
        Type extensionType = extensionTypes[index];
        if (HasExtension(extensionType))
        {
          Debug.LogWarning($"{target} already has the extension {extensionType.Name}");
          return;
        }
        //ExtensibleBehaviour.Extension extension = target.gameObject.GetOrAddComponent(extensionType);
        //ExtensibleBehaviour.Extension extension = UnityEngine.Object.Instantiate(extensionType);
        //ExtensionBehaviour extension = (ExtensionBehaviour)Utilities.Reflection.Instantiate(extensionType);
        //target.Add(extension);
        IExtensionBehaviour extension = target.gameObject.AddComponent(extensionType) as IExtensionBehaviour;
        target.Add(extension);
        (selectedExtension as MonoBehaviour).hideFlags = extensionFlags;
        Trace.Script($"Adding {extensionType.Name}");
        Undo.RecordObject(target, extensionType.Name);
        serializedObject.ApplyModifiedProperties();
        RefreshExtensions();
      }
    }

    private void RemoveExtension(IExtensionBehaviour extension)
    {
      endOfFrameRequests.Add(() =>
      {
        extensionEditor = null;
        extensionIndex = 0;
        target.Remove(extension);
        Undo.DestroyObjectImmediate((extension as MonoBehaviour));
        Undo.RecordObject(target, extension.GetType().Name);
        serializedObject.ApplyModifiedProperties();
        RefreshExtensions();
      });
    }

    private bool HasExtension(Type type)
    {
      return target.extensions.FindFirst((IExtensionBehaviour e) => e.GetType() == type) != null;
    }

    private void GetMatchingExtensionTypes()
    {
      List<Type> matchingTypes = new List<Type>();

      //IEnumerable<Type> imp = System.ASse

      //var allExtensionTypes  = Reflection.Get<ExtensionBehaviour>();
      //var types = Reflection.GetSubclass<Component>(true);
      //var stypes = Reflection.GetSubclass<StratusBehaviour>(true);
      var allExtensionTypes = Reflection.GetInterfacesExhaustive(typeof(MonoBehaviour), extensionType);
      foreach (var type in allExtensionTypes)
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