/******************************************************************************/
/*!
@file   CombatEncounter.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using Stratus.Gameplay;
using System;

namespace Altostratus
{
  /// <summary>
  /// Initiates combat when triggered.
  /// </summary>
  public class CombatEncounter : Triggerable
  {
    /// <summary>
    /// What should happen to the object after combat ends.
    /// </summary>
    public enum ResolutionType { Corpse, Destroy, Dialog }
    
    public ResolutionType Resolution = ResolutionType.Corpse;
    /// <summary>
    /// The music clip to play for this combat encounter.
    /// </summary>
    public AudioClip Theme;
    /// <summary>
    /// The size of the arena to host the combat encounter.
    /// </summary>
    [Range(0, 50)] public int ArenaSize = 30;
    /// <summary>
    /// The camera transition
    /// </summary>
    //public ScreenTransition.TransitionEvent Transition = new ScreenTransition.TransitionEvent();
    /// <summary>
    /// The group of combatants we will be facing.
    /// </summary>
    [HideInInspector] public List<Character> Group = new List<Character>();
    /// <summary>
    /// Whether combat has been started.
    /// </summary>
    bool Engaged;

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    protected override void OnAwake()
    {
    }
    
    /// <summary>
    /// Initiates combat when a valid interaction is received.
    /// </summary>
    protected override void OnTrigger()
    {
      this.Begin();
    }

    protected override void OnReset()
    {
      throw new NotImplementedException();
    }

    /**************************************************************************/
    /*!
    @brief  Begins combat.
    */
    /**************************************************************************/
    void Begin()
    {
      if (Engaged)
        return;

      //if (CombatManager.Engaged)
      //  return;

      Engaged = true;

      var seq = Actions.Sequence(this);
      //Actions.Call(seq, this.IntroSequence);
      //Actions.Delay(seq, this.Transition.Duration);
      //Actions.Call(seq, this.RequestCombatStart);
      //Actions.Delay(seq, this)

      // Give control to the player once camera zooms back 

      // Start combat

      
    }

    void IntroSequence()
    {
      // Move the player's field character away from the enemy (Backstep?)
      //PlayerParty.Current.gameObject.Dispatch<Movement.SidestepEvent>(new Movement.SidestepEvent(Movement.Direction.Backward, Movement.Distance.Short));
      //// Zoom in on the enemy momentarily while actors spawn
      //Scene.Dispatch<ScreenTransition.TransitionEvent>(this.Transition);
      //CameraController.Zoom(Scene, this.gameObject, 15f, 2.0f, Ease.Linear, true);
    }

    void RequestCombatStart()
    {
      //CombatManager.Start(this);
    }

    void DisplayHUD()
    {

    }

    void ZoomOnTarget()
    {

    }


  }

}