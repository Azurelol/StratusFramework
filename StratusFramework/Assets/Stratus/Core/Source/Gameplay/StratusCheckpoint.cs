using UnityEngine;
using Stratus;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Stratus.Gameplay 
{
  public class StratusCheckpoint : Multiton<StratusCheckpoint> 
  {
    //--------------------------------------------------------------------------------------------/
    // Declarations
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Signals that a checkpoint hhas been added
    /// </summary>
    public class AnnounceEvent : Stratus.StratusEvent { public StratusCheckpoint Checkpoint; }
    
    //--------------------------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------------------------/

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    protected override void OnAwake()
    {     
    }

    protected override void OnMultitonEnable()
    {     
    }

    protected override void OnMultitonDisable()
    {     
    }

    protected override void OnReset()
    {      
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Warps the selected transform onto this checkpoint
    /// </summary>
    /// <param name="transform"></param>
    public void WarpOnto(Transform transform)
    {
      // If it's a NavMeshAgent, just warp it here
      NavMeshAgent navMeshAgent = transform.GetComponent<NavMeshAgent>();
      if (navMeshAgent != null)
        navMeshAgent.Warp(this.transform.position);
      else
        transform.position = this.transform.position;
    }

    /// <summary>
    /// Warps the selected transform onto this checkpoint
    /// </summary>
    /// <param name="transform"></param>
    public static void WarpOnto(StratusCheckpoint checkpoint, Transform transform) => checkpoint.WarpOnto(transform);

    /// <summary>
    /// Returns the world space position of the specified checkpoint
    /// </summary>
    /// <param name="checkpointName"></param>
    /// <returns></returns>
    public static Vector3 GetPosition(string checkpointName)
    {
      return available[checkpointName].transform.position;
      //return available.Find(x => x.name == checkpointName).transform.position;
    }


  }  
}
