using UnityEngine;
using Stratus;
using System;

namespace Genitus
{
  public abstract partial class CombatController : StratusBehaviour
  {


    /// <summary>
    /// Queues the given action.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Queue(CombatAction action)
    {
      // Request the controller to attack the given target
      var queuedAction = new CombatAction.QueueEvent();
      queuedAction.Action = action;
      this.gameObject.Dispatch<CombatAction.QueueEvent>(queuedAction);
    }

    /// <summary>
    /// Pauses the controller, cancelling the current action
    /// if there's any, and preventing it from taking action.
    /// </summary>
    protected void Pause()
    {
      this.Cancel();
      this.currentState = State.Inactive;
    }

    /// <summary>
    /// Cancels the controller's current action.
    /// </summary>
    protected void Cancel()
    {
      if (this.currentAction == null)
        return;

      //Trace.Script("Cancelling the current action!", this);
      this.currentAction.Cancel();
      this.OnActionCanceled();
      this.currentAction = null;
    }

    /// <summary>
    /// Delays the controller's current action
    /// </summary>
    /// <param name="delay"></param>
    protected void Delay(float delay)
    {
      this.OnActionDelay(delay);
    }

    /// <summary>
    /// Interrupt's the character's current action
    /// </summary>
    /// <param name="duration"></param>
    protected void Interrupt(float duration)
    {
      this.Cancel();
    }

    /// <summary>
    /// Resumes the controller's activity
    /// </summary>
    protected void Resume()
    {
      this.currentState = State.Active;
      AnnounceStatus<ActiveEvent>();
    }

    /// <summary>
    /// Called upon when this combat controller has been incapacitated.
    /// </summary>
    protected void Incapacitate()
    {
      //Trace.Script("Incapacitated!", this);
      this.currentState = State.Inactive;
      this.Cancel();
      AnnounceStatus<DeathEvent>();
      this.OnIncapacitate();
    }

    /// <summary>
    /// Revives this character, restoring its health and setting it back to active
    /// </summary>
    public void Revive()
    {
      this.OnRestore();
      this.currentState = State.Active;
      AnnounceStatus<ReviveEvent>();
    }

    /// <summary>
    /// Perform any cleanup operations on this controller.
    /// </summary>
    public void Reset()
    {
      //Trace.Script("Resetting!", this);
      this.currentAction = null;
      //AnnounceStatus<InactiveEvent>();
    }

    /// <summary>
    /// Announces a change of state
    /// </summary>
    /// <typeparam name="Event"></typeparam>
    void AnnounceStatus<Event>() where Event : CombatControllerEvent, new()
    {
      var e = new Event();
      e.controller = this;
      this.gameObject.Dispatch<StateChangedEvent>(new StateChangedEvent(this.currentState));
      this.gameObject.Dispatch<Event>(e);
      Scene.Dispatch<Event>(e);
    }


    /// <summary>
    /// Constructs the CombatController.
    /// </summary>
    /// <param name="character">A reference to the character to control.</param>
    /// <param name="mode">Its starting control mode.</param>
    /// <returns></returns>
    public static CombatController Construct(Character character, Character.ControlMode mode)
    {
      var CombatControllerObj = Instantiate(Resources.Load("Combat/CombatController")) as GameObject;
      CombatControllerObj.name = character.name;
      var controlModeEvent = new ChangeControlModeEvent();
      controlModeEvent.mode = mode;
      CombatControllerObj.Dispatch<ChangeControlModeEvent>(controlModeEvent);
      return CombatControllerObj.GetComponent<CombatController>();
    }    



  }
}