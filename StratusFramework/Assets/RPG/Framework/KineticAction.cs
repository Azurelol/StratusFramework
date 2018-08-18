/******************************************************************************/
/*!
@file   KineticAction.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Altostratus
{
  /// <summary>
  /// Defines kinetic actions
  /// </summary>
  public static class KineticAction  
  {
    /// <summary>
    /// Base kinetic event
    /// </summary>
    public abstract class KineticEvent : Stratus.Event
    {
      public Transform Source;
      public float Amount;
      public KineticEvent(Transform source, float amount)
      {
        Source = source;
        Amount = amount;
      }
    }

    /// <summary>
    /// Pulls the character towards the source.
    /// </summary>
    public class PullEvent : KineticEvent
    {
      public PullEvent(Transform source, float amount) : base(source, amount) { }
    }
    /// <summary>
    /// Pushes the character away from the source.
    /// </summary>
    public class PushEvent : KineticEvent
    {
      public PushEvent(Transform source, float amount) : base(source, amount) { }
    }
    /// <summary>
    /// Launches the character into the air.
    /// </summary>
    public class LaunchEvent : KineticEvent
    {
      public LaunchEvent(Transform source, float amount) : base(source, amount) { }
    }

    /// <summary>
    /// Pushes the given target away from the source
    /// </summary>
    /// <param name="target"></param>
    /// <param name="amount"></param>
    public static void Push(Transform target, Transform source, float amount)
    {
      // A push is basically a reverse pull?
      Pull(target, source, -amount);
    }

    /// <summary>
    /// Pulls the target object into the source
    /// </summary>
    /// <param name="target"></param>
    /// <param name="amount"></param>
    public static void Pull(Transform target, Transform source, float amount)
    {
      target.GetComponent<Rigidbody>().AddForce((source.position - target.position) * amount, ForceMode.Impulse);
    }

    /// <summary>
    /// Launches the target into the air at a specified height
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="height"></param>
    public static void Launch(Transform target, float height)
    {
      target.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, height, 0.0f), ForceMode.Impulse);
    }


  }
}
