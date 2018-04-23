//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using XNode;
//using NodeEditorFramework.Standard;
//using NodeEditorFramework;

//namespace Stratus
//{
//  //[System.Serializable]
//  //public class TriggerNode : XNode.Node
//  //{
//  //  [Input] public float a;
//  //  [Input] public float b;
//  //  // The value of an output node field is not used for anything, but could be used for caching output results
//  //  [Output] public float result;
//  //  [Input] public float boo;
//  //
//  //  // Will be displayed as an editable field - just like the normal inspector
//  //  public MathType mathType = MathType.Add;
//  //  public enum MathType { Add, Subtract, Multiply, Divide }
//  //
//  //  // GetValue should be overridden to return a value for any specified output port
//  //  public override object GetValue(XNode.NodePort port)
//  //  {
//  //
//  //    // Get new a and b values from input connections. Fallback to field values if input is not connected
//  //    float a = GetInputValue<float>("a", this.a);
//  //    float b = GetInputValue<float>("b", this.b);
//  //
//  //    // After you've gotten your input values, you can perform your calculations and return a value
//  //    result = 0f;
//  //    if (port.fieldName == "result")
//  //      switch (mathType)
//  //      {
//  //        case MathType.Add: default: result = a + b; break;
//  //        case MathType.Subtract: result = a - b; break;
//  //        case MathType.Multiply: result = a * b; break;
//  //        case MathType.Divide: result = a / b; break;
//  //      }
//  //    return result;
//  //  }
//  //
//  //}

//  //[Node(false, "Trigger", typeof(TriggerSystemCanvas))]
//  //public class TriggerNode : NodeEditorFramework.Node
//  //{
//  //  public override string GetID { get; } = "triggerNode";
//  //
//  //  [ValueConnectionKnob("Out", Direction.Out, typeof(Trigger))]
//  //  public ValueConnectionKnob outputKnob;
//  //  //override No
//  //
//  //}

  

//}