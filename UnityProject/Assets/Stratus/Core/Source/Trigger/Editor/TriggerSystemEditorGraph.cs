//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using XNode;

//namespace Stratus
//{
//  //[SingletonAsset("Stratus/Core/Resources", "Stratus Trigger System Graph")]
//  [Serializable, CreateAssetMenu(fileName = "Trigger System Graph", menuName = "Stratus/Trigger System Graph")]
//  public class TriggerSystemEditorGraph : XNode.NodeGraph
//  {
//    public TriggerSystemEditor triggerSystem;

//    public void Initialize(TriggerSystemEditor triggerSystem)
//    {
//      this.triggerSystem = triggerSystem;
//      Clear();
//      //Generate();
//    }

//    public void Generate()
//    {
//      Clear();

//      //foreach(var trigger in triggerSystem.triggers)
//      //{
//      //  var node = AddNode<TriggerNode>(false);
//      //  //node.trigger = trigger;
//      //  node.name = trigger.GetType().Name;        
//      //}
      
//      //foreach (var triggerable in triggerSystem.triggerables)
//      //{
//      //  var node = AddNode<TriggerableNode>(false);
//      //  node.triggerable = triggerable;
//      //  node.name = triggerable.GetType().Name;
//      //}


//    }
//  }



//  //[CustomNodeGraphEditor(typeof(TriggerSystemEditorGraph))]
//  //public class TriggerSystemEditorGraphEditor : XNodeEditor.NodeGraphEditor
//  //{    
//  //  override    
//  //
//  //}

//}