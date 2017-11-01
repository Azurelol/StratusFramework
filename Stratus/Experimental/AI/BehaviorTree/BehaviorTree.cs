/******************************************************************************/
/*!
@file   BehaviorTree.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
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
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The root node of this behavior tree
      /// </summary>
      public Behavior Root;      

      //----------------------------------------------------------------------/
      // Interface
      //----------------------------------------------------------------------/
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

      protected override void OnStart()
      {
        throw new NotImplementedException();
      }

      protected override void OnSubscribe()
      {
        throw new NotImplementedException();
      }
    }
  }

}