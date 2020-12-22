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
  public class StratusBlackboard : StratusScriptable
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
      public StratusBlackboard blackboard;
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

    /// <summary>
    /// A reference of a symbol within the blackboard
    /// </summary>
    [Serializable]
    public class SymbolReference
    {
      public string key;
      public Scope scope;
      public StratusVariant.VariantType type;

      public object GetValue(StratusBlackboard blackboard, GameObject gameObject)
      {
        if (scope == Scope.Local)
          return blackboard.GetLocal(gameObject, key);
        return blackboard.GetGlobal(key);
      }

      public void SetValue(StratusBlackboard blackboard, GameObject gameObject, object value)
      {
        if (scope == Scope.Local)
          blackboard.SetLocal(gameObject, key, value);
        else
          blackboard.SetGlobal(key, value);
      }
    }

    /// <summary>
    /// A reference of a symbol within the blackboard
    /// </summary>
    [Serializable]
    public class Reference<T>// where T : struct
    {
      public string key;
      public Scope scope;
      public StratusVariant.VariantType type { get; } = VariantUtilities.Convert(typeof(T));

      public T GetValue(StratusBlackboard blackboard, GameObject gameObject)
      {
        if (scope == Scope.Local)
          return blackboard.GetLocal<T>(gameObject, key);
        return blackboard.GetGlobal<T>(key);
      }

      public void SetValue(StratusBlackboard blackboard, GameObject gameObject, T value)
      {
        if (scope == Scope.Local)
          blackboard.SetLocal<T>(gameObject, key, value);
        else 
          blackboard.SetGlobal<T>(key, value);
      }
    }

    /// <summary>
    /// A reference for a Vector3 within a blackboard
    /// </summary>
    public class Vector3Reference : Reference<Vector3>
    {
    }

    /// <summary>
    /// A reference for an integer within a blackboard
    /// </summary>
    public class IntegerReference : Reference<int>
    {      
    }


    public delegate void OnGlobalSymbolChanged(StratusSymbol symbol);
    public delegate void OnLocalSymbolChanged(GameObject gameObject, StratusSymbol symbol);

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
    private static Dictionary<StratusBlackboard, StratusSymbolTable> instancedGlobals = new Dictionary<StratusBlackboard, StratusSymbolTable>();
    /// <summary>
    /// Runtime instantiated locals (symbol tables) for a given blackboard, where we use a gameobject as the key
    /// </summary>
    private static Dictionary<StratusBlackboard, Dictionary<GameObject, StratusSymbolTable>> instancedLocals = new Dictionary<StratusBlackboard, Dictionary<GameObject, StratusSymbolTable>>();

    //----------------------------------------------------------------------/
    // Fields
    //----------------------------------------------------------------------/
    /// <summary>
    /// Symbols which are available to all agents using this blackboard
    /// </summary>
    public StratusSymbolTable globals = new StratusSymbolTable();
    /// <summary>
    /// Symbols specific for each agent of this blackboard
    /// </summary>
    public StratusSymbolTable locals = new StratusSymbolTable();
    /// <summary>
    /// A map of all blackboard instances. This is used to share globals among instances of 
    /// specific blackboards.
    /// </summary>
    private static Dictionary<int, StratusBlackboard> instances = new Dictionary<int, StratusBlackboard>();

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
    /// Adds a local symbol to the blackboard
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="symbol"></param>
    public void Add(StratusSymbol symbol, Scope scope)
    {
      switch (scope)
      {
        case Scope.Local:
          this.AddLocal(symbol);
          break;
        case Scope.Global:
          this.AddGlobal(symbol);
          break;
      }      
    }

    /// <summary>
    /// Adds a local symbol to the blackboard
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="symbol"></param>
    public void AddGlobal(StratusSymbol symbol)
    {
      this.globals.Add(symbol);
    }

    /// <summary>
    /// Adds a global symbol to the blackboard
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="symbol"></param>
    public void AddLocal(StratusSymbol symbol)
    {
      this.locals.Add(symbol);
    }

    /// <summary>
    /// Returns all the global symbols for this blackboard at runtime
    /// </summary>
    /// <returns></returns>
    public StratusSymbolTable GetGlobals()
    {
      if (!instancedGlobals.ContainsKey(this))
        instancedGlobals.Add(this, new StratusSymbolTable(this.globals));
      return instancedGlobals[this];
    }

    /// <summary>
    /// Returns all the local symbols for this blackboard at runtime
    /// </summary>
    /// <param name="local"></param>
    /// <returns></returns>
    public StratusSymbolTable GetLocals(GameObject owner)
    {
      if (!instancedLocals.ContainsKey(this))
        instancedLocals.Add(this, new Dictionary<GameObject, StratusSymbolTable>());

      if (!instancedLocals[this].ContainsKey(owner))
        instancedLocals[this].Add(owner, new StratusSymbolTable(this.locals));

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
      StratusSymbol symbol = GetLocals(owner).Find(key);
      symbol.SetValue(value);
      onLocalSymbolChanged?.Invoke(owner, symbol);
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
      StratusSymbol symbol = GetLocals(owner).Find(key);
      symbol.SetValue(value);
      onLocalSymbolChanged?.Invoke(owner, symbol);
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
      StratusSymbol symbol = GetGlobals().Find(key);
      symbol.SetValue(value);
      onGlobalSymbolChanged?.Invoke(symbol);

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
      StratusSymbol symbol = GetGlobals().Find(key);
      symbol.SetValue(value);
      onGlobalSymbolChanged?.Invoke(symbol);
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
    private T Get<T>(Scope scope, StratusSymbolTable table, string key)
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
    private void Set<T>(Scope scope, StratusSymbolTable table, string key, T value)
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
    public StratusBlackboard Instantiate()
    {
      // Get the ID of this asset
      id = this.GetInstanceID();

      // If an instance of this blackboard has not already been instantiated, add it
      if (!instances.ContainsKey(id))
        instances.Add(id, this);

      // Now create a new blackboard instance, giving it its own local copy
      // and using a reference to the shared one
      var blackboard = ScriptableObject.CreateInstance<StratusBlackboard>();
      blackboard.globals = instances[id].globals;
      blackboard.locals = new StratusSymbolTable(this.locals);
      return blackboard;
    }

  }

}