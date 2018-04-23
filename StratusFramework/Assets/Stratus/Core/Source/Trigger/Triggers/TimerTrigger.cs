/******************************************************************************/
/*!
@file   TimerTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using Stratus.Utilities;

namespace Stratus
{
  /// <summary>
  /// After a specified amount of time, triggers the event
  /// </summary>
  public class TimerTrigger : Trigger
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    public override string automaticDescription => $"On {duration} seconds elapsed";

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Timer")]
    /// <summary>
    /// How long to wait before activating this trigger
    /// </summary>
    [Tooltip("How long to wait before activating this trigger")]    
    public float duration;
    [Tooltip("Reset the current timer if the trigger is disabled")]
    public bool resetOnDisabled = true;
    private Countdown timer;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    protected override void OnAwake()
    {
      timer = new Countdown(duration);
      timer.SetCallback(OnTimerFinished);
      timer.resetOnFinished = resetOnDisabled;
    }

    protected override void OnReset()
    {
    }

    private void Update()
    {
      timer.Update(Time.deltaTime);
    }

    private void OnEnable()
    {      
      //this.RunTimer();
    }

    private void OnDisable()
    {
      if (resetOnDisabled)
        timer.Reset();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    //void RunTimer()
    //{
    //  var seq = Actions.Sequence(this.gameObject.Actions());
    //  Actions.Delay(seq, this.duration);
    //  Actions.Call(seq, this.Activate);
    //}

    private void OnTimerFinished()
    {
      Activate();

    }

  }

}