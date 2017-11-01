
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
  public abstract class PausableObject : MonoBehaviour
  {
    // Properties
    private bool isInitialized { get; set; }

    // Fields
    private Behaviour[] behaviours;
    private Rigidbody[] rigidbodies;
    private ParticleSystem[] particleSystems;
    private bool[] particleStates;
    private Vector3 previousVelocity;

    // Virtual
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
      this.isInitialized = true;
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
      if (!isInitialized)
        RegisterPausableComponents();
      
      ToggleBehaviours(paused);
      ToggleRigidBodies(paused);
      ToggleParticleSystems(paused);
    }

    /// <summary>
    /// Registers at all sibling components that can be paused and adds
    /// them to a list. It will iterate through this list in order to pause/unpause them as needed.
    /// </summary>
    void RegisterPausableComponents()
    {
      // Add all behaviours (except renderers and lights)
      this.behaviours = (from behavior
                        in gameObject.GetComponentsInChildren<Behaviour>()
                         where !(behavior is Light)
                         select behavior).ToArray();

      // Add rigidbodies
      this.rigidbodies = this.gameObject.GetComponentsInChildren<Rigidbody>();

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
    void ToggleBehaviours(bool paused)
    {
      // Other behaviours
      foreach (var behaviour in behaviours)
      {
        behaviour.enabled = !paused;
      }
    }

    /// <summary>
    /// Toggles all rigidbodies
    /// </summary>
    /// <param name="paused"></param>
    void ToggleRigidBodies(bool paused)
    {
      foreach (var rb in rigidbodies)
      {
        if (paused)
        {
          rb.useGravity = false;
          previousVelocity = rb.velocity;
          rb.Sleep();
        }
        else
        {
          rb.useGravity = true;
          rb.WakeUp();
          rb.velocity = previousVelocity;
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