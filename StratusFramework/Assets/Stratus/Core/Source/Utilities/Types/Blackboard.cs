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
    public struct BlackboardKey
    {
      public MonoBehaviour monoBehaviour;
      public Blackboard blackboard;
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
    /// Runtime instantiated locals for a given blackboard, where we use a monobehaviour as the key
    /// </summary>
    private static Dictionary<Tuple<MonoBehaviour, Blackboard>, Symbol.Table> instancedLocals = new Dictionary<Tuple<MonoBehaviour, Blackboard>, Symbol.Table>();

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
    public Symbol.Table GetLocals(MonoBehaviour local)
    {
      Tuple<MonoBehaviour, Blackboard> blackboardKey = new Tuple<MonoBehaviour, Blackboard>(local, this);
      if (!instancedLocals.ContainsKey(blackboardKey))
        instancedLocals.Add(blackboardKey, new Symbol.Table(this.locals));
      return instancedLocals[blackboardKey];
    }

    /// <summary>
    /// Gets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="local"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetLocalValue<T>(MonoBehaviour local, string key) => GetLocals(local).GetValue<T>(key);

    /// <summary>
    /// Gets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="local"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetLocalValue(MonoBehaviour local, string key) => GetLocals(local).GetValue(key);

    /// <summary>
    /// Gets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetGlobalValue<T>(string key) => GetGlobals().GetValue<T>(key);

    /// <summary>
    /// Gets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetGlobalValue(string key) => GetGlobals().GetValue(key);

    /// <summary>
    /// Sets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetLocalValue<T>(MonoBehaviour local, string key, T value) => GetLocals(local).SetValue<T>(key, value);

    /// <summary>
    /// Sets the value of a local symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetLocalValue(MonoBehaviour local, string key, object value) => GetLocals(local).SetValue(key, value);

    /// <summary>
    /// Sets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetGlobalValue<T>(string key, T value) => GetGlobals().SetValue<T>(key, value);

    /// <summary>
    /// Sets the value of a global symbol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetGlobalValue(string key, object value) => GetGlobals().SetValue(key, value);

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