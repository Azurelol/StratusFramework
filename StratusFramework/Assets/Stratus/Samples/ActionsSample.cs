using UnityEngine;
using System.Collections;

using Stratus;

public class ActionsSample : MonoBehaviour {

  Integer SampleInt = 5;
  Real SampleFloat = 0;
  Boolean SampleBool = false;
  Real2 SampleVec2 = new Real2(); // It's a reference type so it has to be initialized...

  void Start () {
    Trace.Object("Let's play children's card games", this);
    Test_ActionSequence();    
  }

  void Test_ActionSequence()
  {
    Trace.Log("Starting Action Sequence/Delay/Call/Property test!");
    var seq = Actions.Sequence(this.gameObject.Actions());
    Actions.Delay(seq, 2.0f);
    Actions.Call(seq, this.PrintValue);
    Actions.Property(seq, this.SampleVec2, new Vector2(5.0f, 3.0f), 2.0f, Ease.Linear);
    Actions.Property(seq, this.SampleInt, 16, 2.0f, Ease.Linear);
    Actions.Property(seq, this.SampleFloat, 25, 2.0f, Ease.Linear);
    Actions.Property(seq, this.SampleBool, true, 2.0f, Ease.Linear);
    Actions.Call(seq, this.JohnNoticeMe);
    Actions.Call(seq, this.PrintValue);
    Actions.Delay(seq, 0.5f);
  }

  void JohnNoticeMe()
  {
    Debug.Log("JOHN!!!!!!!!!!!!");
  }

  void PrintValue()
  {
    Trace.Variable(() => this.SampleBool, this);
    Trace.Variable(() => this.SampleFloat, this);
    Trace.Variable(() => this.SampleInt, this);
    Trace.Variable(() => this.SampleVec2, this);
  }
  
}
