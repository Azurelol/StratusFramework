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


namespace Stratus
{
  namespace Examples
  {
    // Whenever using the Stratus Event System, please use the provided MonoBehaviour
    public class EventsSample : MonoBehaviour
    {
      /// <summary>
      /// Custom events must derive from our custom event class 
      /// </summary>
      public class SampleEvent : Stratus.StratusEvent
      {
        public int number;
        public Vector2 vector2;
        public bool boolean;
      }

      void Start()
      {
        SampleEventToGameObject();
        SampleEventToScene();
      }

      void SampleEventToGameObject()
      {
        StratusDebug.Log("Dispatching event!", this);
        // Connect a member function to the given event
        this.gameObject.Connect<SampleEvent>(OnSampleEvent);
        // Construct the event object
        SampleEvent eventObj = new SampleEvent();
        eventObj.number = 5;
        // Dispatch the event
        StratusDebug.Log("Event dispatched", this);
        this.gameObject.Dispatch<SampleEvent>(eventObj);
      }

      void SampleEventToScene()
      {
        StratusDebug.Log("Connecting the member function on this component to an event sent to the space");
        // Connect a member function to the given event
        Scene.Connect<SampleEvent>(this.OnSampleEvent);
        // Construct the event object
        SampleEvent eventObj = new SampleEvent();
        eventObj.number = 15;
        // Dispatch the event
        StratusDebug.Log("Event about to be dispatched");
        Scene.Dispatch<SampleEvent>(eventObj);
      }

      /// <summary>
      /// The callback function called when an event has been dispatched.
      /// </summary>
      /// <param name="eventObj">The event object, a custom class which may contain member variables.</param>
      public void OnSampleEvent(SampleEvent eventObj)
      {
        StratusDebug.Log("Event received!", this);
        StratusDebug.Log("Number = " + eventObj.number, this);
      }


    } 
  }

}