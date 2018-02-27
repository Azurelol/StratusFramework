/******************************************************************************/
/*!
@file   Checkpoint.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Stratus 
{
  public class Checkpoint : StratusBehaviour 
  {
    /// <summary>
    /// Signals that a checkpoint hhas been added
    /// </summary>
    public class AnnounceEvent : Stratus.Event { public Checkpoint Checkpoint; }

    /// <summary>
    /// A map of all currently enabled checkpoints, indexed by their names
    /// </summary>
    public static Dictionary<string, Checkpoint> available { get; private set; } = new Dictionary<string, Checkpoint>();

    //--------------------------------------------------------------------------------------------/
    // Messages
    //--------------------------------------------------------------------------------------------/
    void Start()
    {
      var e = new AnnounceEvent();
      e.Checkpoint = this;
      Scene.Dispatch<AnnounceEvent>(e);
    }

    private void OnEnable()
    {
      available.Add(name, this);
    }

    private void OnDisable()
    {
      available.Remove(name);
    }

    private void OnValidate()
    {
      
    }

    //--------------------------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------------------------/
    /// <summary>
    /// Warps the selected transform onto this checkpoint
    /// </summary>
    /// <param name="transform"></param>
    public void WarpTo(Transform obj)
    {
      // If it's a NavMeshAgent, just warp it here
      NavMeshAgent navMeshAgent = obj.GetComponent<NavMeshAgent>();
      if (navMeshAgent != null)
        navMeshAgent.Warp(this.transform.position);
      else
        obj.position = this.transform.position;
    }

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
