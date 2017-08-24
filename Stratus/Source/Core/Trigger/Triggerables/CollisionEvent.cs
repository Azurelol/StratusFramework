/******************************************************************************/
/*!
@file   CollisionEvent.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.Events;

namespace Prototype 
{
  [RequireComponent(typeof(Collider))]
  public class CollisionEvent : MonoBehaviour 
  {
    public class CollidedEvent : Stratus.Event { public GameObject Object; }
    public enum DispatchType { Event, Invoke }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    [Tooltip("How should the event be dispatched")] public DispatchType Type = DispatchType.Event;
    [Tooltip("What type of tag the target is using")] public string Tag;
    [Tooltip("The target to inform of this collision")] public GameObject Target;    
    //[HideInInspector] bool WithinColliderBounds;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    void Start()
    {
     // WithinColliderBounds = false;
    }

    void OnTriggerEnter(Collider otherObject)
    {
      // Check that the other object has the right tag
      if (!otherObject.CompareTag(this.Tag))
        return;

      //WithinColliderBounds = true;
      this.Inform(otherObject.gameObject);      
    }

    void OnTriggerExit(Collider otherObject)
    {
     // WithinColliderBounds = false;
    }

    void Inform(GameObject gameobj)
    {
      switch (Type)
      {
        case DispatchType.Event:
        var cO = new CollidedEvent();
        cO.Object = gameobj;
        this.Target.Dispatch<CollidedEvent>(cO);
          break;
        case DispatchType.Invoke:
          break;
      }


    }


  }  
}
