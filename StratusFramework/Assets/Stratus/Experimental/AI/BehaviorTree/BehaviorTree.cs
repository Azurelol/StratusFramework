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
        protected override string GetName()
        {
          if (!string.IsNullOrEmpty(data.label))
            return $"{dataTypeName} ({data.name})";
          return $"{dataTypeName}";
        }

        public void Update(Agent agent) => data.Update(agent);
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
      public BehaviorNode currentNode { get; private set; }
      protected override Behavior currentBehavior => currentNode.data;
      protected override bool hasBehaviors => tree.hasElements;
      protected Stack<BehaviorNode> stack { get; private set; } = new Stack<BehaviorNode>();

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      protected override void OnInitialize()
      {
        this.tree.Iterate(this.SetComposites);
        this.OnReset();
      }

      protected override void OnUpdate(Agent agent)
      {
        //this.current.children
        this.currentNode.data.Update(agent);
      }

      protected override void OnReset()
      {
        if (tree.hasElements)
        {
          this.currentNode = (BehaviorNode)tree.root.GetChild(0);
          this.stack.Push(this.currentNode);
        }
      }

      protected override void OnPrint(StringBuilder builder)
      {

      }

      protected override void OnBehaviorStarted(Behavior behavior)
      {

      }

      protected override void OnBehaviorEnded(Behavior behavior)
      {
      }

      protected override void OnBehaviorAdded(Behavior behavior)
      {
        this.tree.AddElement(behavior);
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
        this.tree.root.data = Behavior.Instantiate(typeof(Sequence));
      }

      private void SetComposites(BehaviorNode behaviorNode)
      {
        Composite composite = behaviorNode.data as Composite;
        composite?.SetChildren(behaviorNode.GetChildrenData());
      }



    }
  }

}