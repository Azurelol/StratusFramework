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
        //public BehaviorNode(BehaviorNode behavior) : base(behavior) {}
        protected override string GetName() => this.data.name;
      }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The root node of this behavior tree
      /// </summary>
      [SerializeField]
      public List<BehaviorNode> nodes = new List<BehaviorNode>();

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

      public override void OnReset()
      {
        this.current = this.nodes[0].data;
      }

      protected override void OnPrint(StringBuilder builder)
      {

      }

      public override void OnBehaviorStarted(Behavior behavior)
      {
        throw new NotImplementedException();
      }

      public override void OnBehaviorEnded(Behavior behavior)
      {
        throw new NotImplementedException();
      }

      public override void OnBehaviorAdded(Behavior behavior)
      {
        BehaviorNode node = new BehaviorNode();
        node.Set(behavior);
        nodes.Add(node);
      }



    }
  }

}