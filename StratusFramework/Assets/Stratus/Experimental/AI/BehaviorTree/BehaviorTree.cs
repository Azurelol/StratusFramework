using UnityEngine;
using Stratus;
using System;
using System.Text;

namespace Stratus
{
  namespace AI
  {
    [CreateAssetMenu(fileName = "Behavior Tree", menuName = "Stratus/AI/Behavior Tree")]
    public partial class BehaviorTree : BehaviorSystem
    {
      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The root node of this behavior tree
      /// </summary>
      public Behavior root;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The current node
      /// </summary>
      public Behavior current;

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
      protected override void OnInitialize()
      {
        
      }

      protected override void OnUpdate(float dt)
      {

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

      public override void OnAssess()
      {
        throw new NotImplementedException();
      }



    }
  }

}