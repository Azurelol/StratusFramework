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
    public ExtensionBehaviour[] extensions => extensionsField.ToArray();
    private Dictionary<Type, ExtensionBehaviour> extensionsMap { get; set; } = new Dictionary<Type, ExtensionBehaviour>();
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
        extensionsMap.Add(extension.GetType(), extension);
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
    /// <summary>
    /// Adds the extension to this behaviour
    /// </summary>
    /// <param name="extension"></param>
    public void Add(ExtensionBehaviour extension)
    {
      extension.extensibleField = this;
      extensionsField.Add(extension);
      extensionsMap.Add(extension.GetType(), extension);
    }

    /// <summary>
    /// Removes the extension from this behaviour
    /// </summary>
    /// <param name="extension"></param>
    public void Remove(ExtensionBehaviour extension)
    {
      extensionsField.Remove(extension);
      extensionsMap.Remove(extension.GetType());
    }

    /// <summary>
    /// Retrieves the extension of the given type, if its present
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetExtension<T>() where T : ExtensionBehaviour
    {
      Type type = typeof(T);
      if (!extensionsMap.ContainsKey(type))
        Trace.Error($"The extension of type {type} is not present!", this);
      return extensionsMap[type] as T;
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
    internal virtual void OnAwake() => OnExtensibleAwake();
    internal void OnStart() => OnExtensibleStart();
    protected abstract void OnExtensibleAwake();
    protected abstract void OnExtensibleStart();
  }

  /// <summary>
  /// A behaviour that acts as an extension to extensible behaviour (one that
  /// derives from ExtensibleBehaviour). Make sure to use the 'CustomExtension' attribute in order
  /// to specify which extensible behaviour this extension is for.
  /// </summary>
  public abstract class ExtensionBehaviour<T> : ExtensionBehaviour where T : ExtensibleBehaviour
  {
    /// <summary>
    /// The extensible component this extension is for
    /// </summary>
    public T extensible { private set; get; }
    internal override void OnAwake()
    {
      extensible = extensibleField as T;
      base.OnAwake();
    }
  }

  /// <summary>
  /// Interface type used to validate all extensible behaviours
  /// </summary>
  public interface IExtensible { }

}