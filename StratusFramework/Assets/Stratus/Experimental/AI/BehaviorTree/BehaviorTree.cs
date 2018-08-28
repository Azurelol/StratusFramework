using UnityEngine;
using Stratus;
using System;
using System.Text;
using System.Collections.Generic;

namespace Stratus
{
  namespace AI
  {
    [CreateAssetMenu(fileName = "Behavior Tree", menuName = "Stratus/AI/Behavior Tree")]
    public partial class BehaviorTree : BehaviorSystem
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      [Serializable]
      public class BehaviorNode : TreeElement<Behavior>
      {
        public override string GetName()
        {
          if (!string.IsNullOrEmpty(data.label))
            return $"{dataTypeName} ({data.name})";
          return $"{dataTypeName}";
        }
      }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The root node of this behavior tree
      /// </summary>
      [OdinSerializer.OdinSerialize, HideInInspector]
      public SerializedTree<BehaviorNode, Behavior> tree = new SerializedTree<BehaviorNode, Behavior>();

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The current node
      /// </summary>
      public Behavior current { get; private set; }

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      protected override void OnInitialize()
      {
        this.Reset();
      }

      protected override void OnUpdate(float dt)
      {

      }

      protected override void OnReset()
      {
        //this.current = this.nodes[0].data;
      }

      protected override void OnPrint(StringBuilder builder)
      {

      }

      protected override void OnBehaviorStarted(Behavior behavior)
      {
        throw new NotImplementedException();
      }

      protected override void OnBehaviorEnded(Behavior behavior)
      {
        throw new NotImplementedException();
      }

      protected override void OnBehaviorAdded(Behavior behavior)
      {
        this.tree.AddElement(behavior);
        //// If there's no root node yet, set it
        //if (nodes.Empty() || !nodes[0].isRoot)
        //{
        //  nodes.Insert(0, TreeElement.MakeRoot<BehaviorNode>());
        //}
        //
        //BehaviorNode node = new BehaviorNode();
        //node.Set(behavior);
        //nodes.Add(node);
      }

      public void RemoveBehavior(BehaviorNode behaviorNode)
      {
        this.tree.RemoveElement(behaviorNode);
      }

      public void AddBehavior(Type behaviorType, BehaviorNode parent)
      {
        Behavior behavior = Behavior.Instantiate(behaviorType);
        if (behavior != null)
          this.tree.AddElement(behavior, parent);
      }

      protected override void OnBehaviorsCleared()
      {
        this.tree.Clear();
      }



    }
  }

}