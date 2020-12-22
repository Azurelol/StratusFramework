using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay.Story
{
	/// <summary>
	/// Interface for easily saving the state of an ink runtime story
	/// </summary>
	[StratusSaveData(saveExtension = ".story")]
	public class StratusStorySave : StratusSave
	{
		/// <summary>
		/// The saved states of all stories that were loaded by a reader
		/// </summary>
		public List<StratusStory> stories = new List<StratusStory>();

		/// <summary>
		/// The story currently being read
		/// </summary>
		public StratusStory currentStory;


		public override void OnBeforeSerialize()
		{
			foreach (var story in stories)
			{
				story.filePath = story.file.name;
			}
			currentStory.filePath = currentStory.file.name;
		}

		public override void OnAfterDeserialize()
		{
			foreach (var story in stories)
			{
				story.file = Resources.Load(story.filePath) as TextAsset;
				if (story.file == null)
				{
					StratusDebug.LogError($"Failed to load {story.filePath}");
					return;
				}
			}
			currentStory.file = Resources.Load(currentStory.filePath) as TextAsset;
			if (currentStory.file == null)
			{
				StratusDebug.LogError($"Failed to load {currentStory.filePath}");
				return;
			}

			return;
		}

		public override void OnAfterSerialize()
		{
		}

		protected override void OnDelete()
		{
		}
	}

	[StratusSaveData(saveExtension = ".actstorysave", folder = "Tactics", namingConvention = "ACTacticsStorySave", suffix = StratusSaveDataSuffixFormat.Incremental)]
	public class StratusStorySaveSystem : StratusSaveSystem<StratusStorySaveSystem, StratusStorySave, StratusJSONSerializer<StratusStorySave>>
	{
		public override bool debug => false;

		protected override void OnBeforeSave(StratusStorySave save)
		{
		}

		protected override void OnCreateSave(StratusStorySave save)
		{
		}

		protected override void OnSaveSystemInitialize()
		{

		}

	}
}