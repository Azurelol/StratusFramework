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

namespace Stratus 
{
  public class Checkpoint : MonoBehaviour 
  {
    public class AnnounceEvent : Stratus.Event { public Checkpoint Checkpoint; }
    public static List<Checkpoint> available { get; private set; } = new List<Checkpoint>();

    void Start()
    {
      // Announce that this checkpoint exists!
      var e = new AnnounceEvent();
      e.Checkpoint = this;
      Scene.Dispatch<AnnounceEvent>(e);
    }

    private void OnEnable()
    {
      //Trace.Script("Checkpoint up!", this);
      available.Add(this);
    }

    private void OnDisable()
    {
      available.Remove(this);
    }

    /// <summary>
    /// Warps the selected transform onto the checkpoint
    /// </summary>
    /// <param name="transform"></param>
    public void WarpTo(Transform obj)
    {
      obj.position = this.transform.position;
    }

    /// <summary>
    /// Returns the world space position of the specified checkpoint
    /// </summary>
    /// <param name="checkpointName"></param>
    /// <returns></returns>
    public static Vector3 GetPosition(string checkpointName)
    {
      return available.Find(x => x.name == checkpointName).transform.position;
    }


  }  
}
