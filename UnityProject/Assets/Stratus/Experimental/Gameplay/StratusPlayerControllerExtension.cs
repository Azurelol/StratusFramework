using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Experimental
{
  [RequireComponent(typeof(StratusPlayerController))]
  public abstract class StratusPlayerControllerExtension : StratusBehaviour
  {
    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/
    protected StratusPlayerController playerController { get; set; }
    protected abstract void OnAwake();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    private void Awake()
    {
      playerController = gameObject.GetComponent<StratusPlayerController>();
      OnAwake();
    }

    private void Reset()
    {
      CheckForPlayerController();
    }

    private void OnEnable()
    {
      
    }

    private void OnDisable()
    {
      
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    private void CheckForPlayerController()
    {
      playerController = gameObject.GetComponent<StratusPlayerController>();
      if (playerController)
      {
        this.hideFlags = HideFlags.HideInInspector;
        playerController.Add(this);
      }
      else
      {
        Trace.Script("Player controller not found");
        this.hideFlags = HideFlags.None;
      }
    }

  }
}
