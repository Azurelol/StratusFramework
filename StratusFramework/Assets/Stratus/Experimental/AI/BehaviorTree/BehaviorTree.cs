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
      protected override Behavior currentBehavior => stack.Last();
      protected override bool hasBehaviors => tree.hasElements;
      protected List<Behavior> stack { get; private set; } = new List<Behavior>();      

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      protected override void OnInitialize()
      {
        this.stack = new List<Behavior>();
        this.tree.Iterate(this.SetChildren);
        this.OnReset();
      }

      protected override void OnUpdate()
      {
        //this.rootNode.data.Update(this.behaviorArguments);
        //if (this.stack.NotEmpty())
        this.currentBehavior.Update(this.behaviorArguments);
      }

      protected override void OnReset()
      {
        if (tree.hasElements)
        {
          this.stack.Clear();
          this.stack.Add(this.rootNode.data);
        }        
      }

      protected override void OnAssert()
      {
        if (this.tree == null)
        {
          this.tree = new SerializedTree<BehaviorNode, Behavior>();
          this.tree.root.data = Behavior.Instantiate(typeof(Sequence));
        }

        try
        {
          this.tree.Assert();
        }
        catch (Exception e)
        {
          Trace.Script($"The tree {name} is damaged: '{e.Message}'. Attempting to repair...");
          this.tree.Repair();

          // Try again

          try
          {
            this.tree.Assert();
          }
          catch (Exception e2)
          {            
            Trace.Script($"The tree {name} is damaged: '{e.Message}'");
            throw e2;
          }

        }        
      }
      
      public override void OnBehaviorStarted(Behavior behavior)
      {
        //Trace.Script($"Adding {behavior}");
        stack.Add(behavior);
      }

      public override void OnBehaviorEnded(Behavior behavior, Behavior.Status status)
      {
        stack.RemoveLast();
        //Trace.Script($"Removing {behavior}");
        if (stack.Empty())
          this.OnReset();
      }

      protected override void OnBehaviorAdded(Behavior behavior)
      {
        this.tree.AddElement(behavior);
      }

      public void RemoveBehavior(BehaviorNode behaviorNode)
      {
        this.tree.RemoveElement(behaviorNode);
      }

      public void RemoveBehaviorExcludeChildren(BehaviorNode behaviorNode)
      {
        this.tree.RemoveElementExcludeChildren(behaviorNode);
      }

      public void ReplaceBehavior(BehaviorNode original, Type replacementBehaviorType)
      {
        Behavior replacementBehavior = Behavior.Instantiate(replacementBehaviorType);
        this.tree.ReplaceElement(original, replacementBehavior);
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

      private void SetChildren(BehaviorNode behaviorNode)
      {
        if (behaviorNode.data is Composite)
        {
          Composite composite = behaviorNode.data as Composite;
          composite?.Set(behaviorNode.GetChildrenData());
        }
        else if (behaviorNode.data is Decorator)
        {
          Decorator decorator = behaviorNode.data as Decorator;
          if (behaviorNode.childrenCount > 0)
            decorator?.Set(behaviorNode.GetChildrenData().First());
        }
      }


    }
  }

}