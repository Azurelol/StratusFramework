using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
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
  /// A generic behaviour that has extensions that can be added or removed to it
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ExtensibleBehaviour : StratusBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/    
    [Serializable]
    public abstract class Extension : System.Object
    { 
      [SerializeField]
      private ExtensibleBehaviour master;
      public abstract void OnAwake();
      public abstract void OnStart();
    }

    //--------------------------------------------------------------------------------------------/
    // Fields
    //--------------------------------------------------------------------------------------------/
    [SerializeField]
    public List<Extension> extensionsField = new List<Extension>();

    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    public Extension[] extensions  => extensionsField.ToArray();
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
      foreach (Extension extension in extensionsField)
        extension.OnAwake();

      OnAwake();
    }

    private void Start()
    {
      foreach (Extension extension in extensionsField)
        extension.OnStart();

      OnStart();
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    public void Add(Extension extension)
    {
      extensionsField.Add(extension);
    }    

    public void Remove(Extension extension)
    {
      extensionsField.Remove(extension);
    }

  }

}