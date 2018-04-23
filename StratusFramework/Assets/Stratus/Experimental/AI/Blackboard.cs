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
  namespace AI
  {
    /// <summary>
    /// A blackboard is a table of symbols which is used by an AI decision maker
    /// </summary>
    [CreateAssetMenu(fileName = "Blackboard", menuName = "Stratus/AI/Blackboard")]
    public class Blackboard : ScriptableObject
    {
      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// Symbols which are available to all agents using this blackboard
      /// </summary>
      public Symbol.Table Globals = new Symbol.Table();
      /// <summary>
      /// Symbols specific for each agent of this blackboard
      /// </summary>
      public Symbol.Table Locals = new Symbol.Table();

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      /// <summary>
      /// An unique ID for this blackboard instance
      /// </summary>
      private int ID;
      /// <summary>
      /// A map of all blackboard instances. This is used to share globals among instances of 
      /// specific blackboards.
      /// </summary>
      private static Dictionary<int, Blackboard> Instances = new Dictionary<int, Blackboard>();

      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// Returns an instance of this blackboard asset, making a copy of its locals
      /// and using a reference to an unique (static) instance for its globals
      /// (so that they can be shared among all agents using that blackboard)
      /// </summary>
      /// <returns></returns>
      public Blackboard Instantiate()
      {
        // Get the ID of this asset
        var id = this.GetInstanceID();
        // If an instance of this blackboard has not already been instantiated, add it
        if (!Instances.ContainsKey(id))
          Instances.Add(id, this);

        // Now create a new blackboard instance, giving it its own local copy
        // and using a reference to the shared one
        var blackboard = ScriptableObject.CreateInstance<Blackboard>();
        blackboard.Globals = Instances[id].Globals;
        blackboard.Locals = new Symbol.Table(this.Locals);
        return blackboard;
      }

    }
  }
}