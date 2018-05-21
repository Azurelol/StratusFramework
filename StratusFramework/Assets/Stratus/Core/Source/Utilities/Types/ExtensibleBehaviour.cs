using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
  /// <summary>
  /// A generic behaviour that has extensions that can be added or removed to it
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ExtensibleBehaviour : StratusBehaviour, IExtensible
  {
    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [SerializeField]
    private List<ExtensionBehaviour> extensionsField = new List<ExtensionBehaviour>();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public ExtensionBehaviour[] extensions  => extensionsField.ToArray();
    public bool hasExtensions => extensionsField.Count > 0;

    //--------------------------------------------------------------------------------------------/
    // Virtual
    //--------------------------------------------------------------------------------------------/
    protected abstract void OnAwake();
    protected abstract void OnStart();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      foreach (ExtensionBehaviour extension in extensionsField)
      {
        extension.OnAwake();
      }

      OnAwake();
    }

    private void Start()
    {
      foreach (ExtensionBehaviour extension in extensionsField)
        extension.OnStart();

      OnStart();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public void Add(ExtensionBehaviour extension)
    {
      extension.extensibleField = this;
      extensionsField.Add(extension);
    }    

    public void Remove(ExtensionBehaviour extension)
    {
      extensionsField.Remove(extension);
    }

  }

  /// <summary>
  /// Tells the Editor which extensible class this extension is for
  /// </summary>
  public class CustomExtension : Attribute
  {
    public CustomExtension(Type extensibleType)
    {
      this.extensibleType = extensibleType;
    }

    public Type extensibleType { get; set; }

  }

  /// <summary>
  /// A behaviour that acts as an extension to extensible behaviour (one that
  /// derives from ExtensibleBehaviour). Make sure to use the 'CustomExtension' attribute in order
  /// to specify which extensible behaviour this extension is for.
  /// </summary>
  [RequireComponent(typeof(IExtensible), typeof(ExtensibleBehaviour))]
  public abstract class ExtensionBehaviour : StratusBehaviour
  {
    [HideInInspector]
    [SerializeField]
    internal ExtensibleBehaviour extensibleField;
    public ExtensibleBehaviour extensible => extensibleField;
    public abstract void OnAwake();
    public abstract void OnStart();
  }

  /// <summary>
  /// Interface type used to validate all extensible behaviours
  /// </summary>
  public interface IExtensible { }

}