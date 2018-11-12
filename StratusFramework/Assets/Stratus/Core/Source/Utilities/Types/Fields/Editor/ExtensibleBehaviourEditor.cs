using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Stratus.Utilities;

namespace Stratus
{
  [CustomEditor(typeof(ExtensibleBehaviour), true)]
  public class ExtensibleBehaviourEditor : StratusBehaviourEditor<ExtensibleBehaviour>
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
    private SerializedProperty selectedExtensionTypeIndexProperty;

    [SerializeField]
    private int extensionTypesIndex = 0;
    [SerializeField]
    private int extensionIndex = 0;

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/    
    private bool hasSelectedExtension => selectedExtensionBehaviour != null;
    private bool removingExtension { get; set; }
    public IExtensionBehaviour selectedExtension => target.hasExtensions ? target.extensions[extensionIndex] : null;
    public MonoBehaviour selectedExtensionBehaviour => target.extensionBehaviours.HasIndex(extensionIndex) ? target.extensionBehaviours[extensionIndex] : null;
    public string selectedExtensionTypeName => extensionTypeNames[extensionTypesIndex];
    public Type selectedExtensionType => extensionTypes[extensionTypesIndex];

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/    
    protected override void OnStratusEditorEnable()
    {
      // Set the draw request
      drawGroupRequests.Add(new DrawGroupRequest(DrawExtensions));

      // Remove any null behaviours
      this.target.extensionBehaviours.RemoveNull();

      // Find all possible extension types
      this.extensibleType = target.GetType();
      this.extensionType = typeof(IExtensionBehaviour);
      this.GetMatchingExtensionTypes();

      // Set the type index
      this.selectedExtensionTypeIndexProperty = propertyMap[nameof(ExtensibleBehaviour.selectedExtensionTypeIndex)];
      this.extensionTypesIndex = this.selectedExtensionTypeIndexProperty.intValue;
      if (this.extensionTypesIndex > this.extensionTypes.Length)
        this.extensionTypesIndex = 0;

      this.RefreshExtensions();
      this.SetExtensionIndex();
      this.TryCreateExtensionEditor();
    }

    protected override void OnStratusEditorDisable()
    {

      propertyMap["selectedExtensionTypeIndex"].intValue = this.extensionTypesIndex;
      this.selectedExtensionTypeIndexProperty.serializedObject.ApplyModifiedProperties();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/    
    private void DrawExtensions(Rect rect)
    {
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Extensions", StratusGUIStyles.header);
      EditorGUILayout.Separator();

      SelectExtension(rect);

      if (this.hasSelectedExtension && this.extensionEditor)
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
            {
              this.RemoveExtension(extensionIndex);
            }
          }
          else
          {
            if (GUILayout.Button("Add", EditorStyles.miniButtonRight))
            {
              this.AddExtension(extensionTypesIndex);
            }
          }
        });

        if (changed && !removingExtension)
        {
          this.SetExtensionIndex();
          this.TryCreateExtensionEditor();
        }



      }
      EditorGUILayout.EndHorizontal();
    }

    private void TryCreateExtensionEditor()
    {
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
      //if (!this.hasSelectedExtension)
      //  return;

      this.extensionEditor = CreateEditor(selectedExtensionBehaviour) as StratusEditor;
      this.extensionEditor.backgroundStyle = StratusGUIStyles.backgroundLight;
    }

    private void DrawExtension(Rect rect)
    {
      DrawEditor(extensionEditor, selectedExtensionTypeName);
    }


    private void AddExtension(int extensionTypeIndex)
    {
      Type extensionType = extensionTypes[extensionTypeIndex];

      IExtensionBehaviour extension = target.gameObject.AddComponent(extensionType) as IExtensionBehaviour;
      if (extension == null)
      {
        Trace.Error($"Failed to construct extension of type {extensionType.Name}");
        return;
      }

      Trace.Script($"Adding extension {extensionType.Name}");
      target.Add(extension);
      Undo.RecordObject(target, extensionType.Name);
      serializedObject.ApplyModifiedProperties();

      this.SetExtensionIndex();
      this.RefreshExtensions();
    }

    private void RemoveExtension(int index)
    {
      MonoBehaviour extensionBehaviour = selectedExtensionBehaviour;
      this.removingExtension = true;
      target.Remove(index);
      extensionEditor = null;

      Undo.RecordObject(target, extensionBehaviour.GetType().Name);
      Undo.DestroyObjectImmediate(extensionBehaviour);
      this.removingExtension = false;
      this.RefreshExtensions();

      //endOfFrameRequests.Add(() =>
      //{
      //});

      EditorGUIUtility.ExitGUI();
    }

    private void RefreshExtensions()
    {
      this.target.extensionBehaviours.RemoveNull();
      // Check that there's no extensions present on the GAmeObject but not added to the extensible
      var extensionBehaviours = ExtensibleBehaviour.GetExtensionBehaviours(this.target.GetComponents<MonoBehaviour>());
      foreach (var behaviour in extensionBehaviours)
      {
        if (behaviour == null || !this.extensionTypes.Contains(behaviour.GetType()))
          continue;

        bool hasExtension = this.target.HasExtension(behaviour);
        if (!hasExtension)
        {
          this.target.Add(behaviour);
        }
      }
    }

    private void SetExtensionIndex()
    {
      this.extensionIndex = this.GetExtensionIndex(selectedExtensionType);
      //this.hasSelectedExtension = this.extensionIndex != -1;
    }

    private int GetExtensionIndex(Type type)
    {
      //target.extensionBehaviours.RemoveNull();
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
  }

}