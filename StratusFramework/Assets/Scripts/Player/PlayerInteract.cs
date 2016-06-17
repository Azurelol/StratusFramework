/******************************************************************************/
/*!
@file   PlayerInteract.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Overworld
{
  /**************************************************************************/
  /*!
  @class PlayerInteract 
  */
  /**************************************************************************/
  public class PlayerInteract : MonoBehaviour
  {
    // Events
    public class RequestInteractionEvent : Stratus.Event { public string Context; }
    public class ScanEvent : Stratus.Event { public string Context; }
    public class InteractEvent : Stratus.Event {}
    // Members
    public bool Debugging = true;
    public float Radius = 5.0f;
    public GameObject ContextText;
    //-----------------------------------/
    private GameObject InteractiveObject;
    private System.Collections.Generic.List<string> Interactions;

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    void Start()
    {
      this.gameObject.Connect<InteractEvent>(this.OnInteractEvent);
      this.gameObject.Connect<RequestInteractionEvent>(this.OnRequestInteractionEvent);
    }

    /**************************************************************************/
    /*!
    @brief When a scan hits a valid interact target, it will report back with some info.
    */
    /**************************************************************************/
    void OnRequestInteractionEvent(RequestInteractionEvent e)
    {
      // If the possible interaction has not been added, do so
      if (!Interactions.Contains(e.Context))
        Interactions.Add(e.Context);
        
    }

    void OnInteractEvent(InteractEvent e)
    {
      if (InteractiveObject)
      {
        Trace.Object("Interacting!", this);
        var interactEvent = new InteractEvent();
        this.InteractiveObject.Dispatch<InteractEvent>(interactEvent);
      }
    }

    /**************************************************************************/
    /*!
    @brief If debugging is active, draw the spherecast.
    */
    /**************************************************************************/
    void OnDrawGizmos()
    {
      if (Debugging)
      {
        var spherePos = this.transform.position + (this.transform.forward * this.Radius);
        Gizmos.DrawWireSphere(spherePos, this.Radius);
      }
    }

    /**************************************************************************/
    /*!
    @brief Look for possible interactions in the field and display them.
    */
    /**************************************************************************/
    void Update()
    {
      this.Scan();
    }

    /**************************************************************************/
    /*!
    @brief Scan for interactive objects in the vicinity
    */
    /**************************************************************************/    
    void Scan()
    {
      var startPos = this.transform.position;
      RaycastHit castResult;
      if (Physics.SphereCast(startPos, this.Radius, transform.forward, out castResult, this.Radius))
      {
        if (castResult.transform.gameObject.tag.Contains("Interactive"))
        {
          //Trace.Script("Can interact!");          
          InteractiveObject = castResult.transform.gameObject;
          var scan = new ScanEvent();
          InteractiveObject.Dispatch<ScanEvent>(scan);
          // Update the context
          ContextText.GetComponent<UnityEngine.UI.Text>().text = scan.Context;
        }
      }
      else
      {
        ContextText.GetComponent<UnityEngine.UI.Text>().text = "";
        //Trace.Script("No interact!");
        InteractiveObject = null;
        // Clear possible interactions
        // Interactions.Clear();
      }
    }

    /**************************************************************************/
    /*!
    @brief Display all possible interactions.
    */
    /**************************************************************************/
    void DisplayInteractions()
    {
      // For every possible interaction, display it, waiting a bit until the next one.
      foreach(var interaction in Interactions)
      {

      }
    }

  }

}