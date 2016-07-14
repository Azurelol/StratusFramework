/******************************************************************************/
/*!
@file   EventsSample.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   7/14/2016
@brief  Sample script demonstrating how the custom event system "works".
*/
/******************************************************************************/
using UnityEngine;
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
    SampleEventToGameObject();
    SampleEventToSpace();
  }
  
  //---------------------------------------------------------------------------------------/  
  void SampleEventToGameObject()
  {
    Trace.Script("Dispatching event!", this);
    // Connect a member function to the given event
    this.gameObject.Connect<SampleEvent>(OnSampleEvent);
    // Construct the event object
    SampleEvent eventObj = new SampleEvent();
    eventObj.Number = 5;
    // Dispatch the event
    Trace.Script("Event dispatched", this);
    this.gameObject.Dispatch<SampleEvent>(eventObj);
  }

  //---------------------------------------------------------------------------------------/
  void SampleEventToSpace()
  {
    Trace.Script("Connecting the member function on this component to an event sent to the space");
    // Connect a member function to the given event
    this.Space().Connect<SampleEvent>(this.OnSampleEvent);
    // Construct the event object
    SampleEvent eventObj = new SampleEvent();
    eventObj.Number = 15;
    // Dispatch the event
    Trace.Script("Event about to be dispatched");
    this.Space().Dispatch<SampleEvent>(eventObj);
  }
  //---------------------------------------------------------------------------------------/

  /**************************************************************************/
  /*!
  @brief The callback function called when an event has been dispatched.
  @param eventObj The event object, a custom class which may contain member variables.
  */
  /**************************************************************************/
  public void OnSampleEvent(SampleEvent eventObj)
  {
    Trace.Script("Event received!", this);
    Trace.Script("Number = " + eventObj.Number, this);
  }


}
