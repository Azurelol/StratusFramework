/******************************************************************************/
/*!
@file   Blackboard.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus.Types;
using System;

namespace Stratus
{
  /// <summary>
  /// A blackboard is a table of symbols
  /// </summary>
  [CreateAssetMenu(fileName = "Blackboard", menuName = "Stratus/Blackboard")]
  public class Blackboard : StratusScriptable
  {
    //----------------------------------------------------------------------/
    // Declarations
    //----------------------------------------------------------------------/
    /// <summary>
    /// The scope of a table on a given blackboard
    /// </summary>
    public enum Scope
    {
      Local,
      Global
    }

    /// <summary>
    /// A field that allows the selection of a given blackboard and specific keys within it
    /// from an inspector window
    /// </summary>
    [Serializable]
    public class Selector
    {
      public Blackboard blackboard;
      public Scope scope;
      public string key;

      /// <summary>
      /// Sets the value of the symbol with the selected key
      /// </summary>
      /// <param name="owner"></param>
      /// <param name="value"></param>
      public void Set(GameObject owner, object value)
      {
        if (blackboard == null)
          throw new NullReferenceException($"No blackboard has been set!");

        switch (scope)
        {
          case Scope.Local:
            blackboard.SetLocal(owner, key, value);
            break;
          case Scope.Global:
            blackboard.SetGlobal(key, value);
            break;
        }
      }

      /// <summary>
      /// Gets the value of the symbol with the selected key
      /// </summary>
      /// <param name="owner"></param>
      /// <param name="value"></param>
      public object Get(GameObject owner)
      {
        if (blackboard == null)
          throw new NullReferenceException($"No blackboard has been set!");

        object value = null;
        switch (scope)
        {
          case Scope.Local:
            value = blackboard.GetLocal(owner, key);
            break;
          case Scope.Global:
            value = blackboard.GetGlobal(key);
            break;
        }
        return value;
      }
    }

    public delegate void OnGlobalSymbolChanged(Symbol symbol);
    public delegate void OnLocalSymbolChanged(GameObject gameObject, Symbol symbol);

    //----------------------------------------------------------------------/
    // Properties:
    //----------------------------------------------------------------------/
    public OnLocalSymbolChanged onLocalSymbolChanged { get; set; }
    public OnGlobalSymbolChanged onGlobalSymbolChanged { get; set; }

    //----------------------------------------------------------------------/
    // Properties: Static
    //----------------------------------------------------------------------/
    /// <summary>
    /// Identifier for this particular blackboard at runtime
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// Runtime instantiated globals for a given blackboard
    /// </summary>
    private static Dictionary<Blackboard, SymbolTable> instancedGlobals = new Dictionary<Blackboard, SymbolTable>();
    /// <summary>
    /// Runtime instantiated locals (symbol tables) for a given blackboard, where we use a gameobject as the key
    /// </summary>
    private static Dictionary<Blackboard, Dictionary<GameObject, SymbolTable>> instancedLocals = new Dictionary<Blackboard, Dictionary<GameObject, SymbolTable>>();

    //----------------------------------------------------------------------/
    // Fields
    //----------------------------------------------------------------------/
    /// <summary>
    /// Symbols which are available to all agents using this blackboard
    /// </summary>
    public SymbolTable globals = new SymbolTable();
    /// <summary>
    /// Symbols specific for each agent of this blackboard
    /// </summary>
    public SymbolTable locals = new SymbolTable();
    /// <summary>
    /// A map of all blackboard instances. This is used to share globals among instances of 
    /// specific blackboards.
    /// </summary>
    private static Dictionary<int, Blackboard> instances = new Dictionary<int, Blackboard>();

    //----------------------------------------------------------------------/
    // Messages
    //----------------------------------------------------------------------/    
    private void OnValidate()
    {
      
    }

    //----------------------------------------------------------------------/
    // Methods
    //----------------------------------------------------------------------/
    /// <summary>
    /// Returns all the global symbols for this blackboard at runtime
    /// </summary>
    /// <returns></returns>
    public SymbolTable GetGlobals()
    {
      if (!instancedGlobals.ContainsKey(this))
        instancedGlobals.Add(this, new SymbolTable(this.globals));
      return instancedGlobals[this];
    }

    /// <summary>
    /// Returns all the local symbols for this blackboard at runtime
    /// </summary>
    /// <param name="local"></param>
    /// <returns></returns>
    public SymbolTable GetLocals(GameObject owner)
    {
      if (!instancedLocals.ContainsKey(this))
        instancedLocals.Add(this, new Dictionary<GameObject, SymbolTable>());

      if (!instancedLocals[this].ContainsKey(owner))
        instancedLocals[this].Add(owner, new SymbolTable(this.locals));

      return instancedLocals[this][owner];
    }

    // Get

    /// <summary>
    /// Gets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="owner"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetLocal<T>(GameObject owner, string key) => GetLocals(owner).GetValue<T>(key);

    /// <summary>
    /// Gets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="owner"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetLocal(GameObject owner, string key) => GetLocals(owner).GetValue(key);

    /// <summary>
    /// Gets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetGlobal<T>(string key) => GetGlobals().GetValue<T>(key);

    /// <summary>
    /// Gets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetGlobal(string key) => GetGlobals().GetValue(key);

    /// <summary>
    /// Sets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetLocal<T>(GameObject owner, string key, T value)
    {
      Symbol symbol = GetLocals(owner).Find(key);
      symbol.SetValue(value);
      onLocalSymbolChanged(owner, symbol);
      //GetLocals(owner).SetValue<T>(key, value);
    }

    /// <summary>
    /// Sets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetLocal(GameObject owner, string key, object value)
    {
      Symbol symbol = GetLocals(owner).Find(key);
      symbol.SetValue(value);
      onLocalSymbolChanged(owner, symbol);
      //GetLocals(owner).SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetGlobal<T>(string key, T value)
    {
      Symbol symbol = GetGlobals().Find(key);
      symbol.SetValue(value);
      onGlobalSymbolChanged(symbol);

      //GetGlobals().SetValue<T>(key, value);
    }

    /// <summary>
    /// Sets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetGlobal(string key, object value)
    {
      Symbol symbol = GetGlobals().Find(key);
      symbol.SetValue(value);
      onGlobalSymbolChanged(symbol);
      //GetGlobals().SetValue(key, value);
    }

    //----------------------------------------------------------------------/
    // Methods
    //----------------------------------------------------------------------/    
    /// <summary>
    /// Gets the value of a symbol from the table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="scope"></param>
    /// <param name="table"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private T Get<T>(Scope scope, SymbolTable table, string key)
    {
      try
      {
        T value = table.GetValue<T>(key);
        return value;
      }
      catch (KeyNotFoundException e)
      {
        throw new KeyNotFoundException($"{name} : {e.Message}");
      }
    }


    /// <summary>
    /// Gets the value of a symbol from the table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="scope"></param>
    /// <param name="table"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private void Set<T>(Scope scope, SymbolTable table, string key, T value)
    {
      try
      {
        table.SetValue(key, value);
      }
      catch (KeyNotFoundException e)
      {
        throw new KeyNotFoundException($"{name} : {e.Message}");
      }
    }


    /// <summary>
    /// Returns an instance of this blackboard asset, making a copy of its locals
    /// and using a reference to an unique (static) instance for its globals
    /// (so that they can be shared among all agents using that blackboard)
    /// </summary>
    /// <returns></returns>
    public Blackboard Instantiate()
    {
      // Get the ID of this asset
      id = this.GetInstanceID();

      // If an instance of this blackboard has not already been instantiated, add it
      if (!instances.ContainsKey(id))
        instances.Add(id, this);

      // Now create a new blackboard instance, giving it its own local copy
      // and using a reference to the shared one
      var blackboard = ScriptableObject.CreateInstance<Blackboard>();
      blackboard.globals = instances[id].globals;
      blackboard.locals = new SymbolTable(this.locals);
      return blackboard;
    }

  }

}