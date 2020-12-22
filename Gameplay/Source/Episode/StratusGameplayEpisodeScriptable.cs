using System.Collections;
using UnityEngine;
using System;

namespace Stratus.Gameplay
{
	public interface IStratusGameplayEpisodeProgression
	{
		bool IsEpisode(IStratusGameplayEpisode episode);
		void UpdateProgression();
	}

	/// <summary>
	/// Records the gameplay episode progression by a player (within a save system)
	/// </summary>
	public abstract class StratusGameplayEpisodeProgression : IStratusGameplayEpisodeProgression
	{
		public string episode;

		public StratusGameplayEpisodeProgression(IStratusGameplayEpisode episode)
		{
			this.episode = episode.label;
		}

		public bool IsEpisode(IStratusGameplayEpisode episode)
		{
			return this.episode == episode.label;
		}

		public abstract void UpdateProgression();
	}

	public interface IStratusGameplayEpisode
	{
		string label { get; }
	}

	public interface IStratusGameplayEpisode<SegmentType> : IStratusGameplayEpisode
		where SegmentType : IStratusGameplaySegment
	{
	}

	public interface IStratusGameplayEpisode<SegmentType, ProgressionType> 
		: IStratusGameplayEpisode<SegmentType>
		where SegmentType : IStratusGameplaySegment
		where ProgressionType : IStratusGameplayEpisodeProgression
	{
		SegmentType GetNextSegment(ProgressionType progression);
		bool IsEnded(ProgressionType progression);
	}

	/// <summary>
	/// A model where gameplay segments are linked together by nodes defined
	/// in scriptables.
	/// </summary>
	public abstract class StratusGameplayEpisodeScriptable<SegmentType, ProgressionType>
		: StratusScriptable, IStratusGameplayEpisode<SegmentType, ProgressionType>
		where SegmentType : class, IStratusGameplaySegment
		where ProgressionType : StratusGameplayEpisodeProgression
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		string IStratusGameplayEpisode.label => this.name;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public abstract SegmentType GetNextSegment(ProgressionType progression);
		public abstract bool IsEnded(ProgressionType progression);


		[StratusInvokeMethod()]
		public abstract void ClearSegments();
	}



}