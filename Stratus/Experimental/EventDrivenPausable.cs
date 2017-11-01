using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Upon receiving specific pause and resume events, pauses the object
  /// </summary>
  /// <typeparam name="PauseEvent"></typeparam>
  /// <typeparam name="ResumeEvent"></typeparam>
  public abstract class EventDrivenPausable<PauseEvent, ResumeEvent> : PausableObject
    where PauseEvent : Stratus.Event
    where ResumeEvent : Stratus.Event
  {
    protected override void SetPauseMechanism()
    {
      Scene.Connect<PauseEvent>(this.OnPauseEvent);
      gameObject.Connect<PauseEvent>(this.OnPauseEvent);

      Scene.Connect<ResumeEvent>(this.OnResumeEvent);
      gameObject.Connect<ResumeEvent>(this.OnResumeEvent);
    }

    void OnPauseEvent(PauseEvent e)
    {
      this.Pause(true);
    }

    void OnResumeEvent(ResumeEvent e)
    {
      this.Pause(false);
    }

  }

}