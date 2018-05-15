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
    }

    //----------------------------------------------------------------------/
    // Properties
    //----------------------------------------------------------------------/
    /// <summary>
    /// Identifier for this particular blackboard at runtime
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// Runtime instantiated globals for a given blackboard
    /// </summary>
    private static Dictionary<Blackboard, Symbol.Table> instancedGlobals = new Dictionary<Blackboard, Symbol.Table>();
    /// <summary>
    /// Runtime instantiated locals (symbol tables) for a given blackboard, where we use a gameobject as the key
    /// </summary>
    private static Dictionary<Blackboard, Dictionary<GameObject, Symbol.Table>> instancedLocals = new Dictionary<Blackboard, Dictionary<GameObject, Symbol.Table>>();

    //----------------------------------------------------------------------/
    // Fields
    //----------------------------------------------------------------------/
    /// <summary>
    /// Symbols which are available to all agents using this blackboard
    /// </summary>
    public Symbol.Table globals = new Symbol.Table();
    /// <summary>
    /// Symbols specific for each agent of this blackboard
    /// </summary>
    public Symbol.Table locals = new Symbol.Table();
    /// <summary>
    /// A map of all blackboard instances. This is used to share globals among instances of 
    /// specific blackboards.
    /// </summary>
    private static Dictionary<int, Blackboard> instances = new Dictionary<int, Blackboard>();


    //----------------------------------------------------------------------/
    // Methods
    //----------------------------------------------------------------------/
    /// <summary>
    /// Returns all the global symbols for this blackboard at runtime
    /// </summary>
    /// <returns></returns>
    public Symbol.Table GetGlobals()
    {
      if (!instancedGlobals.ContainsKey(this))
        instancedGlobals.Add(this, new Symbol.Table(this.globals));
      return instancedGlobals[this];
    }

    /// <summary>
    /// Returns all the local symbols for this blackboard at runtime
    /// </summary>
    /// <param name="local"></param>
    /// <returns></returns>
    public Symbol.Table GetLocals(GameObject owner)
    {
      if (!instancedLocals.ContainsKey(this))
        instancedLocals.Add(this, new Dictionary<GameObject, Symbol.Table>());

      if (!instancedLocals[this].ContainsKey(owner))
        instancedLocals[this].Add(owner, new Symbol.Table(this.locals));

      return instancedLocals[this][owner];
    }

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
    public void SetLocal<T>(GameObject owner, string key, T value) => GetLocals(owner).SetValue<T>(key, value);

    /// <summary>
    /// Sets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetLocal(GameObject owner, string key, object value) => GetLocals(owner).SetValue(key, value);

    /// <summary>
    /// Sets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetGlobal<T>(string key, T value) => GetGlobals().SetValue<T>(key, value);

    /// <summary>
    /// Sets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetGlobal(string key, object value) => GetGlobals().SetValue(key, value);

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
      blackboard.locals = new Symbol.Table(this.locals);
      return blackboard;
    }

  }

}