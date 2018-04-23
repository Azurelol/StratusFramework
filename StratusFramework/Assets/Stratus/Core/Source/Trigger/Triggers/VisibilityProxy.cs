using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Utilities;

namespace Stratus
{
  /// <summary>
  /// Observes whether a GameObject with a mesh renderer is visible to the camera currently
  /// </summary>
  [RequireComponent(typeof(MeshRenderer))]
  public class VisibilityProxy : Proxy
  {
    public delegate void OnVisible(bool visible);

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("How long the target must be visible")]
    public float duration = 1.0f;
    [Tooltip("The object's distance from the camera")]
    [Range(0f, 100f)]
    public float distance = 15f;
    [Tooltip("How long to wait before resetting the timer if visibility is interrupted")]   
    public float resetDelay = 0.5f;

    // Private fields
    private Countdown visibilityTimer;
    [HideInInspector]
    public bool isVisible;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public OnVisible onVisible { get; private set; }
    public bool withinRange
    {
      get
      {
        if (Camera.current == null)
          return false;

        return Vector3.Distance(Camera.current.transform.position, transform.position) <= distance;
      }
    }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Start()
    {
      visibilityTimer = new Countdown(duration);
      //Overlay.Watch(() => visibilityTimer.current, "Visibility Timer");
      //Overlay.Watch(() => isVisible, "Visible");
    }

    private void Update()
    {
      //Trace.Script("Camera main = " + Camera.main);
      // If the object is visible and we haven't run the timer down
      if (isVisible && !visibilityTimer.isFinished && withinRange)
      {
        // If the timer finished, it means we can now announce
        // the object's visibility
        if (visibilityTimer.Update(Time.deltaTime))
          onVisible(true);
      }
      // If the object is not visible (but previously was)
      // announce that too
      else if (!isVisible && visibilityTimer.isFinished)
      {
        onVisible(false);
        visibilityTimer.Reset();
      }
    }

    private void OnBecameVisible()
    {
      isVisible = true;
      //Trace.Script("Visible!");
    }

    private void OnBecameInvisible()
    {
      isVisible = false;
      visibilityTimer.Reset();
      //Trace.Script("Not Visible!");
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Constructs a proxy in order to observe another GameObject's visibility messages
    /// </summary>
    /// <param name="target"></param>
    /// <param name="type"></param>
    /// <param name="onCollision"></param>
    /// <param name="persistent"></param>
    /// <returns></returns>
    public static VisibilityProxy Construct(MeshRenderer target, OnVisible onVisible, float duration, float distance, bool persistent = true)
    {
      var proxy = target.gameObject.AddComponent<VisibilityProxy>();
      proxy.onVisible += onVisible;
      proxy.duration = duration;
      proxy.distance = distance;
      proxy.persistent = persistent;
      return proxy;
    }

  }
}
