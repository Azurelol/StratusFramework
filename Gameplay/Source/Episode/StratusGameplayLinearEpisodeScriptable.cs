using System.Collections.Generic;
using System;
using UnityEngine;

namespace Stratus.Gameplay
{

	[Serializable]
	public class StratusGameplayEpisodeLinearProgression : StratusGameplayEpisodeProgression
	{
		public int segmentIndex;

		public StratusGameplayEpisodeLinearProgression(IStratusGameplayEpisode episode) : base(episode)
		{
		}

		public override void UpdateProgression()
		{
			segmentIndex++;
		}
	}

	public abstract class StratusGameplayLinearEpisodeScriptable<SegmentType, ProgressionType>
		: StratusGameplayEpisodeScriptable<SegmentType, ProgressionType>
		where SegmentType : class, IStratusGameplaySegment
		where ProgressionType : StratusGameplayEpisodeLinearProgression
	{
		[SerializeReference]
		public List<StratusGameplaySegment> segments = new List<StratusGameplaySegment>();

		public int segmentCount => segments.Count;
		
		public override bool IsEnded(ProgressionType progression)
		{
			if (progression.segmentIndex == (segmentCount - 1))
			{
				return true;
			}
			return false;
		}

		public override void ClearSegments()
		{
			segments.Clear();
		}

		public override SegmentType GetNextSegment(ProgressionType progression)
		{
			return GetSegmentByIndex(progression.segmentIndex);
		}

		public SegmentType GetSegmentByIndex(int index)
		{
			if (!segments.ContainsIndex(index))
			{
				this.LogError($"No segment at index {index}");
				return null;
			}
			SegmentType value = segments[index] as SegmentType;
			if (value == null)
			{
				this.LogError($"Null (unassigned) segment at index {index}");
			}
			return value;
		}
	}

}