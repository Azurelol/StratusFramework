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
    public partial class BehaviorTree : StratusBehaviorSystem 
    {
      //------------------------------------------------------------------------/
      // Declarations
      //------------------------------------------------------------------------/
      [Serializable]
      public class BehaviorNode : TreeElement<StratusAIBehavior>
      {
        protected override string GetName()
        {
          if (!string.IsNullOrEmpty(data.label))
            return $"{dataTypeName} ({data.label})";
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
      public StratusSerializedTree<BehaviorNode, StratusAIBehavior> tree = new StratusSerializedTree<BehaviorNode, StratusAIBehavior>();

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public BehaviorNode rootNode => (BehaviorNode)tree.root.GetChild(0);
      protected override StratusAIBehavior currentBehavior => stack.Last();
      protected override bool hasBehaviors => tree.hasElements;
      protected List<StratusAIBehavior> stack { get; private set; } = new List<StratusAIBehavior>();      

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      protected override void OnInitialize()
      {
        this.stack = new List<StratusAIBehavior>();
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
          this.tree = new StratusSerializedTree<BehaviorNode, StratusAIBehavior>();
          this.tree.root.data = StratusAIBehavior.Instantiate(typeof(StratusAISequence));
        }

        try
        {
          this.tree.Assert();
        }
        catch (Exception e)
        {
          StratusDebug.Log($"The tree {name} is damaged: '{e.Message}'. Attempting to repair...");
          this.tree.Repair();

          // Try again

          try
          {
            this.tree.Assert();
          }
          catch (Exception e2)
          {            
            StratusDebug.Log($"The tree {name} is damaged: '{e.Message}'");
            throw e2;
          }

        }        
      }
      
      public override void OnBehaviorStarted(StratusAIBehavior behavior)
      {
        //Trace.Script($"Adding {behavior}");
        stack.Add(behavior);
      }

      public override void OnBehaviorEnded(StratusAIBehavior behavior, StratusAIBehavior.Status status)
      {
        stack.RemoveLast();
        //Trace.Script($"Removing {behavior}");
        if (stack.Empty())
          this.OnReset();
      }

      protected override void OnBehaviorAdded(StratusAIBehavior behavior)
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
        StratusAIBehavior replacementBehavior = StratusAIBehavior.Instantiate(replacementBehaviorType);
        this.tree.ReplaceElement(original, replacementBehavior);
      }

      public StratusAIBehavior AddBehavior(Type behaviorType, BehaviorNode parent)
      {
        StratusAIBehavior behavior = StratusAIBehavior.Instantiate(behaviorType);
        AddBehavior(behavior, parent);
        return behavior;
      }

      public StratusAIBehavior AddParentBehavior(Type behaviorType, BehaviorNode child)
      {
        StratusAIBehavior behavior = StratusAIBehavior.Instantiate(behaviorType);
        AddParentBehavior(behavior, child);
        return behavior;
      }

      public void AddBehavior(StratusAIBehavior behavior, BehaviorNode parent)
      {
        if (behavior != null)
          this.tree.AddChildElement(behavior, parent);
      }

      public void AddParentBehavior(StratusAIBehavior behavior, BehaviorNode child)
      {
        if (behavior != null)
          this.tree.AddParentElement(behavior, child);
      }

      protected override void OnBehaviorsCleared()
      {
        this.tree.Clear();
        this.tree.root.data = StratusAIBehavior.Instantiate(typeof(StratusAISequence));
      }

      private void SetChildren(BehaviorNode behaviorNode)
      {
        if (behaviorNode.data is StratusAIComposite)
        {
          StratusAIComposite composite = behaviorNode.data as StratusAIComposite;
          composite?.Set(behaviorNode.GetChildrenData());
        }
        else if (behaviorNode.data is StratusAIDecorator)
        {
          StratusAIDecorator decorator = behaviorNode.data as StratusAIDecorator;
          if (behaviorNode.childrenCount > 0)
            decorator?.Set(behaviorNode.GetChildrenData().First());
        }
      }


    }
  }

}