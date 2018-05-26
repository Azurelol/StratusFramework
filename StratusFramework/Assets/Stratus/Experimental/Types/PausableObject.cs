/******************************************************************************/
/*!
@file   PauseBehaviour.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus;
using System.Linq;
using UnityEngine.AI;
using System.Linq.Expressions;

namespace Stratus
{
  /// <summary>
  /// Handles pausing the currently running behaviors on the
  /// gameobject (Rigidbodies, AI, etc)
  /// </summary>
  public abstract class PausableObject : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Definitions
    //------------------------------------------------------------------------/
    public class ComponentState<ComponentType, State> where ComponentType : Component where State : struct
    {
      public ComponentType component;
      public State lastState;
      //public State beforeLastState;

      public ComponentState(ComponentType component, State state)
      {
        this.component = component;
        this.lastState = state;
      }      
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("Whether lights should be considered for pausing")]
    public bool ignoreLights = true;

    private List<ComponentState<Behaviour, bool>> behaviours = new List<ComponentState<Behaviour, bool>>();
    private List<ComponentState<Rigidbody, Vector3>> rigidbodies = new List<ComponentState<Rigidbody, Vector3>>();
    private ParticleSystem[] particleSystems;
    private bool[] particleStates;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this pausable objet has been initialized
    /// </summary>
    private bool initialized { get; set; }
    /// <summary>
    /// Whether the objects are being currently paused
    /// </summary>
    public static bool paused { get; private set; }

    //------------------------------------------------------------------------/
    // Virtual
    //------------------------------------------------------------------------/
    protected abstract void SetPauseMechanism();
    protected virtual void OnStart() {}

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Start()
    {
      SetPauseMechanism();
      RegisterPausableComponents();
      OnStart();
      initialized = true;
    }
    
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Pauses/unpauses all registered components.
    /// </summary>
    /// <param name="paused"></param>
    protected void Pause(bool paused)
    {
      if (!initialized)
        RegisterPausableComponents();

      PausableObject.paused = paused;
      bool enabled = !paused;
      ToggleBehaviours(enabled);
      ToggleRigidBodies(enabled);

      ToggleParticleSystems(paused);
    }

    /// <summary>
    /// Registers at all sibling components that can be paused and adds
    /// them to a list. It will iterate through this list in order to pause/unpause them as needed.
    /// </summary>
    void RegisterPausableComponents()
    {
      // Add all behaviours (except renderers and lights)
      foreach (var behaviour in gameObject.GetComponentsInChildren<Behaviour>())
      {
        if (behaviour is Light && ignoreLights)
          continue;

        behaviours.Add(new ComponentState<Behaviour, bool>(behaviour, behaviour.enabled));
      }

      // Add all rigidbodies
      foreach (var rb in gameObject.GetComponentsInChildren<Rigidbody>())
      {
        rigidbodies.Add(new ComponentState<Rigidbody, Vector3>(rb, rb.velocity));
      }

      //this.behaviours = (from behavior
      //                  in gameObject.GetComponentsInChildren<Behaviour>()
      //                   where !(behavior is Light)
      //                   select behavior).ToArray();

      // Add rigidbodies
      //this.rigidbodies = this.gameObject.GetComponentsInChildren<Rigidbody>();

      // Add particles, saving their states
      this.particleSystems = this.gameObject.GetComponentsInChildren<ParticleSystem>();
      this.particleStates = new bool[particleSystems.Length];
      for (var i = 0; i < particleSystems.Length; ++i)
      {
        var particle = particleSystems[i];
        particleStates[i] = particle.isPaused;
      }
    }

    /// <summary>
    /// Toggles all behaviour-derived components
    /// </summary>
    /// <param name="paused"></param>
    void ToggleBehaviours(bool enabled)
    {
      foreach (var behaviour in behaviours)
      {
        // If we should enable this component, only attempt to do so if the following conditions are met:
        // a) the object is currently enabled
        bool currentlyEnabled = behaviour.component.enabled;
        // b) if it was previously enabled before the last pause
        bool previouslyEnabled = (behaviour.lastState == true);
        if (enabled && !currentlyEnabled && previouslyEnabled)
        {
          behaviour.component.enabled = true;        
          behaviour.lastState = currentlyEnabled;
        }

        // If we need to disable this component, only do so if it is currently enabled
        else if (!enabled && currentlyEnabled)
        {
          behaviour.component.enabled = false;
          behaviour.lastState = currentlyEnabled;
        }
      }
    }

    /// <summary>
    /// Toggles all rigidbodies
    /// </summary>
    /// <param name="paused"></param>
    void ToggleRigidBodies(bool enabled)
    {
      foreach (var rb in rigidbodies)
      {
        // If we should enable this rigidbody, only do so if:
        // a) it is currently sleeping
        bool sleeping = rb.component.IsSleeping();
        if (enabled && sleeping)
        {
          rb.component.useGravity = true;
          rb.component.WakeUp();
          rb.component.velocity = rb.lastState;          
        }
        else if (!enabled && !sleeping)
        {
          rb.component.useGravity = false;
          rb.lastState = rb.component.velocity;
          rb.component.Sleep();
        }
      }
    }

    /// <summary>
    /// Toggles all particle systems
    /// </summary>
    /// <param name="paused"></param>
    void ToggleParticleSystems(bool paused)
    {
      for (var i = 0; i < particleSystems.Length; ++i)
      {
        var particle = particleSystems[i];
        var previouslyPaused = particleStates[i];
        var currentlyPaused = false;

        if (paused)
        {
          if (particle.isPlaying)
          {
            currentlyPaused = true;
            particle.Pause(true);
          }
        }
        else
        {
          if (previouslyPaused)
          {
            particle.Play(true);
          }
        }

        particleStates[i] = currentlyPaused;

      }
    }
       

  }

}