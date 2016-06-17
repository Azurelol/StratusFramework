/******************************************************************************/
/*!
@file   EventsSample.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/9/2016
@brief  Sample script demonstrating how the custom event system "works".
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

using Stratus;


// Custom events must derive from our custom event class
public class SampleEvent : Stratus.Event
{
  public int Number;
}

public class EventsSample : MonoBehaviour
{

  /**************************************************************************/
  /*!
  @brief Initializes the script.
  */
  /**************************************************************************/
  void Start()
  {
    //SampleTestBase();
    SampleTestDerived(); // Work in progress...
    //SampleTestScene();
  }

  //---------------------------------------------------------------------------------------/
  void SampleTestBase()
  {
    Trace.Object("", this);    
    // Connect a member function to the given event
    Stratus.Events.Connect(gameObject, "SampleEvent", SampleCallbackBase);
    // Construct the event object
    SampleEvent eventObj = new SampleEvent();
    eventObj.Number = 5;
    // Dispatch the event
    this.gameObject.Dispatch("SampleEvent", eventObj);
    Trace.Object("Event dispatched", this);
  }
  
  public void SampleCallbackBase(Stratus.Event e)
  {
    Trace.Object("Event received!", this);
    // Cast to the derived event
    var eventObj = e as SampleEvent;
    Trace.Object("Number = " + eventObj.Number, this);
    //Trace.Variable(()=>eventObj.Number, this);
  }
  //---------------------------------------------------------------------------------------/  
  void SampleTestDerived()
  {
    Trace.Object("", this);
    // Connect a member function to the given event
    //Stratus.Events.Connect<SampleEvent>(gameObject, SampleCallbackDerived);
    gameObject.Connect<SampleEvent>(SampleCallbackDerived);
    // Construct the event object
    SampleEvent eventObj = new SampleEvent();
    eventObj.Number = 5;
    // Dispatch the event
    Trace.Object("Event dispatched", this);
    gameObject.Dispatch<SampleEvent>(eventObj);
  }

  public void SampleCallbackDerived(SampleEvent eventObj)
  {
    Trace.Object("Event received!", this);
    Trace.Object("Number = " + eventObj.Number, this);
    //Trace.Variable(() => eventObj.Number, this);
  }
  //---------------------------------------------------------------------------------------/
  void SampleTestScene()
  {
    Debug.Log("SampleTestScene: Connecting the member function on this component to an event sent to the space");
    // Connect a member function to the given event
    Stratus.Events.Connect(this.Space(), "SampleEvent", SampleCallbackSpace);
    //this.Space().Connect<LogicUpdate>(OnLogicUpdateEvent);
    // Construct the event object
    SampleEvent eventObj = new SampleEvent();
    eventObj.Number = 15;
    // Dispatch the event
    Debug.Log("Event dispatched");
    gameObject.Dispatch("SampleEvent", eventObj);
  }
  

  void SampleCallbackSpace(Stratus.Event e)  {
    // Cast to the derived event
    var eventObj = e as SampleEvent;
    Debug.Log("Number = '" + eventObj.Number + "', the event was received.. IN AMERICA!");
  }


}
