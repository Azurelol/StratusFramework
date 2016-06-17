using UnityEngine;
using System.Collections;

namespace Events
{
  public class EventsInput : MonoBehaviour
  {
    // Use this for initialization
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
      PollKeyDown();
      PollKeyUp();
      PollMouse();
    }

    void PollKeyDown()
    {
      if (UnityEngine.Input.GetKeyDown(KeyCode.A))
      {

      }
    }

    void PollKeyUp()
    {
    
    }

    void PollMouse()
    {
      if (UnityEngine.Input.GetMouseButton(0))
      {

      }
    }
  }
}
