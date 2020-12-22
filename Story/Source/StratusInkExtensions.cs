using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

namespace Stratus.Gameplay.Story
{
	public static class StratusInkExtensions
	{
		public static void GoToStart(this Ink.Runtime.Story story)
		{
			story.ResetCallstack();
		}

		public static bool HasCurrentKnotBeenVisited(this Ink.Runtime.Story story)
		{
			if (story.state.currentPathString == null)
				return false;

			int count = story.state.VisitCountAtPathString(story.state.currentPathString);
			return count > 1;
		}
	}

}