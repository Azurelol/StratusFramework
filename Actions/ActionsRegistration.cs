/******************************************************************************/
/*!
@file   ActionsRegistration.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Registers this GameObject onto the Action system.
  /// </summary>
  public class ActionsRegistration : MonoBehaviour
  {
    GameObject Owner;
    bool Quitting = false;

    /// <summary>
    /// Saves a reference to this GameObject.
    /// </summary>
    void Start()
    {
      this.Owner = this.gameObject;
    }

    /// <summary>
    /// When this GameObject dies, it gets deregistered from the Stratus
    /// Actions system.
    /// </summary>
    void OnDestroy()
    {
      if (Quitting)
        return;

      //Trace.Script("Unsubscribed!", this);
      if (this.enabled)
        ActionSpace.Unsubscribe(this.Owner);
    }

    /// <summary>
    /// Invoked when the application is about to quit
    /// </summary>
    void OnApplicationQuit()
    {
      Quitting = true;
    }

  }

}