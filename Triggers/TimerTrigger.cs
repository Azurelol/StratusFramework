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

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class TimerTrigger 
  */
  /**************************************************************************/
  public class TimerTrigger : EventTrigger
  {
    public float Timer;

    /**************************************************************************/
    /*!
    @brief  Initializes the TimerTrigger.
    */
    /**************************************************************************/
    protected override void OnInitialize()
    {
      if (Enabled)
        this.RunTimer();
    }

    protected override void OnEnabled()
    {
      this.RunTimer();
    }

    void RunTimer()
    {
      var seq = Actions.Sequence(this.gameObject.Actions());
      Actions.Delay(seq, this.Timer);
      Actions.Call(seq, this.Trigger);
    }

  }

}