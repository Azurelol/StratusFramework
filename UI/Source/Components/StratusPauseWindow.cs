using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.UI
{
	public abstract class StratusPauseWindow : StratusCanvasWindow<StratusPauseWindow>
	{
		[SerializeField]
		private bool pauseOnOpen = true;
		[SerializeField]
		private bool resumeOnClose = true;

		private bool invokingPause { get; set; }

		protected override void OnWindowAwake()
		{
			StratusPauseSystem.onPause += this.OnPause;
		}

		protected override void OnWindowOpen()
		{
			if (pauseOnOpen)
			{
				invokingPause = true;
				Pause();
				invokingPause = false;
			}
		}

		protected override void OnWindowClose()
		{
			if (resumeOnClose)
			{
				invokingPause = true;
				Resume();
				invokingPause = false;
			}
		}

		private void OnPause(bool value)
		{
			if (invokingPause)
			{
				return;
			}
			this.Toggle(value);
		}

		public void Pause() => StratusPauseSystem.instance.Pause();
		public void Resume() => StratusPauseSystem.instance.Resume();
	}

	public abstract class StratusPauseMenuWindow : StratusPauseWindow
	{
		[SerializeField]
		private StratusLayoutTextElementGroupBehaviour layout;

		protected override void OnWindowOpen()
		{
			base.OnWindowOpen();
			GenerateEntries();
		}
		protected override void OnWindowClose()
		{
			base.OnWindowClose();
			layout.Clear();
		}

		private StratusLabeledAction[] GetDefaultActions()
		{
			return new StratusLabeledAction[]
			{
				new StratusLabeledAction("Resume", StratusPauseSystem.instance.Resume)
			};
		}

		protected abstract StratusLabeledAction[] GetActions();

		private void GenerateEntries()
		{
			List<StratusLayoutTextElementEntry> entries = new List<StratusLayoutTextElementEntry>();
			foreach (var action in GetActions())
			{
				entries.Add(new StratusLayoutTextElementEntry(action));
			}
			foreach (var action in GetDefaultActions())
			{
				entries.Add(new StratusLayoutTextElementEntry(action));
			}
			layout.Set(entries);
		}
	}

}