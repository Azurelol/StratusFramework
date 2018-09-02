using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace Stratus.AI
{
  class BehaviorTreeTests 
  {
    private static Blackboard MakeTestBlackboard()
    {
      Blackboard blackboard = ScriptableObject.CreateInstance<Blackboard>();      
      return blackboard;
    }

    //------------------------------------------------------------------------/
    // Blackboards
    //------------------------------------------------------------------------/
    [Test]
    public static void TestBlackboardAccess()
    {
      Blackboard blackboard = MakeTestBlackboard();
      string key = "Lives";
      int value = 7;
      blackboard.globals.Add(new Symbol(key, value));      
      Assert.AreEqual(blackboard.GetGlobal(key), value);
    }

    [Test]
    public static void TestBlackboardUpdate()
    {
      Blackboard blackboard = MakeTestBlackboard();

      string key = "Lives";
      int value = 7;

      blackboard.globals.Add(new Symbol(key, value));
      Assert.AreEqual(blackboard.GetGlobal(key), value);

      int nextValue = 14;
      blackboard.SetGlobal(key, nextValue);
      Assert.AreEqual(blackboard.GetGlobal(key), nextValue);
    }

    //------------------------------------------------------------------------/
    // Behavior Tree
    //------------------------------------------------------------------------/
    [Test]
    public static void Sequence()
    {
      BehaviorTree tree = ScriptableObject.CreateInstance<BehaviorTree>();
      
      tree.blackboard = MakeTestBlackboard();
      tree.blackboard.AddGlobal(new Symbol("Integer", 5));

      Sequence seq = tree.AddBehavior<Sequence>();

      tree.UpdateSystem();
      
    }
  }

}