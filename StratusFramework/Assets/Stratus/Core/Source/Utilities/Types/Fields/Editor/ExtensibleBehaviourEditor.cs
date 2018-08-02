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
    private List<Dictionary<Type, Attribute>> extensionAttributes;
    private string[] extensionTypeNames;
    private StratusEditor extensionEditor;
    private Type extensibleType, extensionType;
    private SerializedProperty selectedExtensionIndexProperty;
    private bool hasSelectedExtension;

    [SerializeField]
    private int extensionTypesIndex = 0;
    [SerializeField]
    private int extensionIndex = 0;    

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/    
    public IExtensionBehaviour selectedExtension => target.hasExtensions ? target.extensions[extensionIndex] : null;
    public MonoBehaviour selectedExtensionBehaviour => target.hasExtensions ? target.extensionBehaviours[extensionIndex] : null;
    public string selectedExtensionTypeName => extensionTypeNames[extensionTypesIndex];

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/    
    protected override void OnStratusEditorEnable()
    {
      // Remove any null behaviours
      this.target.extensionBehaviours.RemoveNull();

      this.selectedExtensionIndexProperty = propertyMap[nameof(ExtensibleBehaviour.selectedExtensionIndex)];
      this.extensibleType = target.GetType();
      this.extensionType = typeof(IExtensionBehaviour);
      this.GetMatchingExtensionTypes();

      // Set the index
      //this.extensionIndex = selectedExtensionIndexProperty.intValue;
      //this.extensionTypesIndex = this.extensionTypeNames.FindIndex(this.selectedExtensionName);
      //this.extensionTypesIndex = this.selectedExtensionBehaviour != null ? this.selectedExtensionBehaviour.name
      if (!this.target.extensions.HasIndex(this.extensionIndex))
        this.extensionIndex = -1;
      this.TryCreateExtensionEditor();

      this.RefreshExtensions();

      // Set the draw request
      drawGroupRequests.Add(new DrawGroupRequest(DrawExtensions));
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/    
    private void DrawExtensions(Rect rect)
    {
      EditorGUILayout.LabelField("Extensions", StratusGUIStyles.header);
      EditorGUILayout.Separator();

      SelectExtension(rect);

      if (this.extensionEditor != null)
      {
        DrawExtension(rect);
      }
      
    }

    private void SelectExtension(Rect rect)
    {            
      EditorGUILayout.BeginHorizontal();
      {
        bool changed = StratusEditorUtility.CheckControlChange(() =>
        {
          // Select an extension
          this.extensionTypesIndex = EditorGUILayout.Popup(this.extensionTypesIndex, this.extensionTypeNames);
          // Add or remove it
          if (this.hasSelectedExtension)
          {
            if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
              this.RemoveExtension(extensionIndex);
          }
          else
          {
            if (GUILayout.Button("Add", EditorStyles.miniButtonRight))
            {
              this.AddExtension(extensionTypesIndex);
            }
          }
        });

        if (changed)
        {
          //Trace.Script("Attributes:");
          //foreach(var attr in this.extensionAttributes[extensionTypesIndex])
          //{
          //  Trace.Script($" - {attr.Value.GetType().Name}");
          //}
          this.extensionIndex = this.GetExtensionIndex(extensionTypes[extensionTypesIndex]);
          this.TryCreateExtensionEditor();
        }



      }
      EditorGUILayout.EndHorizontal();
    }

    private void TryCreateExtensionEditor()
    {
      //this.extensionIndex = this.GetExtensionIndex(extensionTypes[extensionTypesIndex]);
      this.hasSelectedExtension = this.extensionIndex != -1;

      if (this.hasSelectedExtension)
      {
        CreateExtensionEditor();
      }
      else
      {
        this.extensionEditor = null;
      }
    }

    private void CreateExtensionEditor()
    {     
      this.selectedExtensionIndexProperty.intValue = this.extensionIndex;
      this.selectedExtensionIndexProperty.serializedObject.ApplyModifiedProperties();

      this.extensionEditor = UnityEditor.Editor.CreateEditor((MonoBehaviour)selectedExtension) as StratusEditor;
      this.extensionEditor.backgroundStyle = EditorStyles.helpBox;
    }

    private void DrawExtension(Rect rect)
    {
      DrawEditor(extensionEditor, selectedExtensionTypeName);
      // Now draw the selected extension
      //EditorGUI.indentLevel = 1;
      //extensionEditor.OnInspectorGUI();
      //EditorGUI.indentLevel = 0;
    }


    private void AddExtension(int extensionTypeIndex)
    {
      Type extensionType = extensionTypes[extensionTypeIndex];
      //if (GetExtensionIndex(extensionType) == -1)
      //{
      //  Debug.LogWarning($"{target} already has the extension {extensionType.Name}");
      //  return;
      //}

      IExtensionBehaviour extension = target.gameObject.AddComponent(extensionType) as IExtensionBehaviour;
      if (extension == null)
      {
        Trace.Error($"Failed to construct extension of type {extensionType.Name}");
        return;
      }

      target.Add(extension);            
      Undo.RecordObject(target, extensionType.Name);
      serializedObject.ApplyModifiedProperties();

      this.hasSelectedExtension = true;
      this.RefreshExtensions();

      //Trace.Script($"Adding {extensionType.Name}");
    }

    private void RemoveExtension(int index)
    {


      endOfFrameRequests.Add(() =>
      {
        MonoBehaviour extensionBehaviour = selectedExtensionBehaviour;
        target.Remove(index);
        Undo.RecordObject(target, extensionBehaviour.GetType().Name);
        Undo.DestroyObjectImmediate(extensionBehaviour);

        serializedObject.ApplyModifiedProperties();

        extensionIndex = -1;
        extensionEditor = null;
        this.hasSelectedExtension = false;

        this.RefreshExtensions();
      });
    }

    private void RefreshExtensions()
    {
      // Check that there's no extensions present on the GAmeObject but not added to the extensible
      var extensionBehaviours = ExtensibleBehaviour.GetExtensionBehaviours(this.target.GetComponents<MonoBehaviour>());
      foreach(var behaviour in extensionBehaviours)
      {
        if (behaviour == null)
          continue;

        bool hasExtension = this.target.HasExtension(behaviour);
        if (!hasExtension)
        {
          this.target.Add(behaviour);
        }
      }
    }

    //private void RemoveExtension(IExtensionBehaviour extension)
    //{
    //  endOfFrameRequests.Add(() =>
    //  {
    //    extensionEditor = null;
    //    extensionIndex = -1;
    //    this.hasSelectedExtension = false;
    //
    //    target.Remove(extension);
    //    Undo.RecordObject(target, extension.GetType().Name);
    //    Undo.DestroyObjectImmediate((extension as MonoBehaviour));
    //
    //    serializedObject.ApplyModifiedProperties();
    //
    //    RefreshExtensions();
    //  });
    //}

    private int GetExtensionIndex(Type type)
    {
      return target.extensions.FindIndex((IExtensionBehaviour e) => e.GetType() == type);
    }

    private void GetMatchingExtensionTypes()
    {
      this.extensionAttributes = new List<Dictionary<Type, Attribute>>();

      List<Type> matchingTypes = new List<Type>();

      // 1. Get all extensible types who have marked support for this extensible
      var allExtensionTypes = Reflection.GetInterfacesExhaustive(typeof(MonoBehaviour), extensionType);
      foreach (var type in allExtensionTypes)
      {
        var attributeMap = type.MapAttributes();
        this.extensionAttributes.Add(attributeMap);
        CustomExtensionAttribute attribute = type.GetAttribute<CustomExtensionAttribute>();
        if (attribute != null && attribute.supportedExtensibles.Contains(extensibleType))
          matchingTypes.Add(type);
      }

      extensionTypes = matchingTypes.ToArray();
      extensionTypeNames = matchingTypes.Names(x => x.Name);
    }
    
    // 2. Get any extensions marked by 

    //private void Repair()
    //{
    //  
    //}

  }

}