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
      public BehaviorNode rootNode => (BehaviorNode)tree.root.GetChild(0);
      //public BehaviorNode currentNode { get; private set; }
      protected override Behavior currentBehavior => stack.Peek();
      protected override bool hasBehaviors => tree.hasElements;
      protected Stack<Behavior> stack { get; private set; } = new Stack<Behavior>();

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      protected override void OnInitialize()
      {
        this.tree.Iterate(this.SetComposites);
        this.OnReset();
      }

      protected override void OnUpdate()
      {
        this.rootNode.data.Update(this.behaviorArguments);
        //this.currentBehavior.Update(this.behaviorArguments);
      }

      protected override void OnReset()
      {
        if (tree.hasElements)
        {
          //this.currentNode = rootNode;
          this.stack.Clear();
          this.rootNode.data.Reset();
          this.stack.Push(this.rootNode.data);
        }
      }

      public override void OnBehaviorStarted(Behavior behavior)
      {
        stack.Push(behavior);
        Trace.Script($"current behavior = {currentBehavior}");
      }

      public override void OnBehaviorEnded(Behavior behavior, Behavior.Status status)
      {
        stack.Pop();
        if (stack.Count == 0)
        {
          this.OnReset();
        }
        //Trace.Script($"current behavior = {currentBehavior}");
      }

      protected override void OnBehaviorAdded(Behavior behavior)
      {
        this.tree.AddElement(behavior);
      }

      public void RemoveBehavior(BehaviorNode behaviorNode)
      {
        this.tree.RemoveElement(behaviorNode);
      }

      public Behavior AddBehavior(Type behaviorType, BehaviorNode parent)
      {
        Behavior behavior = Behavior.Instantiate(behaviorType);
        AddBehavior(behavior, parent);
        return behavior;
      }

      public Behavior AddParentBehavior(Type behaviorType, BehaviorNode child)
      {
        Behavior behavior = Behavior.Instantiate(behaviorType);
        AddParentBehavior(behavior, child);
        return behavior;
      }

      public void AddBehavior(Behavior behavior, BehaviorNode parent)
      {
        if (behavior != null)
          this.tree.AddChildElement(behavior, parent);
      }

      public void AddParentBehavior(Behavior behavior, BehaviorNode child)
      {
        if (behavior != null)
          this.tree.AddParentElement(behavior, child);
      }

      protected override void OnBehaviorsCleared()
      {
        this.tree.Clear();
        this.tree.root.data = Behavior.Instantiate(typeof(Sequence));
      }

      private void SetComposites(BehaviorNode behaviorNode)
      {
        Composite composite = behaviorNode.data as Composite;
        composite?.Set(behaviorNode.GetChildrenData());
      }



    }
  }

}