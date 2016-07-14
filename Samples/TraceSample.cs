/******************************************************************************/
/*!
@file   TraceSample.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   7/14/2016
@brief  A demonstration of how to use the Trace class.
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

public class TraceSample : MonoBehaviour {

  // Field
  public int MemberInteger = 3;

  // Use this for initialization
  void Start () {
    Trace.Script("This method will decorate this message with the name of this script and the method as prefixes!");
    Trace.Script("The overload will act like the previous, but in addition add the name of the GameObject this component is attached to!", this);
    Trace.MemberValue(()=>this.MemberInteger);

  }
  
}
