/******************************************************************************/
/*!
@file   InteractiveObject.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus.AI
{
  public abstract class Interactable : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether to show debug output for this object
    /// </summary>
    public bool log = false;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Relevant data about this object
    /// </summary>
    protected abstract object[] data { get; }

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void OnAwake();
    protected abstract void OnInteract(Sensor sensor);
    protected abstract void OnDetect(Sensor sensor);

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Awake()
    {
      this.gameObject.Connect<Sensor.InteractEvent>(this.OnInteractEvent);
      this.gameObject.Connect<Sensor.DetectionEvent>(this.OnDetectEvent);
      OnAwake();
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    /// <summary>
    /// Received when this object has been detected by an agent
    /// </summary>
    /// <param name="e"></param>

    void OnDetectEvent(Sensor.DetectionEvent e)
    {
      // Fill out information about this object
      e.scanData = this.data;
      // Inform this object that it has been scanned
      OnDetect(e.sensor);
    }

    /// <summary>
    /// Received when there's a request to interact with this object.
    /// </summary>
    /// <param name="e"></param>
    void OnInteractEvent(Sensor.InteractEvent e)
    {
      // Signal this object that it's being interacted with
      this.OnInteract(e.sensor);
    }

  }
}