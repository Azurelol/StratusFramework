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

public class ActionsSample : StratusBehaviour
{

  // Field: Custom reference-type
  public int SampleInt = 5;
  // Field: Value type
  public float SampleFloat = 0;
  // Property: Value type POD
  public float Floaty { set; get; }
  // Property: Value type STRUCT
  public Vector2 Vecty2 { set; get; }

  void Start()
  {
    Trace.Script("Action Demonstration!", this);
    Floaty = 1.0f;
    Vecty2 = new Vector2();
    Test_ActionSequence();
    Test_ActionCall();
  }

  void Test_ActionSequence()
  {
    var seq = Actions.Sequence(this);
    Actions.Delay(seq, 2.0f);
    Actions.Call(seq, this.PrintValue);
    Actions.Property(seq, () => this.SampleInt, 16, 2.0f, Ease.Linear);
    Actions.Property(seq, () => this.SampleFloat, 25, 2.0f, Ease.Linear);
    Actions.Property(seq, () => this.Floaty, 3.0f, 2.0f, Ease.Linear);
    Actions.Property(seq, () => this.Vecty2, new Vector2(3, 5), 2.0f, Ease.Linear);
    Actions.Trace(seq, "John-upperclassman, notice me!");
    Actions.Call(seq, this.PrintValue);
    Actions.Delay(seq, 0.5f);
  }

  void Test_ActionCall()
  {
    var seq = Actions.Sequence(this);
    Actions.Delay(seq, 2.0f);
    Actions.Call(seq, () => Boop(6f));
    Actions.Call(seq, () => Beep("Hello", "World", this.gameObject));
  }

  void Boop(float boopValue)
  {
    Trace.Script("Booped for '" + boopValue + "' points!", this);
  }

  void Beep(string first, string second, GameObject obj)
  {
    Trace.Script(obj + " says = " + first + " " + second);
  }

  void PrintValue()
  {
    //Trace.Script("Floaty = " + Floaty);
    //Trace.Field(() => this.SampleFloat, this);
    //Trace.Script("Vector2 = " + Vecty2);
  }

}
