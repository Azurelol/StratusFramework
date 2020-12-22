using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
	/// <summary>
	/// Manages the toggle of other behaviours due to playmode state changes
	/// </summary>
	[ExecuteInEditMode]
	public class StratusPlayModeStateToggle : StratusBehaviour
	{
		public List<Behaviour> disabledInEditor = new List<Behaviour>();

		private void OnEnable()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
		}

		private void OnDisable()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
		}

		private void OnDestroy()
		{
			Toggle(true);
		}

		private void Toggle(bool toggle)
		{
			foreach (var behaviour in disabledInEditor)
			{
				behaviour.enabled = toggle;
			}
		}

		private void OnValidate()
		{
			Toggle(!Application.isPlaying);
		}

#if UNITY_EDITOR
		protected virtual void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
		{
			switch (stateChange)
			{
				case UnityEditor.PlayModeStateChange.EnteredEditMode:
					Toggle(false);
					break;

				case UnityEditor.PlayModeStateChange.ExitingEditMode:
					Toggle(true);
					break;

				case UnityEditor.PlayModeStateChange.EnteredPlayMode:
					break;

				case UnityEditor.PlayModeStateChange.ExitingPlayMode:
					break;
			}

		}
#endif

	}
}
