/******************************************************************************/
/*!
@file   ActionsSample.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   7/14/2016
@brief  A demonstration of how to compose Actions.
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

public class ActionsSample : MonoBehaviour {

  // Field: Custom reference-type
  Integer SampleInt = 5;
  // Field: Value type
  public float SampleFloat = 0;
  // Property: Value type POD
  public float Floaty { set; get; }
  // Property: Value type STRUCT
  public Vector2 Vecty2 { set; get; }

  void Start () {
    Trace.Script("Action Demonstration!", this);
    Floaty = 1.0f;
    Vecty2 = new Vector2();
    Test_ActionSequence();
  }

  void Test_ActionSequence()
  {
    Trace.Log("Starting Action Sequence/Delay/Call/Property test!");
    var seq = Actions.Sequence(this.gameObject.Actions());
      // Alternatively, it can be written in shorthand as:
      // var seq = Actions.Sequence(this);

    Actions.Delay(seq, 2.0f);
    Actions.Call(seq, this.PrintValue);
    Actions.Property(seq, this.SampleInt, 16, 2.0f, Ease.Linear);
    Actions.Property(seq, ()=>this.SampleFloat, 25, 2.0f, Ease.Linear);
    Actions.Property(seq, ()=>this.Floaty,      3.0f, 2.0f, Ease.Linear);
    Actions.Property(seq, () => this.Vecty2, new Vector2(3,5), 2.0f, Ease.Linear);
    Actions.Trace(seq, "John-upperclassman, notice me!");
    Actions.Call(seq, this.PrintValue);
    Actions.Delay(seq, 0.5f);
  }

  void Test_Translate()
  {
    var seq = Actions.Sequence(this.gameObject.Actions());
    Actions.Translate(seq, transform, new Vector3(0.0f, 5.0f, 2.0f), 2.0f, Ease.Linear);
    Actions.Color(seq, this.gameObject.GetComponent<MeshRenderer>(), Color.red, 1.5f, Ease.Linear);    
  }
  
  void PrintValue()
  {
    Trace.Script("Floaty = " + Floaty);    
    Trace.Field(() => this.SampleFloat, this);
    Trace.Script("Vector2 = " + Vecty2);
  }
  
}
