using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Stratus
{
	/// <summary>
	/// A system for pausing the simulation. If not already present,
	/// the instance will be constructed on the scene as a singleton behaviour.
	/// </summary>
	[StratusSingleton(instantiate = true, isPlayerOnly = true)]
	public class StratusPauseSystem : StratusSingletonBehaviour<StratusPauseSystem>,
		IStratusInputProvider
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public StratusPauseOptions options = new StratusPauseOptions();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static bool paused { get; private set; }

		public StratusInputAction[] inputs =>
			new StratusInputAction[]
			{
				new StratusInputAction("Pause", Pause),
				new StratusInputAction("Resume", Resume)
			};

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public static event Action<bool> onPause;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			StratusScene.Connect<StratusPauseEvent>(this.OnPauseEvent);
			StratusScene.Connect<StratusResumeEvent>(this.OnResumeEvent);
		}

		private void OnPauseEvent(StratusPauseEvent e)
		{
			this.SetPause(true);
		}

		private void OnResumeEvent(StratusResumeEvent e)
		{
			this.SetPause(false);
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		public void Pause(bool pause)
		{
			SetPause(pause);
		}

		public void Toggle()
		{
			SetPause(!paused);
		}

		public void Pause() => Pause(true);
		public void Resume() => Pause(false);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#else
				Application.Quit(0);
#endif
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void SetPause(bool value)
		{
			if (paused == value)
			{
				return;
			}

			this.Log(value ? $"Pausing..." : "Resuming...");
			onPause?.Invoke(value);
			paused = value;

			if (options.timeScale)
			{
				Time.timeScale = value ? 0f : 1f;
			}
		}
	}
}