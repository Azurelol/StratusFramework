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
	public abstract class StratusEventDrivenPausable<PauseEvent, ResumeEvent> 
		: StratusPausableBehaviour
	  where PauseEvent : StratusEvent
	  where ResumeEvent : StratusEvent
	{
		protected override void SetPauseMechanism()
		{
			StratusScene.Connect<PauseEvent>(this.OnPauseEvent);
			gameObject.Connect<PauseEvent>(this.OnPauseEvent);

			StratusScene.Connect<ResumeEvent>(this.OnResumeEvent);
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

	/// <summary>
	/// Upon receiving the default pause and resume events, pauses the object
	/// </summary>
	public class StratusEventDrivenPausable : StratusEventDrivenPausable<StratusPauseEvent, StratusResumeEvent>
	{ 
	}


}