using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	/// <summary>
	/// Upon being notified of a specific state change, pauses/unpauses the object
	/// </summary>
	/// <typeparam name="State"></typeparam>
	public abstract class StratusStateDrivenPausable<State> : StratusPausableBehaviour where State : Enum
	{
		private StratusGamestate<State>.StateEventHandler pauseHandler { get; set; }
		private State pauseState { get; }

		protected override void SetPauseMechanism()
		{
			pauseHandler = new StratusGamestate<State>.StateEventHandler(pauseState);
			pauseHandler.Set(this.Pause);
		}
	}

}