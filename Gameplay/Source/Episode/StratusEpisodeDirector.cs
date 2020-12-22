using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.NetworkInformation;

namespace Stratus.Gameplay
{
	public abstract class StratusEpisodeDirector<Instance, SegmentType, ProgressionType, EpisodeType, SaveType>
		: StratusSingletonBehaviour<Instance>
		where Instance : StratusEpisodeDirector<Instance, SegmentType, ProgressionType, EpisodeType, SaveType>
		where SegmentType : IStratusGameplaySegment
		where ProgressionType : IStratusGameplayEpisodeProgression
		where EpisodeType : IStratusGameplayEpisode<SegmentType, ProgressionType>, new()
		where SaveType : StratusSave, new()
	{
		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/ 
		public class SaveLoadedEvent : StratusEvent
		{
			public SaveLoadedEvent(SaveType save)
			{
				this.save = save;
			}

			public SaveType save { get; private set; }
		}

		public struct SegmentLoadArgs
		{
			public SegmentLoadArgs(SegmentType segment, Action setupAction, bool promptSave)
			{
				this.segment = segment;
				this.loadAction = setupAction;
				this.promptSave = promptSave;
			}

			public SegmentType segment { get; set; }
			public Action loadAction { get; set; }
			public bool promptSave { get; set; }
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField] private EpisodeType _episode = default;
		[SerializeField] private StratusSnapshotBehaviour _snapshotBehaviour = null;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static SaveType currentSave { get; protected set; }
		public bool hasSave => currentSave != null;
		public EpisodeType episode => _episode;
		public SegmentType segment { get; protected set; }
		public ProgressionType currentProgression
		{
			get
			{
				if (hasSave)
				{
					return GetProgression(currentSave);
				}
				return default;
			}
		}

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnDirectorAwake();
		protected abstract void OnDirectorStart();
		protected abstract ProgressionType GetProgression(SaveType save);
		protected abstract void OnLoadNextSegment(SegmentLoadArgs args);
		protected abstract void OnLoadSaveAsync(Action onLoad);
		protected abstract void OnUnloadSaveAsync(Action saveAction);
		protected abstract void OnLastSegmentEnded();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			OnDirectorAwake();
		}

		private void Start()
		{
			OnDirectorStart();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void LoadSaveAsync(SaveType save, Action onLoad = null)
		{
			this.Log($"Loading save {save}");
			StratusScene.Dispatch<SaveLoadedEvent>(new SaveLoadedEvent(save));

			void action()
			{
				onLoad?.Invoke();
				this.Log("Invoking load node action async");
				StratusValidation loadOperation = save.LoadAsync(() => LoadNextSegmentAsync());
				if (!loadOperation)
				{
					this.LogError($"Failed to load {save}\n{loadOperation.message}");
				}
				currentSave = save;
			}

			OnLoadSaveAsync(action);
		}

		public void UnloadSave(Action onUnload)
		{
			if (currentSave == null)
			{
				this.LogError("No save to unload");
				return;
			}

			void action()
			{
				this.Log($"Unloading current save at {segment}");
				segment?.End();
				currentSave.Unload();
				currentSave = null;
				InvokeNextFrame(onUnload.Invoke);
			}

				OnUnloadSaveAsync(action);

			//if (async)
			//{
			//}
			//else
			//{
			//	action();
			//}
			//StratusTransitionWindow.FadeAction(0.5f, action);
		}

		//------------------------------------------------------------------------/
		// Private
		//------------------------------------------------------------------------/
		protected void LoadNextSegmentAsync(bool promptSave = false)
		{
			if (!currentSave.loaded)
			{
				this.LogError("Save data did not load properly");
				return;
			}

			StratusValidation saveValidation = ValidateSave(currentSave);
			if (!saveValidation)
			{
				this.LogError(saveValidation.message);
				return;
			}

			SegmentType segment = this.episode.GetNextSegment(currentProgression);
			this.segment = segment;
			this.Log($"Now loading node {segment}. Prompt save ? {promptSave}");

			void loadAction()
			{
				segment.onStarted += this.OnSegmentStarted;
				segment.onEnded += this.OnSegmentEnded;
				segment.Load();
			}

			OnLoadNextSegment(new SegmentLoadArgs(segment, loadAction, promptSave));
		}

		protected void OnSegmentStarted(IStratusGameplaySegment segment)
		{
			this.Log($"Segment {segment.label} started...");
		}

		private void OnSegmentEnded(IStratusGameplaySegment segment)
		{
			this.Log($"Node {segment.label} ended...");
			if (this.episode.IsEnded(currentProgression))
			{
				this.Log($"Campaign {episode} ended...");
				OnLastSegmentEnded();
			}
			else
			{
				currentProgression.UpdateProgression();
				this.Log($"Campaign {episode} progressed...");
				LoadNextSegmentAsync(true);
			}
		}

		protected virtual StratusValidation ValidateSave(SaveType save)
		{
			if (!currentProgression.IsEpisode(this.episode))
			{
				return new StratusValidation(false, "This save does not match the campaign");
			}
			return true;
		}

		//------------------------------------------------------------------------/
		// Snapshots
		//------------------------------------------------------------------------/
		public void UpdateSnapshotFromCamera(SaveType save)
		{
			void onSnapshotReady(Texture2D snapshot)
			{
				if (snapshot != null)
				{
					save.SetSnapshot(snapshot);
					this.Log($"Saved snapshot onto {save}");
				}
				else
				{
					this.LogError("Failed to convert texture?");
				}
			}
			TakeSnapshot(onSnapshotReady);
		}

		public void UpdateSnapshotFromSprite(SaveType save, Sprite sprite)
		{
			save.SetSnapshot(sprite.texture);
		}

		public void TakeSnapshot(Action<Texture2D> callback) => _snapshotBehaviour.TakeSnapshot(callback);

	}

}