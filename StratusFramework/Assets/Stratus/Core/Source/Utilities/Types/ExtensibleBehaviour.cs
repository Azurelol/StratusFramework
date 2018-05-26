using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Linq;

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
    private List<MonoBehaviour> extensionsField = new List<MonoBehaviour>();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public IExtensionBehaviour[] extensions => (from MonoBehaviour e in extensionsField select (e as IExtensionBehaviour)).ToArray();
    private Dictionary<Type, IExtensionBehaviour> extensionsMap { get; set; } = new Dictionary<Type, IExtensionBehaviour>();
    private static Dictionary<IExtensionBehaviour, ExtensibleBehaviour> extensionOwnershipMap { get; set; } = new Dictionary<IExtensionBehaviour, ExtensibleBehaviour>();
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
      OnAwake();

      foreach (var extension in extensions)
      {
        extensionsMap.Add(extension.GetType(), extension);
        extension.OnExtensibleAwake(this);
      }

    }

    private void Start()
    {
      OnStart();

      foreach (var extension in extensions)
        extension.OnExtensibleStart();

    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Adds the extension to this behaviour
    /// </summary>
    /// <param name="extension"></param>
    public void Add(IExtensionBehaviour extension)
    {
      //extension.extensibleField = this;
      extensionsField.Add((MonoBehaviour)extension);
      extensionsMap.Add(extension.GetType(), extension);
    }

    /// <summary>
    /// Removes the extension from this behaviour
    /// </summary>
    /// <param name="extension"></param>
    public void Remove(IExtensionBehaviour extension)
    {
      extensionsField.Remove((MonoBehaviour)extension);
      extensionsMap.Remove(extension.GetType());
    }

    /// <summary>
    /// Retrieves the extension of the given type, if its present
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetExtension<T>() where T : IExtensionBehaviour
    {
      Type type = typeof(T);
      if (!extensionsMap.ContainsKey(type))
        Trace.Error($"The extension of type {type} is not present!", this);
      return (T)extensionsMap[type];
    }

    /// <summary>
    /// Retrieves the extension of the given type, if its present
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool HasExtension(Type type) => extensionsMap.ContainsKey(type);

    /// <summary>
    /// Retrieves the extensible that the extension is for
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static T GetExtensible<T>(IExtensionBehaviour extension) where T : ExtensibleBehaviour
    {
      return extensionOwnershipMap[extension] as T;
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

  ///// <summary>
  ///// A behaviour that acts as an extension to extensible behaviour (one that
  ///// derives from ExtensibleBehaviour). Make sure to use the 'CustomExtension' attribute in order
  ///// to specify which extensible behaviour this extension is for.
  ///// </summary>
  //[RequireComponent(typeof(IExtensible), typeof(ExtensibleBehaviour))]
  //public abstract class ExtensionBehaviour : StratusBehaviour
  //{
  //  [HideInInspector]
  //  [SerializeField]
  //  internal ExtensibleBehaviour extensibleField;
  //  internal virtual void OnAwake() => OnExtensibleAwake();
  //  internal void OnStart() => OnExtensibleStart();
  //  protected abstract void OnExtensibleAwake();
  //  protected abstract void OnExtensibleStart();
  //}
  //
  ///// <summary>
  ///// A behaviour that acts as an extension to extensible behaviour (one that
  ///// derives from ExtensibleBehaviour). Make sure to use the 'CustomExtension' attribute in order
  ///// to specify which extensible behaviour this extension is for.
  ///// </summary>
  //public abstract class ExtensionBehaviour<T> : ExtensionBehaviour where T : ExtensibleBehaviour
  //{
  //  /// <summary>
  //  /// The extensible component this extension is for
  //  /// </summary>
  //  public T extensible { private set; get; }
  //  internal override void OnAwake()
  //  {
  //    extensible = extensibleField as T;
  //    base.OnAwake();
  //  }
  //}

  /// <summary>
  /// Interface type used to validate all extensible behaviours
  /// </summary>
  public interface IExtensible { }

  ///// <summary>
  ///// Interface type used to validate all extensible behaviours
  ///// </summary>
  //public interface IExtensibleBehaviour { }

  /// <summary>
  /// Interface type a behaviours that is an extension of another
  /// </summary>
  public interface IExtensionBehaviour
  {
    void OnExtensibleAwake(ExtensibleBehaviour extensible);
    void OnExtensibleStart();
  }

  /// <summary>
  /// Interface type used to validate all extensible behaviours
  /// </summary>
  public interface IExtensionBehaviour<T> : IExtensionBehaviour where T : ExtensibleBehaviour
  {
    /// <summary>
    /// The extensible component this extension is for
    /// </summary>
    T extensible { set; get; }
  }


}