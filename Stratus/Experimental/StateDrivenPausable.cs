using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Upon being notified of a specific state change, pauses/unpauses the object
  /// </summary>
  /// <typeparam name="State"></typeparam>
  public abstract class StateDrivenPausable<State> : PausableObject where State : struct
  {
    private Gamestate<State>.EventHandler pauseHandler { get; set; }
    private State pauseState { get; }

    protected override void SetPauseMechanism()
    {
      pauseHandler = new Gamestate<State>.EventHandler(pauseState);
      pauseHandler.Set(this.Pause);
    }


  }

}