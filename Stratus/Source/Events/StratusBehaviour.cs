/******************************************************************************/
/*!
@file   StratusBehaviour.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{

  //public interface IStratus
  //{
  //  //--------------------------------------------------------------------------/
  //  // Properties
  //  //--------------------------------------------------------------------------/
  //  bool Quitting { get; set; }
  //
  //  /// <summary>
  //  /// Invoked when the application is about to quit
  //  /// </summary>
  //  void OnApplicationQuit()
  //  {
  //    //Trace.Script("Quitting!");
  //    Quitting = true;
  //  }
  //
  //}

  /// <summary>
  /// A MonoBehaviour derivation to be used within the Stratus Event System. 
  /// This is due to needing to remove all delegates a component has subscribed to
  /// when it is being destroyed. Thus, the need to guarantee the cleanup code is
  /// always called on components which subscribe to events.
  /// </summary>
  public class StratusBehaviour : MonoBehaviour
  {
    //--------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------/
    bool Quitting = false;
    //--------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------/
    /// <summary>
    /// This is called by the StratusBehaviour class's OnDestroy method when the
    /// component is being destroyed.
    /// </summary>
    protected virtual void OnDestroyed() {}
    //--------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the StratusBehaviour script.
    /// </summary>
    void OnDestroy()
    {
      // If the application is quitting, do nothing
      if (Quitting)
        return;
      //Trace.Script(name + " is being destroyed!", this);
      // Disconnect from all events
      this.Disconnect();
      // Call the subclass method
      this.OnDestroyed();     
    }

    /// <summary>
    /// Invoked when the application is about to quit
    /// </summary>
    void OnApplicationQuit()
    {
      //Trace.Script("Quitting!");
      Quitting = true;
    }
            

  }



}